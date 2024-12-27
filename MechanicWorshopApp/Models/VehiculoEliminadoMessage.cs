using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Models
{
    public class VehiculoEliminadoMessage
    {
        public int VehiculoId { get; }

        public VehiculoEliminadoMessage(int vehiculoId)
        {
            VehiculoId = vehiculoId;
        }
    }
}
