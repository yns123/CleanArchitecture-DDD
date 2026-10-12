using Orders.Application.Abstractions;
using Orders.Application.Common;
using Orders.Application.Orders;
using Orders.Domain.Common;
using Orders.Domain.Orders;

namespace Orders.Application.Tests;

public class OrderHandlerTests
{
    private readonly FakeOrderRepository _repo = new();
    private readonly FakeProductCatalog _catalog = new();
    private readonly Guid _pencil = Guid.NewGuid();

    public OrderHandlerTests() =>
        _catalog.Products[_pencil] = new ProductInfo(_pencil, "Kalem", new Money(10, "TRY"));

    private PlaceOrderHandler PlaceHandler() => new(_repo, _catalog, _repo);

    [Fact]
    public async Task Place_uses_catalog_price_and_saves()
    {
        var dto = await PlaceHandler().HandleAsync(
            new PlaceOrderCommand(Guid.NewGuid(), "TRY", [new PlaceOrderLine(_pencil, 3)]), default);

        Assert.Equal("Placed", dto.Status);
        Assert.Equal(30m, dto.Total);
        Assert.Equal(1, _repo.SaveCount);
        Assert.Single(_repo.Store);
    }

    [Fact]
    public async Task Place_with_unknown_product_throws_and_saves_nothing()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => PlaceHandler().HandleAsync(
            new PlaceOrderCommand(Guid.NewGuid(), "TRY", [new PlaceOrderLine(Guid.NewGuid(), 1)]), default));

        Assert.Equal(0, _repo.SaveCount);
    }

    [Fact]
    public async Task Place_with_no_lines_violates_domain_rule()
        => await Assert.ThrowsAsync<DomainException>(() => PlaceHandler().HandleAsync(
            new PlaceOrderCommand(Guid.NewGuid(), "TRY", []), default));

    [Fact]
    public async Task Cancel_changes_status()
    {
        var placed = await PlaceHandler().HandleAsync(
            new PlaceOrderCommand(Guid.NewGuid(), "TRY", [new PlaceOrderLine(_pencil, 1)]), default);

        await new CancelOrderHandler(_repo, _repo).HandleAsync(new CancelOrderCommand(placed.Id), default);

        var dto = await new GetOrderHandler(_repo).HandleAsync(new GetOrderQuery(placed.Id), default);
        Assert.Equal("Cancelled", dto.Status);
    }

    [Fact]
    public async Task Get_unknown_order_throws_not_found()
        => await Assert.ThrowsAsync<NotFoundException>(() =>
            new GetOrderHandler(_repo).HandleAsync(new GetOrderQuery(Guid.NewGuid()), default));
}
