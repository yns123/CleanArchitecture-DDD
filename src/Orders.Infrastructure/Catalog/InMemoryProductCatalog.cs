using Orders.Application.Abstractions;
using Orders.Domain.Orders;

namespace Orders.Infrastructure.Catalog;

// Gerçek bir katalog servisi/DB yerine sabit liste. Port aynı kaldığı sürece Application bunu fark etmez.
internal sealed class InMemoryProductCatalog : IProductCatalog
{
    public static readonly Guid Pencil = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid Notebook = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid Backpack = Guid.Parse("33333333-3333-3333-3333-333333333333");

    private static readonly Dictionary<Guid, ProductInfo> Products = new()
    {
        [Pencil] = new(Pencil, "Kalem", new Money(10, "TRY")),
        [Notebook] = new(Notebook, "Defter", new Money(25.50m, "TRY")),
        [Backpack] = new(Backpack, "Sırt Çantası", new Money(450, "TRY")),
    };

    public Task<ProductInfo?> FindAsync(Guid productId, CancellationToken ct)
        => Task.FromResult(Products.GetValueOrDefault(productId));
}
