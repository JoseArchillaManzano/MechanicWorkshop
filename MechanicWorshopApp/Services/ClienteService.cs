using MechanicWorkshopApp.Data;
using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.Utils;
using Microsoft.EntityFrameworkCore;

namespace MechanicWorkshopApp.Services
{
    public class ClienteService
    {
        private readonly Func<TallerContext> _contextFactory;

        public ClienteService(Func<TallerContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public List<Cliente> ObtenerClientes()
        {
            using var _context = _contextFactory();
            return _context.Clientes.ToList();
        }

        public Cliente ObtenerClientePorId(int id)
        {
            using var _context = _contextFactory();
            return _context.Clientes.AsNoTracking().FirstOrDefault(c => c.Id == id);
        }

        public void AgregarCliente(Cliente cliente)
        {
            using var _context = _contextFactory();
            _context.Clientes.Add(cliente);
            _context.SaveChanges();
        }

        public void ActualizarCliente(Cliente cliente)
        {
            using var _context = _contextFactory();
            _context.Clientes.Update(cliente);
            _context.SaveChanges();
        }

        public void EliminarCliente(int id)
        {
            using var _context = _contextFactory();
            var cliente = _context.Clientes.Find(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                _context.SaveChanges();
            }
        }

        public int ObtenerTotalClientes()
        {
            using var _context = _contextFactory();
            return _context.Clientes.Count();
            
        }

        public PagedResult<Cliente> GetClientesPaginated(int page, int pageSize, string searchQuery)
        {
            using var _context = _contextFactory();
            if (page < 1) page = 1;
            
            var query = _context.Clientes.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(c => c.Nombre.ToLower().Contains(searchQuery.ToLower()) || c.DNI_CIF.ToLower().Contains(searchQuery.ToLower()));
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