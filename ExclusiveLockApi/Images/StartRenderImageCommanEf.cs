using ExclusiveLockApi.Entities;
using ExclusiveLockApi.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExclusiveLockApi.Images;

public record StartRenderImageEfCommand : IRequest;

public class StartRenderImageEfCommandHandler : IRequestHandler<StartRenderImageEfCommand>
{
    private readonly ExclusiveLockDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public StartRenderImageEfCommandHandler(ExclusiveLockDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task Handle(StartRenderImageEfCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        var account = await _dbContext.Accounts
            .FromSql($"SELECT * FROM accounts WHERE user_id = {_currentUserService.CurrentUserId} FOR UPDATE")
            .FirstAsync(cancellationToken);

        if (account.Volume >= Render.Cost)
        {
            account.Volume -= Render.Cost;
            account.BlockedVolume += Render.Cost;

            var renderImage = new RenderImage
            {
                UserId = _currentUserService.CurrentUserId,
                Status = "Queued"
            };
            _dbContext.RenderImages.Add(renderImage);
        }
        else
        {
            var renderImage = new RenderImage
            {
                UserId = _currentUserService.CurrentUserId,
                Status = "NotEnoughMoney"
            };
            _dbContext.RenderImages.Add(renderImage);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }
}