using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MechanicWorkshopApp.Configuration;
using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.Services;
using MechanicWorkshopApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MechanicWorkshopApp.ViewModels
{
    public partial class OrdenReparacionViewModel : ObservableObject
    {
        private readonly OrdenReparacionService _ordenReparacionService;
        private readonly VehiculoService _vehiculoService;
        private readonly ClienteService _clienteService;
        private readonly Func<OrdenReparacionForm> _ordenFormFactory;
        private readonly Func<SelectorClienteView> _clienteSelectorFactory;
        private readonly Func<SelectorVehiculosView> _vehiculoSelectorFactory;
        private readonly Func<LineaOrdenFormView> _lineaOrdenFormFactory;

        [ObservableProperty]
        private int currentPage = 1;

        [ObservableProperty]
        private int totalPages;

        [ObservableProperty]
        private ObservableCollection<OrdenReparacion> ordenes;

        [ObservableProperty]
        private string searchQuery = string.Empty;

        [ObservableProperty]
        private OrdenReparacion selectedOrden;
        [ObservableProperty]
        private int pageSize = AppSettings.PageSize;

        private readonly Action<bool> _callback;

        public event Action OnClose;

        public ICommand AgregarOrdenCommand { get; }
        public ICommand EditarOrdenCommand { get; }
        public ICommand EliminarOrdenCommand { get; }
        public ICommand SearchCommand { get; }

        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        public OrdenReparacionViewModel(
            OrdenReparacionService ordenReparacionService,
            VehiculoService vehiculoService,
            ClienteService clienteService,
            Func<SelectorClienteView> clienteSelectorFactory,
            Func<SelectorVehiculosView> vehiculoSelectorFactory,
            Func<LineaOrdenFormView> lineaOrdenFormFactory,
            Func<OrdenReparacionForm> ordenFormFactory)
        {
            _ordenReparacionService = ordenReparacionService;
            _vehiculoService = vehiculoService;
            _clienteService = clienteService;
            _clienteSelectorFactory = clienteSelectorFactory;
            _vehiculoSelectorFactory = vehiculoSelectorFactory;
            _lineaOrdenFormFactory = lineaOrdenFormFactory;
            _ordenFormFactory = ordenFormFactory;

            AgregarOrdenCommand = new RelayCommand(AgregarOrden);
            EditarOrdenCommand = new RelayCommand(EditarOrden);
            EliminarOrdenCommand = new RelayCommand(EliminarOrden);

            NextPageCommand = new RelayCommand(NextPage, CanNextPage);
            PreviousPageCommand = new RelayCommand(PreviousPage, CanPreviousPage);

            UpdateOrdenes();
        }

        public void UpdateOrdenes()
        {
            var result = _ordenReparacionService.ObtenerOrdenesPaginadas(currentPage,PageSize, SearchQuery);
            Ordenes = new ObservableCollection<OrdenReparacion>(result.Items);
            TotalPages = result.TotalPages;

            // Actualiza los estados de los comandos
            (EditarOrdenCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (EliminarOrdenCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (NextPageCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (PreviousPageCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }

        private void AgregarOrden()
        {
            var ordenReparacion = new OrdenReparacion
            {
                FechaEntrada = DateTime.Now,
                LineasOrden = new ObservableCollection<LineaOrden>()
            };

            var ordenForm = _ordenFormFactory();

            //var ordenFormViewModel = new OrdenReparacionFormViewModel(
            //   _ordenReparacionService,
            //   _vehiculoService,
            //   _clienteService,
            //   _clienteSelectorFactory,
            //   _vehiculoSelectorFactory,
            //   _lineaOrdenFormFactory,
            //   result =>
            //   {
            //       if (result) UpdateOrdenes();
            //   });

            ordenForm.Initialize(ordenReparacion);

            if (ordenForm.ShowDialog() == true)
            {
                //_ordenReparacionService.CrearOrden(ordenReparacion);
                UpdateOrdenes();
            }
        }

        private void EditarOrden()
        {
            
            if (SelectedOrden != null)
            {
                var ordenForm = _ordenFormFactory();

                //var ordenFormViewModel = new OrdenReparacionFormViewModel(
                //   _ordenReparacionService,
                //   _vehiculoService,
                //   _clienteService,
                //   _clienteSelectorFactory,
                //   _vehiculoSelectorFactory,
                //   _lineaOrdenFormFactory,
                //   result =>
                //   {
                //       if (result) UpdateOrdenes();
                //   });

                ordenForm.Initialize(SelectedOrden);

                if (ordenForm.ShowDialog() == true)
                {
                    //_ordenReparacionService.ActualizarOrden(SelectedOrden);
                    UpdateOrdenes();
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar una orden de reparación para editar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        //private bool CanEditarOrden() => SelectedOrden != null;

        private void EliminarOrden()
        {
            if (SelectedOrden != null)
            {
                var result = System.Windows.MessageBox.Show(
                    "¿Estás seguro de que deseas eliminar esta orden de reparación?",
                    "Confirmar Eliminación",
                    System.Windows.MessageBoxButton.YesNo);

                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    _ordenReparacionService.EliminarOrden(SelectedOrden.Id);
                    UpdateOrdenes();
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar una orden de reparación para eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        //private bool CanEliminarOrden() => SelectedOrden != null;

        private void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                UpdateOrdenes();
            }
        }

        private bool CanNextPage() => CurrentPage < TotalPages;

        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                UpdateOrdenes();
            }
        }

        private bool CanPreviousPage() => CurrentPage > 1;
    }
}

