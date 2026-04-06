namespace Shop.Application.Features.Orders.Events;

public record OrderCompletedEvent(Guid UserId, decimal TotalAmount) : INotification;