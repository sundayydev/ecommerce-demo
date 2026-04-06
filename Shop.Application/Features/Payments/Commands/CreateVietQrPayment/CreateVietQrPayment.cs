using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shop.Application.Common.Exceptions;
using Shop.Application.Common.Interfaces;
using Shop.Domain.Constants;
using Shop.Domain.Entities;
using NotFoundException = Ardalis.GuardClauses.NotFoundException;

namespace Shop.Application.Features.Payments.Commands.CreateVietQrPayment;

public record VietQrResponse(
    Guid PaymentId,
    string QrUrl,          
    decimal Amount,       
    string TransferContent 
);

public record CreateVietQrPaymentCommand(Guid OrderId) : IRequest<VietQrResponse>;

public class CreateVietQrPaymentCommandHandler : IRequestHandler<CreateVietQrPaymentCommand, VietQrResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IConfiguration _configuration;

    public CreateVietQrPaymentCommandHandler(
        IApplicationDbContext context, 
        ICurrentUserService currentUserService,
        IConfiguration configuration)
    {
        _context = context;
        _currentUserService = currentUserService;
        _configuration = configuration;
    }

    public async Task<VietQrResponse> Handle(CreateVietQrPaymentCommand request, CancellationToken cancellationToken)
{
    var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException("Cần đăng nhập.");

    var order = await _context.Orders
        .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == userId, cancellationToken);

    if (order == null) throw new NotFoundException(nameof(Order), request.OrderId.ToString());
    if (order.Status == OrderStatus.Paid || order.Status == OrderStatus.Cancelled)
        throw new Exception($"Không thể thanh toán. Đơn hàng đang ở trạng thái: {order.Status}");

    var existingPayment = await _context.Payments
        .FirstOrDefaultAsync(p => p.OrderId == order.Id && p.Status == OrderStatus.Pending && p.Method == "VietQR", cancellationToken);

    Payment payment;
    string transferContent;

    if (existingPayment != null)
    {
        payment = existingPayment;
        transferContent = payment.TransactionId!;
    }
    else
    {
        transferContent = $"SHOP {order.Id.ToString().Substring(0, 8).ToUpper()}";
        
        payment = new Payment
        {
            OrderId = order.Id,
            Method = "VietQR",
            TransactionId = transferContent, 
            Status = PaymentStatus.Pending
        };
        _context.Payments.Add(payment);
    }

    var bankId = _configuration["VietQR:BankId"];
    var accountNo = _configuration["VietQR:AccountNo"];
    var accountName = Uri.EscapeDataString(_configuration["VietQR:AccountName"]!); 
    var template = _configuration["VietQR:Template"];
    var amount = (int)order.TotalAmount; 

    var qrUrl = $"https://img.vietqr.io/image/{bankId}-{accountNo}-{template}.png?amount={amount}&addInfo={Uri.EscapeDataString(transferContent)}&accountName={accountName}";

    await _context.SaveChangesAsync(cancellationToken);

    return new VietQrResponse(
        payment.Id,
        qrUrl,
        order.TotalAmount,
        transferContent
    );
}
}