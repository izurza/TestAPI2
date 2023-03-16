using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TestAPI2.Models.DTOs;

using static BlazorAppTest.Services.TokenService;

namespace BlazorAppTest.Services
{
    public class ClienteService : IClienteService
    {
        private readonly HttpClient _http;
        

        public ClienteService(IConfiguration configuration, HttpClient http)
        {
            _http = http;
            TokenService ts = new TokenService(configuration,http);
            string access = ts.GetAccessToken();
            _http.DefaultRequestHeaders.Add("Authorization",ts.GetAccessToken());
        }
        public Task AddClientes()
        {
            throw new NotImplementedException();
        }

        public Task DeleteCliente(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ClienteDto>?> GetClientes()
        {
            
            return await _http.GetFromJsonAsync<List<ClienteDto>>($"Clientes");
        }

        public async Task<byte[]> GetCSV()
        {
            var res = await _http.GetStreamAsync("/Clientes/CSV");
            using var ms = new MemoryStream();
            await res.CopyToAsync(ms);
            return ms.GetBuffer();
        }

        public async Task UploadCSV(IBrowserFile file)
        {
            using var ms = new MemoryStream();
            await file.OpenReadStream().CopyToAsync(ms);
            await _http.PostAsync("/Clientes/CSV", new ByteArrayContent(ms.GetBuffer()));
        }
    }
}
