using MechanicWorkshopApp.Data;
using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MechanicWorkshopApp.Services
{
    public class VehiculoService 
    {
        private readonly TallerContext _context;
        public VehiculoService(TallerContext context)
        {
            _context = context;
        }
        public List<Vehiculo> ObtenerVehiculosPorCliente(int clienteId)
        {
            return _context.Vehiculos.Where(v => v.ClienteId == clienteId).Include(v => v.Cliente).ToList();
            
        }

        public int ObtenerTotalVehiculos()
        {
            return _context.Vehiculos.Count();

        }

        public void AgregarVehiculo(Vehiculo vehiculo)
        {
            _context.Vehiculos.Add(vehiculo);
            _context.SaveChanges();
            
        }

        public void ActualizarVehiculo(Vehiculo vehiculo)
        {
            _context.Vehiculos.Update(vehiculo);
            _context.SaveChanges();
            
        }

        public void EliminarVehiculo(int vehiculoId)
        {
             var vehiculo = _context.Vehiculos.Find(vehiculoId);
             if (vehiculo != null)
             {
                 _context.Vehiculos.Remove(vehiculo);
                 _context.SaveChanges();
             }
            
        }

        public PagedResult<Vehiculo> ObtenerVehiculosPaginadosPorCliente(int clienteId, int paginaActual, int pageSize, string filtro = null)
        {
            var query = _context.Vehiculos
                                .Where(v => v.ClienteId == clienteId)
                                .Include(v => v.Cliente)
                                .AsQueryable();

            // Aplicar filtro si existe
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(v => v.Matricula.ToLower().Contains(filtro.ToLower()) || v.Modelo.ToLower().Contains(filtro.ToLower()));
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
