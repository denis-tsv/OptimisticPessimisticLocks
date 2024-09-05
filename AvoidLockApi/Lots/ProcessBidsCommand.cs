using AvoidLockApi.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AvoidLockApi.Lots;

public record ProcessBidsCommand(ProcessBidsDto Dto) : IRequest;

public record ProcessBidsDto(BidDto[] Bids);
public record BidDto(int LotId, int UserId, decimal Price, DateTime CreatedAt);

public class ProcessBidsCommandHandler : IRequestHandler<ProcessBidsCommand>
{
    private readonly AvoidLockDbContext _dbContext;

    public ProcessBidsCommandHandler(AvoidLockDbContext dbContext) => _dbContext = dbContext;

    public async Task Handle(ProcessBidsCommand request, CancellationToken cancellationToken)
    {
        foreach (var bidDtos in request.Dto.Bids.GroupBy(x => x.LotId))
        {
            var lot = await _dbContext.Lots.FirstAsync(x => x.Id == bidDtos.Key, cancellationToken);

            foreach (var bidDto in bidDtos.OrderBy(x => x.CreatedAt))
            {
                string bidStatus;
                if (bidDto.Price > lot.Price)
                {
                    lot.UserId = bidDto.UserId;
                    lot.Price = bidDto.Price;
                    lot.UpdatedAt = bidDto.CreatedAt;

                    bidStatus = "Success";
                }
                else
                {
                    bidStatus = "Fail";
                }
                
                var bid = new Bid
                {
                    UserId = bidDto.UserId,
                    LotId = bidDto.LotId,
                    CreatedAt = bidDto.CreatedAt,
                    Price = bidDto.Price,
                    Status = bidStatus
                };
                _dbContext.Bids.Add(bid);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}