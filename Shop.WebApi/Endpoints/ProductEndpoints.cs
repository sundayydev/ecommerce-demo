using Microsoft.AspNetCore.Http.HttpResults;
using Shop.Application.Common.Models;
using Shop.Application.Features.Products.Commands.CreateProduct;
using Shop.Application.Features.Products.Commands.DeleteProduct;
using Shop.Application.Features.Products.Commands.UpdateProduct;
using Shop.Application.Features.Products.Queries;
using Shop.Application.Features.Reviews.Queries;
using Shop.Domain.Entities;
using Shop.WebApi.Extensions;


namespace Shop.WebApi.Endpoints;

public class ProductEndpoints : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet("/", GetProductsWithPagination)
            .WithSummary("Lấy danh sách sản phẩm (Public)");
                
        groupBuilder.MapGet("/{id:guid}", GetProductById)
            .WithSummary("Lấy chi tiết sản phẩm (Public)");

        groupBuilder.MapPost("/", CreateProduct)
            .WithSummary("Thêm sản phẩm (Admin)")
            .RequireAuthorization("RequireAdminRole"); 

        groupBuilder.MapPut("/{id:guid}", UpdateProduct)
            .WithSummary("Sửa sản phẩm (Admin)")
            .RequireAuthorization("RequireAdminRole");

        groupBuilder.MapDelete("/{id:guid}", DeleteProduct)
            .WithSummary("Xóa mềm sản phẩm (Admin)")
            .RequireAuthorization("RequireAdminRole");
        
        groupBuilder.MapGet("{productId:guid}/review/", GetProductReviews)
            .WithSummary("Lấy danh sách đánh giá của sản phẩm (có phân trang)");
    }

    private static async Task<Ok<PaginatedList<ProductDto>>> GetProductsWithPagination(
        ISender sender, 
        [AsParameters] GetProductsWithPaginationQuery query) 
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }
    
    private static async Task<Ok<ProductDto>> GetProductById(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetProductByIdQuery(id));
        return TypedResults.Ok(result); 
    }
    
    private static async Task<Created<Guid>> CreateProduct(ISender sender, CreateProductCommand command)
    {
        var id = await sender.Send(command);

        return TypedResults.Created($"/{nameof(Product)}/{id}", id);
    }

    private static async Task<Results<NoContent, BadRequest>> UpdateProduct(ISender sender, Guid id, UpdateProductCommand command)
    {
        if (id != command.Id)
            return TypedResults.BadRequest();

        await sender.Send(command);
        return TypedResults.NoContent();
    }
    
    public static async Task<NoContent> DeleteProduct(ISender sender, Guid id)
    {
        await sender.Send(new DeleteProductCommand(id));
    
        return TypedResults.NoContent();
    }
    
    private static async Task<Ok<ReviewSummaryResponse>> GetProductReviews(
        ISender sender, 
        Guid productId, 
        int pageNumber = 1, 
        int pageSize = 10)
    {
        var result = await sender.Send(new GetProductReviewsQuery(productId, pageNumber, pageSize));
        return TypedResults.Ok(result);
    }
}