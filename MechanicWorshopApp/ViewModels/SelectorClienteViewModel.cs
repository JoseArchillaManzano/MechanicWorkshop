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
    public partial class SelectorClienteViewModel : ObservableObject
    {
        private readonly ClienteService _clienteService;
        private readonly Action<Cliente> _onClienteSeleccionado; // Acción para notificar el cliente seleccionado

        [ObservableProperty]
        private ObservableCollection<Cliente> clientes;

        [ObservableProperty]
        private int currentPage = 1;

        [ObservableProperty]
        private Cliente clienteSeleccionado;

        [ObservableProperty]
        private string searchQuery;

        public IRelayCommand AceptarCommand { get; }
        public IRelayCommand CancelarCommand { get; }

        private readonly System.Timers.Timer _debounceTimer;

        public SelectorClienteViewModel(ClienteService clienteService, Action<Cliente> onClienteSeleccionado)
        {
            _clienteService = clienteService;
            _onClienteSeleccionado = onClienteSeleccionado;

            AceptarCommand = new RelayCommand(Aceptar, () => ClienteSeleccionado != null);
            CancelarCommand = new RelayCommand(Cancelar);

            _debounceTimer = new System.Timers.Timer(300);
            _debounceTimer.AutoReset = false; // Solo se dispara una vez
            _debounceTimer.Elapsed += (s, e) =>
            {
                // Actualizar clientes en el hilo de la interfaz
                App.Current.Dispatcher.Invoke(LoadClientes);
            };
            LoadClientes();
        }

        private void LoadClientes()
        {
            var result = _clienteService.GetClientesPaginated(CurrentPage, AppSettings.PageSize, SearchQuery);

            // Actualizar propiedades
            Clientes = new ObservableCollection<Cliente>(result.Items);
        }

        private void Aceptar()
        {
            if (ClienteSeleccionado != null)
            {
                _onClienteSeleccionado?.Invoke(ClienteSeleccionado); // Notifica cliente seleccionado
                CerrarVentanaActual(); // Cierra la ventana actual
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un cliente antes de aceptar.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancelar()
        {
            _onClienteSeleccionado?.Invoke(null); // Notifica que no se seleccionó cliente
            CerrarVentanaActual(); // Cierra la ventana actual
        }

        private void CerrarVentanaActual()
        {
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
        }

        partial void OnClienteSeleccionadoChanged(Cliente oldValue, Cliente newValue)
        {
            // Notificar que el comando puede ejecutarse
            ((RelayCommand)AceptarCommand).NotifyCanExecuteChanged();
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

