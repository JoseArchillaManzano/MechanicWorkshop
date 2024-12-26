using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MechanicWorkshopApp.Configuration;
using MechanicWorkshopApp.Data;
using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.Services;
using MechanicWorkshopApp.Utils;
using MechanicWorkshopApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace MechanicWorkshopApp.ViewModels
{
    public partial class ClientesViewModel :  ObservableObject
    {
        private readonly ClienteService _clienteService;
        private readonly Func<ClienteForm> _clienteFormFactory;
        private readonly Func<VehiculosView> _vehiculosViewFactory;

        [ObservableProperty]
        private int currentPage = 1; // Valor inicial

        [ObservableProperty]
        private int totalPages;

        [ObservableProperty]
        private ObservableCollection<Cliente> clientes;

        [ObservableProperty]
        private int pageSize = AppSettings.PageSize;

        [ObservableProperty]
        private ObservableCollection<int> pageSizes; // Tamaños de página disponibles

        [ObservableProperty]
        private string searchQuery = string.Empty;

        [ObservableProperty]
        private Cliente selectedCliente;

        private readonly System.Timers.Timer _debounceTimer;

        public IRelayCommand AgregarClienteCommand { get; }
        public IRelayCommand EditarClienteCommand { get; }
        public IRelayCommand EliminarClienteCommand { get; }
        public IRelayCommand MostrarVehiculosCommand { get; }

        public ClientesViewModel(ClienteService clienteService, Func<ClienteForm> clienteFormFactory, Func<VehiculosView> vehiculosViewFactory)
        {
            _clienteService = clienteService;

            _clienteFormFactory = clienteFormFactory;
            _vehiculosViewFactory = vehiculosViewFactory;

            _debounceTimer = new System.Timers.Timer(300); 
            _debounceTimer.AutoReset = false; // Solo se dispara una vez
            _debounceTimer.Elapsed += (s, e) =>
            {
                // Actualizar clientes en el hilo de la interfaz
                App.Current.Dispatcher.Invoke(UpdateClientes);
            };

            AgregarClienteCommand = new RelayCommand(ExecuteAgregarCliente);
            EditarClienteCommand = new RelayCommand(ExecuteEditarCliente);
            EliminarClienteCommand = new RelayCommand(ExecuteEliminarCliente);
            MostrarVehiculosCommand = new RelayCommand(ExecuteMostrarVehiculos);

            // Inicializar comandos
            NextPageCommand = new RelayCommand(ExecuteNextPage, CanExecuteNextPage);
            PreviousPageCommand = new RelayCommand(ExecutePreviousPage, CanExecutePreviousPage);
            PageSizes = new ObservableCollection<int>(AppSettings.AvailablePageSizes);
            // Inicializar datos
            UpdateClientes();
        }

        public RelayCommand NextPageCommand { get; }
        public RelayCommand PreviousPageCommand { get; }

        private void ExecuteNextPage()
        {
            if (CurrentPage < TotalPages) // Usa los campos internos
            {
                CurrentPage++;
                UpdateClientes();
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
                UpdateClientes();
            }
        }

        private bool CanExecutePreviousPage()
        {
            return CurrentPage > 1; // Usa los campos internos
        }

        partial void OnSearchQueryChanged(string value)
        {
            CurrentPage = 1; // Reinicia a la primera página
                             // Reiniciar el temporizador
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        public void UpdateClientes()
        {
            // Obtener clientes paginados
            var result = _clienteService.GetClientesPaginated(CurrentPage, PageSize, SearchQuery); // Usa el campo interno

            // Actualizar propiedades
            Clientes = new ObservableCollection<Cliente>(result.Items);
            TotalPages = result.TotalPages;

            // Notificar cambios
            OnPropertyChanged(nameof(CurrentPage));
            OnPropertyChanged(nameof(TotalPages));
            // Actualizar estados de los botones
            NextPageCommand.NotifyCanExecuteChanged();
            PreviousPageCommand.NotifyCanExecuteChanged();

            EditarClienteCommand.NotifyCanExecuteChanged();
            EliminarClienteCommand.NotifyCanExecuteChanged();
            MostrarVehiculosCommand.NotifyCanExecuteChanged();
        }

        partial void OnPageSizeChanged(int value)
        {
            CurrentPage = 1; // Reiniciar a la primera página
            UpdateClientes(); // Actualizar la lista con el nuevo tamaño de página
        }

        private void DebounceTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Ejecutar actualización en el hilo de la interfaz
            App.Current.Dispatcher.Invoke(UpdateClientes);
        }

        private void ExecuteAgregarCliente()
        {
            var clienteForm = _clienteFormFactory();
            var viewModel = new ClientesFormViewModel(
                null,
                _clienteService,
                result =>
                {
                    if (result) UpdateClientes();
                }
            );

            clienteForm.Initialize(viewModel);
            clienteForm.ShowDialog();
        }

        private void ExecuteEditarCliente()
        {
            if (SelectedCliente == null)
            {
                MessageBox.Show("Por favor, selecciona un cliente para editar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var clienteForm = _clienteFormFactory();
            var viewModel = new ClientesFormViewModel(
                SelectedCliente,
                _clienteService,
                result =>
                {
                    if (result) UpdateClientes();
                }
            );

            clienteForm.Initialize(viewModel);
            clienteForm.ShowDialog();
        }

        private void ExecuteEliminarCliente()
        {
            if (SelectedCliente == null)
            {
                MessageBox.Show("Por favor, selecciona un cliente para eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("¿Estás seguro de que deseas eliminar este cliente?",
                "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _clienteService.EliminarCliente(SelectedCliente.Id);
                UpdateClientes();
            }
        }

        private void ExecuteMostrarVehiculos()
        {
            if (SelectedCliente == null) return;

            var vehiculosView = _vehiculosViewFactory();
            var viewModel = (VehiculosViewModel)vehiculosView.DataContext;

            viewModel.Initialize(SelectedCliente.Id);
            vehiculosView.ShowDialog();
        }
    }
}