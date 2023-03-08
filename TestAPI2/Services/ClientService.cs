using Microsoft.EntityFrameworkCore;
using TestAPI2.Models.DTOs;
using TestAPI2.Models;
using TestAPI2.Services.Interfaces;
using TestAPI2.Context;

namespace TestAPI2.Services
{
    public class ClientService : IClientService
    {

        private readonly VentaContext _context;

        public ClientService(VentaContext context)
        {
            _context = context;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Clientes

        public async Task<ClienteDto?> GetClienteAsync(int id)
        {
            try
            {
                return await _context.Clientes
                    .AsNoTracking()
                    .Where(i => i.IdCliente == id)
                    .Select(e => new ClienteDto
                    {
                        NombreCliente = e.NombreCliente.Trim(),
                        ApellidoCliente = e.ApellidoCliente.Trim(),
                        EmailCliente = e.EmailCliente.Trim(),
                        DireccionCliente = e.DireccionCliente.Trim()
                    })
                    .SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Cliente?> GetClienteByNameAsync(string name, string apellido)
        {
            try
            {
                return await _context.Clientes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.NombreCliente.ToLower() == name.Trim().ToLower() && i.ApellidoCliente.ToLower() == apellido.Trim().ToLower());
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<ClienteDto>> GetClientesAsync()
        {
            try
            {
                return await _context.Clientes
            .AsNoTracking()
            .Select(e => new ClienteDto
            {
                NombreCliente = e.NombreCliente.Trim(),
                ApellidoCliente = e.ApellidoCliente.Trim(),
                EmailCliente = e.EmailCliente.Trim(),
                DireccionCliente = e.DireccionCliente.Trim()
            })
            .ToListAsync();
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
                return await _context.Clientes
                     .AsNoTracking()
                     .Where(e => e.IdCliente == client.IdCliente)
                     .Select(e => new ClienteDto
                     {
                         NombreCliente = e.NombreCliente.Trim(),
                         ApellidoCliente = e.ApellidoCliente.Trim(),
                         EmailCliente = e.EmailCliente.Trim(),
                         DireccionCliente = e.DireccionCliente.Trim()
                     })
                     .SingleOrDefaultAsync();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ClienteDto?> UpdateClienteAsync(Cliente client)
        {
            try
            {
                _context.Entry(client).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return await _context.Clientes
                     .AsNoTracking()
                     .Where(e => e.IdCliente == client.IdCliente)
                     .Select(e => new ClienteDto
                     {
                         NombreCliente = e.NombreCliente.Trim(),
                         ApellidoCliente = e.ApellidoCliente.Trim(),
                         EmailCliente = e.EmailCliente.Trim(),
                         DireccionCliente = e.DireccionCliente.Trim()
                     })
                     .SingleOrDefaultAsync();
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

        public async Task<(bool, string)> DeleteClienteTransactionAsync(int clientId)
        {

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var dbClient = await _context.Clientes
         .Where(i => i.IdCliente == clientId)
         .SingleOrDefaultAsync();
                if (dbClient is null)
                {
                    return (false, "Cliente no encontrado");
                }
                var facturas = await _context.Facturas
                    .Where(i => i.FkCliente == clientId)
                    .ToListAsync();
                foreach (var factura in facturas)
                {
                    var detalles = await _context.Detalles
                        .Where(i => i.FkFactura == factura.IdFactura)
                        .ToListAsync();
                    foreach (var detalle in detalles)
                    {
                        _context.Remove(detalle);
                        //await _context.SaveChangesAsync();
                    }
                    _context.Remove(factura);
                    //await _context.SaveChangesAsync();
                }
                _context.Remove(dbClient);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return (true, "Cliente eliminado");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Error {ex.Message}");
            }



        }

        #endregion Clientes
    }
}
