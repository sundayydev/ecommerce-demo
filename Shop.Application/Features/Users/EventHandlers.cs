using Shop.Application.Common.Interfaces;
using Shop.Application.Features.Orders.Events;

namespace Shop.Application.Features.Users;

public class LoyaltyPointsEarnedEventHandler : INotificationHandler<OrderCompletedEvent>
{
    private readonly IApplicationDbContext _context;

    public LoyaltyPointsEarnedEventHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(OrderCompletedEvent notification, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == notification.UserId, cancellationToken);
        
        if (user == null) return;

        int pointsEarned = (int)(notification.TotalAmount / 100000);

        if (pointsEarned > 0)
        {
            user.TotalPoints += pointsEarned;

            if (user.TotalPoints >= 5000) 
                user.Rank = "Diamond";
            else if (user.TotalPoints >= 2000) 
                user.Rank = "Gold";
            else if (user.TotalPoints >= 500) 
                user.Rank = "Silver";

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}