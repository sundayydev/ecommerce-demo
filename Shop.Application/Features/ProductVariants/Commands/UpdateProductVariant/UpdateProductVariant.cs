using Shop.Application.Common.Interfaces;
using Shop.Domain.Entities;

namespace Shop.Application.Features.ProductVariants.Commands.UpdateProductVariant;

public record UpdateProductVariantCommand(Guid Id, string Size, string Color, decimal? Price, int Stock) : IRequest;

public class UpdateProductVariantCommandHandler : IRequestHandler<UpdateProductVariantCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductVariantCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateProductVariantCommand request, CancellationToken cancellationToken)
    {
        var variant = await _context.ProductVariants.FindAsync(new object[] { request.Id }, cancellationToken);
        
        if (variant == null) throw new NotFoundException(nameof(ProductVariant), request.Id.ToString());

        variant.Size = request.Size;
        variant.Color = variant.Color;
        variant.Price = request.Price ?? 0;
        variant.Stock = request.Stock;

        await _context.SaveChangesAsync(cancellationToken);
    }
}