using TestAPI2.Authorization;

namespace TestAPI2.Extensiones;

public static class PoliciesExtension
{
    public static IServiceCollection AddConfiguredPoliciesAuthorization(this IServiceCollection services, string domain)
    {
        services.AddAuthorization(options =>
        {

            Dictionary<string, string> policies = new Dictionary<string, string>()
            {
                {"read:Clientes","read:Clientes"},
                {"write:Clientes","write:Clientes"},
                {"delete:Clientes","delete:Clientes"},
                {"write:Productos","write:Productos"},
                {"delete:Productos","delete:Productos"}
            };
            foreach((string key, string value) in policies)
            {
                options.AddPolicy(key, policy => policy.Requirements.Add(new HasScopeRequirement(value, domain)));
            }
            //options.AddPolicy("read:Clientes", policy => policy.Requirements.Add(new HasScopeRequirement("read:Clientes", domain)));
            //options.AddPolicy("write:Clientes", policy => policy.Requirements.Add(new HasScopeRequirement("read:Clientes", domain)));
            //options.AddPolicy("delete:Clientes", policy => policy.Requirements.Add(new HasScopeRequirement("read:Clientes", domain)));
            //options.AddPolicy("write:Productos", policy => policy.Requirements.Add(new HasScopeRequirement("read:Clientes", domain)));
            //options.AddPolicy("delete:Productos", policy => policy.Requirements.Add(new HasScopeRequirement("read:Clientes", domain)));

        });


        return services;
    }
}
