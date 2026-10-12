using Orders.Application.Abstractions;
using Orders.Application.Common;
using Orders.Application.Orders;
using Orders.Domain.Common;
using Orders.Domain.Orders;
using Orders.Domain.Orders.Events;

namespace Orders.Application.Tests;

public class OrderHandlerTests
{
    private readonly FakeOrderRepository _repo = new();
    private readonly FakeProductCatalog _catalog = new();
    private readonly RecordingEventHandler<OrderPlaced> _placed = new();
    private readonly RecordingEventHandler<OrderCancelled> _cancelled = new();
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly Guid _pencil = Guid.NewGuid();

    public OrderHandlerTests()
    {
        _catalog.Products[_pencil] = new ProductInfo(_pencil, "Kalem", new Money(10, "TRY"));
        _dispatcher = TestDispatcher.Create((typeof(OrderPlaced), _placed), (typeof(OrderCancelled), _cancelled));
    }

    private PlaceOrderHandler PlaceHandler() => new(_repo, _catalog, _repo, _dispatcher);
    private CancelOrderHandler CancelHandler() => new(_repo, _repo, _dispatcher);

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
    public async Task Place_dispatches_OrderPlaced_and_clears_pending_events()
    {
        var dto = await PlaceHandler().HandleAsync(
            new PlaceOrderCommand(Guid.NewGuid(), "TRY", [new PlaceOrderLine(_pencil, 3)]), default);

        var received = Assert.Single(_placed.Received);
        Assert.Equal(dto.Id, received.OrderId);
        Assert.Equal(new Money(30, "TRY"), received.Total);
        Assert.Empty(_repo.Store[dto.Id].DomainEvents);
    }

    [Fact]
    public async Task Place_with_unknown_product_throws_saves_nothing_and_dispatches_nothing()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => PlaceHandler().HandleAsync(
            new PlaceOrderCommand(Guid.NewGuid(), "TRY", [new PlaceOrderLine(Guid.NewGuid(), 1)]), default));

        Assert.Equal(0, _repo.SaveCount);
        Assert.Empty(_placed.Received);
    }

    [Fact]
    public async Task Place_with_no_lines_violates_domain_rule()
        => await Assert.ThrowsAsync<DomainException>(() => PlaceHandler().HandleAsync(
            new PlaceOrderCommand(Guid.NewGuid(), "TRY", []), default));

    [Fact]
    public async Task Cancel_changes_status_and_dispatches_OrderCancelled()
    {
        var placed = await PlaceHandler().HandleAsync(
            new PlaceOrderCommand(Guid.NewGuid(), "TRY", [new PlaceOrderLine(_pencil, 1)]), default);

        await CancelHandler().HandleAsync(new CancelOrderCommand(placed.Id), default);

        var dto = await new GetOrderHandler(_repo).HandleAsync(new GetOrderQuery(placed.Id), default);
        Assert.Equal("Cancelled", dto.Status);
        Assert.Equal(placed.Id, Assert.Single(_cancelled.Received).OrderId);
    }

    [Fact]
    public async Task Cancelling_twice_dispatches_only_one_event()
    {
        var placed = await PlaceHandler().HandleAsync(
            new PlaceOrderCommand(Guid.NewGuid(), "TRY", [new PlaceOrderLine(_pencil, 1)]), default);

        await CancelHandler().HandleAsync(new CancelOrderCommand(placed.Id), default);
        await Assert.ThrowsAsync<DomainException>(() =>
            CancelHandler().HandleAsync(new CancelOrderCommand(placed.Id), default));

        Assert.Single(_cancelled.Received);
    }

    [Fact]
    public async Task Get_unknown_order_throws_not_found()
        => await Assert.ThrowsAsync<NotFoundException>(() =>
            new GetOrderHandler(_repo).HandleAsync(new GetOrderQuery(Guid.NewGuid()), default));
}
