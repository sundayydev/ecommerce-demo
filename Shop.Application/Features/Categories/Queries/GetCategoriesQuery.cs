using Shop.Application.Common.Interfaces;

namespace Shop.Application.Features.Categories.Queries;

public record GetCategoriesQuery : IRequest<List<CategoryDto>>;
public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly IApplicationDbContext _context;
    public GetCategoriesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _context.Categories.ToListAsync(cancellationToken);
        
        return categories.Select(c => new CategoryDto(c.Id, c.Name, c.Slug, c.ParentId)).ToList();
    }
}
