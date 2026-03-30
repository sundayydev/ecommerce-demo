namespace Shop.Domain.Constants;

public static class OrderStatus
{
    public const string Pending = nameof(Pending);
    public const string Paid = nameof(Paid);
    public const string PartiallyPaid =  nameof(PartiallyPaid);
    public const string Shipped = nameof(Shipped);
    public const string Completed = nameof(Completed);
    public const string Cancelled = nameof(Cancelled);
}