using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net;
using System.Text;
using TestAPI2.Context;
using TestAPI2.Models;
using TestAPI2.Models.DTOs;
using TestAPI2.Services;
using TestAPI2.Services.Interfaces;

namespace TestAPI2.Modulos;

public class ClientModule : CarterModule
{
    public ClientModule()
    {
        
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        #region Clientes
        var clientes = app.MapGroup("/Clientes").WithTags("Clientes");

        clientes.MapGet("/", async (IClientService clientService) =>
        {
            return await clientService.GetClientesAsync()
                         is List<ClienteDto> clientes
                         ? Results.Ok(clientes)
                         : Results.NoContent();
        })
            .Produces<List<ClienteDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization("read:Clientes");

         clientes.MapGet("/CSV", async (IClientService clientService) =>
        {
            return await clientService.ClientesToCSV()
                         is IResult clientesCsv
                         ? clientesCsv
                         : Results.NoContent();
        })
            .Produces<IResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization("read:Clientes");

        clientes.MapGet("/{id:int}", async (int id, IClientService clientService) =>
        {
            return await clientService.GetClienteAsync(id)
                    is ClienteDto clienteDto
                        ? Results.Ok(clienteDto)
                        : Results.NotFound();
        })
            .Produces<ClienteDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        //.RequireAuthorization("read:Clientes");


        clientes.MapPost("/", async (Cliente cliente, IClientService clientService) =>
        {
            return await clientService.AddClienteAsync(cliente)
                is ClienteDto clienteDto
                    ? Results.Ok(clienteDto)
                    : Results.BadRequest();
        })
            .Produces<ClienteDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
        //.RequireAuthorization("write:Clientes");
        
        clientes.MapPost("/CSV", async (HttpRequest request, IClientService clientService) =>
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8);
            string filecontent = await reader.ReadToEndAsync();
            Console.WriteLine(filecontent);
            Console.WriteLine("File succesfully processed");
            return "File succesfully processed";/* await clientService.AddClienteAsync(cliente)
                is ClienteDto clienteDto
                    ? Results.Ok(clienteDto)
                    : Results.BadRequest();*/
        })
            .Accepts<IFormFile>("text/csv");


        clientes.MapPut("/{id:int}", async (Cliente cliente, IClientService clientService) =>
        {
            return await clientService.UpdateClienteAsync(cliente)
                is ClienteDto clienteDto
                    ? Results.Ok(clienteDto)
                    : Results.BadRequest();
        })
            .Produces<ClienteDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
        //.RequireAuthorization("write:Clientes");


        clientes.MapDelete("/{id:int}", async ([FromRoute] int id, IClientService clientService) =>
        {
            var result = await clientService.DeleteClienteAsync(id);
            return result.Item1 ? Results.Ok(result.Item2) : Results.BadRequest(result.Item2);
        })
            .Produces<(bool, string)>(StatusCodes.Status200OK)
            .Produces<(bool, string)>(StatusCodes.Status400BadRequest);
            //.RequireAuthorization("delete:Clientes");
        #endregion Clientes
    }
}
