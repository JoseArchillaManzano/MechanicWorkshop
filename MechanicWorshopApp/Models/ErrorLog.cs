using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Models
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
        public string StackTrace { get; set; }
    }
}
