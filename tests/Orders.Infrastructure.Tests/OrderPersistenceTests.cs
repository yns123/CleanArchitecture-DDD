using Microsoft.EntityFrameworkCore;
using Orders.Domain.Orders;
using Orders.Infrastructure.Persistence;

namespace Orders.Infrastructure.Tests;

// Gerçek LocalDB üzerinde, ayrı bir test veritabanıyla çalışır (OrdersDb'ye dokunmaz).
public class OrderPersistenceTests : IAsyncLifetime
{
    private const string ConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Database=OrdersDb_Tests;Trusted_Connection=True;TrustServerCertificate=True";

    private static OrderDbContext NewContext() =>
        new(new DbContextOptionsBuilder<OrderDbContext>().UseSqlServer(ConnectionString).Options);

    public async Task InitializeAsync()
    {
        await using var db = NewContext();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await using var db = NewContext();
        await db.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task Order_round_trips_with_items_and_money()
    {
        var order = Order.Create(Guid.NewGuid(), "TRY");
        order.AddItem(Guid.NewGuid(), "Kalem", new Money(10, "TRY"), 2);
        order.AddItem(Guid.NewGuid(), "Defter", new Money(25.50m, "TRY"), 1);
        order.Place();

        await using (var db = NewContext())
        {
            db.Orders.Add(order);
            await db.SaveChangesAsync();
        }

        await using (var db = NewContext())
        {
            var loaded = await db.Orders.Include(o => o.Items).SingleAsync(o => o.Id == order.Id);

            Assert.Equal(OrderStatus.Placed, loaded.Status);
            Assert.Equal(2, loaded.Items.Count);
            Assert.Equal(new Money(45.50m, "TRY"), loaded.Total);
        }
    }
}
