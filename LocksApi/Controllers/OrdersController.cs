using LocksApi.UseCases.Orders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocksApi.Controllers;

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

    [HttpPut]
    public async Task<IActionResult> UpdateOrder([FromBody]UpdateOrderDto dto, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateOrderCommand(dto), cancellationToken);
        
        return Ok();
    }
}