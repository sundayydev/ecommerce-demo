using Shop.Domain.Common;

namespace Shop.Domain.Entities;

public class ProductVariant : BaseAuditableEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string Size { get; set; } = null!;
    public string Color { get; set; } = null!;

    public decimal  Price { get; set; }
    public int Stock { get; set; }
}