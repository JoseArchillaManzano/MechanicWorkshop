using MechanicWorkshopApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Data
{
    public class TallerContext : DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<OrdenReparacion> OrdenesReparacion { get; set; }
        public DbSet<LineaOrden> LineasOrden { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configura la conexión con PostgreSQL
            optionsBuilder.UseNpgsql("Host=localhost;Database=Taller;Username=postgres;Password=Postgresi-20");
        }
    }
}
