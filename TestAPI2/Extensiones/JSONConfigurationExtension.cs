using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;

namespace TestAPI2.Extensiones;

public static class JSONConfigurationExtension
{
    public static IServiceCollection ConfigureJsonOption(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(o =>
        {
            o.SerializerOptions.IncludeFields = true;
            o.SerializerOptions.MaxDepth = 0;
            o.SerializerOptions.PropertyNameCaseInsensitive = true;
            o.SerializerOptions.WriteIndented = false;
            o.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });


        return services;
    }
    public static IServiceCollection ConfigureJsonOption(this IServiceCollection services,bool includeFields, int maxDepth, bool propertyNameCaseInsensitive, bool writeIndented, bool ignoreCycles)
    {
        services.Configure<JsonOptions>(o =>
        {
            o.SerializerOptions.IncludeFields = includeFields;
            o.SerializerOptions.MaxDepth = maxDepth;
            o.SerializerOptions.PropertyNameCaseInsensitive = propertyNameCaseInsensitive;
            o.SerializerOptions.WriteIndented = writeIndented;
            if (ignoreCycles) o.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });


        return services;
    }
}
