using Microsoft.Extensions.Caching.Distributed;
using Shop.Application.Common.Exceptions;
using Shop.Application.Common.Interfaces;
using Shop.Domain.Constants;
using Shop.Domain.Entities;

namespace Shop.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string Email, string Password, string FullName) : IRequest<AuthResponse>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IDistributedCache _cache;
    public RegisterCommandHandler(IApplicationDbContext context, ITokenService tokenService, IDistributedCache cache)
    {
        _context = context;
        _tokenService = tokenService;
        _cache = cache;
    }
    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
        {
            throw new ConflictException($"Email '{request.Email}' đã được sử dụng. Vui lòng chọn email khác.");
        }
        
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        
        var user = new User()
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            FullName = request.FullName,
            Role = Roles.User,
            Rank = ""
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        
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