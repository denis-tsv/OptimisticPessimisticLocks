using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleApi.Services;

namespace SampleApi.Orders;

public record UnlockOrderCommand(UnlockOrderDto Dto) : IRequest;
public record UnlockOrderDto(int Id) : IRequest;

public class UnlockOrderCommandHandler : IRequestHandler<UnlockOrderCommand>
{
    private readonly LocksDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    
    public UnlockOrderCommandHandler(LocksDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UnlockOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FirstAsync(x => x.Id == request.Dto.Id, cancellationToken);
        if (order.LockOwnerId == null) throw new ApplicationException("Order must be locked to unlock it");
        if (order.LockOwnerId != _currentUserService.CurrentUserId) throw new ApplicationException("Locked by another user");

        order.LockOwnerId = null;
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}