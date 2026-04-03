using FluentValidation;
using Shop.Domain.Constants;

namespace Shop.Application.Features.Orders.Commands.CreateCoupon;

public class CreateCouponCommandValidator : AbstractValidator<CreateCouponCommand>
{
    public CreateCouponCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Mã giảm giá không được để trống.")
            .MinimumLength(3).WithMessage("Mã giảm giá phải có ít nhất 3 ký tự.");

        RuleFor(x => x.Value)
            .GreaterThan(0).WithMessage("Giá trị giảm phải lớn hơn 0.");

        RuleFor(x => x.Value)
            .LessThanOrEqualTo(100)
            .When(x => x.DiscountType == DiscountType.PERCENTAGE)
            .WithMessage("Giảm giá phần trăm không được vượt quá 100%.");

        RuleFor(x => x.MinOrderValue)
            .GreaterThanOrEqualTo(0).WithMessage("Giá trị đơn hàng tối thiểu không hợp lệ.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).WithMessage("Ngày kết thúc phải lớn hơn ngày bắt đầu.")
            .GreaterThan(DateTime.UtcNow).WithMessage("Ngày kết thúc phải ở tương lai.");

        RuleFor(x => x.UsageLimit)
            .GreaterThan(0).WithMessage("Giới hạn sử dụng phải lớn hơn 0.");
    }
}