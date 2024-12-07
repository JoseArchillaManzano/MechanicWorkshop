using MechanicWorkshopApp.Data;
using MechanicWorkshopApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Services
{
    public class ClienteService
    {
        private readonly TallerContext _context;

        public ClienteService(TallerContext context)
        {
            _context = context;
        }

        public List<Cliente> ObtenerClientes()
        {
            return _context.Clientes.ToList();
        }

        public Cliente ObtenerClientePorId(int id)
        {
            return _context.Clientes.FirstOrDefault(c => c.Id == id);
        }

        public void AgregarCliente(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            _context.SaveChanges();
        }

        public void ActualizarCliente(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            _context.SaveChanges();
        }

        public void EliminarCliente(int id)
        {
            var cliente = _context.Clientes.Find(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                _context.SaveChanges();
            }
        }
    }
}