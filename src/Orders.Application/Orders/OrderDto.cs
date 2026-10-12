using Orders.Domain.Orders;

namespace Orders.Application.Orders;

// Domain nesnelerini dışarıya sızdırmayız; API'ye DTO döneriz.
public sealed record OrderItemDto(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity, decimal Total);

public sealed record OrderDto(
    Guid Id, Guid CustomerId, string Status, string Currency, decimal Total, IReadOnlyList<OrderItemDto> Items)
{
    public static OrderDto From(Order o) => new(
        o.Id, o.CustomerId, o.Status.ToString(), o.Currency, o.Total.Amount,
        o.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.UnitPrice.Amount, i.Quantity, i.Total.Amount)).ToList());
}
