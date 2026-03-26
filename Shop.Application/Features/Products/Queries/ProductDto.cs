namespace Shop.Application.Features.Products.Queries;

public record ProductDto
(
    Guid Id, 
    string Name, 
    string Slug, 
    string? Description,
    decimal Price, 
    Guid CategoryId, 
    string CategoryName
);