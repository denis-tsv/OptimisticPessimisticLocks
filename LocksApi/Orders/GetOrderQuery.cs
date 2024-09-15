using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleApi.Entities;
using SampleApi.Services;

namespace SampleApi.Orders;

public record GetOrderQuery(int Id) : IRequest<GetOrderDto>;

public record GetOrderDto(
    int Id, 
    string Name, 
    int? LockOwnerId,
    GetOrderItemDto[] Items);

public record GetOrderItemDto(int Id, int ProductId, decimal Price, int Quantity); 

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, GetOrderDto>
{
    private readonly LocksDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetOrderQueryHandler(LocksDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<GetOrderDto> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (order == null) throw new ApplicationException("Not found");

        var @lock = await _dbContext.Locks.FirstOrDefaultAsync(x =>
                x.OwnerId == _currentUserService.CurrentUserId &&
                x.EntityType == nameof(Order) &&
                x.Id == request.Id,
            cancellationToken);
        
        return new GetOrderDto(
            order.Id,
            order.Name,
            @lock?.OwnerId,
            order.Items.Select(x => new GetOrderItemDto(x.Id, x.ProductId, x.Price, x.Quantity)).ToArray()
        );
    }
} 