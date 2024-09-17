using ExclusiveLockApi.Entities;
using ExclusiveLockApi.Services;
using LinqToDB;
using LinqToDB.DataProvider.PostgreSQL;
using LinqToDB.EntityFrameworkCore;
using MediatR;

namespace ExclusiveLockApi.UseCases.Images;

public record StartRenderImageCommand : IRequest;

public class StartRenderImageCommandHandler : IRequestHandler<StartRenderImageCommand>
{
    private readonly ExclusiveLockDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public StartRenderImageCommandHandler(ExclusiveLockDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task Handle(StartRenderImageCommand request, CancellationToken cancellationToken)
    {
        await using var dataConnection = _dbContext.CreateLinqToDBConnection();

        var transaction = await dataConnection.BeginTransactionAsync(cancellationToken);
        
        var account = await dataConnection.GetTable<Account>()
            .Where(x => x.UserId == _currentUserService.CurrentUserId)
            .SubQueryHint(PostgreSQLHints.ForUpdate)
            .FirstAsync(cancellationToken);

        if (account.Volume >= Render.Cost)
        {
            await dataConnection.GetTable<Account>()
                .Where(x => x.Id == account.Id)
                .Set(x => x.Volume, account.Volume - Render.Cost)
                .Set(x => x.BlockedVolume, account.BlockedVolume + Render.Cost)
                .UpdateAsync(cancellationToken);
            
            await dataConnection.GetTable<RenderImage>()
                .Value(x => x.UserId, _currentUserService.CurrentUserId)
                .Value(x => x.Status, "Queued")
                .InsertAsync(cancellationToken);
        }
        else
        {
            await dataConnection.GetTable<RenderImage>()
                .Value(x => x.UserId, _currentUserService.CurrentUserId)
                .Value(x => x.Status, "NotEnoughMoney")
                .InsertAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
    }
}