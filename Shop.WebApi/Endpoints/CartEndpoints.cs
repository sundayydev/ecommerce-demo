using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Shop.Application.Common.Exceptions;
using Shop.Application.Features.Carts.Commands;
using Shop.Application.Features.Carts.Commands.AddToCart;
using Shop.Application.Features.Carts.Commands.UpdateCartItemQuantity;
using Shop.Application.Features.Carts.Queries;
using Shop.Domain.Entities;

namespace Shop.WebApi.Endpoints;

public class CartEndpoints : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost("/add", AddToCart)
            .WithSummary("Thêm sản phẩm (Biến thể) vào giỏ hàng (User)")
            .RequireAuthorization("RequireUserRole"); 
        
        groupBuilder.MapGet("/", GetCart)
            .WithSummary("Xem giỏ hàng của tôi")
            .RequireAuthorization("RequireUserRole");

        groupBuilder.MapPut("/items/{id:guid}", UpdateItemQuantity)
            .WithSummary("Cập nhật số lượng món hàng (User)")
            .RequireAuthorization("RequireUserRole");
    }
    
    private static async Task<Ok<Guid>> AddToCart(ISender sender, AddToCartCommand command)
    {
        var cartId = await sender.Send(command);
        return TypedResults.Ok(cartId);
    }
    
    public static async Task<Ok<CartDto>> GetCart(ISender sender)
    {
        var result = await sender.Send(new GetCartQuery());
        return TypedResults.Ok(result);
    }

    public static async Task<NoContent> UpdateItemQuantity(ISender sender, Guid id, UpdateCartItemQuantityCommand command)
    {
        if (id != command.Id) 
        {
            throw new ValidationException(new[] { 
                new ValidationFailure("Id", "ID không khớp.") 
            });
        }

        await sender.Send(command);
        return TypedResults.NoContent();
    }
}