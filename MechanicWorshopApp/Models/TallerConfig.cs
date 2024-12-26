using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Models
{
    public class TallerConfig
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string CIF { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string RegistroIndustrial { get; set; }
        public double HoraManoObra { get; set; }
        public int IVA { get; set; }
    }
}
