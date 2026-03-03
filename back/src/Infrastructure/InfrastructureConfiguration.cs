using System.Reflection;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Infrastructure.Events;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SqlDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("SQLConnection"), b => b.MigrationsAssembly(typeof(SqlDbContext).Assembly.FullName)));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        
        services.AddScoped<IDomainEventHandler, DomainEventHandler>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IEmailService, EmailService>();
        services.Configure<AzureEmailOptions>(options =>
        {
            options.ConnectionString = configuration[$"{AzureEmailOptions.SectionName}:ConnectionString"] ?? string.Empty;
            options.SenderAddress = configuration[$"{AzureEmailOptions.SectionName}:SenderAddress"] ?? string.Empty;
            options.RecipientAddress = configuration[$"{AzureEmailOptions.SectionName}:RecipientAddress"] ?? "developers@inspand.com.br";
        });

        services.AddScoped<SqlDbContext>();

        return services;
    }
}