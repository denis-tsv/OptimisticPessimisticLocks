using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LocksApi.UseCases.Products;

public record GetProductQuery(int Id) : IRequest<GetProductDto>;

public record GetProductDto(int Id, string Name);

public class GetOrderQueryHandler : IRequestHandler<GetProductQuery, GetProductDto>
{
    private readonly LocksDbContext _dbContext;

    public GetOrderQueryHandler(LocksDbContext dbContext) => _dbContext = dbContext;

    public async Task<GetProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Products
            .Where(x => x.Id == request.Id)
            .Select(x => new GetProductDto(x.Id, x.Name))
            .FirstOrDefaultAsync(cancellationToken);
        if (result == null) throw new ApplicationException("Not found");

        return result;
    }
} 