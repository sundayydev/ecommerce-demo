using Shop.Application.Common.Interfaces;

namespace Shop.Application.Features.Orders.Queries;

public record GetMyOrdersQuery() : IRequest<List<OrderDto>>;


public class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, List<OrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetMyOrdersQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<OrderDto>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("Cần đăng nhập.");

        var orders = await _context.Orders
            .AsNoTracking() // Dùng AsNoTracking cho API Get để tăng tốc độ (chỉ đọc)
            .Include(o => o.Items)
                .ThenInclude(i => i.ProductVariant)
                    .ThenInclude(v => v.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.Created) // Đơn hàng mới nhất xếp lên đầu
            .Select(o => new OrderDto(
                o.Id,
                o.Created,
                o.Status,
                o.TotalAmount,
                o.ShippingAddress.ToString(),
                o.Items.Select(i => new OrderItemDto(
                    i.Id,
                    i.ProductVariant.Product.Name,
                    $"{i.ProductVariant.Size} - {i.ProductVariant.Color}", 
                    i.Quantity,
                    i.UnitPrice,
                    i.Quantity*i.UnitPrice
                )).ToList()
            ))
            .ToListAsync(cancellationToken);

        return orders;
    }
}