using FluentValidation;
using Shop.Application.Common.Interfaces;
using Shop.Domain.Interfaces;

namespace Shop.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateCategoryCommandValidator(IApplicationDbContext context)
    {
        _context = context;
        
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Tên danh mục không được để trống.")
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Tên danh mục không được chỉ chứa khoảng trắng.")
            .MaximumLength(200).WithMessage("Tên danh mục không được vượt quá 200 ký tự.")
            .MustAsync(BeUniqueName)
            .WithMessage("Danh mục với tên '{PropertyValue}' đã tồn tại.")
            .WithErrorCode("Unique");
        
        RuleFor(v => v.Slug)
            .MaximumLength(200).WithMessage("Slug không được vượt quá 200 ký tự.");
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        var isExisted = await _context.Categories.AnyAsync(c => c.Name == name);
        return !isExisted;
    }
}