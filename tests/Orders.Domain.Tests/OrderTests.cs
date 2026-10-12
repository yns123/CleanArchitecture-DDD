using Orders.Domain.Common;
using Orders.Domain.Orders;

namespace Orders.Domain.Tests;

public class OrderTests
{
    private static Order NewOrder() => Order.Create(Guid.NewGuid(), "TRY");

    [Fact]
    public void Total_is_sum_of_items()
    {
        var order = NewOrder();
        order.AddItem(Guid.NewGuid(), "Kalem", new Money(10, "TRY"), 2);
        order.AddItem(Guid.NewGuid(), "Defter", new Money(25.50m, "TRY"), 1);

        Assert.Equal(new Money(45.50m, "TRY"), order.Total);
    }

    [Fact]
    public void Adding_same_product_increases_quantity()
    {
        var order = NewOrder();
        var productId = Guid.NewGuid();
        order.AddItem(productId, "Kalem", new Money(10, "TRY"), 1);
        order.AddItem(productId, "Kalem", new Money(10, "TRY"), 2);

        Assert.Equal(3, Assert.Single(order.Items).Quantity);
    }

    [Fact]
    public void Empty_order_cannot_be_placed()
        => Assert.Throws<DomainException>(() => NewOrder().Place());

    [Fact]
    public void Placed_order_cannot_be_modified()
    {
        var order = NewOrder();
        order.AddItem(Guid.NewGuid(), "Kalem", new Money(10, "TRY"), 1);
        order.Place();

        Assert.Throws<DomainException>(() =>
            order.AddItem(Guid.NewGuid(), "Defter", new Money(5, "TRY"), 1));
    }

    [Fact]
    public void Item_currency_must_match_order_currency()
        => Assert.Throws<DomainException>(() =>
            NewOrder().AddItem(Guid.NewGuid(), "Kalem", new Money(10, "USD"), 1));

    [Fact]
    public void Cancelled_order_cannot_be_cancelled_again()
    {
        var order = NewOrder();
        order.Cancel();
        Assert.Throws<DomainException>(order.Cancel);
    }
}
