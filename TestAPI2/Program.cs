using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text.Json.Serialization;
using TestAPI2;
using TestAPI2.Authorization;
using TestAPI2.Context;
using TestAPI2.Services;
using TestAPI2.Services.Interfaces;
using Serilog;
using Microsoft.AspNetCore.Authorization;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // Add services to the container
    string _connectionString = builder.Configuration.GetConnectionString("LocalDB");
builder.Services.AddControllers();


builder.Services
       .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
           options.Authority = "https://" + builder.Configuration["Auth0:Domain"] + "/";
           options.Audience = builder.Configuration["Auth0:Audience"];
           // If the access token does not have a `sub` claim, `User.Identity.Name` will be `null`. Map it to a different claim by setting the NameClaimType below.
           options.TokenValidationParameters = new TokenValidationParameters
           {
               NameClaimType = ClaimTypes.NameIdentifier
           };
       });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("read:Clientes", policy => policy.Requirements.Add(new HasScopeRequirement("read:Clientes", "https://" + builder.Configuration["Auth0:Domain"] + "/")));
    options.AddPolicy("write:Clientes", policy => policy.Requirements.Add(new HasScopeRequirement("read:Clientes", "https://" + builder.Configuration["Auth0:Domain"] + "/")));
    options.AddPolicy("delete:Clientes", policy => policy.Requirements.Add(new HasScopeRequirement("read:Clientes", "https://" + builder.Configuration["Auth0:Domain"] + "/")));
    options.AddPolicy("write:Productos", policy => policy.Requirements.Add(new HasScopeRequirement("read:Clientes", "https://" + builder.Configuration["Auth0:Domain"] + "/")));
    options.AddPolicy("delete:Productos", policy => policy.Requirements.Add(new HasScopeRequirement("read:Clientes", "https://" + builder.Configuration["Auth0:Domain"] + "/")));
});
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddDbContext<VentaContext>(opt => opt.UseSqlServer(_connectionString ?? throw new Exception("Missing Connection String"))/*, ServiceLifetime.Transient*/);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
            }
    });
});
builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
    var app = builder.Build();
    app.UseSerilogRequestLogging();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

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