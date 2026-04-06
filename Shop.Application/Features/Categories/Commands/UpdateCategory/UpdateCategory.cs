using Shop.Application.Common.Interfaces;
using Shop.Domain.Helpers;

namespace Shop.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand : IRequest
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
}

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly IApplicationDbContext _context;
    
    public UpdateCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Categories
            .FindAsync(request.Id, cancellationToken);
        
        Guard.Against.NotFound(request.Id, entity);
        
        entity.Name = request.Name;
        entity.Slug = string.IsNullOrWhiteSpace(request.Slug)
            ? request.Name.ToSlug()
            : request.Slug.ToSlug();
        
        await _context.SaveChangesAsync(cancellationToken);
    }
}