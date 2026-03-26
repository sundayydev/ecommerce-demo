using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Shop.Application.Common.Interfaces;

namespace Shop.WebApi.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                return userId;
            }
            return null;
        }
    }
}