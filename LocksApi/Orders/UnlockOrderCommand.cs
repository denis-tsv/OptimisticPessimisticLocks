using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleApi.Entities;
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
        var deleted = await _dbContext.Locks
            .Where(x => x.OwnerId == _currentUserService.CurrentUserId && 
                        x.EntityType == nameof(Order) && 
                        x.EntityId == request.Dto.Id)
            .ExecuteDeleteAsync(cancellationToken);

        if (deleted == 0) throw new ApplicationException("Order must be locked before unlock");
    }
}