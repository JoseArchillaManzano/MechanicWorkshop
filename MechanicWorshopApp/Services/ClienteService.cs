using MechanicWorkshopApp.Data;
using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Services
{
    public class ClienteService
    {
        private readonly TallerContext _context;

        public ClienteService(TallerContext context)
        {
            _context = context;
        }

        public List<Cliente> ObtenerClientes()
        {
            return _context.Clientes.ToList();
        }

        public Cliente ObtenerClientePorId(int id)
        {
            return _context.Clientes.FirstOrDefault(c => c.Id == id);
        }

        public void AgregarCliente(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            _context.SaveChanges();
        }

        public void ActualizarCliente(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            _context.SaveChanges();
        }

        public void EliminarCliente(int id)
        {
            var cliente = _context.Clientes.Find(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                _context.SaveChanges();
            }
        }

        public int ObtenerTotalClientes()
        {
            using (var context = new TallerContext())
            {
                return context.Clientes.Count();
            }
        }

        public List<Cliente> ObtenerClientesPaginados(int paginaActual, int tamanoPagina)
        {
            using (var context = new TallerContext())
            {
                return context.Clientes
                              .OrderBy(c => c.Id)
                              .Skip((paginaActual - 1) * tamanoPagina)
                              .Take(tamanoPagina)
                              .ToList();
            }
        }

        public PagedResult<Cliente> GetClientesPaginated(int page, int pageSize, string searchQuery)
        {
            if (page < 1) page = 1;
            using (var context = new TallerContext())
            {
                var query = context.Clientes.AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    query = query.Where(c => c.Nombre.Contains(searchQuery) || c.DNI_CIF.Contains(searchQuery));
                }
                var totalItems = query.Count(); // Total de clientes
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                var items = query
                    .OrderBy(c => c.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new PagedResult<Cliente>(items, totalItems, page, pageSize);
            }
        }
    }
}