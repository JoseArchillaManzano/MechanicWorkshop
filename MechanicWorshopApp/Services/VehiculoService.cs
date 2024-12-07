using MechanicWorkshopApp.Data;
using MechanicWorkshopApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
