using System.Text.RegularExpressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shop.Application.Common.Interfaces;
using Shop.Domain.Constants;

namespace Shop.Application.Features.Payments.Commands.ProcessWebhook;

public record ProcessWebhookCommand(
    string Gateway,        
    decimal TransferAmount, 
    string Content,         
    string ReferenceCode    
) : IRequest<bool>;

public class ProcessWebhookCommandHandler : IRequestHandler<ProcessWebhookCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public ProcessWebhookCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
    {

        var match = Regex.Match(request.Content, @"SHOP\s*[a-zA-Z0-9]{8}", RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            return true;
        }

        var transactionCode = Regex.Replace(match.Value.ToUpper(), @"\s+", "");

        var targetPayment = await _context.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p =>
                
                    p.TransactionId != null &&
                    p.TransactionId.ToUpper().Replace(" ", "") == transactionCode &&
                    p.Method == "VietQR",
                cancellationToken);
        

        // Nếu không tìm thấy, hoặc đơn đã xử lý (Status != Pending) thì bỏ qua
        if (targetPayment == null || targetPayment.Status != PaymentStatus.Pending) 
            return false;

        if (request.TransferAmount >= targetPayment.Order.TotalAmount)
        {
            targetPayment.Status = PaymentStatus.Success;
            targetPayment.Order.Status = OrderStatus.Paid; 
            
            targetPayment.TransactionId = $"{transactionCode} | Ref: {request.ReferenceCode}";
        }
        else 
        {
            // Khách chuyển thiếu tiền
            targetPayment.Status = "PartialPaid";
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}