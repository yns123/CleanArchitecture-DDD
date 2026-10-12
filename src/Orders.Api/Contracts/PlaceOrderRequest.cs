using System.ComponentModel.DataAnnotations;
using Orders.Application.Orders;

namespace Orders.Api.Contracts;

// HTTP sözleşmesi Api'ye ait; Application komutundan ayrı tutulur.
// Biçim doğrulaması (boş mu, pozitif mi) burada; iş kuralları Domain'de.
// Fiyat alanı bilerek YOK: fiyatı katalog belirler.
public sealed record PlaceOrderRequest(
    [Required] Guid CustomerId,
    [Required, StringLength(3, MinimumLength = 3)] string Currency,
    [Required, MinLength(1)] List<PlaceOrderLineRequest> Lines)
{
    public PlaceOrderCommand ToCommand()
        => new(CustomerId, Currency, Lines.Select(l => new PlaceOrderLine(l.ProductId, l.Quantity)).ToList());
}

public sealed record PlaceOrderLineRequest(
    [Required] Guid ProductId,
    [Range(1, 1000)] int Quantity);
