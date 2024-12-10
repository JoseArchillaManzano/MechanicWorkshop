using MechanicWorkshopApp.Data;
using MechanicWorkshopApp.Services;
using MechanicWorkshopApp.ViewModels;
using MechanicWorkshopApp.Views;
using Microsoft.Extensions.DependencyInjection;
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
        public IServiceProvider Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Registrar el DbContext
            services.AddDbContext<TallerContext>();

            // Registrar servicios
            services.AddTransient<ClienteService>();
            services.AddTransient<VehiculoService>();

            // Registrar ViewModels
            services.AddTransient<ClientesViewModel>();
            services.AddTransient<VehiculosViewModel>();

            // Registrar ventanas
            services.AddTransient<MainWindow>();
            services.AddTransient<ClientesView>();
            services.AddTransient<ClienteForm>();
            services.AddTransient<ClienteSearchDialog>();
            services.AddTransient<VehiculosView>();
            services.AddTransient<VehiculoForm>();

            services.AddTransient<Func<VehiculosView>>(sp => () => sp.GetRequiredService<VehiculosView>());
            // Registro de la Factory para formularios
            services.AddTransient<Func<ClienteForm>>(provider => () => provider.GetRequiredService<ClienteForm>());
            services.AddTransient<Func<VehiculoForm>>(provider => () => provider.GetRequiredService<VehiculoForm>());
        }
    }
}