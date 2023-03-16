using Microsoft.EntityFrameworkCore;
using Carter;
using TestAPI2.Extensiones;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TestAPI2.Authorization;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .WriteTo.Console()
    //.WriteTo.Debug()
    .CreateBootstrapLogger();
    //.CreateLogger();

try
{
    Log.Information("Starting api");
    var builder = WebApplication.CreateBuilder(args);
    //builder.Services.AddAuthentication().AddJwtBearer();
    //builder.Services.AddAuthorization();
    //builder.Services.AddAuthorization();
    //builder.Host.UseSerilog();

    string _connectionString = builder.Configuration.GetConnectionString("LocalDB");
    string _domain = "https://" + builder.Configuration["Auth0:Domain"] + "/";
    bool blazorTesting = false;
    builder.Services.ScanServices();
    //builder.Services.Scan(s => s
        //.FromAssemblyOf<IApiService>()
        //.AddClasses(c => c.AssignableTo<IApiService>())
        //.AsImplementedInterfaces()
        //.WithScopedLifetime());

    builder.Services.ConfigureJsonOption();//Replaces function below
    // Customize the JSON serialization options used by minimal with following line
    //builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(o => 
    //    {
    //        o.SerializerOptions.IncludeFields = true;
    //        //o.SerializerOptions.MaxDepth = 0;
    //        o.SerializerOptions.PropertyNameCaseInsensitive = true;
    //        o.SerializerOptions.WriteIndented = true;
    //        o.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    //    });

    builder.Services.AddPersistence(_connectionString);// Replaces the line below
    //builder.Services.AddDbContext<VentaContext>(opt => opt.UseSqlServer(_connectionString ?? throw new Exception("Missing Connection String"))/*, ServiceLifetime.Transient*/);

    builder.Services.AddSwagger();//Replaces the 2 below
    //builder.Services.AddEndpointsApiExplorer();
    //builder.Services.AddSwaggerGen();

    builder.Services.AddCarter();

    if (!blazorTesting)
    {
        builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = _domain;
            options.Audience = builder.Configuration["auth0:audience"];
            // if the access token does not have a `sub` claim, `user.identity.name` will be `null`. map it to a different claim by setting the nameclaimtype below.
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = ClaimTypes.NameIdentifier
            };
        });
        builder.Services.AddConfiguredPoliciesAuthorization(_domain);
    }
    

    //builder.Services.AddAuthorization(options =>
    //{
    //    options.AddPolicy("read:Clientes", policy => policy.Requirements.Add(new HasScopeRequirement("read:Clientes", "https://" + builder.Configuration["Auth0:Domain"] + "/")));
    //    options.AddPolicy("write:Clientes", policy => policy.Requirements.Add(new HasScopeRequirement("read:Clientes", "https://" + builder.Configuration["Auth0:Domain"] + "/")));
    //    options.AddPolicy("delete:Clientes", policy => policy.Requirements.Add(new HasScopeRequirement("read:Clientes", "https://" + builder.Configuration["Auth0:Domain"] + "/")));
    //    options.AddPolicy("write:Productos", policy => policy.Requirements.Add(new HasScopeRequirement("read:Clientes", "https://" + builder.Configuration["Auth0:Domain"] + "/")));
    //    options.AddPolicy("delete:Productos", policy => policy.Requirements.Add(new HasScopeRequirement("read:Clientes", "https://" + builder.Configuration["Auth0:Domain"] + "/")));

    //});
    builder.Host.UseSerilog((context, services, configuration) => configuration
      .ReadFrom.Configuration(context.Configuration)
      .ReadFrom.Services(services)
      .Enrich.FromLogContext()
      .WriteTo.File(new CompactJsonFormatter(), "./logs/myapp.json")
      .WriteTo.Console());

    builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
    var app = builder.Build();

    //app.UseConfiguredSerilogRequestLogging();
    app.UseSerilogRequestLogging();//Set before noisy handlers to exclude them from logging
    

    app.MapSwagger();

    app.UseHttpsRedirection();

    app.MapCarter();

    if (!blazorTesting)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }



    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}