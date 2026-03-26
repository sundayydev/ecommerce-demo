using Microsoft.AspNetCore.Http.HttpResults;
using Shop.Application.Features.Auth.Commands;
using Shop.WebApi.Extensions;

namespace Shop.WebApi.Endpoints;

public class AuthEndpoints : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(Register,"/register").WithSummary("Đăng ký tài khoản mới");
        groupBuilder.MapPost(Login,"/login").WithSummary("Đăng nhập và nhận Token");
    }

    public static async Task<Ok<Guid>> Register(ISender sender, RegisterCommand command)
    {
        var result = await sender.Send(command);
        return TypedResults.Ok(result);
    }

    public static async Task<Ok<string>> Login(ISender sender, LoginCommand command)
    {
        var token = await sender.Send(command);
        return TypedResults.Ok(token);
    }
}

