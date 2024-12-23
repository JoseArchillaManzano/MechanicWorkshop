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
        public DbSet<TallerConfig> TallerConfig { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    // Configura la conexión con PostgreSQL
        //    optionsBuilder.UseNpgsql("Host=localhost;Database=Taller;Username=postgres;Password=Postgresi-20");
        //}
        public TallerContext(DbContextOptions<TallerContext> options)
        : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vehiculo>()
                .HasOne(v => v.Cliente)
                .WithMany(c => c.Vehiculos)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LineaOrden>(entity =>
            {
                entity.Property(lo => lo.TipoLinea)
                      .HasConversion<string>() // Guardar como texto
                      .IsRequired();
            });
        }
    }
}
