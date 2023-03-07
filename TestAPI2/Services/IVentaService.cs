using System.Collections.Generic;
using System.Threading.Tasks;
using TestAPI2.Models;
using TestAPI2.Models.DTOs;

namespace TestAPI2.Services
{
    public interface IVentaService
    {
        //Clientes
        Task<List<Cliente>> GetClientesAsync();//Get ALL Clientes
        Task<Cliente> GetClienteAsync(int id);//Get a single Client
        Task<Cliente> GetClienteByNameAsync(string name, string apellido);
        Task<ClienteDto> AddClienteAsync(Cliente client);//POST Client
        Task<Cliente> UpdateClienteAsync(Cliente client);//PUT
        Task<(bool,string)> DeleteClienteAsync(Cliente client);//DELETE
        Task<(bool, string)> DeleteClienteTransactionAsync(Cliente client);

        //Producto
        Task<List<Producto>> GetProductosAsync();//Get ALL 
        Task<Producto> GetProductoAsync(int id);//Get single
        Task<Producto> AddProductoAsync(Producto producto);//POST 
        Task<Producto> UpdateProductoAsync(Producto producto);//PUT
        Task<(bool, string)> DeleteProductoAsync(Producto producto);//DELETE

        //Factura
        Task<List<Factura>> GetFacturasAsync();//Get ALL 
        Task<Factura> GetFacturaAsync(int id);//Get single
        Task<int> GetLastFacturaIdAsync();//
        Task<Factura> AddFacturaAsync(Factura factura);//POST 
        Task<Factura> UpdateFacturaAsync(Factura factura);//PUT
        Task<(bool, string)> DeleteFacturaAsync(Factura factura);//DELETE
        Task<(bool, string)> DeleteFacturasByClienteAsync(Cliente client);

        //Detalle
        Task<List<Detalle>> GetDetallesAsync();//Get ALL 
        Task<Detalle> GetDetalleAsync(int id);//Get single
        Task<Detalle> AddDetalleAsync(Detalle detalle);//POST 
        Task<Detalle> UpdateDetalleAsync(Detalle detalle);//PUT
        Task<(bool, string)> DeleteDetalleAsync(Detalle detalle);//DELETE
    }
}
