using Shop.Domain.Common;

namespace Shop.Domain.Entities;

public class ProductImage : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string Url { get; set; } = null!;
    public bool IsMain { get; set; }
}