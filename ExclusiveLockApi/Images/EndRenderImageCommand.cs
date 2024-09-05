using ExclusiveLockApi.Entities;
using ExclusiveLockApi.Services;
using LinqToDB;
using LinqToDB.DataProvider.PostgreSQL;
using LinqToDB.EntityFrameworkCore;
using MediatR;

namespace ExclusiveLockApi.Images;

public record EndRenderImageCommand(int RenderId) : IRequest;

public class EndRenderImageCommandHandler : IRequestHandler<EndRenderImageCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ExclusiveLockDbContext _dbContext;

    public EndRenderImageCommandHandler(ICurrentUserService currentUserService, ExclusiveLockDbContext dbContext)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
    }
    
    public async Task Handle(EndRenderImageCommand request, CancellationToken cancellationToken)
    {
        await using var dataConnection = _dbContext.CreateLinqToDBConnection();

        var transaction = await dataConnection.BeginTransactionAsync(cancellationToken);
        
        var account = await dataConnection.GetTable<Account>()
            .Where(x => x.UserId == _currentUserService.CurrentUserId)
            .SubQueryHint(PostgreSQLHints.ForUpdate)
            .FirstAsync(cancellationToken);
        
        await dataConnection.GetTable<Account>()
            .Where(x => x.Id == account.Id)
            .Set(x => x.BlockedVolume, account.BlockedVolume - Render.Cost)
            .UpdateAsync(cancellationToken);
        
        await dataConnection.GetTable<RenderImage>()
            .Where(x => x.Id == request.RenderId)
            .Set(x => x.Status, "Finished")
            .UpdateAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }
}