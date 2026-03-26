using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc; 
using Shop.Application.Features.Categories.Commands.CreateCategory;
using Shop.Application.Features.Categories.Commands.DeleteCategory;
using Shop.Application.Features.Categories.Commands.UpdateCategory;
using Shop.Application.Features.Categories.Queries;
using Shop.Domain.Entities;
using Shop.WebApi.Extensions;
using Shop.WebApi.Infrastructure;

namespace Shop.WebApi.Endpoints;

public class CategoryEndpoints : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CreateCategory);
        groupBuilder.MapGet(GetCategories);
        groupBuilder.MapGet(GetCategoryById, "{id}");
        groupBuilder.MapPut(UpdateCategory, "{id}");
        groupBuilder.MapDelete(DeleteCategory, "{id}");

    }
    
    [EndpointSummary("Tạo mới danh mục")]
    [EndpointDescription("")]
    private static async Task<Created<Guid>> CreateCategory(ISender sender, CreateCategoryCommand command)
    {
        var id = await sender.Send(command);

        return TypedResults.Created($"/{nameof(Category)}/{id}", id);
    }
    
    [EndpointSummary("Lấy tất danh sách danh mục")]
    private static async Task<Ok<List<CategoryDto>>> GetCategories(ISender sender)
    {
        var result = await sender.Send(new GetCategoriesQuery());
        return TypedResults.Ok(result); 
    }
    
    [EndpointSummary("Lấy danh mục theo Id")]
    private static async Task<Ok<CategoryDto>> GetCategoryById(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetCategoryByIdQuery(id));
        return TypedResults.Ok(result); 
    }

    [EndpointSummary("Cập nhập danh mục")]
    private static async Task<Results<NoContent, BadRequest>> UpdateCategory(ISender sender, Guid id,
        UpdateCategoryCommand command)
    {
        if (id != command.Id)
            return TypedResults.BadRequest();

        await sender.Send(command);
        return TypedResults.NoContent();

    }
    
    [EndpointSummary("Xoá 1 danh mục")]
    private static async Task<NoContent> DeleteCategory(ISender sender, Guid id)
    {
        await sender.Send(new DeleteCategoryCommand(id));
        return TypedResults.NoContent();
    }
}