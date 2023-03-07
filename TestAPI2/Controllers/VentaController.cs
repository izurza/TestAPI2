using Microsoft.AspNetCore.Mvc;
using TestAPI2.Models;
using TestAPI2.Services;

namespace TestAPI2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentaController : ControllerBase
    {

        private readonly IVentaService _ventaService;

        public VentaController(IVentaService ventaService)
        {
            _ventaService = ventaService;
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

            Console.WriteLine(nombreCliente);
            if (nombreCliente is null || apellidoCliente is null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Introduce nombre y apellido de cliente");
            }
            else
            {
                var clienteExistente = await _ventaService.GetClienteByNameAsync(nombreCliente, apellidoCliente);

                if (clienteExistente is null)
                {
                    Cliente nuevoCliente = new Cliente();
                    nuevoCliente.NombreCliente = nombreCliente;
                    nuevoCliente.ApellidoCliente = apellidoCliente;
                    await _ventaService.AddClienteAsync(nuevoCliente);
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

        [HttpDelete("DeleteClientCascada")]
        public async Task<IActionResult> DeleteClientCascada([FromForm] Cliente client)
        {

            //var facturas = await _ventaService.DeleteFacturasByClienteAsync(client);
            
            //Console.WriteLine(facturas.Item2);

            var result = await _ventaService.DeleteClienteAsync(client);
            return StatusCode(StatusCodes.Status200OK);//result.Item1 ? StatusCode(StatusCodes.Status200OK, result.Item2) : StatusCode(StatusCodes.Status400BadRequest, result.Item2); 
        }
        
        [HttpDelete("DeleteClientTransaction")]
        public async Task<IActionResult> DeleteClientTransaction([FromForm] Cliente client)
        { 
            var result = await _ventaService.DeleteClienteTransactionAsync(client);
            return StatusCode(StatusCodes.Status200OK);//result.Item1 ? StatusCode(StatusCodes.Status200OK, result.Item2) : StatusCode(StatusCodes.Status400BadRequest, result.Item2); 
        }




    }
}
