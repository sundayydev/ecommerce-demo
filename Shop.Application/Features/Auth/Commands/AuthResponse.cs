namespace Shop.Application.Features.Auth.Commands;

public record AuthResponse(
    Guid UserId, 
    string AccessToken, 
    string RefreshToken
);