using MediatR;
using Shop.Application.Common.Interfaces;
using Shop.Domain.Constants;
using Shop.Domain.Entities;

namespace Shop.Application.Features.Orders.Commands.CreateCoupon;

public record CreateCouponCommand(
    string Code,
    string DiscountType,
    decimal Value,
    decimal MinOrderValue,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    int UsageLimit
) : IRequest<Guid>; 

public class CreateCouponCommandHandler : IRequestHandler<CreateCouponCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateCouponCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
    {
        var normalizedCode = request.Code.Trim().ToUpper();

        var exists = await _context.Coupons
            .AnyAsync(c => c.Code == normalizedCode, cancellationToken);

        if (exists)
        {
            throw new Exception($"Mã giảm giá '{normalizedCode}' đã tồn tại trong hệ thống.");
        }

        var coupon = new Coupon
        {
            Code = normalizedCode,
            DiscountType = request.DiscountType,
            Value = request.Value,
            MinOrderValue = request.MinOrderValue,
            StartDate = request.StartDate.ToUniversalTime(),
            EndDate = request.EndDate.ToUniversalTime(),
            UsageLimit = request.UsageLimit,
            UsedCount = 0,
            IsActive = true
        };

        _context.Coupons.Add(coupon);
        await _context.SaveChangesAsync(cancellationToken);

        return coupon.Id;
    }
}