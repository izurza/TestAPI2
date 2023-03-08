using TestAPI2.Models.DTOs;
using TestAPI2.Models;

namespace TestAPI2.Services.Interfaces
{
    public interface IClientService
    {
        //Clientes
        Task<List<ClienteDto>> GetClientesAsync();//Get ALL Clientes
        Task<ClienteDto> GetClienteAsync(int id);//Get a single Client
        Task<byte[]> ClientesToCSV();
        Task<Cliente> GetClienteByNameAsync(string name, string apellido);
        Task<ClienteDto> AddClienteAsync(Cliente client);//POST Client
        Task<int> ImportClientesFromCSVAsync(IFormFile clientCSV);
        Task<ClienteDto> UpdateClienteAsync(Cliente client);//PUT
        Task<(bool, string)> DeleteClienteAsync(int clientId);//DELETE
        Task<(bool, string)> DeleteClienteTransactionAsync(int clientId);
        Task<(bool, string)> DeleteTestAsync();
    }
}
