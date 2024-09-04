using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleApi.Services;

namespace SampleApi.Orders;

public record LockOrderCommand(LockOrderDto Dto) : IRequest;
public record LockOrderDto(int Id, uint Version) : IRequest;

public class LockOrderCommandHandler : IRequestHandler<LockOrderCommand>
{
    private readonly LocksDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    
    public LockOrderCommandHandler(LocksDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task Handle(LockOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FirstAsync(x => x.Id == request.Dto.Id, cancellationToken);
        if (order.Version != request.Dto.Version) throw new ApplicationException("Refresh and try again");
        if (order.LockedById != null) throw new ApplicationException("Order must be unlocked to lock it");

        order.LockedById = _currentUserService.CurrentUserId;
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}