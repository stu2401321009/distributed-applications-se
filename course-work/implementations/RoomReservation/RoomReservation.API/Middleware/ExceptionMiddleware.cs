using System.Net;
using System.Text.Json;

namespace RoomReservation.API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Неочаквана грешка: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var problem = new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            title = "Вътрешна сървърна грешка",
            status = 500,
            detail = "Възникна неочаквана грешка. Моля, опитайте отново.",
            instance = context.Request.Path.Value
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}
