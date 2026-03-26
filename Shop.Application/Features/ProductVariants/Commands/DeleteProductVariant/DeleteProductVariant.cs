using Shop.Application.Common.Interfaces;
using Shop.Domain.Entities;

namespace Shop.Application.Features.ProductVariants.Commands.DeleteProductVariant;

public record DeleteProductVariantCommand(Guid Id) : IRequest;

public class DeleteProductVariantCommandHandler : IRequestHandler<DeleteProductVariantCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductVariantCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteProductVariantCommand request, CancellationToken cancellationToken)
    {
        var variant = await _context.ProductVariants.FindAsync(new object[] { request.Id }, cancellationToken);
        
        if (variant == null) throw new NotFoundException(nameof(ProductVariant), request.Id.ToString());

        _context.ProductVariants.Remove(variant);
        await _context.SaveChangesAsync(cancellationToken);
    }
}