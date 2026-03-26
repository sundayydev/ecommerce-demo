using Shop.Application.Features.Carts.Commands.UpdateCartItemQuantity;

namespace Shop.Application.Carts.Commands;

public class UpdateCartItemQuantityCommandValidator : AbstractValidator<UpdateCartItemQuantityCommand>
{
    public UpdateCartItemQuantityCommandValidator()
    {
        // Ép số lượng phải > 0. (Nếu muốn xóa thì dùng API DELETE riêng)
        //RuleFor(v => v.Quantity).GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0.");
    }
}