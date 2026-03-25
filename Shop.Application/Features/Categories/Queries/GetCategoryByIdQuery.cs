using Shop.Application.Common.Interfaces;
using Shop.Domain.Entities;
using NotFoundException = Ardalis.GuardClauses.NotFoundException;

namespace Shop.Application.Features.Categories.Queries;

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto>;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly IApplicationDbContext _context;
    public GetCategoryByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FindAsync(request.Id, cancellationToken);
        
        if (category == null)
            throw new NotFoundException(nameof(Category), request.Id.ToString()); 

        return new CategoryDto(category.Id, category.Name, category.Slug, category.ParentId);
    }
}