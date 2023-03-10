using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using TestAPI2.Context;
using TestAPI2.Models.DTOs;
using TestAPI2.Models;
using TestAPI2.Services.Interfaces;

namespace TestAPI2.Services
{
    public class ProductService : IProductService, IScopedServices
    {

        private readonly VentaContext _context;

        public ProductService(VentaContext context) 
        { 
            _context = context;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Productos

        #region Get
        public async Task<ProductoDto?> GetProductoAsync(int id)
        {
            try
            {
                return await _context.Productos
                    .AsNoTracking()
                    .Where(i => i.IdProducto == id)
                    .Select(p => new ProductoDto
                    {
                        NombreProducto = p.NombreProducto,
                        DescripcionProducto = p.DescripcionProducto,
                        PrecioProducto = p.PrecioProducto
                    })
                    .SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<ProductoDto>> GetProductosAsync()
        {
            try
            {
                return await _context.Productos
                    .AsNoTracking()
                    .Select(p => new ProductoDto
                    {
                        NombreProducto = p.NombreProducto,
                        DescripcionProducto = p.DescripcionProducto,
                        PrecioProducto = p.PrecioProducto
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
#endregion Get

        public async Task<ProductoDto?> AddProductoAsync(Producto producto)
        {
            try
            {
                await _context.Productos.AddAsync(producto);
                await _context.SaveChangesAsync();
                return await _context.Productos
                    .AsNoTracking()
                    .Where(i => i.IdProducto == producto.IdProducto)
                    .Select(p => new ProductoDto
                    {
                        NombreProducto = p.NombreProducto,
                        DescripcionProducto = p.DescripcionProducto,
                        PrecioProducto = p.PrecioProducto
                    })
                    .SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ProductoDto?> UpdateProductoAsync(Producto producto)
        {
            try
            {
                _context.Entry(producto).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return await _context.Productos
                    .AsNoTracking()
                    .Where(i => i.IdProducto == producto.IdProducto)
                    .Select(p => new ProductoDto
                    {
                        NombreProducto = p.NombreProducto,
                        DescripcionProducto = p.DescripcionProducto,
                        PrecioProducto = p.PrecioProducto
                    })
                    .SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<(bool, string)> DeleteProductoAsync(Producto producto)
        {
            try
            {
                var dbProduct = await _context.Productos.FindAsync(producto.IdProducto);

                if (dbProduct is null)
                {
                    return (false, "Producto no encontrado");
                }

                _context.Remove(producto);
                await _context.SaveChangesAsync();

                return (true, "Producto eliminado");
            }
            catch (Exception ex)
            {
                return (false, $"Error {ex.Message}");
            }
        }

        #endregion Productos



    }
}
