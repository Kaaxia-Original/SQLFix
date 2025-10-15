using SQLFix.Data;
using SQLFix.Entities;
using MediatR;
using SQLFix.Application.Commands;

namespace EcommerceCQRS.Application.Handlers;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly WriteDbContext _context;

    public CreateProductCommandHandler(WriteDbContext context)
    {
        _context = context;
     }

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            CreatedAt = TimeSpan.Zero,
            IsActive = true
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
