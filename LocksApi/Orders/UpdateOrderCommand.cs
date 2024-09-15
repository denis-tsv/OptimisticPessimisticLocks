using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleApi.Entities;
using SampleApi.Services;

namespace SampleApi.Orders;

public record UpdateOrderCommand(UpdateOrderDto Dto) : IRequest;

public record UpdateOrderDto(
    int Id,
    string? Name,
    CreateOrderItemDto[]? CreatedItems,
    UpdateOrderItemDto[]? UpdatedItems,
    int[]? DeletedItems
);
public record CreateOrderItemDto(int ProductId, int Quantity, decimal Price);
public record UpdateOrderItemDto(int Id, int Quantity);

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
{
    private readonly LocksDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public UpdateOrderCommandHandler(LocksDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var lockExists = await _dbContext.Locks
            .AnyAsync(x => x.OwnerId == _currentUserService.CurrentUserId &&
                           x.EntityType == nameof(Order) &&
                           x.EntityId == request.Dto.Id, 
                cancellationToken);
        if (!lockExists) throw new ApplicationException("Order must be locked before edit");
        
        var order = await _dbContext.Orders
            .Include(x => x.Items)
            .FirstAsync(x => x.Id == request.Dto.Id, cancellationToken);

        if (!string.IsNullOrEmpty(request.Dto.Name))
        {
            order.Name = request.Dto.Name;
        }

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
        
        order.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}