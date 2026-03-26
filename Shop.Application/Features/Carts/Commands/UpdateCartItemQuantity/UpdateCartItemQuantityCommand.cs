using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop.Application.Common.Exceptions; 
using Shop.Application.Common.Interfaces;
using NotFoundException = Ardalis.GuardClauses.NotFoundException;
using ValidationException = FluentValidation.ValidationException;

namespace Shop.Application.Features.Carts.Commands.UpdateCartItemQuantity;

public record UpdateCartItemQuantityCommand(Guid Id, int Quantity) : IRequest;

public class UpdateCartItemQuantityCommandHandler : IRequestHandler<UpdateCartItemQuantityCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCartItemQuantityCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdateCartItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("Cần đăng nhập.");
        
        var cartItem = await _context.CartItems
            .Include(i => i.Cart)
            .Include(i => i.ProductVariant)
            .FirstOrDefaultAsync(i => i.Id == request.Id && i.Cart.UserId == userId, cancellationToken);

        if (cartItem == null)
        {
            throw new NotFoundException(nameof(Shop.Domain.Entities.CartItem), request.Id.ToString());
        }

        // 1. TÍNH TOÁN SỐ LƯỢNG MỚI TRƯỚC
        var newQuantity = cartItem.Quantity + request.Quantity;

        // 2. Kiểm tra số lượng hợp lệ (Bé hơn hoặc bằng 0 thì báo lỗi)
        // Dùng luôn ValidationException để hệ thống của chúng ta trả về mã 400 Bad Request đẹp mắt
        if (newQuantity <= 0)
        {
             throw new ValidationException(new[] {
                new FluentValidation.Results.ValidationFailure("Quantity", 
                    "Số lượng sản phẩm trong giỏ hàng không được bé hơn hoặc bằng 0.")
            });
        }

        // 3. Kiểm tra tồn kho (Lấy TỔNG SỐ LƯỢNG MỚI so sánh với Tồn kho)
        if (newQuantity > cartItem.ProductVariant.Stock)
        {
            throw new ValidationException(new[] {
                new FluentValidation.Results.ValidationFailure("Quantity", 
                    $"Biến thể này chỉ còn {cartItem.ProductVariant.Stock} cái trong kho (Bạn đang yêu cầu tổng cộng {newQuantity} cái).")
            });
        }

        // 4. Gán lại số lượng mới và lưu Database
        cartItem.Quantity = newQuantity;

        await _context.SaveChangesAsync(cancellationToken);
    }
}