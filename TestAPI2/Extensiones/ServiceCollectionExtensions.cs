using Microsoft.EntityFrameworkCore;
using TestAPI2.Context;

namespace TestAPI2.Extensiones;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        
        services.AddDbContext<VentaContext>(opt => opt.UseSqlServer(connectionString ?? throw new Exception("Missing Connection String"))/*, ServiceLifetime.Transient*/);
        return services;
    }
}
