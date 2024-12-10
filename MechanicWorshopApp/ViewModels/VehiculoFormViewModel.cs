using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.Services;
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
        private readonly Action<bool> _callback;

        public event Action OnClose;

        [ObservableProperty]
        private Vehiculo vehiculo;

        [ObservableProperty]
        private string clienteNombre;

        public IRelayCommand GuardarCommand { get; }
        public IRelayCommand CancelarCommand { get; }
        public IRelayCommand SeleccionarClienteCommand { get; }

        public VehiculoFormViewModel(Vehiculo vehiculo, VehiculoService vehiculoService, Action<bool> callback)
        {
            Vehiculo = vehiculo ?? new Vehiculo();
            ClienteNombre = vehiculo.Cliente?.Nombre ?? "Seleccionar cliente...";
            _vehiculoService = vehiculoService;
            _callback = callback;

            GuardarCommand = new RelayCommand(GuardarVehiculo);
            CancelarCommand = new RelayCommand(Cancelar);
            SeleccionarClienteCommand = new RelayCommand(SeleccionarCliente);
        }

        private void GuardarVehiculo()
        {
            if (string.IsNullOrWhiteSpace(Vehiculo.Marca) ||
               string.IsNullOrWhiteSpace(Vehiculo.Modelo) ||
               string.IsNullOrWhiteSpace(Vehiculo.Matricula))
            {
                // Aquí puedes usar un servicio de notificación para mostrar errores.
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
            _callback?.Invoke(true);
            OnClose?.Invoke();
        }

        private void SeleccionarCliente()
        {
            // Lógica para seleccionar cliente
        }
    }
}