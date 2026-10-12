using Orders.Domain.Orders;

namespace Orders.Application.Abstractions;

public sealed record ProductInfo(Guid Id, string Name, Money Price);

// Fiyatı istemci değil katalog belirler. Gerçek hayatta bu bir servis/DB olurdu;
// Application sadece ihtiyacını tarif eder.
public interface IProductCatalog
{
    Task<ProductInfo?> FindAsync(Guid productId, CancellationToken ct);
}
