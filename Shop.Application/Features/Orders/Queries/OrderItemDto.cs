namespace Shop.Application.Features.Orders.Queries;

public record OrderItemDto(    
    Guid Id,
    string ProductName,
    string VariantName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice
    );