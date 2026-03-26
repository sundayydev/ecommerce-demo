using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shop.Application.Common.Interfaces;
using Shop.Application.Features.Products.Commands.UpdateProduct;

namespace Shop.Application.Products.Commands;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("ID sản phẩm không được để trống.");

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Tên sản phẩm không được để trống.")
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


    private async Task<bool> BeUniqueName(UpdateProductCommand model, string name, CancellationToken cancellationToken)
    {
        return !await _context.Products
            .AnyAsync(p => p.Id != model.Id && p.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    private async Task<bool> CategoryExists(Guid categoryId, CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AnyAsync(c => c.Id == categoryId, cancellationToken);
    }
}