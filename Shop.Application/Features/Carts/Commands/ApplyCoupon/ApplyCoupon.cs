using Shop.Application.Common.Interfaces;
using Shop.Domain.Constants;

namespace Shop.Application.Features.Carts.Commands.ApplyCoupon;

public record ApplyCouponCommand(Guid UserId, string CouponCode) : IRequest<bool>;
public class ApplyCouponCommandHandler : IRequestHandler<ApplyCouponCommand, bool>
{
    private IApplicationDbContext _context;

    public ApplyCouponCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<bool> Handle(ApplyCouponCommand request, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.ProductVariant)
            .ThenInclude(v => v.Product)
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);
        
        if (cart == null || !cart.Items.Any())
            throw new ValidationException("Giỏ hàng đang trống.");

        var normalizedCode = request.CouponCode.Trim().ToUpper();
        var coupon = await _context.Coupons
            .FirstOrDefaultAsync(c => c.Code == normalizedCode, cancellationToken);

        if (coupon == null || !coupon.IsValid())
            throw new ValidationException("Mã giảm giá không hợp lệ, đã hết hạn hoặc hết lượt sử dụng.");

        if (cart.SubTotal < coupon.MinOrderValue)
            throw new ValidationException($"Đơn hàng chưa đạt mức tối thiểu {coupon.MinOrderValue:N0}đ để áp dụng mã này.");
        Console.WriteLine($"[DEBUG 1] SubTotal của giỏ hàng: {cart.SubTotal}");
        Console.WriteLine($"[DEBUG 2] Loại giảm giá (Type): {coupon.DiscountType}");
        Console.WriteLine($"[DEBUG 3] Giá trị giảm (Value): {coupon.Value}");
        decimal discountAmount = 0;
        if (coupon.DiscountType == DiscountType.PERCENTAGE)
        {
            discountAmount = cart.SubTotal * (coupon.Value / 100m);
        }
        else if (coupon.DiscountType == DiscountType.FIXED_AMOUNT)
        {
            discountAmount = coupon.Value;
        }

        discountAmount = Math.Min(discountAmount, cart.SubTotal);

        cart.AppliedCouponCode = coupon.Code;
        cart.DiscountAmount = discountAmount;
        
        Console.WriteLine($"[DEBUG] Tổng tiền giỏ hàng (SubTotal) hiện tại là: {cart.SubTotal}");
        
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}