using EShop.Payments.Grpc;
using Grpc.Core;

namespace EShop.Payments.Api.Grpc;

public class PaymentsGrpcService : PaymentsService.PaymentsServiceBase
{
    private readonly ILogger<PaymentsGrpcService> _logger;

    public PaymentsGrpcService(ILogger<PaymentsGrpcService> logger) => _logger = logger;

    public override Task<AuthorizeReply> Authorize(AuthorizeRequest request, ServerCallContext context)
    {
        _logger.LogInformation(
            "Authorize gRPC for Order {OrderId}, Customer {CustomerId}, Amount {Amount} {Currency}",
            request.OrderId, request.CustomerId, request.Amount, request.Currency);

        // Toy decline rule — same shape as the old REST stub
        if (request.Amount > 10_000)
        {
            return Task.FromResult(new AuthorizeReply
            {
                PaymentId = Guid.NewGuid().ToString(),
                Status = AuthorizeReply.Types.Status.Declined,
                Reason = "Amount exceeds limit"
            });
        }

        return Task.FromResult(new AuthorizeReply
        {
            PaymentId = Guid.NewGuid().ToString(),
            Status = AuthorizeReply.Types.Status.Approved,
            Reason = string.Empty
        });
    }
}
