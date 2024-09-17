using MediatR;
using Microsoft.AspNetCore.Mvc;
using SampleApi.UseCases.Products;

namespace SampleApi.Controllers;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender) => _sender = sender;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetProductQuery(id), cancellationToken);
        
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto, CancellationToken cancellationToken)
    {
        await _sender.Send(new CreateProductCommand(dto), cancellationToken);
        
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProduct([FromBody]UpdateProductDto dto, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateProductCommand(dto), cancellationToken);
        
        return Ok();
    }
}