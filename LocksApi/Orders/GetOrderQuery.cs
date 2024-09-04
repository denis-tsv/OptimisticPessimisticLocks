using MediatR;
using Microsoft.EntityFrameworkCore;

namespace SampleApi.Orders;

public record GetOrderQuery(int Id) : IRequest<GetOrderDto>;

public record GetOrderDto(int Id, string Name);

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, GetOrderDto>
{
    private readonly LocksDbContext _dbContext;

    public GetOrderQueryHandler(LocksDbContext dbContext) => _dbContext = dbContext;

    public async Task<GetOrderDto> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Orders
            .Where(x => x.Id == request.Id)
            .Select(x => new GetOrderDto(x.Id, x.Name))
            .FirstOrDefaultAsync(cancellationToken);
        if (result == null) throw new ApplicationException("Not found");

        return result;
    }
} 