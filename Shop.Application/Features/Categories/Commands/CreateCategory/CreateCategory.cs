using Shop.Application.Common.Interfaces;
using Shop.Domain.Entities;
using Shop.Domain.Helpers;

namespace Shop.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand : IRequest<Guid>
{
    public string Name { get; init; } = null!;
    public string Slug { get; init; } = null!;
    public Guid? ParentId { get; init; }
}

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    
    public CreateCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category()
        {
            Id = Guid.NewGuid(), 
            Name = request.Name,
            
            Slug = string.IsNullOrWhiteSpace(request.Slug) 
                ? request.Name.ToSlug() 
                : request.Slug.ToSlug(),
                   
            ParentId = request.ParentId
        };

        await _context.Categories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}