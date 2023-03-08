using TestAPI2.Models.DTOs;
using TestAPI2.Models;

namespace TestAPI2.Services.Interfaces
{
    public interface IProductService
    {
        //Producto
        Task<List<ProductoDto>> GetProductosAsync();//Get ALL 
        Task<ProductoDto> GetProductoAsync(int id);//Get single
        Task<ProductoDto> AddProductoAsync(Producto producto);//POST 
        Task<ProductoDto> UpdateProductoAsync(Producto producto);//PUT
        Task<(bool, string)> DeleteProductoAsync(int productoId);//DELETE
    }
}
