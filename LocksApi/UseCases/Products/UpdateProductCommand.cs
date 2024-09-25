using LocksApi.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LocksApi.UseCases.Products;

public record UpdateProductCommand(UpdateProductDto Dto) : IRequest;

public record UpdateProductDto(int Id, string Name, uint Version);

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly LocksDbContext _dbContext;

    public UpdateProductCommandHandler(LocksDbContext dbContext) => _dbContext = dbContext;

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.Dto.Id, cancellationToken);
        if (product == null) throw new NotFoundApplicationException();

        if (product.Version != request.Dto.Version) throw new VersionObsoleteApplicationException();

        product.Name = request.Dto.Name;
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}