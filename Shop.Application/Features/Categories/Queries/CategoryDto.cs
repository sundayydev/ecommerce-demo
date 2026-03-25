namespace Shop.Application.Features.Categories.Queries;

public record CategoryDto(Guid Id, string Name, string Slug, Guid? ParentId);