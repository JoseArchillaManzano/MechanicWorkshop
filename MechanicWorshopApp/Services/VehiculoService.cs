using MechanicWorkshopApp.Data;
using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.Utils;
using Microsoft.EntityFrameworkCore;

namespace MechanicWorkshopApp.Services
{
    public class VehiculoService 
    {
        private readonly Func<TallerContext> _contextFactory;
        public VehiculoService(Func<TallerContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public List<Vehiculo> ObtenerVehiculosPorCliente(int clienteId)
        {
            using var _context = _contextFactory();
            return _context.Vehiculos.AsNoTracking().Where(v => v.ClienteId == clienteId).Include(v => v.Cliente).ToList();
            
        }

        public Vehiculo ObtenerVehiculoParaEdicion(int id)
        {
            using var _context = _contextFactory();
            return _context.Vehiculos
                .AsNoTracking()
               .FirstOrDefault(v => v.Id == id);
        }

        public int ObtenerTotalVehiculos()
        {
            using var _context = _contextFactory();
            return _context.Vehiculos.Count();

        }

        public void AgregarVehiculo(Vehiculo vehiculo)
        {
            //vehiculo.ClienteId = cliente.Id; // Asignar la clave externa directamente
            vehiculo.Cliente = null; // Opcionalmente desvincular el objeto Cliente
            using var _context = _contextFactory();
            _context.Vehiculos.Add(vehiculo);
            _context.SaveChanges();
            
        }

        public void ActualizarVehiculo(Vehiculo vehiculo)
        {
            using var _context = _contextFactory();
            _context.Vehiculos.Update(vehiculo);
            _context.SaveChanges();
            
        }

        public void EliminarVehiculo(int vehiculoId)
        {
            using var _context = _contextFactory();
            var vehiculo = _context.Vehiculos.Find(vehiculoId);
             if (vehiculo != null)
             {
                 _context.Vehiculos.Remove(vehiculo);
                 _context.SaveChanges();
             }
            
        }

        public PagedResult<Vehiculo> ObtenerVehiculosPaginadosPorCliente(int clienteId, int paginaActual, int pageSize, string filtro = null)
        {
            using var _context = _contextFactory();
            var query = _context.Vehiculos
                                .AsNoTracking()
                                .Where(v => v.ClienteId == clienteId)
                                .Include(v => v.Cliente)
                                .AsQueryable();

            // Aplicar filtro si existe
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(v => v.Matricula.ToLower().Contains(filtro.ToLower()) || v.Bastidor.ToLower().Contains(filtro.ToLower()));
            }

            // Calcular totales
            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Obtener los elementos paginados
            var items = query
                .OrderBy(v => v.Id)
                .Skip((paginaActual - 1) * pageSize)
                .Take(pageSize)
            .ToList();

            return new PagedResult<Vehiculo>(items, totalItems, totalPages, pageSize);
        }
    }
}
