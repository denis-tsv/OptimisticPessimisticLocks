using MediatR;
using Microsoft.EntityFrameworkCore;

namespace SampleApi.Orders;

public record UpdateOrderCommand(UpdateOrderDto Dto) : IRequest;

public record UpdateOrderDto(int Id, string Name);

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
{
    private readonly LocksDbContext _dbContext;

    public UpdateOrderCommandHandler(LocksDbContext dbContext) => _dbContext = dbContext;

    public async Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FirstAsync(x => x.Id == request.Dto.Id, cancellationToken);

        order.Name = request.Dto.Name;

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken); //IsolationLevel.ReadCommitted
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
    }
}