using MediatR;
using Shop.Application.Common.Interfaces;
using Shop.Application.Common.Models;

namespace Shop.Application.Features.Products.Queries;

public class GetProductsWithPaginationQuery : IRequest<PaginatedList<ProductDto>>
{
    public int? PageNumber { get; init; } = 1;
    public int? PageSize { get; init; } = 10;
    
    public Guid? CategoryId { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
}

public class GetProductsWithPaginationQueryHandler : IRequestHandler<GetProductsWithPaginationQuery, PaginatedList<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsWithPaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<PaginatedList<ProductDto>> Handle(GetProductsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products
            .Include(p => p.Category) 
            .AsNoTracking() 
            .AsQueryable(); 
        
        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);
        }

        if (request.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= request.MaxPrice.Value);
        }
        
        query = query.OrderByDescending(p => p.Created);
        
        var selectQuery = query.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Slug,
            p.Description,
            p.Price,
            p.CategoryId,
            p.Category.Name 
        ));
        
        return await PaginatedList<ProductDto>.CreateAsync(selectQuery,
            request.PageNumber ?? 1,
            request.PageSize ?? 10);
    }
}