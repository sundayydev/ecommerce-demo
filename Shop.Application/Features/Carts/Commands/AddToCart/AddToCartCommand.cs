using Shop.Application.Common.Interfaces;
using Shop.Domain.Entities;

namespace Shop.Application.Features.Carts.Commands.AddToCart;
public record AddToCartCommand(Guid ProductVariantId, int Quantity) : IRequest<Guid>;
public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Guid> 
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    
    public AddToCartCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }
    public async Task<Guid> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException("Cần đăng nhập.");

        var variant = await _context.ProductVariants
            .Include(v => v.Product)
            .FirstOrDefaultAsync(v => v.Id == request.ProductVariantId, cancellationToken);

        if (variant == null)
        {
            throw new NotFoundException(nameof(ProductVariant), request.ProductVariantId.ToString());
        }

        var cart = await _context.Carts
                       .Include(c => c.Items)
                       .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken) 
                   ?? new Domain.Entities.Cart() { UserId = userId };

        if (cart.Id == Guid.Empty) _context.Carts.Add(cart);

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductVariantId == request.ProductVariantId);
        var currentQuantity = existingItem?.Quantity ?? 0;
    
        if (currentQuantity + request.Quantity > variant.Stock)
        {
            throw new ValidationException(new[] {
                new FluentValidation.Results.ValidationFailure("Quantity", 
                    $"Sản phẩm '{variant.Product.Name} - {variant.Size} - {variant.Color}' chỉ còn {variant.Stock} cái.")
            });
        }

        if (existingItem != null)
        {
            existingItem.Quantity += request.Quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductVariantId = request.ProductVariantId,
                Quantity = request.Quantity
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        return cart.Id;
    }
}