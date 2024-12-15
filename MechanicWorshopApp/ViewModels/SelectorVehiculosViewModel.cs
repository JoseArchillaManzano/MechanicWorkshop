using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MechanicWorkshopApp.Configuration;
using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MechanicWorkshopApp.ViewModels
{
    public partial class SelectorVehiculosViewModel : ObservableObject
    {
        private readonly VehiculoService _vehiculoService;
        private readonly Action<Vehiculo> _onVehiculoSeleccionado; // Acción que notifica el vehículo seleccionado
        private readonly int _clienteId;

        [ObservableProperty]
        private ObservableCollection<Vehiculo> vehiculos;

        [ObservableProperty]
        private Vehiculo vehiculoSeleccionado;

        [ObservableProperty]
        private string searchQuery;

        [ObservableProperty]
        private int currentPage = 1;

        public IRelayCommand AceptarCommand { get; }
        public IRelayCommand CancelarCommand { get; }

        private readonly System.Timers.Timer _debounceTimer;

        public SelectorVehiculosViewModel(int clienteId, VehiculoService vehiculoService, Action<Vehiculo> onVehiculoSeleccionado)
        {
            _clienteId = clienteId;
            _vehiculoService = vehiculoService;
            _onVehiculoSeleccionado = onVehiculoSeleccionado;

            AceptarCommand = new RelayCommand(Aceptar);
            CancelarCommand = new RelayCommand(Cancelar);

            _debounceTimer = new System.Timers.Timer(300);
            _debounceTimer.AutoReset = false; // Solo se dispara una vez
            _debounceTimer.Elapsed += (s, e) =>
            {
                // Actualizar clientes en el hilo de la interfaz
                App.Current.Dispatcher.Invoke(LoadVehiculos);
            };

            LoadVehiculos();
        }

        private void LoadVehiculos()
        {
            var result = _vehiculoService.ObtenerVehiculosPaginadosPorCliente(
                _clienteId, CurrentPage, AppSettings.PageSize, SearchQuery);

            Vehiculos = new ObservableCollection<Vehiculo>(result.Items);

            if (!Vehiculos.Any())
            {
                MessageBox.Show("No se encontraron vehículos para el cliente seleccionado.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Aceptar()
        {

            if (VehiculoSeleccionado != null)
            {
                _onVehiculoSeleccionado?.Invoke(VehiculoSeleccionado); // Notifica vehiculo seleccionado
                CerrarVentanaActual(); // Cierra la ventana actual
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un cliente antes de aceptar.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancelar()
        {
            _onVehiculoSeleccionado?.Invoke(null); // Notificar que no se seleccionó ningún vehículo
            CerrarVentanaActual();
        }

        private void CerrarVentanaActual()
        {
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
        }

        partial void OnSearchQueryChanged(string value)
        {
            CurrentPage = 1; // Reinicia a la primera página
                             // Reiniciar el temporizador
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }
    }
}
