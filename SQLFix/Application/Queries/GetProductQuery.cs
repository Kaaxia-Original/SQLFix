
using MediatR;

namespace SQLFix.Application.Queries;

public class GetProductQuery : IRequest<ProductDto>
{
    public int Id { get; set; }
}

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
