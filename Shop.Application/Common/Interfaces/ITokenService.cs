namespace Shop.Application.Common.Interfaces;

public interface ITokenService
{
    string CreateAccessToken(Guid userId, string email, string role); 
    string CreateRefreshToken();
}