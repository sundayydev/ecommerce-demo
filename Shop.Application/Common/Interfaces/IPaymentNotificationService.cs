namespace Shop.Application.Common.Interfaces;

public interface IPaymentNotificationService
{
    Task SendPaymentSuccessNotificationAsync(Guid orderId, decimal amount, CancellationToken cancellationToken = default);
}