using MechanicWorkshopApp.Data;
using MechanicWorkshopApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Services
{
    public class TallerConfigService
    {
        private readonly TallerContext _context;

        public TallerConfigService(TallerContext context)
        {
            _context = context;
        }

        public TallerConfig ObtenerConfiguracion()
        {
            return _context.TallerConfig.FirstOrDefault();
        }

        public void ActualizarConfiguracion(TallerConfig configuracion)
        {
            var existente = _context.TallerConfig.FirstOrDefault();
            if (existente != null)
            {
                existente.Nombre = configuracion.Nombre;
                existente.CIF = configuracion.CIF;
                existente.Direccion = configuracion.Direccion;
                existente.Telefono = configuracion.Telefono;
                existente.RegistroIndustrial = configuracion.RegistroIndustrial;
                _context.SaveChanges();
            }
        }
    }
}
