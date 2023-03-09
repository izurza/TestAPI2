using Microsoft.EntityFrameworkCore;
using TestAPI2.Services;
using TestAPI2.Services.Interfaces;
using Carter;
using TestAPI2.Extensiones;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .WriteTo.Console()
    //.WriteTo.Debug()
    .CreateBootstrapLogger();
    //.CreateLogger();

try
{
    Log.Information("Starting web application");
    var builder = WebApplication.CreateBuilder(args);

    //builder.Host.UseSerilog();
  
    string _connectionString = builder.Configuration.GetConnectionString("LocalDB");


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


    builder.Host.UseSerilog((context, services, configuration) => configuration
      .ReadFrom.Configuration(context.Configuration)
      .ReadFrom.Services(services)
      .Enrich.FromLogContext()
      .WriteTo.File(new CompactJsonFormatter(), "./logs/myapp.json")
      .WriteTo.Console());

    var app = builder.Build();

    //app.UseConfiguredSerilogRequestLogging();
    app.UseSerilogRequestLogging();//Set before noisy handlers to exclude them from logging
    

    app.MapSwagger();

    app.UseHttpsRedirection();

    //app.UseAuthorization();

    app.MapCarter();

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