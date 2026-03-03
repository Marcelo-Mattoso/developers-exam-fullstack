using System.Text.Json;
using Domain.Exceptions;
using WebAPI.Contracts;

namespace WebAPI.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = MapError(exception, context.TraceIdentifier);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.StatusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static ApiErrorResponse MapError(Exception exception, string traceId)
    {
        return exception switch
        {
            NotFoundException notFoundException => new ApiErrorResponse(404, notFoundException.Message, [], traceId),
            ConflictException conflictException => new ApiErrorResponse(409, conflictException.Message, [], traceId),
            DomainValidationException validationException => new ApiErrorResponse(400, validationException.Message, validationException.Errors, traceId),
            _ => new ApiErrorResponse(500, "Ocorreu um erro interno no servidor.", [], traceId)
        };
    }
}
