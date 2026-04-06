using Shop.Application.Common.Interfaces;
using Shop.Application.Common.Models;

namespace Shop.Application.Features.Reviews.Queries;
public record ReviewDto(Guid Id, string UserName, int Rating, string Comment, DateTimeOffset Created);

public record ReviewSummaryResponse(
    double AverageRating, 
    PaginatedList<ReviewDto> Reviews 
);

public record GetProductReviewsQuery(Guid ProductId, int PageNumber = 1, int PageSize = 10) : IRequest<ReviewSummaryResponse>;
public class GetProductReviewsQueryHandler : IRequestHandler<GetProductReviewsQuery, ReviewSummaryResponse>{
    private readonly IApplicationDbContext _context;

    public GetProductReviewsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ReviewSummaryResponse> Handle(GetProductReviewsQuery request, CancellationToken cancellationToken)
    {
        var baseQuery = _context.Reviews
            .Include(r => r.User)
            .Where(r => r.ProductId == request.ProductId)
            .AsNoTracking();

        double averageRating = 0;
        if (await baseQuery.AnyAsync(cancellationToken))
        {
            averageRating = await baseQuery.AverageAsync(r => (double)r.Rating, cancellationToken);
            averageRating = Math.Round(averageRating, 1);
        }

        var projectedQuery = baseQuery
            .OrderByDescending(r => r.Created)
            .Select(r => new ReviewDto(
                r.Id,
                r.User.FullName ?? "Người dùng ẩn danh",
                r.Rating,
                r.Comment ?? "",
                r.Created
            ));

        var paginatedReviews = await PaginatedList<ReviewDto>.CreateAsync(
            projectedQuery, 
            request.PageNumber, 
            request.PageSize);

        return new ReviewSummaryResponse(averageRating, paginatedReviews);
    }
}