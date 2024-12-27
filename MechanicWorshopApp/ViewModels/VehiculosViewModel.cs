using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MechanicWorkshopApp.Configuration;
using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.Services;
using MechanicWorkshopApp.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MechanicWorkshopApp.ViewModels
{
    public partial class VehiculosViewModel : ObservableObject
    {
        private readonly VehiculoService _vehiculoService;
        private readonly ClienteService _clienteService;
        private readonly Func<VehiculoForm> _vehiculoFormFactory;
        private  int _clienteId;

        [ObservableProperty]
        private int currentPage = 1;

        [ObservableProperty]
        private int totalPages;

        [ObservableProperty]
        private ObservableCollection<Vehiculo> vehiculos;

        [ObservableProperty]
        private string searchQuery = string.Empty;

        [ObservableProperty]
        private int pageSize = AppSettings.PageSize;

        [ObservableProperty]
        private Vehiculo selectedVehiculo;

        public RelayCommand NextPageCommand { get; }
        public RelayCommand PreviousPageCommand { get; }

        public ICommand AgregarVehiculoCommand { get; }
        public ICommand EditarVehiculoCommand { get; }
        public ICommand EliminarVehiculoCommand { get; }

        private readonly System.Timers.Timer _debounceTimer;

        public VehiculosViewModel(VehiculoService vehiculoService, ClienteService clienteService, Func<VehiculoForm> vehiculoFormFactory)
        {
            _vehiculoService = vehiculoService;
            _clienteService = clienteService;
            _vehiculoFormFactory = vehiculoFormFactory;
            // Configurar comandos
            NextPageCommand = new RelayCommand(ExecuteNextPage, CanExecuteNextPage);
            PreviousPageCommand = new RelayCommand(ExecutePreviousPage, CanExecutePreviousPage);

            // Inicializar comandos
            AgregarVehiculoCommand = new RelayCommand(ExecuteAgregarVehiculo);
            EditarVehiculoCommand = new RelayCommand(ExecuteEditarVehiculo);
            EliminarVehiculoCommand = new RelayCommand(ExecuteEliminarVehiculo);

            _debounceTimer = new System.Timers.Timer(300); // 300 ms de retraso
            _debounceTimer.AutoReset = false; // Solo se dispara una vez
            _debounceTimer.Elapsed += (s, e) =>
            {
                // Actualizar clientes en el hilo de la interfaz
                App.Current.Dispatcher.Invoke(UpdateVehiculos);
            };

            // Cargar datos iniciales
            UpdateVehiculos();
        }
        public void Initialize(int clienteId)
        {
            _clienteId = clienteId;
            UpdateVehiculos();
        }

        partial void OnSearchQueryChanged(string value)
        {
            CurrentPage = 1; // Reinicia a la primera página
                             // Reiniciar el temporizador
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        private void ExecuteNextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                UpdateVehiculos();
            }
        }

        private bool CanExecuteNextPage() => CurrentPage < TotalPages;

        private void ExecutePreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                UpdateVehiculos();
            }
        }

        private bool CanExecutePreviousPage() => CurrentPage > 1;

        public void UpdateVehiculos()
        {
            var result = _vehiculoService.ObtenerVehiculosPaginadosPorCliente(
                _clienteId, CurrentPage, PageSize, SearchQuery);

            Vehiculos = new ObservableCollection<Vehiculo>(result.Items);
            TotalPages = result.TotalPages;

            // Actualizar estados de los comandos
            NextPageCommand.NotifyCanExecuteChanged();
            PreviousPageCommand.NotifyCanExecuteChanged();
        }

        private void ExecuteAgregarVehiculo()
        {
            var cliente = _clienteService.ObtenerClientePorId(_clienteId);
            var vehiculoNuevo = new Vehiculo
            {
                Cliente = cliente,
                ClienteId = _clienteId
            };
            var vehiculoForm = _vehiculoFormFactory();
            var viewModel = new VehiculoFormViewModel(
                vehiculoNuevo,
                _vehiculoService,
                cliente.Nombre,
                result =>
                {
                    if (result) UpdateVehiculos();
                }
            );

            vehiculoForm.Initialize(viewModel);

            vehiculoForm.ShowDialog();
        }

        private void ExecuteEditarVehiculo()
        {
            if (SelectedVehiculo != null)
            {
                var vehiculoEditable = _vehiculoService.ObtenerVehiculoParaEdicion(SelectedVehiculo.Id);
                var vehiculoForm = _vehiculoFormFactory();
                var viewModel = new VehiculoFormViewModel(
                    vehiculoEditable,
                    _vehiculoService,
                    SelectedVehiculo.Cliente.Nombre,
                    result =>
                    {
                       UpdateVehiculos();
                    }
                );

                vehiculoForm.Initialize(viewModel);

                vehiculoForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Debe seleccionar un vehículo para editar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void ExecuteEliminarVehiculo()
        {
            if (SelectedVehiculo != null)
            {
                var result = MessageBox.Show("¿Estás seguro de que deseas eliminar este vehículo? Las reparaciones vinculadas al vehículo también serán eliminadas.",
                    "Confirmar Eliminación", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    int idVehiculo = SelectedVehiculo.Id;
                    _vehiculoService.EliminarVehiculo(idVehiculo);
                    UpdateVehiculos();
                    WeakReferenceMessenger.Default.Send(new VehiculoEliminadoMessage(idVehiculo));
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar un vehículo para eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        partial void OnSelectedVehiculoChanged(Vehiculo value)
        {
            ((RelayCommand)EditarVehiculoCommand).NotifyCanExecuteChanged();
            ((RelayCommand)EliminarVehiculoCommand).NotifyCanExecuteChanged();
        }
    }
}
