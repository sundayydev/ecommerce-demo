using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop.Application.Common.Interfaces;
using Shop.Application.Features.Orders.Events;

namespace Shop.Application.Users.EventHandlers;

// Implements INotificationHandler để lắng nghe OrderCompletedEvent
public class LoyaltyPointsEarnedEventHandler : INotificationHandler<OrderCompletedEvent>
{
    private readonly IApplicationDbContext _context;

    public LoyaltyPointsEarnedEventHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(OrderCompletedEvent notification, CancellationToken cancellationToken)
    {
        // 1. Tìm User trong Database
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == notification.UserId, cancellationToken);
        
        if (user == null) return;

        // 2. Tính điểm: 100,000 VNĐ = 1 điểm
        int pointsEarned = (int)(notification.TotalAmount / 100000);

        if (pointsEarned > 0)
        {
            // 3. Cộng điểm
            user.TotalPoints += pointsEarned;

            // 4. Xét duyệt lên hạng (Ranking)
            if (user.TotalPoints >= 5000) 
                user.Rank = "Diamond";
            else if (user.TotalPoints >= 2000) 
                user.Rank = "Gold";
            else if (user.TotalPoints >= 500) 
                user.Rank = "Silver";

            // 5. Lưu xuống DB
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}