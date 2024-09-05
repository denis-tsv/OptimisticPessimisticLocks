using AvoidLockApi.Lots;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AvoidLockApi.Controllers;

[ApiController]
[Route("lots")]
public class LotsController : ControllerBase
{
    private readonly ISender _sender;

    public LotsController(ISender sender) => _sender = sender;
    
    [HttpPost("bids")]
    public async Task<IActionResult> ProcessBids([FromBody]ProcessBidsDto dto, CancellationToken cancellationToken)
    {
        await _sender.Send(new ProcessBidsCommand(dto), cancellationToken);
        
        return Ok();
    }
}