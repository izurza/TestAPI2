using TestAPI2.Models.DTOs;
using TestAPI2.Models;

namespace TestAPI2.Services.Interfaces
{
    public interface IClientService
    {
        //Clientes
        Task<List<ClienteDto>> GetClientesAsync();//Get ALL Clientes
        Task<ClienteDto> GetClienteAsync(int id);//Get a single Client
        Task<Cliente> GetClienteByNameAsync(string name, string apellido);
        Task<ClienteDto> AddClienteAsync(Cliente client);//POST Client
        Task<ClienteDto> UpdateClienteAsync(Cliente client);//PUT
        Task<(bool, string)> DeleteClienteAsync(Cliente client);//DELETE
        Task<(bool, string)> DeleteClienteTransactionAsync(int clientId);
    }
}
