using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Shop.Application.Features.ProductVariants.Commands.DeleteProductVariant;
using Shop.Application.Features.ProductVariants.Commands.UpdateProductVariant;
using Shop.Application.Features.ProductVariants.Queries;
using Shop.Application.ProductVariants.Commands;
namespace Shop.WebApi.Endpoints;

public class ProductVariantEndpoints : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet("/product/{productId:guid}", GetVariantsByProduct)
            .WithSummary("Lấy danh sách biến thể của 1 sản phẩm");

        groupBuilder.MapPost("/", CreateVariant)
            .WithSummary("Thêm biến thể mới")
            .RequireAuthorization("RequireAdminRole");

        groupBuilder.MapPut("/{id:guid}", UpdateVariant)
            .WithSummary("Cập nhật biến thể")
            .RequireAuthorization("RequireAdminRole");

        groupBuilder.MapDelete("/{id:guid}", DeleteVariant)
            .WithSummary("Xóa biến thể")
            .RequireAuthorization("RequireAdminRole");
    }

    private static async Task<Ok<List<ProductVariantDto>>> GetVariantsByProduct(ISender sender, Guid productId)
    {
        var result = await sender.Send(new GetVariantsByProductIdQuery(productId));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<Guid>> CreateVariant(ISender sender, CreateProductVariantCommand command)
    {
        var result = await sender.Send(command);
        return TypedResults.Ok(result);
    }

    private static async Task<NoContent> UpdateVariant(ISender sender, Guid id, UpdateProductVariantCommand command)
    {
        if (id != command.Id) throw new ValidationException(new[] 
            { new FluentValidation.Results
                .ValidationFailure("Id", "ID trên URL và Body không khớp.") 
            });
        
        await sender.Send(command);
        return TypedResults.NoContent();
    }

    private static async Task<NoContent> DeleteVariant(ISender sender, Guid id)
    {
        await sender.Send(new DeleteProductVariantCommand(id));
        return TypedResults.NoContent();
    }
}