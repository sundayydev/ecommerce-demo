namespace Shop.Application.Features.Carts.Queries;

public record CartItemDto(
    Guid Id, 
    Guid ProductVariantId, 
    string ProductName, 
    string Size,
    string Color,
    decimal Price, 
    int Quantity, 
    decimal TotalPrice
);
