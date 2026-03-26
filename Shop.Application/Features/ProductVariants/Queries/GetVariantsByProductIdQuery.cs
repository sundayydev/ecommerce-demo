using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop.Application.Common.Interfaces;

namespace Shop.Application.Features.ProductVariants.Queries;

public record GetVariantsByProductIdQuery(Guid ProductId) : IRequest<List<ProductVariantDto>>;

public class GetVariantsByProductIdQueryHandler : IRequestHandler<GetVariantsByProductIdQuery, List<ProductVariantDto>>
{
    private readonly IApplicationDbContext _context;

    public GetVariantsByProductIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductVariantDto>> Handle(GetVariantsByProductIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.ProductVariants
            .AsNoTracking()
            .Where(v => v.ProductId == request.ProductId)
            .Select(v => new ProductVariantDto(v.Id, v.ProductId, v.Product.Name, v.Size, v.Color, v.Price, v.Stock))
            .ToListAsync(cancellationToken);
    }
}