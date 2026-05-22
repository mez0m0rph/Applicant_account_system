using System.Net;
using System.Text.Json;

namespace AdmissionService.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;  // тк обработчик идет первым в конвейере
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;   // для записи логов в консоль/файл

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)  // автоматически вызывается при каждом запросе
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteErrorAsync(context, HttpStatusCode.Unauthorized, ex.Message);
        }
        catch (ArgumentException ex)  // неверные данные (отрицательный Id/пустой email)
        {
            await WriteErrorAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (KeyNotFoundException ex)  // когда не нашли что-то в БД
        {
            await WriteErrorAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (Exception ex)  // все остальные ошибки 
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            error = message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}