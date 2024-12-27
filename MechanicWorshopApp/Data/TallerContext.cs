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
                .OnDelete(DeleteBehavior.Cascade); // Eliminar un Cliente elimina todos sus Vehiculos

            // Relación Vehiculo -> OrdenReparacion (1 Vehiculo tiene muchas OrdenesReparacion)
            modelBuilder.Entity<OrdenReparacion>()
                .HasOne(o => o.Vehiculo)
                .WithMany(v => v.OrdenesReparacion)
                .HasForeignKey(o => o.VehiculoId)
                .OnDelete(DeleteBehavior.Cascade); // Eliminar un Vehiculo elimina todas sus OrdenesReparacion

            // Relación Cliente -> OrdenReparacion (1 Cliente tiene muchas OrdenesReparacion)
            modelBuilder.Entity<OrdenReparacion>()
                .HasOne(o => o.Cliente)
                .WithMany(c => c.OrdenesReparacion)
                .HasForeignKey(o => o.ClienteId)
                .OnDelete(DeleteBehavior.Cascade); // Eliminar un Cliente no elimina las Ordenes directamente

            // Relación OrdenReparacion -> LineaOrden (1 OrdenReparacion tiene muchas LineasOrden)
            modelBuilder.Entity<LineaOrden>()
                .HasOne(l => l.OrdenReparacion)
                .WithMany(o => o.LineasOrden)
                .HasForeignKey(l => l.OrdenReparacionId)
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
