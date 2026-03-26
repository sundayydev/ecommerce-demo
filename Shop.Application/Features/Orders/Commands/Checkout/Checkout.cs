using Shop.Application.Common.Interfaces;
using Shop.Domain.Constants;
using Shop.Domain.Entities;
using Shop.Domain.ValueObject;

namespace Shop.Application.Features.Orders.Commands.Checkout;

public record CheckoutCommand(string Detail, string Ward, string City) : IRequest<Guid>;

public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CheckoutCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("Cần đăng nhập.");

        // 1. Kéo giỏ hàng lên
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.ProductVariant)
            .ThenInclude(v => v.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart == null || !cart.Items.Any())
            throw new Exception("Giỏ hàng của bạn đang trống.");
        
        var shippingAddress = new Address(request.Detail, request.Ward, request.City);

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // 3. KHỞI TẠO ORDER CHUẨN DDD
            var order = new Order
            {
                UserId = userId,
                ShippingAddress = shippingAddress, // Gắn Value Object vào đây
                Status = OrderStatus.Pending,
                TotalAmount = 0,
                Items = new List<OrderItem>()
            };

            foreach (var cartItem in cart.Items)
            {
                var variant = cartItem.ProductVariant;

                if (cartItem.Quantity > variant.Stock)
                {
                    throw new Exception(
                        $"Sản phẩm '{variant.Product.Name} - {variant.Size} - {variant.Color}' chỉ còn {variant.Stock} cái.");
                }

                variant.Stock -= cartItem.Quantity;

                var price = (variant.Price ?? 0) > 0 ? variant.Price.Value : variant.Product.Price;

                order.TotalAmount += price * cartItem.Quantity;

                order.Items.Add(new OrderItem
                {
                    ProductVariantId = variant.Id,
                    Quantity = cartItem.Quantity,
                    UnitPrice = price,
                });
            }

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cart.Items);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return order.Id;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}