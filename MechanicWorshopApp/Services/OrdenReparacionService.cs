using MechanicWorkshopApp.Data;
using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Services
{
    public class OrdenReparacionService
    {
        private readonly TallerContext _context;

        public OrdenReparacionService(TallerContext context)
        {
            _context = context;
        }

        public PagedResult<OrdenReparacion> ObtenerOrdenesPaginadas(int page, int pageSize, string? searchQuery = null)
        {
            var query = _context.OrdenesReparacion
                                .Include(o => o.Cliente)
                                .Include(o => o.Vehiculo)
                                .Include(o => o.LineasOrden)
                                .AsQueryable();

            // Aplicar búsqueda si se proporciona un query
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(o =>
                    o.Cliente.Nombre.Contains(searchQuery) ||
                    o.Vehiculo.Matricula.Contains(searchQuery) ||
                    o.Descripcion.Contains(searchQuery));
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var items = query
                .OrderByDescending(o => o.FechaEntrada)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<OrdenReparacion>(items, totalItems, totalPages, pageSize);
        }

        public OrdenReparacion ObtenerOrdenPorId(int id)
        {
            return _context.OrdenesReparacion
                           .Include(o => o.Cliente)
                           .Include(o => o.Vehiculo)
                           .Include(o => o.LineasOrden)
                           .FirstOrDefault(o => o.Id == id);
        }

        public void CrearOrden(OrdenReparacion orden)
        {
            if (orden.Cliente != null && _context.Entry(orden.Cliente).State != EntityState.Detached)
            {
                _context.Entry(orden.Cliente).State = EntityState.Detached;
            }

            // Si el vehículo está rastreado, desvincularlo
            if (orden.Vehiculo != null && _context.Entry(orden.Vehiculo).State != EntityState.Detached)
            {
                _context.Entry(orden.Vehiculo).State = EntityState.Detached;
            }

            // Adjuntar cliente y vehículo al contexto actual
            if (orden.Cliente != null)
            {
                _context.Attach(orden.Cliente);
            }

            if (orden.Vehiculo != null)
            {
                _context.Attach(orden.Vehiculo);
            }

            AdjustDatesToUtc(orden);

            _context.OrdenesReparacion.Add(orden);
            _context.SaveChanges();
        }

        public void ActualizarOrden(OrdenReparacion orden)
        {
            AdjustDatesToUtc(orden);
            _context.OrdenesReparacion.Update(orden);
            _context.SaveChanges();
        }

        public void EliminarOrden(int id)
        {
            var orden = _context.OrdenesReparacion.Find(id);
            if (orden != null)
            {
                _context.OrdenesReparacion.Remove(orden);
                _context.SaveChanges();
            }
        }

        public void ActualizarFechaSalida(int ordenId, DateTime fechaSalida)
        {
            var orden = _context.OrdenesReparacion.Find(ordenId);
            if (orden != null)
            {
                orden.FechaSalida = fechaSalida;
                _context.SaveChanges();
            }
        }

        private void AdjustDatesToUtc(OrdenReparacion orden)
        {
            if (orden.FechaEntrada.Kind == DateTimeKind.Local || orden.FechaEntrada.Kind == DateTimeKind.Unspecified)
            {
                orden.FechaEntrada = DateTime.SpecifyKind(orden.FechaEntrada, DateTimeKind.Local).ToUniversalTime();
            }

            // Normalizar FechaSalida
            if (orden.FechaSalida.HasValue)
            {
                var fechaSalida = orden.FechaSalida.Value.AddHours(1);
                if (fechaSalida.Kind == DateTimeKind.Local || fechaSalida.Kind == DateTimeKind.Unspecified)
                {
                    orden.FechaSalida = DateTime.SpecifyKind(fechaSalida, DateTimeKind.Unspecified).ToUniversalTime();
                }
            }
        }
    }
}

//var totalMateriales = _context.LineasOrden
//                              .Where(lo => lo.TipoLinea == TipoLinea.Material)
//                              .Sum(lo => lo.Total);

//var totalManoDeObra = _context.LineasOrden
//                              .Where(lo => lo.TipoLinea == TipoLinea.ManoDeObra)
//                              .Sum(lo => lo.Total);