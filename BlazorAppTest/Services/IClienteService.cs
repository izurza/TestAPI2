using Microsoft.AspNetCore.Components.Forms;
using TestAPI2.Models.DTOs;

namespace BlazorAppTest.Services;

public interface IClienteService
{
    Task<List<ClienteDto>> GetClientes();
    Task AddClientes();
    Task DeleteCliente(int id);
    Task<byte[]> GetCSV();
    Task UploadCSV(IBrowserFile file);
}
