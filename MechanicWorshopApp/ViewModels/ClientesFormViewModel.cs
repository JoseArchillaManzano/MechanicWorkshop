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
    public partial class ClientesFormViewModel : ObservableObject
    {
        private readonly ClienteService _clienteService;
        private readonly Action<bool> _callback;

        public event Action OnClose;

        [ObservableProperty]
        private Cliente cliente;

        public IRelayCommand GuardarCommand { get; }
        public IRelayCommand CancelarCommand { get; }

        public ClientesFormViewModel(Cliente cliente, ClienteService clienteService, Action<bool> callback)
        {
            Cliente = cliente ?? new Cliente();
            _clienteService = clienteService;
            _callback = callback;

            GuardarCommand = new RelayCommand(ExecuteGuardar);
            CancelarCommand = new RelayCommand(ExecuteCancelar);
        }

        private void ExecuteGuardar()
        {
            Cliente.ForzarValidacion(); // Forzar la validación de todos los campos

            // Verificar si hay errores antes de guardar
            var tieneErrores = typeof(Cliente)
                .GetProperties()
                .Any(prop => !string.IsNullOrEmpty(Cliente[prop.Name]));

            if (tieneErrores)
            {
                // Mostrar mensaje de error si hay campos inválidos
                System.Windows.MessageBox.Show("Por favor, corrige los errores antes de guardar.", "Errores de validación", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }
            if (Cliente.Id == 0)
            {
                _clienteService.AgregarCliente(Cliente);
            }
            else
            {
                _clienteService.ActualizarCliente(Cliente);
            }

            _callback?.Invoke(true);
            OnClose?.Invoke();
        }

        private void ExecuteCancelar()
        {
            _callback?.Invoke(false);
            OnClose?.Invoke();
        }
    }
}

