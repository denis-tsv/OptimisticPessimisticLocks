using LocksApi.Entities;
using MediatR;

namespace LocksApi.UseCases.Orders;

public record CreateOrderCommand(CreateOrderDto Dto) : IRequest;

public record CreateOrderDto(CreateOrderItemDto[] Items);

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand>
{
    private readonly LocksDbContext _dbContext;

    public CreateOrderCommandHandler(LocksDbContext dbContext) => _dbContext = dbContext;

    public async Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            Items = request.Dto.Items.Select(x => new OrderItem
            {
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                Price = x.Price
            }).ToList()
        };
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}