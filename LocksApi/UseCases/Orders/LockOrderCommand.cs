using LocksApi.Entities;
using LocksApi.Services;
using MediatR;

namespace LocksApi.UseCases.Orders;

public record LockOrderCommand(LockOrderDto Dto) : IRequest;
public record LockOrderDto(int Id) : IRequest;

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
        var @lock = new Lock
        {
            OwnerId = _currentUserService.CurrentUserId,
            EntityId = request.Dto.Id,
            EntityType = nameof(Order),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Locks.Add(@lock);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}