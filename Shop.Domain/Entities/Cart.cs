using Shop.Domain.Common;

namespace Shop.Domain.Entities;

public class Cart : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string? AppliedCouponCode { get; set; }
    public decimal DiscountAmount { get; set; }

    public decimal SubTotal => Items?
        .Where(i => true) 
        .Sum(i => 
            (i.ProductVariant.Price ?? i.ProductVariant.Product?.Price ?? 0) * i.Quantity
        ) ?? 0;
    public decimal FinalTotal => Math.Max(0, SubTotal - DiscountAmount);
    
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}