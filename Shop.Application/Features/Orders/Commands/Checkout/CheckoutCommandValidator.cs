namespace Shop.Application.Features.Orders.Commands.Checkout;



// 2. VALIDATOR
public class CheckoutCommandValidator : AbstractValidator<CheckoutCommand>
{
    public CheckoutCommandValidator()
    {
        // Validate cơ bản ở Application Layer (Mặc dù Domain đã có kiểm tra, nhưng bắt sớm ở đây để trả về mã 400 đẹp hơn)
        RuleFor(v => v.Detail).NotEmpty().WithMessage("Vui lòng nhập địa chỉ chi tiết (số nhà, đường).");
        RuleFor(v => v.Ward).NotEmpty().WithMessage("Vui lòng nhập Xã/Phường.");
        RuleFor(v => v.City).NotEmpty().WithMessage("Vui lòng nhập Tỉnh/Thành phố.");
    }
}