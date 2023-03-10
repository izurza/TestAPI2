using TestAPI2.Services.Interfaces;

namespace TestAPI2.Extensiones;

public static class ScrutorExtension
{
    public static IServiceCollection ScanServices(this IServiceCollection services)
    {
        services.Scan(s => s
        .FromAssemblyOf<IScopedServices>()
        .AddClasses(c => c.AssignableTo<IScopedServices>())
        .AsImplementedInterfaces()
        .WithScopedLifetime()
        );

        //services.Scan(s => s
        //.FromAssemblyOf<ISingletonServices>()
        //.AddClasses(c => c.AssignableTo<ISingletonServices>())
        //.AsImplementedInterfaces()
        //.WithSingletonLifetime()
        //);
        return services;
    }
}