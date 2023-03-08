using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Configuration;
using TestAPI2.Context;
using TestAPI2.Models;
using TestAPI2.Models.DTOs;
using TestAPI2.Services.Interfaces;

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
        #region Facturas

        public async Task<FacturaDto?> GetFacturaAsync(int id)
        {
            try
            {
                return await _context.Facturas
                    .AsNoTracking()
                    .Where(i=> i.IdFactura == id)
                    .Select(f => new FacturaDto
                    {
                        Cliente = new ClienteDto
                        {
                            NombreCliente = f.FkClienteNavigation.NombreCliente,
                            ApellidoCliente = f.FkClienteNavigation.ApellidoCliente,
                            EmailCliente = f.FkClienteNavigation.EmailCliente,
                            DireccionCliente = f.FkClienteNavigation.DireccionCliente
                        },
                        Fecha = f.Fecha
                    })
                    .SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<FacturaDto>> GetFacturasAsync()
        {
            try
            {

                return await _context.Facturas
                    .AsNoTracking()
                    .Select(f => new FacturaDto
                    {
                        Cliente = new ClienteDto
                        {
                            NombreCliente = f.FkClienteNavigation.NombreCliente,
                            ApellidoCliente = f.FkClienteNavigation.ApellidoCliente,
                            EmailCliente = f.FkClienteNavigation.EmailCliente,
                            DireccionCliente = f.FkClienteNavigation.DireccionCliente
                        },
                        Fecha = f.Fecha
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<FacturaDto?> AddFacturaDtoAsync(Factura factura)
        {
            try
            {
                await _context.Facturas.AddAsync(factura);
                await _context.SaveChangesAsync();
                return await _context.Facturas
                    .AsNoTracking()
                    .Where(i=> i.IdFactura == factura.IdFactura)
                    .Select(f => new FacturaDto
                    {
                        Cliente = new ClienteDto
                        {
                            NombreCliente = f.FkClienteNavigation.NombreCliente,
                            ApellidoCliente = f.FkClienteNavigation.ApellidoCliente,
                            EmailCliente = f.FkClienteNavigation.EmailCliente,
                            DireccionCliente = f.FkClienteNavigation.DireccionCliente
                        },
                        Fecha = f.Fecha
                    })
                    .SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<Factura?> AddFacturaAsync(Factura factura)
        {
            try
            {
                await _context.Facturas.AddAsync(factura);
                await _context.SaveChangesAsync();
                return await _context.Facturas
                    .AsNoTracking()
                    .Where(i => i.IdFactura == factura.IdFactura)
                    .SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<FacturaDto?> UpdateFacturaAsync(Factura factura)
        {
            try
            {
                _context.Entry(factura).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return await _context.Facturas
                    .AsNoTracking()
                    .Where(i=>i.IdFactura == factura.IdFactura)
                    .Select(f => new FacturaDto
                    {
                        Cliente = new ClienteDto
                        {
                            NombreCliente = f.FkClienteNavigation.NombreCliente,
                            ApellidoCliente = f.FkClienteNavigation.ApellidoCliente,
                            EmailCliente = f.FkClienteNavigation.EmailCliente,
                            DireccionCliente = f.FkClienteNavigation.DireccionCliente
                        },
                        Fecha = f.Fecha
                    })
                    .SingleOrDefaultAsync();
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
        #region Detalles

        public async Task<DetalleDto?> GetDetalleAsync(int id)
        {
            try
            {
                return await _context.Detalles
                    .AsNoTracking()
                    .Where(i=>i.IdDetalle==id)
                    .Select(d=>new DetalleDto
                    {
                        Factura = new FacturaDto
                        {
                            Cliente = new ClienteDto
                            {
                                NombreCliente = d.FkFacturaNavigation.FkClienteNavigation.NombreCliente,
                                ApellidoCliente = d.FkFacturaNavigation.FkClienteNavigation.ApellidoCliente,
                                EmailCliente = d.FkFacturaNavigation.FkClienteNavigation.EmailCliente,
                                DireccionCliente = d.FkFacturaNavigation.FkClienteNavigation.DireccionCliente
                            },
                            Fecha = d.FkFacturaNavigation.Fecha
                        },
                        Cantidad = d.Cantidad,
                        Producto = new ProductoDto
                        {
                            NombreProducto = d.FkProductoNavigation.NombreProducto,
                            DescripcionProducto = d.FkProductoNavigation.DescripcionProducto,
                            PrecioProducto = d.FkProductoNavigation.PrecioProducto
                        }


                    })
                    .SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<DetalleDto>> GetDetallesAsync()
        {
            try
            {
                return await _context.Detalles
            .AsNoTracking()
            .Select(d => new DetalleDto
            {
                Factura = new FacturaDto
                {
                    Cliente = new ClienteDto
                    {
                        NombreCliente = d.FkFacturaNavigation.FkClienteNavigation.NombreCliente,
                        ApellidoCliente = d.FkFacturaNavigation.FkClienteNavigation.ApellidoCliente,
                        EmailCliente = d.FkFacturaNavigation.FkClienteNavigation.EmailCliente,
                        DireccionCliente = d.FkFacturaNavigation.FkClienteNavigation.DireccionCliente
                    },
                    Fecha = d.FkFacturaNavigation.Fecha
                },
                Cantidad = d.Cantidad,
                Producto = new ProductoDto
                {
                    NombreProducto = d.FkProductoNavigation.NombreProducto,
                    DescripcionProducto = d.FkProductoNavigation.DescripcionProducto,
                    PrecioProducto = d.FkProductoNavigation.PrecioProducto
                }


            })
            .ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<DetalleDto?> AddDetalleAsync(Detalle detalle)
        {
            try
            {
                _context.Detalles.AddAsync(detalle);
                await _context.SaveChangesAsync();
                return await _context.Detalles
                    .Where(i=> i.IdDetalle == detalle.IdDetalle)
                    .Select(d => new DetalleDto
                    {
                        Factura = new FacturaDto
                        {
                            Cliente = new ClienteDto
                            {
                                NombreCliente = d.FkFacturaNavigation.FkClienteNavigation.NombreCliente,
                                ApellidoCliente = d.FkFacturaNavigation.FkClienteNavigation.ApellidoCliente,
                                EmailCliente = d.FkFacturaNavigation.FkClienteNavigation.EmailCliente,
                                DireccionCliente = d.FkFacturaNavigation.FkClienteNavigation.DireccionCliente
                            },
                            Fecha = d.FkFacturaNavigation.Fecha
                        },
                        Cantidad = d.Cantidad,
                        Producto = new ProductoDto
                        {
                            NombreProducto = d.FkProductoNavigation.NombreProducto,
                            DescripcionProducto = d.FkProductoNavigation.DescripcionProducto,
                            PrecioProducto = d.FkProductoNavigation.PrecioProducto
                        }


                    })
                    .SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<DetalleDto?> UpdateDetalleAsync(Detalle detalle)
        {
            try
            {
                _context.Entry(detalle).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return await _context.Detalles
                    .Where(i => i.IdDetalle == detalle.IdDetalle)
                    .Select(d => new DetalleDto
                    {
                        Factura = new FacturaDto
                        {
                            Cliente = new ClienteDto
                            {
                                NombreCliente = d.FkFacturaNavigation.FkClienteNavigation.NombreCliente,
                                ApellidoCliente = d.FkFacturaNavigation.FkClienteNavigation.ApellidoCliente,
                                EmailCliente = d.FkFacturaNavigation.FkClienteNavigation.EmailCliente,
                                DireccionCliente = d.FkFacturaNavigation.FkClienteNavigation.DireccionCliente
                            },
                            Fecha = d.FkFacturaNavigation.Fecha
                        },
                        Cantidad = d.Cantidad,
                        Producto = new ProductoDto
                        {
                            NombreProducto = d.FkProductoNavigation.NombreProducto,
                            DescripcionProducto = d.FkProductoNavigation.DescripcionProducto,
                            PrecioProducto = d.FkProductoNavigation.PrecioProducto
                        }


                    })
                    .SingleOrDefaultAsync(); ;
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

       
        public async Task<int> FillDb()
        {

            //List<Cliente> list = new List<Cliente>();
            //List<Producto> list2 = new List<Producto>();
            //list.Add(new Cliente
            //{
            //    NombreCliente = "Papa",
            //ApellidoCliente = "Ratzinger",
            //   EmailCliente = "@gmail.com",
            //    DireccionCliente = "Roma"
            //});
            //list.Add(new Cliente
            //{
            //    NombreCliente = "Salma",
            //    ApellidoCliente = "Jayek",
            //    EmailCliente = "@gmail.com",
            //    DireccionCliente = "NY"
            //});
            //list.Add(new Cliente
            //{
            //    NombreCliente = "Jackie",
            //    ApellidoCliente = "Chan",
            //    EmailCliente = "@gmail.com",
            //    DireccionCliente = "China"
            //});

            //await _context.AddRangeAsync(list);
            //await _context.SaveChangesAsync();

            //foreach (var item in list)
            //{
            //    await _context.Clientes.AddAsync(item);
            //    await _context.SaveChangesAsync();
            //}

            //list2.Add(new Producto
            //{
            //    IdProducto = 0,
            //    NombreProducto = "chocolate",
            //    DescripcionProducto = "negro",
            //    PrecioProducto = new Random().NextDouble()*100
            //});
            //list2.Add(new Producto
            //{
            //    IdProducto = 19,
            //    NombreProducto = "platanos",
            //    DescripcionProducto = "canarias",
            //    PrecioProducto = new Random().NextDouble()*100
            //});
            //list2.Add(new Producto
            //{
            //    IdProducto = 64,
            //    NombreProducto = "Hamburgues",
            //    DescripcionProducto = "pollo",
            //    PrecioProducto = new Random().NextDouble()*100
            //});

            //foreach (var item in list2)
            //{
            //    await _context.Productos.AddAsync(item);
            //    await _context.SaveChangesAsync();
            //}
            List<int> randomClientes = await _context.Clientes
                .Select(i => i.IdCliente)
                .ToListAsync();
            List<int> randomProductos = await _context.Productos
                .Select(i => i.IdProducto)
                .ToListAsync();

            for (int i =0; i<50; i++)
            {
                Factura factura = new Factura
                {
                    FkCliente = randomClientes[new Random().Next(randomClientes.Count)],
                    Fecha = DateTime.Now
                };
                await _context.Facturas.AddAsync(factura);
                //await _context.SaveChangesAsync();

                Factura? facturaActual = await _context.Facturas.OrderBy(i => i.IdFactura).LastOrDefaultAsync();
                
                for (int x=0; x<new Random().Next(10);x++)
                {
                    Detalle detalle = new Detalle
                    {
                        FkFactura = facturaActual.IdFactura,
                        Cantidad = new Random().NextDouble() * 200,
                        FkProducto = randomProductos[new Random().Next(randomProductos.Count)]
                    };

                    await _context.Detalles.AddAsync(detalle);
                    //await _context.SaveChangesAsync();
                }
                //await _context.SaveChangesAsync();
            }
            await _context.SaveChangesAsync();
            return 1;
        }
    }
}
