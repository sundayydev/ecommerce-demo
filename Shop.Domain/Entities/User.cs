using Shop.Domain.Common;
using Shop.Domain.Constants;

namespace Shop.Domain.Entities;

public class User : BaseAuditableEntity
{
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FullName { get; set; } = null!;

    public string Role { get; set; } = null!;
}