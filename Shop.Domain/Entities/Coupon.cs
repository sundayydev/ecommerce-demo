
namespace Shop.Domain.Entities;

public class Coupon
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string DiscountType { get; set; }
    public decimal Value { get; set; }
    public decimal MinOrderValue { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public int UsageLimit { get; set; }
    public int UsedCount { get; set; } 
    public bool IsActive { get; set; } = true;
    
    public bool IsValid()
    {
        var now = DateTimeOffset.UtcNow;
        return IsActive && 
               now >= StartDate && 
               now <= EndDate && 
               UsedCount < UsageLimit;
    }
}