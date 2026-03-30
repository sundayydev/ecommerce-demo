using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.Features.Payments.Commands.CreateVietQrPayment;
using Shop.Application.Features.Payments.Commands.ProcessWebhook;
using Shop.WebApi.Extensions;

namespace Shop.WebApi.Endpoints;

public class PaymentEndpoints : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CreateVietQr, "/vietqr")
            .WithSummary("Tạo mã QR chuyển khoản VietQR (Chỉ User)")
            .RequireAuthorization("RequireUserRole");
        
        groupBuilder.MapPost(ProcessWebhook, "/webhook")
            .WithSummary("Webhook nhận thông báo biến động số dư từ SePay")
            .AllowAnonymous();
    }
    
    private static async Task<Ok<VietQrResponse>> CreateVietQr(ISender sender, CreateVietQrPaymentCommand command)
    {
        var response = await sender.Send(command);
        return TypedResults.Ok(response);
    }
    
    private static async Task<IResult> ProcessWebhook(
        ISender sender, 
        IConfiguration config,
        [FromHeader(Name = "Authorization")] string apiKey,
        [FromBody] ProcessWebhookCommand command) 
    {
        var expectedToken = config["VietQR:WebhookToken"];
        
        if (string.IsNullOrEmpty(apiKey) || !apiKey.Contains(expectedToken!))
        {
            return TypedResults.Unauthorized(); 
        }

        var check = await sender.Send(command);

        return TypedResults.Ok(new { success = check, message = "Webhook processed successfully" });
    }
}