using LocksApi.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LocksApi.UseCases.Orders;

public record GetOrderQuery(int Id) : IRequest<GetOrderDto>;

public record GetOrderDto(int Id, GetOrderItemDto[] Items);
public record GetOrderItemDto(int Id, int ProductId, decimal Price, int Quantity); 

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, GetOrderDto>
{
    private readonly LocksDbContext _dbContext;

    public GetOrderQueryHandler(LocksDbContext dbContext) => _dbContext = dbContext;

    public async Task<GetOrderDto> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (order == null) throw new NotFoundApplicationException();

        return new GetOrderDto(
            order.Id,
            order.Items.Select(x => new GetOrderItemDto(x.Id, x.ProductId, x.Price, x.Quantity)).ToArray()
        );
    }
} 