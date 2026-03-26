using Shop.Application.Common.Interfaces;
using Shop.Domain.Entities;

namespace Shop.Application.Features.Products.Queries;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;


public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IApplicationDbContext _context;

    public GetProductByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .Include(p => p.Category) 
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
        {
            throw new NotFoundException(nameof(Product), request.Id.ToString());
        }

        return new ProductDto(product.Id, product.Name, product.Slug, product.Description,product.Price, product.CategoryId, product.Category.Name);
    }
}