using TestAPI2.Services.Interfaces;

namespace TestAPI2.Extensiones;

public static class ScrutorExtension
{
    public static IServiceCollection ScanServices(this IServiceCollection services)
    {
        services.Scan(s => s
        .FromAssemblyOf<IApiService>()
        .AddClasses(c => c.AssignableTo<IApiService>())
        .AsImplementedInterfaces()
        .WithScopedLifetime()
        );
        return services;
    }
}