using Microsoft.EntityFrameworkCore;
using TestAPI2.Context;
using TestAPI2.Models;
using TestAPI2.Models.DTOs;

namespace TestAPI2.Services
{
    public class VentaService : IVentaService
    {

        private readonly VentaContext _context;

        public VentaService(VentaContext context)
        {
            _context = context;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Clientes

        public async Task<Cliente> GetClienteAsync(int id)
        {
            try
            {
                return await _context.Clientes.FirstOrDefaultAsync(i => i.IdCliente == id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Cliente> GetClienteByNameAsync(string name, string apellido)
        {
            try
            {
                return await _context.Clientes.FirstOrDefaultAsync(i => i.NombreCliente.ToLower() == name.Trim().ToLower() && i.ApellidoCliente.ToLower() == apellido.Trim().ToLower());
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Cliente>> GetClientesAsync()
                {
                    try
                    {
                        return await _context.Clientes.AsNoTracking().ToListAsync();
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }


                public async Task<ClienteDto?> AddClienteAsync(Cliente client)
                {
                    try
                    {
                        await _context.Clientes.AddAsync(client);
                        await _context.SaveChangesAsync();
                //return await _context.Clientes.FindAsync(client.IdCliente);
                return await _context.Clientes.AsNoTracking()
                     .Where(e => e.IdCliente == client.IdCliente)
                     .Select(e => new ClienteDto
                     {
                         NombreCliente = e.NombreCliente
                     }).SingleOrDefaultAsync();
                        
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }

                public async Task<Cliente> UpdateClienteAsync(Cliente client)
                {
                    try
                    {
                        _context.Entry(client).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        return client;
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }

                public async Task<(bool, string)> DeleteClienteAsync(Cliente client)//Delete en cascada no mola
                {
                    try
                    {
                var dbClient = _context.Clientes
            .Where(i => i.IdCliente == client.IdCliente)
            .Include("Facturas.Detalles")
            .SingleOrDefault();
                       if (dbClient is null)
                        {
                            return (false, "Cliente no encontrado");
                        }

                        _context.Clientes.Remove(dbClient);
                        await _context.SaveChangesAsync();

                        return (true, "Cliente eliminado");
                    }
                    catch (Exception ex)
                    {
                        return (false, $"Error {ex.Message}");
                    }
                }

        public async Task<(bool, string)> DeleteClienteTransactionAsync(Cliente client)
        {
            
            using (var transaction = _context.Database.BeginTransaction())
                try
                {
                    var dbClient = _context.Clientes
             .Where(i => i.IdCliente == client.IdCliente)
             .SingleOrDefault();
                    if (dbClient is null)
                    {
                        return (false, "Cliente no encontrado");
                    }
                    var facturas = await _context.Facturas
                        .Where(i => i.FkCliente == client.IdCliente)
                        .ToListAsync();
                    foreach (var factura in facturas)
                    {
                        var detalles = await _context.Detalles
                            .Where(i => i.FkFactura == factura.IdFactura)
                            .ToListAsync();
                        foreach (var detalle in detalles)
                        {
                            _context.Remove(detalle);
                            await _context.SaveChangesAsync();
                        }
                        _context.Remove(factura);
                        await _context.SaveChangesAsync();
                    }
                    _context.Remove(dbClient);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    return (true, "Cliente eliminado");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return (false, $"Error {ex.Message}");
                }



        }

        #endregion Clientes
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Facturas

        public async Task<Factura> GetFacturaAsync(int id)
        {
            try
            {
                return await _context.Facturas.FirstOrDefaultAsync(i => i.IdFactura == id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Factura>> GetFacturasAsync()
        {
            try
            {

                return await _context.Facturas
            .Include(c => c.FkClienteNavigation)
            .ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<int> GetLastFacturaIdAsync()
        {
            var lastFactura = await _context.Facturas.OrderBy(i=>i.IdFactura).LastOrDefaultAsync();
            return lastFactura.IdFactura+1;
        }

        public async Task<Factura> AddFacturaAsync(Factura factura)
        {
            try
            {
                await _context.Facturas.AddAsync(factura);
                await _context.SaveChangesAsync();
                return await _context.Facturas.FindAsync(factura.IdFactura);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Factura> UpdateFacturaAsync(Factura factura)
        {
            try
            {
                _context.Entry(factura).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return factura;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<(bool, string)> DeleteFacturaAsync(Factura factura)
        {
            try
            {
                var dbFactura = await _context.Facturas.FindAsync(factura.IdFactura);

                if (dbFactura is null)
                {
                    return (false, "Factura no encontrada");
                }

                _context.Remove(factura);
                await _context.SaveChangesAsync();
                return (true, "Factura eliminada");
            }
            catch (Exception ex)
            {
                return (false, $"Error , {ex.Message}");
            }
        }

        public async Task<(bool, string)> DeleteFacturasByClienteAsync(Cliente client)
        {
            using var transaction = _context.Database.BeginTransaction();//
            try
            {
                var facturasCliente = await _context.Facturas.Where(i=>i.FkCliente == client.IdCliente).Include(d=>d.Detalles).ToListAsync();

                if (facturasCliente is null)
                {
                    return (false, "El Cliente no tiene facturas");
                }
                foreach(var factura in facturasCliente) 
                {
                    _context.Remove(factura);
                    await _context.SaveChangesAsync();
                }

                transaction.Commit();
                return (true, "Facturas eliminadas");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return (false, $"Error , {ex.Message}");
            }
        }

        #endregion Facturas
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Productos

        public async Task<Producto> GetProductoAsync(int id)
        {
            try
            {
                return await _context.Productos.FirstOrDefaultAsync(i => i.IdProducto == id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Producto>> GetProductosAsync()
        {
            try
            {
                return await _context.Productos.ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Producto> AddProductoAsync(Producto producto)
        {
            try
            {
                await _context.Productos.AddAsync(producto);
                await _context.SaveChangesAsync();
                return await _context.Productos.FindAsync(producto.IdProducto);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<Producto> UpdateProductoAsync(Producto producto)
        {
            try
            {
                _context.Entry(producto).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return producto;
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
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Detalles

        public async Task<Detalle> GetDetalleAsync(int id)
        {
            try
            {
                return await _context.Detalles.FirstOrDefaultAsync(i => i.IdDetalle == id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Detalle>> GetDetallesAsync()
        {
            try
            {
                return await _context.Detalles
            .AsNoTracking()
            .Include("FkFacturaNavigation.FkClienteNavigation")
            //.Include(c => c.FkFacturaNavigation.FkClienteNavigation)
            .Include(p => p.FkProductoNavigation)
            .ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Detalle> AddDetalleAsync(Detalle detalle)
        {
            try
            {
                _context.Detalles.AddAsync(detalle);
                await _context.SaveChangesAsync();
                return await _context.Detalles.FindAsync(detalle.IdDetalle);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Detalle> UpdateDetalleAsync(Detalle detalle)
        {
            try
            {
                _context.Entry(detalle).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return detalle;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<(bool, string)> DeleteDetalleAsync(Detalle detalle)
        {
            try
            {
                var dbDetalle = await _context.Detalles.FindAsync(detalle.IdDetalle);

                if (dbDetalle is null)
                {
                    return (false, "Detalle no encontrado");
                }

                _context.Remove(detalle);
                await _context.SaveChangesAsync();
                return (true, "Detalle eliminado");
            }
            catch (Exception ex)
            {
                return (false, $"Error {ex.Message}");

            }
        }
        #endregion Detalles
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //public async Task<T> GetVenta()
        //{

        //}

    }
}
