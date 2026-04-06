using Microsoft.AspNetCore.Http.HttpResults;
using Shop.Application.Features.Categories.Commands.CreateCategory;
using Shop.Application.Features.Categories.Commands.DeleteCategory;
using Shop.Application.Features.Categories.Commands.UpdateCategory;
using Shop.Application.Features.Categories.Queries;
using Shop.Domain.Entities;
using Shop.WebApi.Extensions;

namespace Shop.WebApi.Endpoints;

public class CategoryEndpoints : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CreateCategory).WithSummary("Tạo mới danh mục");
        groupBuilder.MapGet(GetCategories).WithSummary("Lấy tất danh sách danh mục");
        groupBuilder.MapGet(GetCategoryById, "{id}").WithSummary("Lấy danh mục theo Id");
        groupBuilder.MapPut(UpdateCategory, "{id}").WithSummary("Cập nhập danh mục");
        groupBuilder.MapDelete(DeleteCategory, "{id}").WithSummary("Xoá 1 danh mục");

    }
    
    private static async Task<Created<Guid>> CreateCategory(ISender sender, CreateCategoryCommand command)
    {
        var id = await sender.Send(command);

        return TypedResults.Created($"/{nameof(Category)}/{id}", id);
    }
    
    private static async Task<Ok<List<CategoryDto>>> GetCategories(ISender sender)
    {
        var result = await sender.Send(new GetCategoriesQuery());
        return TypedResults.Ok(result); 
    }
    
    private static async Task<Ok<CategoryDto>> GetCategoryById(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetCategoryByIdQuery(id));
        return TypedResults.Ok(result); 
    }

    private static async Task<Results<NoContent, BadRequest>> UpdateCategory(ISender sender, Guid id,
        UpdateCategoryCommand command)
    {
        if (id != command.Id)
            return TypedResults.BadRequest();

        await sender.Send(command);
        return TypedResults.NoContent();

    }
    
    private static async Task<NoContent> DeleteCategory(ISender sender, Guid id)
    {
        await sender.Send(new DeleteCategoryCommand(id));
        return TypedResults.NoContent();
    }
}