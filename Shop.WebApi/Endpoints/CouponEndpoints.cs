using Shop.Application.Features.Orders.Commands.CreateCoupon;
using Shop.Domain.Entities;

namespace Shop.WebApi.Endpoints;

public class CouponEndpoints : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost("/", CreateCoupon)
            .RequireAuthorization("RequireAdminRole")
            .WithSummary("Tạo mã giảm giá mới");
    }
    
    private static async Task<IResult> CreateCoupon(ISender sender, CreateCouponCommand command)
    {
        var couponId = await sender.Send(command);
        return TypedResults.Created($"/{nameof(Category)}/{couponId}", couponId);
    }
}