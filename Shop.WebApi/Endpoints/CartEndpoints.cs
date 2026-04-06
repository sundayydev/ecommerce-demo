using System.Security.Claims;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.Common.Exceptions;
using Shop.Application.Features.Carts.Commands;
using Shop.Application.Features.Carts.Commands.AddToCart;
using Shop.Application.Features.Carts.Commands.ApplyCoupon;
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
        
        groupBuilder.MapPost("/apply-coupon", ApplyCoupon)
            .WithSummary("Áp dụng mã giảm giá vào giỏ hàng");
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
    
    private static async Task<IResult> ApplyCoupon(ISender sender, HttpContext context, [FromBody] ApplyCouponCommand request)
    {
        var userIdString = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? context.User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Results.Unauthorized();
        }

        var command = new ApplyCouponCommand(userId, request.CouponCode);
        
        var result = await sender.Send(command);

        if (result)
        {
            return Results.Ok(new 
            { 
                Success = true, 
                Message = "Áp dụng mã giảm giá thành công!" 
            });
        }

        return Results.BadRequest(new { Success = false, Message = "Không thể áp dụng mã giảm giá." });
    }
}