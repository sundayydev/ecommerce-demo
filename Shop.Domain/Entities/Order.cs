using Shop.Domain.Common;
using Shop.Domain.ValueObject;

namespace Shop.Domain.Entities;

public class Order : BaseAuditableEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public Address ShippingAddress { get; set; } = null!;
    public Decimal TotalAmount { get; set; }

    public string Status { get; set; } = "Pending";

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}