using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Models
{
    public class ClienteEliminadoMessage
    {
        public int ClienteId { get; }

        public ClienteEliminadoMessage(int clienteId)
        {
            ClienteId = clienteId;
        }
    }
}
