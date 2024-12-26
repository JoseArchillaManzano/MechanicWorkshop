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
        private int totalPages;

        [ObservableProperty]
        private int currentPage = 1;

        [ObservableProperty]
        private Cliente clienteSeleccionado;

        [ObservableProperty]
        private int pageSize = AppSettings.PageSize;

        [ObservableProperty]
        private ObservableCollection<int> pageSizes; // Tamaños de página disponibles

        [ObservableProperty]
        private string searchQuery;

        public IRelayCommand AceptarCommand { get; }
        public IRelayCommand CancelarCommand { get; }

        private readonly System.Timers.Timer _debounceTimer;

        public SelectorClienteViewModel(ClienteService clienteService, Action<Cliente> onClienteSeleccionado)
        {
            _clienteService = clienteService;
            _onClienteSeleccionado = onClienteSeleccionado;

            AceptarCommand = new RelayCommand(Aceptar);
            CancelarCommand = new RelayCommand(Cancelar);

            _debounceTimer = new System.Timers.Timer(300);
            _debounceTimer.AutoReset = false; // Solo se dispara una vez
            _debounceTimer.Elapsed += (s, e) =>
            {
                // Actualizar clientes en el hilo de la interfaz
                App.Current.Dispatcher.Invoke(LoadClientes);
            };

            NextPageCommand = new RelayCommand(ExecuteNextPage, CanExecuteNextPage);
            PreviousPageCommand = new RelayCommand(ExecutePreviousPage, CanExecutePreviousPage);
            PageSizes = new ObservableCollection<int>(AppSettings.AvailablePageSizes);
            LoadClientes();
        }

        public RelayCommand NextPageCommand { get; }
        public RelayCommand PreviousPageCommand { get; }

        private void ExecuteNextPage()
        {
            if (CurrentPage < TotalPages) // Usa los campos internos
            {
                CurrentPage++;
                LoadClientes();
            }
        }

        private bool CanExecuteNextPage()
        {
            return CurrentPage < TotalPages; // Usa los campos internos
        }

        private void ExecutePreviousPage()
        {
            if (CurrentPage > 1) // Usa los campos internos
            {
                CurrentPage--;
                LoadClientes();
            }
        }

        private bool CanExecutePreviousPage()
        {
            return CurrentPage > 1; // Usa los campos internos
        }

        private void LoadClientes()
        {
            var result = _clienteService.GetClientesPaginated(CurrentPage, PageSize, SearchQuery);

            Clientes = new ObservableCollection<Cliente>(result.Items);
            TotalPages = result.TotalPages;

            // Notificar cambios
            OnPropertyChanged(nameof(CurrentPage));
            OnPropertyChanged(nameof(TotalPages));
            // Actualizar estados de los botones
            NextPageCommand.NotifyCanExecuteChanged();
            PreviousPageCommand.NotifyCanExecuteChanged();
        }

        partial void OnPageSizeChanged(int value)
        {
            CurrentPage = 1; // Reiniciar a la primera página
            LoadClientes(); // Actualizar la lista con el nuevo tamaño de página
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

