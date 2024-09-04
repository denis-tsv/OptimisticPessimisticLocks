using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleApi.Services;

namespace SampleApi.Orders;

public record UnlockOrderCommand(UnlockOrderDto Dto) : IRequest;
public record UnlockOrderDto(int Id, uint Version) : IRequest;

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
        if (order.Version != request.Dto.Version) throw new ApplicationException("Refresh and try again");
        if (order.LockedById == null) throw new ApplicationException("Order must be locked to unlock it");
        if (order.LockedById != _currentUserService.CurrentUserId) throw new ApplicationException("Locked by another user");

        order.LockedById = null;
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}