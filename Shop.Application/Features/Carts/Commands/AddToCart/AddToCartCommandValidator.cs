namespace Shop.Application.Features.Carts.Commands.AddToCart;

public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartCommandValidator()
    {
        RuleFor(v => v.ProductVariantId)
            .NotEmpty().WithMessage("Vui lòng chọn sản phẩm.");

        RuleFor(v => v.Quantity)
            .GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0.");
    }
}