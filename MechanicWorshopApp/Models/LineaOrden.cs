using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Models
{
    public class LineaOrden
    {
        public int Id { get; set; }
        public int OrdenId { get; set; }
        public OrdenReparacion Orden { get; set; }
        public string Concepto { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Descuento { get; set; }
    }
}
