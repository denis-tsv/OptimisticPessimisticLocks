using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleApi.Entities;

namespace SampleApi.UseCases.Orders;

public record UpdateOrderCommand(UpdateOrderDto Dto) : IRequest;

public record UpdateOrderDto(
    int Id,
    CreateOrderItemDto[]? CreatedItems,
    UpdateOrderItemDto[]? UpdatedItems,
    int[]? DeletedItems
);
public record CreateOrderItemDto(int ProductId, int Quantity, decimal Price);
public record UpdateOrderItemDto(int Id, int Quantity);

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
{
    private readonly LocksDbContext _dbContext;

    public UpdateOrderCommandHandler(LocksDbContext dbContext) => _dbContext = dbContext;

    public async Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .Include(x => x.Items)
            .FirstAsync(x => x.Id == request.Dto.Id, cancellationToken);

        if (request.Dto.CreatedItems != null)
        {
            order.Items.AddRange(request.Dto.CreatedItems.Select(x => new OrderItem
            {
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                Price = x.Price
            }));

        }

        if (request.Dto.UpdatedItems != null)
        {
            foreach (var orderItemDto in request.Dto.UpdatedItems)
            {
                var orderItem = order.Items.First(x => x.Id == orderItemDto.Id);
                orderItem.Quantity = orderItemDto.Quantity;
            }
        }

        if (request.Dto.DeletedItems != null)
        {
            order.Items.RemoveAll(x => request.Dto.DeletedItems.Contains(x.Id));
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}