# CleanArchitecture-DDD

Clean Architecture ve Domain-Driven Design'ı öğrenmek için yazılmış küçük bir sipariş (Order) uygulaması.
Amaç "enterprise" olmak değil, desenleri elle yazıp ne işe yaradıklarını görmek.
Bu yüzden MediatR, AutoMapper, FluentValidation gibi kütüphaneler bilerek kullanılmadı.

## Katmanlar

```
Api  ──►  Infrastructure  ──►  Application  ──►  Domain
 └─────────────────────────────►┘
```

Bağımlılıklar hep içeri doğrudur. Domain hiçbir şeye bağlı değildir.

| Proje | Sorumluluk |
|---|---|
| `Orders.Domain` | İş kuralları: `Order` (aggregate root), `OrderItem` (entity), `Money` (value object). Saf C#, bağımlılık yok. |
| `Orders.Application` | Use case'ler (`PlaceOrderHandler`, `CancelOrderHandler`, `GetOrderHandler`), DTO'lar ve port'lar (`IOrderRepository`, `IUnitOfWork`, `IProductCatalog`). |
| `Orders.Infrastructure` | Port'ların implementasyonu: EF Core + SQL Server, repository, sabit ürün kataloğu. |
| `Orders.Api` | İnce controller'lar, istek sözleşmeleri, exception middleware. |

## Öğrenilen kavramlar ve kodda nerede oldukları

| Kavram | Yer |
|---|---|
| Value Object | `Money`: değişmez, değer eşitliği, kendi kurallarını korur |
| Entity | `OrderItem`: kimliği var, constructor'ı `internal` |
| Aggregate Root | `Order`: dışarıya tek kapı, tüm invariant'lar burada |
| Invariant | Boş sipariş verilemez, verilen sipariş değiştirilemez, para birimi tutarlı olmalı |
| Repository | Arayüz Application'da, implementasyon Infrastructure'da (bağımlılık tersine çevrilir) |
| Unit of Work | `OrderDbContext`, `IUnitOfWork` port'unu karşılar |
| Domain Event | `OrderPlaced`, `OrderCancelled`: `Order` içinde üretilir (`Raise`), handler kayıttan sonra `DomainEventDispatcher` ile dağıtır, dinleyiciler `IDomainEventHandler<T>` |
| Use case / orkestrasyon | Handler'larda iş kuralı yok, sadece sıralama var |
| Anti-corruption (basit) | Domain nesneleri API'ye sızmaz, `OrderDto` döner |
| Hata eşleme | `DomainException` → 422, `NotFoundException` → 404, diğerleri → 500 (`ExceptionHandlingMiddleware`) |

İstemciden fiyat alınmaz; fiyatı `IProductCatalog` belirler.

## Çalıştırma

Gereksinimler: .NET 10 SDK, SQL Server LocalDB (`(localdb)\MSSQLLocalDB`), `dotnet-ef` aracı.

```bash
dotnet build CleanDdd.sln
dotnet test CleanDdd.sln

dotnet ef database update --project src/Orders.Infrastructure --startup-project src/Orders.Api
dotnet run --project src/Orders.Api --launch-profile http
```

API `http://localhost:5134` adresinde açılır. Örnek istekler: [`src/Orders.Api/Orders.Api.http`](src/Orders.Api/Orders.Api.http)

Hazır ürünler: Kalem `11111111-1111-1111-1111-111111111111`, Defter `22222222-2222-2222-2222-222222222222`, Sırt Çantası `33333333-3333-3333-3333-333333333333`.

| Endpoint | Açıklama |
|---|---|
| `POST /api/orders` | Sipariş ver |
| `GET /api/orders/{id}` | Siparişi getir |
| `POST /api/orders/{id}/cancel` | Siparişi iptal et |

## Testler

- `Orders.Domain.Tests`: veritabanı olmadan iş kuralı testleri
- `Orders.Application.Tests`: elle yazılmış fake'lerle handler testleri (mock kütüphanesi yok)
- `Orders.Infrastructure.Tests`: gerçek LocalDB üzerinde, ayrı `OrdersDb_Tests` veritabanıyla kaydet/oku testi

## Yol haritası

- [x] Domain katmanı
- [x] Application katmanı
- [x] Infrastructure katmanı
- [x] Api katmanı ve middleware
- [x] Domain event'ler (`OrderPlaced`, `OrderCancelled`)
- [ ] Domain servisi (ör. kupon/indirim hesabı)
