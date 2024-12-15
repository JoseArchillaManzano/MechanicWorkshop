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
    public class LineaOrdenService
    {
        private readonly TallerContext _context;

        public LineaOrdenService(TallerContext context)
        {
            _context = context;
        }

        public PagedResult<LineaOrden> ObtenerLineasPorOrdenPaginadas(int ordenId, int page, int pageSize)
        {
            var query = _context.LineasOrden
                                .Where(l => l.OrdenReparacionId == ordenId)
                                .AsQueryable();

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var items = query
                .OrderBy(l => l.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<LineaOrden>(items, totalItems, totalPages, pageSize);
        }

        public void CrearLineaOrden(LineaOrden linea)
        {
            _context.LineasOrden.Add(linea);
            _context.SaveChanges();
        }

        public void ActualizarLineaOrden(LineaOrden linea)
        {
            _context.LineasOrden.Update(linea);
            _context.SaveChanges();
        }

        public void EliminarLineaOrden(int id)
        {
            var linea = _context.LineasOrden.Find(id);
            if (linea != null)
            {
                _context.LineasOrden.Remove(linea);
                _context.SaveChanges();
            }
        }
    }
}
