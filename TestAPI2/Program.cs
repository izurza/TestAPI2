using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using TestAPI2;
using TestAPI2.Context;
using TestAPI2.Services;
using TestAPI2.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container
string _connectionString = builder.Configuration.GetConnectionString("LocalDB");
builder.Services.AddControllers();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddDbContext<VentaContext>(opt => opt.UseSqlServer(_connectionString ?? throw new Exception("Missing Connection String"))/*, ServiceLifetime.Transient*/);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
