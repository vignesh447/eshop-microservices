using Microsoft.AspNetCore.Mvc;

namespace EShop.Payments.Api.Controllers;

[ApiController, Route("api/payments")]
public class PaymentsController : ControllerBase
{
    public record ChargeRequest(Guid OrderId, decimal Amount, string Currency = "USD");
    public record ChargeReply(bool Success, string TxId);

    [HttpPost("charge")]
    public ActionResult<ChargeReply> Charge(ChargeRequest r)
    {
        if (r.Amount <= 0) return BadRequest("Amount must be > 0");
        if (r.Amount > 10_000) return Ok(new ChargeReply(false, "")); // simulated decline
        return Ok(new ChargeReply(true, "TX-" + Guid.NewGuid().ToString("N")[..10]));
    }
}