using Microsoft.Extensions.Caching.Distributed;
using Shop.Application.Common.Interfaces;

namespace Shop.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IDistributedCache _cache; 

    public LoginCommandHandler(
        IApplicationDbContext context, 
        IPasswordHasher passwordHasher, 
        ITokenService tokenService, 
        IDistributedCache cache)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _cache = cache;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Email hoặc mật khẩu không chính xác.");
        }

        var accessToken = _tokenService.CreateAccessToken(user.Id, user.Email, user.Role);
        var refreshToken = _tokenService.CreateRefreshToken();

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
        };
        await _cache.SetStringAsync($"RefreshToken:{refreshToken}", user.Id.ToString(), cacheOptions, cancellationToken);

        return new AuthResponse(user.Id, accessToken, refreshToken);
    }
}