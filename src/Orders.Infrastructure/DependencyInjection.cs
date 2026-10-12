using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application.Abstractions;
using Orders.Infrastructure.Catalog;
using Orders.Infrastructure.Persistence;

namespace Orders.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<OrderDbContext>(o => o.UseSqlServer(connectionString));
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<OrderDbContext>());
        services.AddSingleton<IProductCatalog, InMemoryProductCatalog>();
        return services;
    }
}
