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
using TestAPI2.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // Add services to the container
    string _connectionString = builder.Configuration.GetConnectionString("LocalDB");
    string _domain = "https://" + builder.Configuration["Auth0:Domain"] + "/";
    builder.Services.AddControllers();


    builder.Services
           .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.Authority = _domain;
               options.Audience = builder.Configuration["Auth0:Audience"];
               // If the access token does not have a `sub` claim, `User.Identity.Name` will be `null`. Map it to a different claim by setting the NameClaimType below.
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   NameClaimType = ClaimTypes.NameIdentifier
               };
           });
    builder.Services.AddConfiguredPoliciesAuthorization(_domain);
    builder.Services.ScanServices();
    builder.Services.ConfigureJsonOption();
    builder.Services.AddPersistence(_connectionString);
    builder.Services.AddSwagger();
    
    builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
    var app = builder.Build();
    app.UseSerilogRequestLogging();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapSwagger();
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