using Orders.Domain.Common;

namespace Orders.Domain.Orders;

// Entity: kimliği (Id) var. Aggregate'in parçası; dışarıdan değil Order üzerinden değiştirilir.
// Bu yüzden constructor'ı internal: sadece Order oluşturabilir.
public sealed class OrderItem
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public Money UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    public Money Total => UnitPrice.Multiply(Quantity);

    // EF Core materialize ederken kullanır. Domain'e yapılan tek "persistence tavizi".
    private OrderItem() { ProductName = default!; UnitPrice = default!; }

    internal OrderItem(Guid productId, string productName, Money unitPrice, int quantity)
    {
        if (quantity <= 0) throw new DomainException("Adet 0'dan büyük olmalı.");
        if (string.IsNullOrWhiteSpace(productName)) throw new DomainException("Ürün adı zorunlu.");

        Id = Guid.NewGuid();
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    internal void IncreaseQuantity(int amount)
    {
        if (amount <= 0) throw new DomainException("Artış 0'dan büyük olmalı.");
        Quantity += amount;
    }
}
