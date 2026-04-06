using Shop.Application.Common.Interfaces;
using Shop.Domain.Entities;
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
            throw new NotFoundException(nameof(CartItem), request.Id.ToString());
        }

        var newQuantity = cartItem.Quantity + request.Quantity;
        
        if (newQuantity <= 0)
        {
             throw new ValidationException(new[] {
                new FluentValidation.Results.ValidationFailure("Quantity", 
                    "Số lượng sản phẩm trong giỏ hàng không được bé hơn hoặc bằng 0.")
            });
        }

        if (newQuantity > cartItem.ProductVariant.Stock)
        {
            throw new ValidationException(new[] {
                new FluentValidation.Results.ValidationFailure("Quantity", 
                    $"Biến thể này chỉ còn {cartItem.ProductVariant.Stock} cái trong kho (Bạn đang yêu cầu tổng cộng {newQuantity} cái).")
            });
        }

        cartItem.Quantity = newQuantity;

        await _context.SaveChangesAsync(cancellationToken);
    }
}