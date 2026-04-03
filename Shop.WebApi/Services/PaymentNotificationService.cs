using Microsoft.AspNetCore.SignalR;
using Shop.Application.Common.Interfaces;
using Shop.WebApi.Hubs;

namespace Shop.WebApi.Services; 

public class PaymentNotificationService : IPaymentNotificationService
{
    private readonly IHubContext<PaymentHub> _hubContext;

    public PaymentNotificationService(IHubContext<PaymentHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendPaymentSuccessNotificationAsync(Guid orderId, decimal amount, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(orderId.ToString())
            .SendAsync("PaymentSuccess", new 
            { 
                OrderId = orderId,
                Message = "Thanh toán thành công!",
                Amount = amount
            }, cancellationToken);
    }
}