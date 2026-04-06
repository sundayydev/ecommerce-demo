using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Shop.Application.Features.Reviews.Commands.CreateReview;


namespace Shop.WebApi.Endpoints;

public class ReviewEndpoints : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {

        groupBuilder.MapPost("/", CreateReview)
            .WithSummary("Tạo đánh giá mới cho sản phẩm")
            .RequireAuthorization(); 
    }

    private static async Task<Results<Ok<Guid>, UnauthorizedHttpResult>> CreateReview(
        ISender sender, 
        HttpContext context, 
        CreateReviewRequest request)
    {
        var userIdString = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                        ?? context.User.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return TypedResults.Unauthorized();
        }

        var command = new CreateReviewCommand(userId, request.ProductId, request.Rating, request.Comment);
        var result = await sender.Send(command);
        
        return TypedResults.Ok(result);
    }
}


public record CreateReviewRequest(Guid ProductId, int Rating, string Comment);