using Infrastructure;
using WebAPI.Contracts;
using WebAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
}));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.HasStarted)
        return;

    var statusCode = context.HttpContext.Response.StatusCode;

    if (statusCode < 400)
        return;

    context.HttpContext.Response.ContentType = "application/json";
    var response = new ApiErrorResponse(
        statusCode,
        $"Erro HTTP {statusCode}.",
        [],
        context.HttpContext.TraceIdentifier);

    await context.HttpContext.Response.WriteAsJsonAsync(response);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Ok("API is healthy!"));
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
