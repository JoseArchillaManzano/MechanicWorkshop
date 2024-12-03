using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Models
{
    public class Vehiculo
    {
        public int Id { get; set; }
        public string Matricula { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Kilometros { get; set; }
        public string Bastidor { get; set; }
        public string Deposito { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
    }
}
