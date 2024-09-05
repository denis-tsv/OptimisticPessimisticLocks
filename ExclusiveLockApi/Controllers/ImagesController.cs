using ExclusiveLockApi.Images;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExclusiveLockApi.Controllers;

[ApiController]
[Route("images")]
public class ImagesController : ControllerBase
{
    private readonly ISender _sender;

    public ImagesController(ISender sender) => _sender = sender;
    
    [HttpPost("start-render")]
    public async Task<IActionResult> StartRender(CancellationToken cancellationToken)
    {
        await _sender.Send(new StartRenderImageEfCommand(), cancellationToken);
        
        return Ok();
    }
    
    [HttpPost("{id}/end-render")]
    public async Task<IActionResult> EndRender([FromRoute]int id, CancellationToken cancellationToken)
    {
        await _sender.Send(new EndRenderImageCommand(id), cancellationToken);
        
        return Ok();
    }
}