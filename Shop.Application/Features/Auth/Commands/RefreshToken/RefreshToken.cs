using Microsoft.Extensions.Caching.Distributed;
using Shop.Application.Common.Interfaces;

namespace Shop.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponse>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IDistributedCache _cache; // Dùng Redis

    public RefreshTokenCommandHandler(IApplicationDbContext context, ITokenService tokenService, IDistributedCache cache)
    {
        _context = context;
        _tokenService = tokenService;
        _cache = cache;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Kiểm tra Redis xem Refresh Token này có hợp lệ không
        var userIdString = await _cache.GetStringAsync($"RefreshToken:{request.RefreshToken}", cancellationToken);

        if (string.IsNullOrEmpty(userIdString))
        {
            throw new UnauthorizedAccessException("Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.");
        }

        var userId = Guid.Parse(userIdString);

        // Lấy thông tin User từ DB để đóng gói vào Token mới
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null) 
            throw new UnauthorizedAccessException("Người dùng không tồn tại.");

        // Xóa Token cũ khỏi Redis (Mỗi Refresh Token chỉ được xài 1 lần -> Chống hacker trộm token xài nhiều lần)
        await _cache.RemoveAsync($"RefreshToken:{request.RefreshToken}", cancellationToken);

        // Tạo cặp Token MỚI
        var newAccessToken = _tokenService.CreateAccessToken(user.Id, user.Email, user.Role);
        var newRefreshToken = _tokenService.CreateRefreshToken();

        // Lưu Token MỚI vào Redis (Gia hạn thêm 7 ngày)
        var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7) };
        await _cache.SetStringAsync($"RefreshToken:{newRefreshToken}", user.Id.ToString(), cacheOptions, cancellationToken);

        // Trả về cho Endpoint xử lý tiếp
        return new AuthResponse(user.Id, newAccessToken, newRefreshToken);
    }
}