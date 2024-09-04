using MediatR;
using Microsoft.AspNetCore.Mvc;
using SampleApi.Orders;

namespace SampleApi.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly ISender _sender;

    public OrdersController(ISender sender) => _sender = sender;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetOrderQuery(id), cancellationToken);
        
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto, CancellationToken cancellationToken)
    {
        await _sender.Send(new CreateOrderCommand(dto), cancellationToken);
        
        return Ok();
    }
    
    [HttpPost("lock")]
    public async Task<IActionResult> LockOrder([FromBody] LockOrderDto dto, CancellationToken cancellationToken)
    {
        await _sender.Send(new LockOrderCommand(dto), cancellationToken);
        
        return Ok();
    }
    
    [HttpPost("unlock")]
    public async Task<IActionResult> UnlockOrder([FromBody] UnlockOrderDto dto, CancellationToken cancellationToken)
    {
        await _sender.Send(new UnlockOrderCommand(dto), cancellationToken);
        
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateOrder([FromBody]UpdateOrderDto dto, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateOrderCommand(dto), cancellationToken);
        
        return Ok();
    }
}