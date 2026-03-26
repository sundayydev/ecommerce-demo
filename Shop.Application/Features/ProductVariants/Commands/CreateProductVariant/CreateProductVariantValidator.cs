using Shop.Application.ProductVariants.Commands;

namespace Shop.Application.Features.ProductVariants.Commands.CreateProductVariant;

public class CreateProductVariantCommandValidator : AbstractValidator<CreateProductVariantCommand>
{
    public CreateProductVariantCommandValidator()
    {
        RuleFor(v => v.ProductId)
            .NotEmpty()
            .WithMessage("Bắt buộc phải chọn Sản phẩm gốc.");
        
        RuleFor(v => v.Size)
            .NotEmpty()
            .MaximumLength(20)
            .WithMessage("Size không hợp lệ.");
        
        RuleFor(v => v.Color)
            .NotEmpty()
            .MaximumLength(30)
            .WithMessage("Color không hợp lệ.");
        
        RuleFor(v => v.Price)
            .GreaterThan(0)
            .When(v => v.Price.HasValue)
            .WithMessage("Giá phải lớn hơn 0.");
        
        RuleFor(v => v.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Tồn kho không được âm.");
    }
}