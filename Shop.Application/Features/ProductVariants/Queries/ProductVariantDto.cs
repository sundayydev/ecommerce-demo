namespace Shop.Application.Features.ProductVariants.Queries;

public record ProductVariantDto(
    Guid Id,
    Guid ProductId,
    string Name,
    string Size,
    string Color,
    decimal? Price,
    int Stock
);