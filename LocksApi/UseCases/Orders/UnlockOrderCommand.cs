using LocksApi.Entities;
using LocksApi.Services;
using LocksApi.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LocksApi.UseCases.Orders;

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
        var @lock = await _dbContext.Locks
            .FirstOrDefaultAsync(x => x.EntityType == nameof(Order) && x.EntityId == request.Dto.Id, cancellationToken);
        if (@lock == null) throw new ApplicationException("Order must be locked before unlock");
        if (@lock.OwnerId != _currentUserService.CurrentUserId) throw new LockedApplicationException();

        _dbContext.Locks.Remove(@lock);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}