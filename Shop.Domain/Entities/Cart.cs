using Shop.Domain.Common;

namespace Shop.Domain.Entities;

public class Cart : BaseAuditableEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}