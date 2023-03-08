using System.Collections.Generic;
using System.Threading.Tasks;
using TestAPI2.Models;
using TestAPI2.Models.DTOs;

namespace TestAPI2.Services.Interfaces
{
    public interface IVentaService
    {
       
        //Factura
        Task<List<FacturaDto>> GetFacturasAsync();//Get ALL 
        Task<FacturaDto> GetFacturaAsync(int id);//Get single
        Task<int> GetLastFacturaIdAsync();//
        Task<Factura> AddFacturaAsync(Factura factura);//POST 
        Task<FacturaDto?> AddFacturaDtoAsync(Factura factura);
        Task<FacturaDto> UpdateFacturaAsync(Factura factura);//PUT
        Task<(bool, string)> DeleteFacturaAsync(Factura factura);//DELETE
        Task<(bool, string)> DeleteFacturasByClienteAsync(Cliente client);

        //Detalle
        Task<List<DetalleDto>> GetDetallesAsync();//Get ALL 
        Task<DetalleDto> GetDetalleAsync(int id);//Get single
        Task<DetalleDto> AddDetalleAsync(Detalle detalle);//POST 
        Task<DetalleDto> UpdateDetalleAsync(Detalle detalle);//PUT
        Task<(bool, string)> DeleteDetalleAsync(Detalle detalle);//DELETE

        Task<int> FillDb();
    }
}
