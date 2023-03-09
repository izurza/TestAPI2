using Carter;
using Microsoft.AspNetCore.Mvc;
using TestAPI2.Models.DTOs;
using TestAPI2.Models;
using TestAPI2.Services.Interfaces;
using TestAPI2.Services;

namespace TestAPI2.Modulos;

public class VentasModule : CarterModule
{
    public VentasModule()
    {

    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        #region Ventas
        var ventas = app.MapGroup("/Ventas").WithTags("Ventas");

        ventas.MapGet("/", async(IVentaService ventaService) =>
        {
            return await ventaService.GetDetallesAsync()
                is List<DetalleDto> detallesDto
                    ? Results.Ok(detallesDto)
                    : Results.NoContent();
        })
            .Produces<List<DetalleDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent);

        ventas.MapGet("/{id:int}", async ([FromRoute] int id, IVentaService ventaService) =>
    {
            return await ventaService.GetDetalleAsync(id)
                is DetalleDto detalleDto
                    ? Results.Ok(detalleDto)
                    : Results.NotFound();
        })
            .Produces<DetalleDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        ventas.MapGet("/Facturas", async (IVentaService ventaService) =>
    {
            return await ventaService.GetFacturasAsync()
                is List<FacturaDto> facturasDto
                    ? Results.Ok(facturasDto)
                    : Results.NoContent();
        })
            .Produces<List<FacturaDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent);
            
        ventas.MapPost("/Comprar", async (IProductService productService, IClientService clientService, IVentaService ventaService) =>
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
        })
            .Produces<Factura>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
        #endregion Ventas
    }
}
