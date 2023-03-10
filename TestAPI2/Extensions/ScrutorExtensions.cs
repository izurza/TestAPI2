using TestAPI2.Services.Interfaces;

namespace TestAPI2.Extensions;

public static class ScrutorExtensions
{

    public static IServiceCollection ScanServices(this IServiceCollection services)
    {
        services.Scan(s => s
    .FromAssemblyOf<IScopedServices>()
    .AddClasses(c => c.AssignableTo<IScopedServices>())
    .AsImplementedInterfaces()
    .WithScopedLifetime()
    );

        return services;
    }
}
