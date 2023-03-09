using Microsoft.EntityFrameworkCore;
using TestAPI2.Services;
using TestAPI2.Services.Interfaces;
using Carter;
using TestAPI2.Extensiones;

var builder = WebApplication.CreateBuilder(args);

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

//builder.Services.AddControllers(); // Not needed on Minimal APIs

//builder.Services.AddScoped<IProductService, ProductService>();
//builder.Services.AddScoped<IVentaService, VentaService>();
//builder.Services.AddScoped<IClientService, ClientService>();


builder.Services.AddPersistence(_connectionString);// Replaces the line below
//builder.Services.AddDbContext<VentaContext>(opt => opt.UseSqlServer(_connectionString ?? throw new Exception("Missing Connection String"))/*, ServiceLifetime.Transient*/);

builder.Services.AddSwagger();//Replaces the 2 below
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddCarter();

var app = builder.Build();



app.MapSwagger();

app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();// Not needed on Minimal APIs
app.MapCarter();

app.Run();
