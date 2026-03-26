using Shop.Application.Common.Interfaces;
using Shop.Domain.Entities;
using Shop.Domain.Helpers;

namespace Shop.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand : IRequest<Guid>
{
    public string Name { get; init; } = null!;
    public string Slug { get; init; } = null!;
    public string Description { get; init; } = null!;
    public decimal Price { get; init; }
    public Guid CategoryId { get; init; }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = string.IsNullOrWhiteSpace(request.Slug)
                ? request.Name.ToSlug()
                : request.Slug.ToSlug(),

            Description = request.Description,
            CategoryId = request.CategoryId,
            IsDeleted = false
        };

        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return product.Id;
    }
}