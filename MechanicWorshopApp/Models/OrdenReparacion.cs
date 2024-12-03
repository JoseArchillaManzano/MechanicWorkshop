using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Models
{
    public class OrdenReparacion
    {
        public int Id { get; set; }
        public DateTime FechaEntrada { get; set; }
        public string Descripcion { get; set; }

        // Relación con Cliente
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        // Relación con Vehiculo
        public int VehiculoId { get; set; }
        public Vehiculo Vehiculo { get; set; }

        // Lista de conceptos
        public List<LineaOrden> LineasOrden { get; set; }
    }
}
