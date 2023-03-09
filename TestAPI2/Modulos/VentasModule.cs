using Carter;
using Microsoft.AspNetCore.Mvc;
using TestAPI2.Models.DTOs;
using TestAPI2.Models;
using TestAPI2.Services.Interfaces;

namespace TestAPI2.Modulos;

public class VentasModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        #region Ventas
        var ventas = app.MapGroup("/Ventas");
        ventas.MapGet("/", GetVentas);

        ventas.MapGet("/{id:int}", GetDetalle);

        ventas.MapGet("/Facturas", GetFacturas);

        ventas.MapPost("/Comprar", Comprar);
        #endregion Ventas
    }
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
}
