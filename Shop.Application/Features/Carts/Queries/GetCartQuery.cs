using Shop.Application.Common.Interfaces;

namespace Shop.Application.Features.Carts.Queries;

public record GetCartQuery : IRequest<CartDto>;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetCartQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("Cần đăng nhập.");

        var cart = await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
                    .ThenInclude(v => v.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart == null)
        {
            return new CartDto(Guid.Empty, new List<CartItemDto>(), 0);
        }
        
        var itemDtos = cart.Items.Select(i => 
        {

            var price = (i.ProductVariant.Price ?? 0) > 0 
                ? i.ProductVariant.Price.Value 
                : i.ProductVariant.Product.Price;

            return new CartItemDto(
                i.Id,
                i.ProductVariantId,
                i.ProductVariant.Product.Name,
                i.ProductVariant.Size,
                i.ProductVariant.Color,
                price,
                i.Quantity,
                price * i.Quantity 
            );
        }).ToList();

        var totalCartPrice = itemDtos.Sum(i => i.TotalPrice);

        return new CartDto(cart.Id, itemDtos, totalCartPrice);
    }
}