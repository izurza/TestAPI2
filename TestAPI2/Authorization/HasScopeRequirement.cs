using Microsoft.AspNetCore.Authorization;
using TestAPI2.Services.Interfaces;

namespace TestAPI2.Authorization;

public class HasScopeRequirement : IAuthorizationRequirement, ISingletonServices
{
    public string Issuer { get; }
    public string Scope { get; }

    public HasScopeRequirement(string scope, string issuer)
    {
        Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
    }
}