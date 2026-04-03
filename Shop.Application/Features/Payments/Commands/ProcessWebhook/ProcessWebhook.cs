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
    private readonly IPaymentNotificationService _notificationService; 

    public ProcessWebhookCommandHandler(IApplicationDbContext context, IPaymentNotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
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
    
    if (targetPayment == null) 
        return true; 
    

    if (targetPayment.Status != PaymentStatus.Pending) 
        return true; 
    

    if (request.TransferAmount == targetPayment.Order.TotalAmount)
    {
        targetPayment.Status = PaymentStatus.Success;
        targetPayment.Order.Status = OrderStatus.Paid; 
    }
    else if (request.TransferAmount > targetPayment.Order.TotalAmount)
    {
        targetPayment.Status = PaymentStatus.OverPaid; 
        targetPayment.Order.Status = OrderStatus.Paid; 
        
        targetPayment.Note = $"Khách chuyển thừa {request.TransferAmount - targetPayment.Order.TotalAmount}đ";
    }
    else 
    {
        targetPayment.Status = PaymentStatus.PartialPaid;
        targetPayment.Order.Status = OrderStatus.PartiallyPaid;
        targetPayment.Note = $"Khách chuyển thiếu {targetPayment.Order.TotalAmount - request.TransferAmount}đ";
    }

    targetPayment.TransactionId = $"{transactionCode} | Ref: {request.ReferenceCode}";

    await _context.SaveChangesAsync(cancellationToken);
    await _notificationService.SendPaymentSuccessNotificationAsync(
        targetPayment.OrderId, 
        request.TransferAmount, 
        cancellationToken);
    
    return true;
}}