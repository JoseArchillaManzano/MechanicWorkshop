using MechanicWorkshopApp.Data;
using MechanicWorkshopApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Services
{
    public class ErrorLoggingService
    {
        private readonly TallerContext _context;

        public ErrorLoggingService(TallerContext context)
        {
            _context = context;
        }

        public void LogException(Exception ex)
        {
            var errorLog = new ErrorLog
            {
                Mensaje = ex.Message,
                StackTrace = ex.StackTrace,
                Fecha = DateTime.Now.ToUniversalTime()
            };

            _context.ErrorLogs.Add(errorLog);
            _context.SaveChanges(); // Se guarda en BD
        }
    }
}
