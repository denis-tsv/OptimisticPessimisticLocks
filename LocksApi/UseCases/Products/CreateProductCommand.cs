using MediatR;
using SampleApi.Entities;

namespace SampleApi.UseCases.Products;

public record CreateProductCommand(CreateProductDto Dto) : IRequest;

public record CreateProductDto(string Name);

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand>
{
    private readonly LocksDbContext _dbContext;

    public CreateProductCommandHandler(LocksDbContext dbContext) => _dbContext = dbContext;

    public async Task Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Dto.Name
        };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}