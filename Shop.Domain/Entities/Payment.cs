using Shop.Domain.Common;

namespace Shop.Domain.Entities;

public class Payment : BaseAuditableEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public string? TransactionId { get; set; }
    public string Method { get; set; } = null!; 
    public string Status { get; set; } = null!;
    public string? Note { get; set; }
}