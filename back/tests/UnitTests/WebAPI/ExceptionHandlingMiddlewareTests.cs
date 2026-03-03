using System.Text.Json;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using WebAPI.Contracts;
using WebAPI.Middlewares;

namespace UnitTests.WebAPI;

public class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldReturn404_WhenNotFoundExceptionIsThrown()
    {
        var middleware = new ExceptionHandlingMiddleware(_ => throw new NotFoundException("Livro não encontrado."));
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        var response = await ReadErrorResponse(context);

        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
        Assert.Equal(404, response.StatusCode);
        Assert.Equal("Livro não encontrado.", response.Message);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn409_WhenConflictExceptionIsThrown()
    {
        var middleware = new ExceptionHandlingMiddleware(_ => throw new ConflictException("Já existe um livro com este título."));
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        var response = await ReadErrorResponse(context);

        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
        Assert.Equal(409, response.StatusCode);
        Assert.Equal("Já existe um livro com este título.", response.Message);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn400_WhenDomainValidationExceptionIsThrown()
    {
        var errors = new[]
        {
            "Título deve ter entre 10 e 100 caracteres.",
            "Descrição deve ter no máximo 1024 caracteres."
        };

        var middleware = new ExceptionHandlingMiddleware(_ => throw new DomainValidationException(errors));
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        var response = await ReadErrorResponse(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Equal(400, response.StatusCode);
        Assert.Equal("Erro de validação.", response.Message);
        Assert.Equal(2, response.Errors.Count);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn500_WhenUnhandledExceptionIsThrown()
    {
        var middleware = new ExceptionHandlingMiddleware(_ => throw new InvalidOperationException("unexpected"));
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        var response = await ReadErrorResponse(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Equal(500, response.StatusCode);
        Assert.Equal("Ocorreu um erro interno no servidor.", response.Message);
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<ApiErrorResponse> ReadErrorResponse(DefaultHttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var payload = await reader.ReadToEndAsync();

        var response = JsonSerializer.Deserialize<ApiErrorResponse>(payload, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return response ?? throw new InvalidOperationException("Falha ao desserializar ApiErrorResponse.");
    }
}
