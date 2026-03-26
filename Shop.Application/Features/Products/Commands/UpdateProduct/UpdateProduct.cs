using Shop.Application.Common.Interfaces;
using Shop.Domain.Entities;
using Shop.Domain.Helpers;

namespace Shop.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand : IRequest
{
    public Guid Id { get; init; }
    
    private string _name = string.Empty;
    public string Name 
    { 
        get => _name; 
        set => _name = value?.Trim() ?? string.Empty; 
    }
    
    public string Slug { get; init; }
    
    public string Description { get; init; }
    
    public decimal Price  { get; init; }
        
    public Guid CategoryId { get; init; }
    
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Products
            .FindAsync(request.Id, cancellationToken);
        
        Guard.Against.NotFound(request.Id, entity);
        
        entity.Name = request.Name;
        entity.Slug = string.IsNullOrWhiteSpace(request.Slug)
            ? request.Name.ToSlug()
            : request.Slug.ToSlug();

        entity.Description = entity.Description;
        entity.Price = entity.Price;

        await _context.SaveChangesAsync(cancellationToken);
    }
}