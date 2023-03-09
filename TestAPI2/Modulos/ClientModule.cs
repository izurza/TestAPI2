using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAPI2.Context;
using TestAPI2.Models;
using TestAPI2.Models.DTOs;
using TestAPI2.Services.Interfaces;

namespace TestAPI2.Modulos;

public class ClientModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        #region Clientes
        var clientes = app.MapGroup("/Clientes");

        clientes.MapGet("/Clientes", GetClientesFromService)
            .Produces<List<ClienteDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent);

        clientes.MapGet("/Clientes2", GetClientesFromContext)
            .Produces<List<ClienteDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent);


        clientes.MapGet("/Clientes/{id:int}", GetCliente)
            .Produces<ClienteDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);


        clientes.MapPost("/Clientes", AddCliente)
            .Produces<ClienteDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);


        clientes.MapPut("/Clientes/{id:int}", UpdateCliente)
            .Produces<ClienteDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);


        clientes.MapDelete("/Clientes/{id:int}", DeleteClient)
            .Produces<(bool, string)>(StatusCodes.Status200OK)
            .Produces<(bool, string)>(StatusCodes.Status400BadRequest);
        #endregion Clientes=
    }
    #region Client Functions
    static async Task<IResult> GetClientesFromService(IClientService clientService)
    {
        //Con Servicio
        return await clientService.GetClientesAsync()
          is List<ClienteDto> clientes
          ? Results.Ok(clientes)
          : Results.NoContent();
    }
    static async Task<IResult> GetClientesFromContext(VentaContext context)
    { //Con Contexto
        return await context.Clientes.ToListAsync()
            is List<Cliente> clientes
               ? Results.Ok(clientes)
               : Results.NoContent();
    }
    static async Task<IResult> GetCliente(int id, IClientService clientService)
    {
        return await clientService.GetClienteAsync(id)
                is ClienteDto clienteDto
                    ? Results.Ok(clienteDto)
                    : Results.NotFound();
    }
    static async Task<IResult> AddCliente(/*[FromForm]*/ Cliente cliente, IClientService clientService)
    {
        return await clientService.AddClienteAsync(cliente)
            is ClienteDto clienteDto
                ? Results.Ok(clienteDto)
                : Results.BadRequest();
    }
    static async Task<IResult> UpdateCliente(Cliente cliente, IClientService clientService)
    {
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
}
