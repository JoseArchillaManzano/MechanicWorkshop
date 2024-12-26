using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MechanicWorkshopApp.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MechanicWorkshopApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;

        public MainWindowViewModel()
        {
            // Resuelve el contenedor de dependencias
            _serviceProvider = ((App)Application.Current).Services;

            AbrirClientesCommand = new RelayCommand(AbrirClientes);
            AbrirOrdenesCommand = new RelayCommand(AbrirOrdenes);
            AbrirConfiguracionCommand = new RelayCommand(AbrirConfiguracion);
            AbrirMetricasCommand = new RelayCommand(AbrirMetricas);
        }

        public IRelayCommand AbrirClientesCommand { get; }
        public IRelayCommand AbrirOrdenesCommand { get; }
        public IRelayCommand AbrirConfiguracionCommand { get; }
        public IRelayCommand AbrirMetricasCommand { get; }

        private void AbrirClientes()
        {
            // Resuelve y muestra la ventana de Clientes
            var clientesView = _serviceProvider.GetRequiredService<ClientesView>();
            clientesView.Show();
        }

        private void AbrirOrdenes()
        {
            // Resuelve y muestra la ventana de Órdenes de Reparación
            var ordenesView = _serviceProvider.GetRequiredService<OrdenReparacionView>();
            ordenesView.Show();
        }

        private void AbrirConfiguracion()
        {
            // Resuelve y muestra la ventana de Configuración del Taller
            var configView = _serviceProvider.GetRequiredService<TallerConfigView>();
            configView.Show();
        }

        private void AbrirMetricas()
        {
            // Resuelve y muestra la ventana de Configuración del Taller
            var dashboardView = _serviceProvider.GetRequiredService<MetricasView>();
            dashboardView.Show();
        }
    }
}
