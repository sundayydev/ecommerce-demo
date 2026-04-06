using Shop.Application.Common.Interfaces;
using Shop.Domain.Constants;
using Shop.Domain.Entities;

// Chứa OrderStatus

namespace Shop.Application.Features.Reviews.Commands.CreateReview;

public record CreateReviewCommand(Guid UserId, Guid ProductId, int Rating, string Comment) : IRequest<Guid>;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateReviewCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        if (request.Rating < 1 || request.Rating > 5)
            throw new ValidationException("Số sao đánh giá phải từ 1 đến 5.");

        var hasBought = await _context.Orders
            .AnyAsync(o => 
                o.UserId == request.UserId && 
                o.Status == OrderStatus.Paid && 
                o.Items.Any(i => i.ProductVariant.ProductId == request.ProductId), 
            cancellationToken);

        if (!hasBought)
            throw new ValidationException("Bạn chỉ được đánh giá sản phẩm sau khi đã mua và nhận hàng thành công.");

        var alreadyReviewed = await _context.Reviews
            .AnyAsync(r => r.UserId == request.UserId && r.ProductId == request.ProductId, cancellationToken);
            
        if (alreadyReviewed)
            throw new ValidationException("Bạn đã đánh giá sản phẩm này rồi. Cảm ơn bạn!");

        var review = new Review
        {
            UserId = request.UserId,
            ProductId = request.ProductId,
            Rating = request.Rating,
            Comment = request.Comment
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync(cancellationToken);

        return review.Id;
    }
}