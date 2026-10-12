using Orders.Api.Middleware;
using Orders.Application;
using Orders.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("OrdersDb")
    ?? throw new InvalidOperationException("ConnectionStrings:OrdersDb eksik."));

var app = builder.Build();

// Pipeline sırası önemli: hata yakalayıcı en dışta, böylece içerideki her şeyin hatasını görür.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
