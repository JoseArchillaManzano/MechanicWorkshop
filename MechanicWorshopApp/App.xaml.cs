using MechanicWorkshopApp.Data;
using System.Configuration;
using System.Data;
using System.Windows;

namespace MechanicWorkshopApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Inicializa el contexto de la base de datos
            using (var context = new TallerContext())
            {
                context.Database.EnsureCreated(); // Crea las tablas si no existen
            }

            base.OnStartup(e);
        }
    }

}
