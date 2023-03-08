using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using TestAPI2;
using TestAPI2.Models.DTOs;
using TestAPI2.Context;
using TestAPI2.Services;
using TestAPI2.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using TestAPI2.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.DependencyResolver;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container
string _connectionString = builder.Configuration.GetConnectionString("LocalDB");
//builder.Services.AddControllers();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<IClientService, ClientService>();
//builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddDbContext<VentaContext>(opt => opt.UseSqlServer(_connectionString ?? throw new Exception("Missing Connection String"))/*, ServiceLifetime.Transient*/);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
#region Clientes

var clientes = app.MapGroup("/Clientes");

clientes.MapGet("/Clientes", GetClientesFromService);

clientes.MapGet("/Clientes2", GetClientesFromContext);

clientes.MapGet("/Clientes/{id:int}", GetCliente);

clientes.MapPost("/Clientes", AddCliente);

clientes.MapPut("/Clientes/{id:int}", UpdateCliente);

clientes.MapDelete("/Clientes/{id:int}", DeleteClient);

#endregion Clientes

#region Productos

var productos = app.MapGroup("/Productos");

productos.MapGet("/", GetProductos);

productos.MapGet("/{id:int}", GetProducto);

productos.MapPost("", AddProducto);

productos.MapPut("/{id:int}", UpdateProducto);

productos.MapDelete("/{id:int}", DeleteProducto);
#endregion Productos

#region Ventas
var ventas = app.MapGroup("/Ventas");
ventas.MapGet("/", GetVentas);

ventas.MapGet("/{id:int}", GetDetalle);

ventas.MapGet("/Facturas", GetFacturas);

ventas.MapPost("/Comprar", Comprar);
#endregion Ventas
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

app.Run();

#region Client Functions
static async Task<IResult> GetClientesFromService(IClientService clientService) {
    //Con Servicio
  return  await clientService.GetClientesAsync()
    is List<ClienteDto> clientes
    ? Results.Ok(clientes)
    : Results.NoContent();
}
static async Task<IResult> GetClientesFromContext(VentaContext context) { //Con Contexto
 return   await context.Clientes.ToListAsync()
     is List<Cliente> clientes
        ? Results.Ok(clientes)
        : Results.NoContent();
}
static async Task<IResult> GetCliente(int id, IClientService clientService)
{
    return await clientService.GetClienteAsync(id)
            is ClienteDto clienteDto
                ? Results.Ok(clienteDto)
                : Results.NotFound()
}
static async Task<IResult> AddCliente(/*[FromForm]*/ Cliente cliente, IClientService clientService)
{
    return await clientService.AddClienteAsync(cliente)
        is ClienteDto clienteDto
            ? Results.Ok(clienteDto)
            : Results.BadRequest();
}
static async Task<IResult> UpdateCliente(Cliente cliente, IClientService clientService) {
    return await clientService.UpdateClienteAsync(cliente)
        is ClienteDto clienteDto
            ? Results.Ok(clienteDto)
            : Results.BadRequest();
}
static async Task<IResult> DeleteClient([FromRoute] int id, IClientService clientService)
{
    var result = await clientService.DeleteClienteAsync(id);
    return result.Item1 ? Results.Ok(result.Item2) : Results.BadRequest(result.Item2);
}
#endregion Client Funcions

#region Product Functions
static async Task<IResult> GetProductos(IProductService productService)
{
    return await productService.GetProductosAsync()
        is List<ProductoDto> products
            ? Results.Ok(products)
            : Results.NotFound();
}
static async Task<IResult> GetProducto([FromRoute] int id, IProductService productService)
{
    return await productService.GetProductoAsync(id)
        is ProductoDto productoDto
            ? Results.Ok(productoDto)
            : Results.NotFound();
}
static async Task<IResult> AddProducto(Producto product, IProductService productService)
{
    return await productService.AddProductoAsync(product)
        is ProductoDto productoDto
            ? Results.Ok(productoDto)
            : Results.BadRequest();
}
static async Task<IResult> UpdateProducto(Producto product, IProductService productService)
{
    return await productService.UpdateProductoAsync(product)
        is ProductoDto productoDto
            ? Results.Ok(productoDto)
            : Results.BadRequest();
}
static async Task<IResult> DeleteProducto([FromRoute] int id, IProductService productService)
{
    var result = await productService.DeleteProductoAsync(id);
    return result.Item1 ? Results.Ok(result.Item2) : Results.BadRequest(result.Item2);
}
#endregion Product Functions

#region Ventas Functions
static async Task<IResult> GetVentas(IVentaService ventaService)
{
    return await ventaService.GetDetallesAsync()
        is List<DetalleDto> detallesDto
            ? Results.Ok(detallesDto)
            : Results.NoContent();
}
static async Task<IResult> GetDetalle([FromRoute] int id, IVentaService ventaService)
{
    return await ventaService.GetDetalleAsync(id)
        is DetalleDto detalleDto
            ? Results.Ok(detalleDto)
            : Results.NotFound();
}
static async Task<IResult> GetFacturas(IVentaService ventaService) 
{
    return await ventaService.GetFacturasAsync()
        is List<FacturaDto> facturasDto
            ? Results.Ok(facturasDto)
            : Results.NoContent();
}
static async Task<IResult> Comprar(IProductService productService, IClientService clientService, IVentaService ventaService)
{
    Factura facturaActual = await ventaService.AddFacturaAsync(new Factura
    {
        Fecha = DateTime.Now,
        FkCliente = 4
    });

    for (int i = 0; i < 2; i++)
    {
        Detalle detalle = new Detalle
        {
            FkFactura = facturaActual.IdFactura,
            Cantidad = 2,
            FkProducto = 89
        };
        //facturaActual.Detalles.Add(detalle);
        await ventaService.AddDetalleAsync(detalle);

    }
    return Results.Ok(facturaActual);
}
#endregion Ventas Functions