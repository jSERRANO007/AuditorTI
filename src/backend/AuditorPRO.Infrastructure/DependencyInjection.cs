using AuditorPRO.Domain.Interfaces;
using AuditorPRO.Infrastructure.Persistence;
using AuditorPRO.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuditorPRO.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.EnableRetryOnFailure(3)));

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ISimulacionRepository, SimulacionRepository>();
        services.AddScoped<IHallazgoRepository, HallazgoRepository>();
        services.AddScoped<IBitacoraRepository, BitacoraRepository>();

        // Services
        services.AddScoped<IAuditLoggerService, AuditLoggerService>();
        services.AddScoped<IAzureOpenAIService, AzureOpenAIService>();
        services.AddScoped<IBlobStorageService, BlobStorageService>();
        services.AddScoped<IDocumentGeneratorService, DocumentGeneratorService>();

        return services;
    }
}
