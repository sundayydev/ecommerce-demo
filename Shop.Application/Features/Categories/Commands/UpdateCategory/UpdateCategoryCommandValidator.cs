using Shop.Application.Common.Interfaces;

namespace Shop.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateCategoryCommandValidator(IApplicationDbContext context)
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
        var isExisted = await _context.Categories
            .AnyAsync(c => c.Name == name, cancellationToken);
        return !isExisted;
    }
}