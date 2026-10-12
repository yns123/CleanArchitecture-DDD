using Orders.Domain.Common;

namespace Orders.Domain.Orders;

// Aggregate Root: dışarıya açılan tek kapı. Tüm iş kuralları (invariant) burada korunur.
// Dışarıdan Items.Add(...) yapılamaz; sadece AddItem() gibi davranışlarla değişir.
public sealed class Order
{
    private readonly List<OrderItem> _items = [];

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public string Currency { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public Money Total => _items.Aggregate(Money.Zero(Currency), (sum, i) => sum.Add(i.Total));

    private Order() { Currency = default!; } // EF Core için (ileride)

    public static Order Create(Guid customerId, string currency)
    {
        if (customerId == Guid.Empty) throw new DomainException("Müşteri zorunlu.");
        if (string.IsNullOrWhiteSpace(currency)) throw new DomainException("Para birimi zorunlu.");

        return new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Currency = currency.ToUpperInvariant(),
            Status = OrderStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void AddItem(Guid productId, string productName, Money unitPrice, int quantity)
    {
        EnsureDraft();
        if (unitPrice.Currency != Currency)
            throw new DomainException("Ürün para birimi sipariş para birimiyle aynı olmalı.");

        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
            existing.IncreaseQuantity(quantity);
        else
            _items.Add(new OrderItem(productId, productName, unitPrice, quantity));
    }

    public void Place()
    {
        EnsureDraft();
        if (_items.Count == 0) throw new DomainException("Boş sipariş verilemez.");
        Status = OrderStatus.Placed;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Cancelled) throw new DomainException("Sipariş zaten iptal edilmiş.");
        Status = OrderStatus.Cancelled;
    }

    private void EnsureDraft()
    {
        if (Status != OrderStatus.Draft)
            throw new DomainException("Sadece taslak sipariş değiştirilebilir.");
    }
}
