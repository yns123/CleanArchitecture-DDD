using Orders.Domain.Common;
using Orders.Domain.Orders;
using Orders.Domain.Orders.Events;

namespace Orders.Domain.Tests;

public class OrderEventTests
{
    private static Order OrderWithItem()
    {
        var order = Order.Create(Guid.NewGuid(), "TRY");
        order.AddItem(Guid.NewGuid(), "Kalem", new Money(10, "TRY"), 2);
        return order;
    }

    [Fact]
    public void New_order_has_no_events()
        => Assert.Empty(OrderWithItem().DomainEvents);

    [Fact]
    public void Place_raises_OrderPlaced_with_total()
    {
        var order = OrderWithItem();
        order.Place();

        var e = Assert.IsType<OrderPlaced>(Assert.Single(order.DomainEvents));
        Assert.Equal(order.Id, e.OrderId);
        Assert.Equal(new Money(20, "TRY"), e.Total);
    }

    [Fact]
    public void Failed_place_raises_no_event()
    {
        var order = Order.Create(Guid.NewGuid(), "TRY");
        Assert.Throws<DomainException>(order.Place);
        Assert.Empty(order.DomainEvents);
    }

    [Fact]
    public void Cancel_raises_OrderCancelled()
    {
        var order = OrderWithItem();
        order.Cancel();

        Assert.IsType<OrderCancelled>(Assert.Single(order.DomainEvents));
    }

    [Fact]
    public void ClearDomainEvents_empties_the_list()
    {
        var order = OrderWithItem();
        order.Place();
        order.ClearDomainEvents();

        Assert.Empty(order.DomainEvents);
    }
}
