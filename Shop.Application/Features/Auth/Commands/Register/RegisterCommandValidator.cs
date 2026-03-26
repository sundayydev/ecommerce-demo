using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shop.Application.Common.Interfaces;
using Shop.Application.Features.Auth.Commands;

namespace Shop.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private readonly IApplicationDbContext _context;

    public RegisterCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.FullName)
            .NotEmpty().WithMessage("Họ tên không được để trống.")
            .MaximumLength(100).WithMessage("Họ tên không được vượt quá 100 ký tự.");

        RuleFor(v => v.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Email không đúng định dạng.") // Tự động check format a@b.com
            .MaximumLength(100).WithMessage("Email không được vượt quá 100 ký tự.")
            .MustAsync(BeUniqueEmail).WithMessage("Email '{PropertyValue}' đã được sử dụng. Vui lòng chọn email khác.");

        RuleFor(v => v.Password)
            .NotEmpty().WithMessage("Mật khẩu không được để trống.")
            .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự.")
            .Matches("[A-Z]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ in hoa.")
            .Matches("[a-z]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ thường.")
            .Matches("[0-9]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ số.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt (VD: @, #, $, !).");
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return !await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }
}