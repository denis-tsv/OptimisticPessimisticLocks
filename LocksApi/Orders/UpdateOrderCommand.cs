using System.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace SampleApi.Orders;

public record UpdateOrderCommand(UpdateOrderDto Dto) : IRequest;

public record UpdateOrderDto(int Id, string Name, uint Version);

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
{
    private readonly LocksDbContext _dbContext;

    public UpdateOrderCommandHandler(LocksDbContext dbContext) => _dbContext = dbContext;

    public async Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FirstAsync(x => x.Id == request.Dto.Id, cancellationToken);
        if (order.Version != request.Dto.Version) throw new ApplicationException("409 Conflict");
        
        order.Name = request.Dto.Name;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}