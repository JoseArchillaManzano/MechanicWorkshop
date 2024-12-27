using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.Services;
using MechanicWorkshopApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.ViewModels
{
    public partial class VehiculoFormViewModel : ObservableObject
    {
        private readonly VehiculoService _vehiculoService;
        private readonly ClienteService _clienteService;
        private readonly Action<bool> _callback;

        public event Action OnClose;

        [ObservableProperty]
        private Vehiculo vehiculo;

        [ObservableProperty]
        private string clienteNombre;

        public IRelayCommand GuardarCommand { get; }
        public IRelayCommand CancelarCommand { get; }
        public IRelayCommand SeleccionarClienteCommand { get; }

        public VehiculoFormViewModel(Vehiculo vehiculo, VehiculoService vehiculoService, string nombreCliente, Action<bool> callback)
        {
            Vehiculo = vehiculo ?? new Vehiculo();
            ClienteNombre = nombreCliente;
            _vehiculoService = vehiculoService;
            _callback = callback;

            GuardarCommand = new RelayCommand(GuardarVehiculo);
            CancelarCommand = new RelayCommand(Cancelar);
        }

        private void GuardarVehiculo()
        {
            Vehiculo.ForzarValidacion();
            if (string.IsNullOrWhiteSpace(Vehiculo.Marca) ||
               string.IsNullOrWhiteSpace(Vehiculo.Modelo) ||
               string.IsNullOrWhiteSpace(Vehiculo.Matricula))
            {
                System.Windows.MessageBox.Show("Todos los campos del vehículo son obligatorios.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            if (Vehiculo.ClienteId == 0)
            {
                System.Windows.MessageBox.Show("Debe seleccionar un cliente para asociar el vehículo.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            if (Vehiculo.Id == 0)
            {
                _vehiculoService.AgregarVehiculo(Vehiculo);
            }
            else
            {
                _vehiculoService.ActualizarVehiculo(Vehiculo);
            }
            _callback?.Invoke(true);
            OnClose?.Invoke();
        }

        private void Cancelar()
        {
            _callback?.Invoke(false);
            OnClose?.Invoke();
        }
    }
}