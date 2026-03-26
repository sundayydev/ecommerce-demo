namespace Shop.Application.Features.Carts.Queries;

public record CartDto(Guid Id, List<CartItemDto> Items, decimal TotalCartPrice);