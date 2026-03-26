using Shop.Application.Common.Interfaces;
using Shop.Domain.Entities;

namespace Shop.Application.Features.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, product);

        product.IsDeleted = true;

        await _context.SaveChangesAsync(cancellationToken);
    }
}