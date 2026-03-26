using Shop.Application.Common.Interfaces;

namespace Shop.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IApplicationDbContext _context;
    public CreateProductCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Tên sản phẩm không được để trống.")
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("Tên sản phẩm không được chỉ chứa khoảng trắng.")
            .MaximumLength(200).WithMessage("Tên sản phẩm không được vượt quá 200 ký tự.")
            .MustAsync(BeUniqueName).WithMessage("Sản phẩm với tên '{PropertyValue}' đã tồn tại trong hệ thống.");

        RuleFor(v => v.Price)
            .GreaterThan(0).WithMessage("Giá sản phẩm phải lớn hơn 0 VNĐ.");

        RuleFor(v => v.Description)
            .MaximumLength(2000).WithMessage("Mô tả sản phẩm không được vượt quá 2000 ký tự.");

        RuleFor(v => v.CategoryId)
            .NotEmpty().WithMessage("Vui lòng chọn danh mục cho sản phẩm.")
            .MustAsync(CategoryExists).WithMessage("Danh mục bạn chọn không tồn tại hoặc đã bị xóa.");
    }


    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return !await _context.Products
            .AnyAsync(p => p.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    private async Task<bool> CategoryExists(Guid categoryId, CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AnyAsync(c => c.Id == categoryId, cancellationToken);
    }
}