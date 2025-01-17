﻿using MechanicWorkshopApp.Data;
using MechanicWorkshopApp.Services;
using MechanicWorkshopApp.ViewModels;
using MechanicWorkshopApp.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace MechanicWorkshopApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Services { get; private set; }
        private ErrorLoggingService _errorLoggingService;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            QuestPDF.Settings.License = LicenseType.Community;
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();
            _errorLoggingService = new ErrorLoggingService(Services.GetRequiredService<TallerContext>());
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Registrar el DbContext
            services.AddDbContext<TallerContext>(options =>
            {
                options.UseNpgsql("Host=localhost;Database=Taller;Username=postgres;Password=Postgresi-20");
                options.EnableSensitiveDataLogging(); // Habilitar para depuración
              
            }, ServiceLifetime.Transient);

            // Registrar servicios
            services.AddTransient<ClienteService>();
            services.AddTransient<VehiculoService>();
            services.AddTransient<OrdenReparacionService>();
            services.AddTransient<LineaOrdenService>();
            services.AddTransient<TallerConfigService>();
            services.AddTransient<PrinterService>();

            // Registrar ViewModels
            services.AddTransient<ClientesViewModel>();
            services.AddTransient<ClientesFormViewModel>();
            services.AddTransient<SelectorClienteViewModel>();
            services.AddTransient<VehiculosViewModel>();
            services.AddTransient<VehiculoFormViewModel>();
            services.AddTransient<SelectorVehiculosViewModel>();
            services.AddSingleton<OrdenReparacionViewModel>();
            services.AddTransient<OrdenReparacionFormViewModel>();
            services.AddTransient<LineaOrdenFormViewModel>();
            services.AddTransient<TallerConfigViewModel>();
            services.AddTransient<MetricasViewModel>();

            // Registrar ventanas
            services.AddTransient<MainWindow>();
            services.AddTransient<ClientesView>();
            services.AddTransient<ClienteForm>();
            services.AddTransient<VehiculosView>();
            services.AddTransient<VehiculoForm>();
            services.AddTransient<SelectorClienteView>();
            services.AddTransient<SelectorVehiculosView>();
            services.AddTransient<LineaOrdenFormView>();
            services.AddTransient<OrdenReparacionView>();
            services.AddTransient<OrdenReparacionForm>();
            services.AddTransient<TallerConfigView>();
            services.AddTransient<MetricasView>();

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
                        MessageBox.Show("La orden de reparación se guardó correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("La operación fue cancelada.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                });

                return new OrdenReparacionForm(ordenReparacionFormViewModel);
            });

            services.AddTransient<Func<Action<bool>, OrdenReparacionFormViewModel>>(provider => (callback) =>
            {
                var ordenReparacionService = provider.GetRequiredService<OrdenReparacionService>();
                var vehiculoService = provider.GetRequiredService<VehiculoService>();
                var clienteService = provider.GetRequiredService<ClienteService>();
                var lineaOrdenService = provider.GetRequiredService<LineaOrdenService>();
                var tallerConfigService = provider.GetRequiredService<TallerConfigService>();
                var printerService = provider.GetRequiredService<PrinterService>();
                var clienteSelectorFactory = provider.GetRequiredService<Func<SelectorClienteView>>();
                var vehiculoSelectorFactory = provider.GetRequiredService<Func<SelectorVehiculosView>>();
                var lineaOrdenFormFactory = provider.GetRequiredService<Func<LineaOrdenFormView>>();
                var vehiculoFormFactory = provider.GetRequiredService<Func<VehiculoForm>>();

                return new OrdenReparacionFormViewModel(
                    ordenReparacionService,
                    vehiculoService,
                    clienteService,
                    lineaOrdenService,
                    tallerConfigService,
                    printerService,
                    clienteSelectorFactory,
                    vehiculoSelectorFactory,
                    lineaOrdenFormFactory,
                    vehiculoFormFactory,
                    callback
                );
            });
            services.AddTransient<Func<TallerContext>>(sp => () => sp.GetRequiredService<TallerContext>());
        }
        private void Application_DispatcherUnhandledException(
            object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true; // Evitamos que la excepción cierre la app
            _errorLoggingService.LogException(e.Exception);
            MessageBox.Show(
                $"Ocurrió un error no controlado:\n{e.Exception.Message}\n Si se vuelve a repetir contactad con el desarrollador del programa",
                "Error inesperado",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

    }
}