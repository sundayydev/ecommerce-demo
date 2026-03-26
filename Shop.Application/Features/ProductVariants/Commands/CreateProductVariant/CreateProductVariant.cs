using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop.Application.Common.Exceptions;
using Shop.Application.Common.Interfaces;
using Shop.Domain.Entities;
using NotFoundException = Ardalis.GuardClauses.NotFoundException;

namespace Shop.Application.ProductVariants.Commands;

public record CreateProductVariantCommand(Guid ProductId, string Size, string Color, decimal? Price, int Stock) : IRequest<Guid>;

public class CreateProductVariantCommandHandler : IRequestHandler<CreateProductVariantCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateProductVariantCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateProductVariantCommand request, CancellationToken cancellationToken)
    {
        // Kiểm tra xem Product gốc có tồn tại không
        var productExists = await _context.Products.AnyAsync(p => p.Id == request.ProductId, cancellationToken);
        if (!productExists) throw new NotFoundException(nameof(Product), request.ProductId.ToString());

        var variant = new ProductVariant
        {
            ProductId = request.ProductId,
            Size = request.Size,
            Color = request.Color,
            Price = request.Price ?? 0,
            Stock = request.Stock
        };

        _context.ProductVariants.Add(variant);
        await _context.SaveChangesAsync(cancellationToken);

        return variant.Id;
    }
}