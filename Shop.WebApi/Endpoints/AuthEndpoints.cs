using Microsoft.AspNetCore.Http.HttpResults;
using Shop.Application.Features.Auth.Commands;
using Shop.Application.Features.Auth.Commands.Login;
using Shop.Application.Features.Auth.Commands.RefreshToken;
using Shop.Application.Features.Auth.Commands.Register;
using Shop.WebApi.Extensions;

namespace Shop.WebApi.Endpoints;

public class AuthEndpoints : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(Register,"/register").WithSummary("Đăng ký tài khoản mới");
        groupBuilder.MapPost(Login,"/login").WithSummary("Đăng nhập và nhận Token");
        groupBuilder.MapPost(RefreshToken, "/refresh-token")
            .WithSummary("Làm mới Token (Tự động đọc từ Cookie)");
    }

    public static async Task<Ok<AuthResponse>> Register(ISender sender, HttpContext httpContext, RegisterCommand command)
    {
        var result = await sender.Send(command);
        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, 
            Secure = false,   
            SameSite = SameSiteMode.Strict, 
            Expires = DateTime.UtcNow.AddDays(7)
        };

        httpContext.Response.Cookies.Append("AccessToken", result.AccessToken, cookieOptions);
        httpContext.Response.Cookies.Append("RefreshToken", result.RefreshToken, cookieOptions);
        return TypedResults.Ok(result);
    }

    public static async Task<Ok<AuthResponse>> Login(ISender sender, HttpContext httpContext, LoginCommand command)
    {
        var result = await sender.Send(command);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, 
            Secure = false,   
            SameSite = SameSiteMode.Strict, 
            Expires = DateTime.UtcNow.AddDays(7)
        };

        httpContext.Response.Cookies.Append("AccessToken", result.AccessToken, cookieOptions);
        httpContext.Response.Cookies.Append("RefreshToken", result.RefreshToken, cookieOptions);

        return TypedResults.Ok(result);
    }
    
    public static async Task<IResult> RefreshToken(ISender sender, HttpContext httpContext)
    {
        var refreshToken = httpContext.Request.Cookies["RefreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return TypedResults.Unauthorized(); 
        }

        var result = await sender.Send(new RefreshTokenCommand(refreshToken));

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, 
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        httpContext.Response.Cookies.Append("AccessToken", result.AccessToken, cookieOptions);
        httpContext.Response.Cookies.Append("RefreshToken", result.RefreshToken, cookieOptions);

        return TypedResults.Ok(new { Message = "Gia hạn Token thành công" });
    }
}

