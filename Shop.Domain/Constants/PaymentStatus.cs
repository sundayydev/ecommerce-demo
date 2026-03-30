namespace Shop.Domain.Constants;

public static class PaymentStatus
{
    public const string Pending = nameof(Pending);
    public const string Success = nameof(Success);
    public const string OverPaid = nameof(OverPaid);
    public const string PartialPaid = nameof(PartialPaid);
    
    public const string Failed = nameof(Failed);
    public const string Refunded = nameof(Refunded);
}