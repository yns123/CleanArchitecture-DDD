using Microsoft.AspNetCore.Mvc;
using Orders.Application.Common;
using Orders.Domain.Common;

namespace Orders.Api.Middleware;

// Exception -> HTTP eşlemesi tek yerde. Controller'larda try/catch yok.
//   DomainException   -> 422 (iş kuralı ihlali)
//   NotFoundException -> 404
//   diğerleri         -> 500 (detay istemciye sızdırılmaz, sadece loglanır)
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex) when (!context.Response.HasStarted)
        {
            var (status, title, detail) = ex switch
            {
                DomainException => (StatusCodes.Status422UnprocessableEntity, "İş kuralı ihlali", ex.Message),
                NotFoundException => (StatusCodes.Status404NotFound, "Kayıt bulunamadı", ex.Message),
                _ => (StatusCodes.Status500InternalServerError, "Beklenmeyen hata", "Bir hata oluştu.")
            };

            if (status == StatusCodes.Status500InternalServerError)
                logger.LogError(ex, "Beklenmeyen hata: {Path}", context.Request.Path);
            else
                logger.LogInformation("{Type}: {Message}", ex.GetType().Name, ex.Message);

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json");
        }
    }
}
