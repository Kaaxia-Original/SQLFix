using MediatR;
using Microsoft.AspNetCore.Mvc;
using SQLFix.Application.Commands;
using SQLFix.Application.Queries;

namespace SQLFix.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { ProductId = id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var query = new GetProductQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

}
