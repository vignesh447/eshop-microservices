using Microsoft.AspNetCore.Mvc;

namespace EShop.Inventory.Api.Controllers;

[ApiController, Route("api/inventory")]
public class InventoryController : ControllerBase
{
    // in-memory stock; we'll swap for a DB later
    private static readonly Dictionary<Guid, int> _stock = new()
    {
        [Guid.Parse("22222222-2222-2222-2222-222222222222")] = 100,
        [Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6")] = 50
    };

    public record ReserveRequest(Guid ProductId, int Qty);
    public record ReserveReply(bool Ok, int Remaining);

    [HttpPost("reserve")]
    public ActionResult<ReserveReply> Reserve(ReserveRequest r)
    {
        if (r.Qty <= 0) return BadRequest("Qty must be > 0");

        lock (_stock)
        {
            var cur = _stock.GetValueOrDefault(r.ProductId, 0);
            if (cur < r.Qty) return Ok(new ReserveReply(false, cur));
            _stock[r.ProductId] = cur - r.Qty;
            return Ok(new ReserveReply(true, cur - r.Qty));
        }
    }

    [HttpGet("{productId:guid}")]
    public ActionResult<int> Get(Guid productId)
        => Ok(_stock.GetValueOrDefault(productId, 0));
}