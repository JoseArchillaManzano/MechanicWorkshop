using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Configuration
{
    public static class AppSettings
    {
        public static int PageSize { get; set; } = 10; // Valor por defecto
        public static List<int> AvailablePageSizes { get; set; } = new List<int> { 5, 10, 20, 50 };
    }
}
