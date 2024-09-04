using MediatR;
using SampleApi.Entities;

namespace SampleApi.Orders;

public record CreateOrderCommand(CreateOrderDto Dto) : IRequest;

public record CreateOrderDto(string Name);

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand>
{
    private readonly LocksDbContext _dbContext;

    public CreateOrderCommandHandler(LocksDbContext dbContext) => _dbContext = dbContext;

    public async Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            Name = request.Dto.Name
        };
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}