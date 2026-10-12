using Microsoft.AspNetCore.Mvc;
using Orders.Api.Contracts;
using Orders.Application.Orders;

namespace Orders.Api.Controllers;

// İnce controller: isteği komuta çevir, handler'ı çağır, sonucu HTTP'ye çevir. Başka bir şey yok.
[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<OrderDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Place(
        PlaceOrderRequest request, [FromServices] PlaceOrderHandler handler, CancellationToken ct)
    {
        var order = await handler.HandleAsync(request.ToCommand(), ct);
        return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<OrderDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid id, [FromServices] GetOrderHandler handler, CancellationToken ct)
        => Ok(await handler.HandleAsync(new GetOrderQuery(id), ct));

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Cancel(Guid id, [FromServices] CancelOrderHandler handler, CancellationToken ct)
    {
        await handler.HandleAsync(new CancelOrderCommand(id), ct);
        return NoContent();
    }
}
