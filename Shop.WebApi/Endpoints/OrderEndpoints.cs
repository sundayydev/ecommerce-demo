using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Shop.Application.Features.Orders.Commands.Checkout;
using Shop.Application.Features.Orders.Commands;
using Shop.Application.Features.Orders.Queries;
using Shop.WebApi.Extensions;
using Shop.WebApi.Infrastructure;

namespace Shop.WebApi.Endpoints;

public class OrderEndpoints : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost( Checkout, "/checkout")
            .WithSummary("Đặt hàng từ giỏ hàng (Chỉ User)")
            .RequireAuthorization("RequireUserRole");
        
        groupBuilder.MapGet( GetMyOrders, "/")
            .WithSummary("Lấy danh sách đơn hàng của tôi (Chỉ User)")
            .RequireAuthorization("RequireUserRole");
    }

    private static async Task<Ok<Guid>> Checkout(ISender sender, CheckoutCommand command)
    {
        var orderId = await sender.Send(command);
        return TypedResults.Ok(orderId);
    }
    
    private static async Task<Ok<List<OrderDto>>> GetMyOrders(ISender sender)
    {
        var result = await sender.Send(new GetMyOrdersQuery());
        return TypedResults.Ok(result);
    }
}