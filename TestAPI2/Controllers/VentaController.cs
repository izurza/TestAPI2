using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Net.Mime;
using TestAPI2.Models;
using TestAPI2.Services.Interfaces;

namespace TestAPI2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VentaController : ControllerBase
{

    private readonly IVentaService _ventaService;
    private readonly IClientService _clientService;

    public VentaController(IVentaService ventaService, IClientService clientService)
    {
        _ventaService = ventaService;
        _clientService = clientService;
    }

    [HttpGet("ventas")]
    public async Task<IActionResult> GetVentas()
    {
        var ventas = await _ventaService.GetDetallesAsync();

        if (ventas is null) 
        {
            return StatusCode(StatusCodes.Status204NoContent, "No se han encontrado ventas");
        }

        return StatusCode(StatusCodes.Status200OK, ventas);
    }
    [HttpGet("detalle/{id}")]
    public async Task<IActionResult> GetDetalle(int id)
    {
        var detalle = await _ventaService.GetDetalleAsync(id);

        if (detalle is null)
        {
            return StatusCode(StatusCodes.Status204NoContent, "No se han encontrado detalle");
        }

        return StatusCode(StatusCodes.Status200OK, detalle);
    }

    

    [HttpGet("Facturas")]
    public async Task<IActionResult> GetFacturas()
    {
        var facturas = await _ventaService.GetFacturasAsync();

        if (facturas is null)
        {
            return StatusCode(StatusCodes.Status204NoContent, "No se han encontrado ventas");
        }

        return StatusCode(StatusCodes.Status200OK, facturas);
    }

    [HttpPost("Comprar")]
    public async Task<IActionResult> Comprar([FromForm] string nombreCliente, [FromForm] string apellidoCliente, [FromForm] int[] cantidades, [FromForm] int[] productos)//), [FromForm] string nombre)
    {

        if (nombreCliente is null || apellidoCliente is null)
        {
            return StatusCode(StatusCodes.Status400BadRequest, "Introduce nombre y apellido de cliente");
        }
        else
        {
            var clienteExistente = await /*_ventaService*/_clientService.GetClienteByNameAsync(nombreCliente, apellidoCliente);

            if (clienteExistente is null)
            {
                Cliente nuevoCliente = new Cliente();
                nuevoCliente.NombreCliente = nombreCliente;
                nuevoCliente.ApellidoCliente = apellidoCliente;
                await /*_ventaService*/_clientService.AddClienteAsync(nuevoCliente);
                clienteExistente = nuevoCliente;
            }

            Factura factura = new Factura();
            
            factura.FkCliente = clienteExistente.IdCliente;
            factura.Fecha = DateTime.Now;
            Factura facturaActual = await _ventaService.AddFacturaAsync(factura);

            for (int i =0; i<cantidades.Length; i++)
            {
                Detalle detalle = new Detalle();
                detalle.FkFactura = facturaActual.IdFactura;
                detalle.Cantidad = cantidades[i];
                detalle.FkProducto = productos[i];
                facturaActual.Detalles.Add(detalle);
                await _ventaService.AddDetalleAsync(detalle);
            }
            //foreach (var cantidad_producto in cesta)
            //{
            //    Detalle detalle = new Detalle();
            //    detalle.FkFactura = factura.IdFactura;
            //    detalle.Cantidad = cantidad_producto[0];
            //    detalle.FkProducto = cantidad_producto[1];
            //    await _ventaService.AddDetalleAsync(detalle);
            //}

            return StatusCode(StatusCodes.Status201Created, facturaActual);
        }
        

    }

 

    [HttpPost("Fill")]
    public async Task<IActionResult> FillDb()
    {

        await _ventaService.FillDb();

        return StatusCode(StatusCodes.Status200OK);
    }

    





}
