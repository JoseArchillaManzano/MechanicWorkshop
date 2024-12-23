﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.Services;
using MechanicWorkshopApp.Utils;
using MechanicWorkshopApp.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MechanicWorkshopApp.ViewModels
{
    public partial class OrdenReparacionFormViewModel : ObservableObject
    {
        private readonly OrdenReparacionService _ordenReparacionService;
        private readonly ClienteService _clienteService;
        private readonly VehiculoService _vehiculoService;
        private readonly LineaOrdenService _lineaOrdenService;
        private readonly TallerConfigService _tallerConfigService;
        private readonly PrinterService _printerService;
        private readonly Func<SelectorClienteView> _clienteSelectorFactory;
        private readonly Func<SelectorVehiculosView> _vehiculoSelectorFactory;
        private readonly Func<LineaOrdenFormView> _lineaOrdenFormFactory;
        private readonly Action<bool> _callback;
        public event Action OnClose;
        private readonly List<int> _lineasEliminadas = new();
        // Callback para cerrar la ventana
        public Action CloseWindow { get; set; }

        [ObservableProperty]
        private OrdenReparacion orden;

        [ObservableProperty]
        private Cliente clienteSeleccionado;

        [ObservableProperty]
        private Vehiculo vehiculoSeleccionado;

        [ObservableProperty]
        private LineaOrden lineaSeleccionada;

        [ObservableProperty]
        private ObservableCollection<LineaOrden> lineasOrden;

        public ICommand SeleccionarClienteCommand { get; }
        public ICommand SeleccionarVehiculoCommand { get; }
        public ICommand AgregarLineaCommand { get; }
        public ICommand EditarLineaCommand { get; }
        public ICommand EliminarLineaCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand CancelarCommand { get; }
        public ICommand GenerarFacturaCommand { get; }
        public OrdenReparacionFormViewModel(
            OrdenReparacionService ordenReparacionService,
            VehiculoService vehiculoService,
            ClienteService clienteService,
            LineaOrdenService lineaOrdenService,
            TallerConfigService tallerConfigService,
            PrinterService printerService,
            Func<SelectorClienteView> clienteSelectorFactory,
            Func<SelectorVehiculosView> vehiculoSelectorFactory,
            Func<LineaOrdenFormView> lineaOrdenFormFactory,
            Action<bool> callback)
        {
            _ordenReparacionService = ordenReparacionService;
            _vehiculoService = vehiculoService;
            _clienteService = clienteService;
            _lineaOrdenService = lineaOrdenService;
            _tallerConfigService = tallerConfigService;
            _printerService = printerService;
            _clienteSelectorFactory = clienteSelectorFactory;
            _vehiculoSelectorFactory = vehiculoSelectorFactory;
            _lineaOrdenFormFactory = lineaOrdenFormFactory;
            _callback = callback;

            SeleccionarClienteCommand = new RelayCommand(SeleccionarCliente);
            SeleccionarVehiculoCommand = new RelayCommand(SeleccionarVehiculo);
            AgregarLineaCommand = new RelayCommand(AgregarLinea);
            EditarLineaCommand = new RelayCommand(EditarLinea);
            EliminarLineaCommand = new RelayCommand(EliminarLinea);
            GuardarCommand = new RelayCommand(() => GuardarOrden(true));
            CancelarCommand = new RelayCommand(Cancelar);
            GenerarFacturaCommand = new RelayCommand(GenerarFactura);

            Orden ??= new OrdenReparacion
            {
                LineasOrden = new ObservableCollection<LineaOrden>()
            };
            LineasOrden = new ObservableCollection<LineaOrden>();
        }

        public void Initialize(OrdenReparacion orden)
        {
            Orden = orden;
            ClienteSeleccionado = orden.Cliente;
            VehiculoSeleccionado = orden.Vehiculo;
            LineasOrden = new ObservableCollection<LineaOrden>(orden.LineasOrden);

            // Sincronizar ClienteId por si no se hizo antes
            if (orden.Cliente != null)
            {
                Orden.ClienteId = orden.Cliente.Id;
            }
        }

        private void AgregarLinea()
        {
            var nuevaLinea = new LineaOrden
            {
                Cantidad = 1, // Valor inicial
                TipoLinea = TipoLinea.Material // Tipo inicial por defecto
            };

            var lineaFormViewModel = new LineaOrdenFormViewModel(nuevaLinea, result =>
            {
                if (result)
                {
                    LineasOrden.Add(nuevaLinea);
                    Orden.LineasOrden.Add(nuevaLinea);
                }
            });

            var lineaForm = new LineaOrdenFormView(lineaFormViewModel);
            lineaForm.ShowDialog();
        }

        private void EditarLinea()
        {
            if (LineaSeleccionada != null)
            {
                var copiaLinea = new LineaOrden
                {
                    Concepto = LineaSeleccionada.Concepto,
                    Cantidad = LineaSeleccionada.Cantidad,
                    PrecioUnitario = LineaSeleccionada.PrecioUnitario,
                    TipoLinea = LineaSeleccionada.TipoLinea
                };
                var lineaFormViewModel = new LineaOrdenFormViewModel(LineaSeleccionada, result =>
                {
                    if (result)
                    {
                        // Actualiza automáticamente ya que LineaSeleccionada apunta a la misma instancia.
                        ActualizarLineasOrden();
                    }
                });

                var lineaForm = new LineaOrdenFormView(lineaFormViewModel);
                lineaForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una línea para editar.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ActualizarLineasOrden()
        {
            // Forzar la notificación de cambio para la colección
            LineasOrden = new ObservableCollection<LineaOrden>(LineasOrden);
            OnPropertyChanged(nameof(LineasOrden));
        }

        private void EliminarLinea()
        {
            if (LineaSeleccionada != null)
            {
                var result = MessageBox.Show("¿Estás seguro de que deseas eliminar esta línea?",
                                             "Confirmar Eliminación",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    // Mover la línea a la lista de eliminadas
                    _lineasEliminadas.Add(LineaSeleccionada.Id);
                    LineasOrden.Remove(lineaSeleccionada);
                    Orden.LineasOrden.Remove(lineaSeleccionada);
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una línea para eliminar.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SeleccionarCliente()
        {
            var selectorClientesView = _clienteSelectorFactory();

            var viewModel = new SelectorClienteViewModel(
                _clienteService,
                cliente =>
                {
                    if (cliente != null)
                    {
                        ClienteSeleccionado = cliente;
                        Orden.Cliente = cliente;
                        Orden.ClienteId = cliente.Id; // Sincronizar ClienteId
                        VehiculoSeleccionado = null; // Resetear vehículo seleccionado
                        Orden.Vehiculo = null; // Limpiar vehículo en la orden
                        OnPropertyChanged(nameof(ClienteSeleccionado)); // Notifica el cambio
                    }
                }
            );

            selectorClientesView.DataContext = viewModel;
            selectorClientesView.ShowDialog();
        }

        private void SeleccionarVehiculo()
        {
            var serviceProvider = ((App)App.Current).Services;
            var selectorVehiculosView = serviceProvider.GetRequiredService<SelectorVehiculosView>();

            var viewModel = new SelectorVehiculosViewModel(
                Orden.ClienteId, // Cliente actual
                _vehiculoService,
                vehiculo =>
                {
                    if (vehiculo != null)
                    {
                        VehiculoSeleccionado = vehiculo;
                        Orden.Vehiculo = vehiculo; // Asignar vehículo a la orden
                        Orden.VehiculoId = vehiculo.Id;
                    }
                }
            );

            selectorVehiculosView.DataContext = viewModel;
            selectorVehiculosView.ShowDialog();
        }

        private void GuardarOrden(bool cerrarFormulario = true)
        {
            if(ClienteSeleccionado is null || VehiculoSeleccionado is null)
            {
                MessageBox.Show("Debe seleccionar un cliente y un vehiculo", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            foreach (var lineaEliminada in _lineasEliminadas)
            {
                _lineaOrdenService.EliminarLineaOrden(lineaEliminada);
            }

            if (Orden.Id == 0)
            {
                _ordenReparacionService.CrearOrden(Orden);
            }
            else
            {
                _ordenReparacionService.ActualizarOrden(Orden);
            }

            if (cerrarFormulario)
            {
                //MessageBox.Show("La orden se guardó correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                _callback?.Invoke(true);
                OnClose?.Invoke();
            }
            
        }

        private void Cancelar()
        {
            var result = MessageBox.Show("Los cambios que haya realizado se perderán si no los guarda. ¿Está seguro que desea salir?",
                                             "Confirmar",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _callback?.Invoke(false); // Notificar que se canceló la operación
                OnClose?.Invoke(); // Evento para cerrar cualquier lógica adicional asociada
            }
            
        }

        private void GenerarFactura()
        {

            GuardarOrden(false);
            var tallerConfig = _tallerConfigService.ObtenerConfiguracion();
            var facturaGenerator = new FacturaPdfGenerator(Orden, tallerConfig);

            var directorio = @"C:\Facturas";
            var filePath = $"Factura_{Orden.Id}.pdf";

            facturaGenerator.GenerarFactura(filePath);
            var rutaFactura = Path.Combine(directorio, filePath);

            MessageBox.Show($"Factura generada correctamente en {rutaFactura}.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);

            _printerService.AbrirPDFEnVisor(rutaFactura);
        }
    }
}
