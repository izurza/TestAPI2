﻿using Microsoft.AspNetCore.Mvc;
using TestAPI2.Models;
using TestAPI2.Services;
using TestAPI2.Services.Interfaces;

namespace TestAPI2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }


        [HttpGet("clientes")]
        public async Task<IActionResult> GetClientes()
        {
            var clientes = await _clientService.GetClientesAsync();

            if (clientes is null)
            {
                return StatusCode(StatusCodes.Status204NoContent, "No se han encontrado clientes");
            }

            return StatusCode(StatusCodes.Status200OK, clientes);
        }


        [HttpPost]
        public async Task<IActionResult> DownloadCustomers()
        {



            //lo que sea
            return File(null, "text/csv", "Clientes.csv");// --> Array de bytes y el content string de un csv
        }


        [HttpDelete("DeleteClientCascada")]
        public async Task<IActionResult> DeleteClientCascada([FromForm] Cliente client)
        {

            //var facturas = await _ventaService.DeleteFacturasByClienteAsync(client);

            //Console.WriteLine(facturas.Item2);

            var result = await _clientService.DeleteClienteAsync(client);
            return StatusCode(StatusCodes.Status200OK);//result.Item1 ? StatusCode(StatusCodes.Status200OK, result.Item2) : StatusCode(StatusCodes.Status400BadRequest, result.Item2); 
        }

        [HttpDelete("DeleteClientTransaction/{clientId:int}")]
        public async Task<IActionResult> DeleteClientTransaction([FromRoute] int clientId)
        {
            var result = await _clientService.DeleteClienteTransactionAsync(clientId);
            return StatusCode(StatusCodes.Status200OK);//result.Item1 ? StatusCode(StatusCodes.Status200OK, result.Item2) : StatusCode(StatusCodes.Status400BadRequest, result.Item2); 
        }
    }
}