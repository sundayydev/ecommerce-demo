using Shop.Application.Auth.Commands.Login;

namespace Shop.Application.Features.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(v => v.Email)
            .NotEmpty().WithMessage("Vui lòng nhập Email.")
            .EmailAddress().WithMessage("Email không đúng định dạng.");

        RuleFor(v => v.Password)
            .NotEmpty().WithMessage("Vui lòng nhập Mật khẩu.");
    }
}