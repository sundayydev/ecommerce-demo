using Shop.Domain.Common;

namespace Shop.Domain.Entities;

public class Category : BaseAuditableEntity
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;

    public int? ParentId { get; set; }
    public Category? Parent { get; set; }

    public ICollection<Category> Children { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}