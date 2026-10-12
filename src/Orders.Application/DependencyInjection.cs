using Microsoft.Extensions.DependencyInjection;
using Orders.Application.Abstractions;
using Orders.Application.Common;
using Orders.Application.Orders;
using Orders.Application.Orders.EventHandlers;
using Orders.Domain.Orders.Events;

namespace Orders.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<PlaceOrderHandler>();
        services.AddScoped<CancelOrderHandler>();
        services.AddScoped<GetOrderHandler>();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IDomainEventHandler<OrderPlaced>, LogOrderPlacedHandler>();
        services.AddScoped<IDomainEventHandler<OrderCancelled>, LogOrderCancelledHandler>();
        return services;
    }
}
