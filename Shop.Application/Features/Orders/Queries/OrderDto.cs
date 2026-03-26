namespace Shop.Application.Features.Orders.Queries;

public record OrderDto
(
    Guid Id,
    DateTimeOffset OrderDate,
    string Status,
    decimal TotalAmount,
    string ShippingAddress,
    List<OrderItemDto> Items
    );