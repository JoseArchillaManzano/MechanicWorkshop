﻿using MechanicWorkshopApp.Data;
using MechanicWorkshopApp.Services;
using MechanicWorkshopApp.ViewModels;
using MechanicWorkshopApp.Views;
using Microsoft.EntityFrameworkCore;
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
            services.AddDbContext<TallerContext>(options =>
            {
                options.UseNpgsql("Host=localhost;Database=Taller;Username=postgres;Password=Postgresi-20");
                options.EnableSensitiveDataLogging(); // Habilitar para depuración
            }, ServiceLifetime.Scoped);

            // Registrar servicios
            services.AddScoped<ClienteService>();
            services.AddScoped<VehiculoService>();
            services.AddScoped<OrdenReparacionService>();
            services.AddScoped<LineaOrdenService>();

            // Registrar ViewModels
            services.AddTransient<ClientesViewModel>();
            services.AddTransient<ClientesFormViewModel>();
            services.AddTransient<SelectorClienteViewModel>();
            services.AddTransient<VehiculosViewModel>();
            services.AddTransient<VehiculoFormViewModel>();
            services.AddTransient<SelectorVehiculosViewModel>();
            services.AddTransient<OrdenReparacionViewModel>();
            services.AddTransient<OrdenReparacionFormViewModel>();
            services.AddTransient<LineaOrdenFormViewModel>();

            // Registrar ventanas
            services.AddTransient<MainWindow>();
            services.AddTransient<ClientesView>();
            services.AddTransient<ClienteForm>();
            services.AddTransient<ClienteSearchDialog>();
            services.AddTransient<VehiculosView>();
            services.AddTransient<VehiculoForm>();
            services.AddTransient<SelectorClienteView>();
            services.AddTransient<SelectorVehiculosView>();
            services.AddTransient<LineaOrdenFormView>();
            services.AddTransient<OrdenReparacionView>();
            services.AddTransient<OrdenReparacionForm>();

            services.AddTransient<Func<VehiculosView>>(sp => () => sp.GetRequiredService<VehiculosView>());
            // Registro de la Factory para formularios
            services.AddTransient<Func<ClienteForm>>(provider => () => provider.GetRequiredService<ClienteForm>());
            services.AddTransient<Func<VehiculoForm>>(provider => () => provider.GetRequiredService<VehiculoForm>());
            services.AddTransient<Func<LineaOrdenFormView>>(provider => () => provider.GetRequiredService<LineaOrdenFormView>());
            services.AddTransient<Func<OrdenReparacionForm>>(provider => () => provider.GetRequiredService<OrdenReparacionForm>());
            services.AddTransient<Func<SelectorClienteView>>(provider => () => provider.GetRequiredService<SelectorClienteView>());
            services.AddTransient<Func<SelectorVehiculosView>>(provider => () => provider.GetRequiredService<SelectorVehiculosView>());

            services.AddTransient<Func<OrdenReparacionForm>>(provider => () =>
            {
                var viewModelFactory = provider.GetRequiredService<Func<Action<bool>, OrdenReparacionFormViewModel>>();
                var ordenReparacionFormViewModel = viewModelFactory((result) =>
                {
                    if (result)
                    {
                        // La orden se guardó correctamente
                        MessageBox.Show("La orden de reparación se guardó correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Aquí puedes llamar a un método para actualizar la lista de órdenes en la vista principal
                        var ordenReparacionViewModel = provider.GetRequiredService<OrdenReparacionViewModel>();
                        ordenReparacionViewModel.UpdateOrdenes();
                    }
                    else
                    {
                        // La operación fue cancelada o hubo un error
                        MessageBox.Show("La operación fue cancelada.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    // (Podrías agregar lógica adicional aquí si es necesario)
                });

                return new OrdenReparacionForm(ordenReparacionFormViewModel);
            });

            services.AddTransient<Func<Action<bool>, OrdenReparacionFormViewModel>>(provider => (callback) =>
            {
                var ordenReparacionService = provider.GetRequiredService<OrdenReparacionService>();
                var vehiculoService = provider.GetRequiredService<VehiculoService>();
                var clienteService = provider.GetRequiredService<ClienteService>();
                var lineaOrdenService = provider.GetRequiredService<LineaOrdenService>();
                var clienteSelectorFactory = provider.GetRequiredService<Func<SelectorClienteView>>();
                var vehiculoSelectorFactory = provider.GetRequiredService<Func<SelectorVehiculosView>>();
                var lineaOrdenFormFactory = provider.GetRequiredService<Func<LineaOrdenFormView>>();

                return new OrdenReparacionFormViewModel(
                    ordenReparacionService,
                    vehiculoService,
                    clienteService,
                    lineaOrdenService,
                    clienteSelectorFactory,
                    vehiculoSelectorFactory,
                    lineaOrdenFormFactory,
                    callback
                );
            });
        }
    }
}