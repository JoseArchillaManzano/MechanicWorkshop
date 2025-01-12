using CommunityToolkit.Mvvm.ComponentModel;
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
        private readonly Func<VehiculoForm> _vehiculoFormFactory;
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

        private Cliente _clienteOriginal;
        private OrdenReparacion _ordenCopia;

        public ICommand SeleccionarClienteCommand { get; }
        public ICommand SeleccionarVehiculoCommand { get; }
        public ICommand AgregarLineaCommand { get; }
        public ICommand EditarLineaCommand { get; }
        public ICommand EliminarLineaCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand CancelarCommand { get; }
        public ICommand GenerarFacturaCommand { get; }
        public ICommand GenerarPresupuestoCommand { get; }
        public ICommand GenerarOrdenReparacionCommand { get; }
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
            Func<VehiculoForm> vehiculoFormFactory,
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
            _vehiculoFormFactory = vehiculoFormFactory;
            _callback = callback;

            SeleccionarClienteCommand = new RelayCommand(SeleccionarCliente);
            SeleccionarVehiculoCommand = new RelayCommand(SeleccionarVehiculo);
            AgregarLineaCommand = new RelayCommand(AgregarLinea);
            EditarLineaCommand = new RelayCommand(EditarLinea);
            EliminarLineaCommand = new RelayCommand(EliminarLinea);
            GuardarCommand = new RelayCommand(() => GuardarOrden(true));
            CancelarCommand = new RelayCommand(Cancelar);
            GenerarFacturaCommand = new RelayCommand(GenerarFactura);
            GenerarPresupuestoCommand = new RelayCommand(GenerarPresupuesto);
            GenerarOrdenReparacionCommand = new RelayCommand(GenerarOrdenReparacion);

            Orden ??= new OrdenReparacion
            {
                LineasOrden = new ObservableCollection<LineaOrden>()
            };
            LineasOrden = new ObservableCollection<LineaOrden>();
        }

        public void Initialize(OrdenReparacion orden)
        {
            Orden = orden;
            ClienteSeleccionado = Orden.Cliente;
            VehiculoSeleccionado = Orden.Vehiculo;
            LineasOrden = new ObservableCollection<LineaOrden>(Orden.LineasOrden);

        }

        private void AgregarLinea()
        {
            var nuevaLinea = new LineaOrden
            {
                Cantidad = 1, // Valor inicial
                TipoLinea = TipoLinea.Material // Tipo inicial por defecto
            };

            var lineaFormViewModel = new LineaOrdenFormViewModel(nuevaLinea, _tallerConfigService.ObtenerConfiguracion(), result =>
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
                var lineaFormViewModel = new LineaOrdenFormViewModel(LineaSeleccionada, _tallerConfigService.ObtenerConfiguracion(), result =>
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
                        Orden.Cliente = null; //para que no de problemas con el tracking
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
            var vehiculos = _vehiculoService.ObtenerVehiculosPorCliente(ClienteSeleccionado.Id);

            if (vehiculos == null || !vehiculos.Any())
            {
                // Si no hay vehículos asociados, abrir el formulario para agregar uno
                MessageBox.Show("No se encontraron vehículos para el cliente seleccionado. Debe de añadir un vehiculo", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                var nuevoVehiculo = new Vehiculo
                {
                    ClienteId = ClienteSeleccionado.Id,
                    Cliente = ClienteSeleccionado,
                };

                var vehiculoFormViewModel = new VehiculoFormViewModel(nuevoVehiculo, _vehiculoService, ClienteSeleccionado.Nombre, result =>
                {
                    if (result)
                    {
                        VehiculoSeleccionado = nuevoVehiculo;
                        Orden.Vehiculo = nuevoVehiculo;
                        Orden.VehiculoId = nuevoVehiculo.Id;
                    }
                });

                var vehiculoForm = _vehiculoFormFactory();
                
                vehiculoForm.Initialize(vehiculoFormViewModel);

                vehiculoForm.ShowDialog();
            }
            else
            {
                // Si hay vehículos, abrir el selector de vehículos
                var selectorVehiculosView = _vehiculoSelectorFactory();

                var viewModel = new SelectorVehiculosViewModel(
                    ClienteSeleccionado.Id,
                    _vehiculoService,
                    vehiculo =>
                    {
                        if (vehiculo != null)
                        {
                            VehiculoSeleccionado = vehiculo;
                            Orden.Vehiculo = vehiculo;
                            Orden.VehiculoId = vehiculo.Id;
                        }
                    }
                );

                selectorVehiculosView.DataContext = viewModel;
                selectorVehiculosView.ShowDialog();
            }
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
                var linea = Orden.LineasOrden.FirstOrDefault(l => l.Id == lineaEliminada);
                if (linea != null)
                {
                    Orden.LineasOrden.Remove(linea); // Elimina de la colección en memoria
                    _lineaOrdenService.EliminarLineaOrden(lineaEliminada); // Elimina en la base de datos
                }
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
                _callback?.Invoke(true);
                OnClose?.Invoke();
            }
            
        }

        public void Cancelar()
        {
            var result = MessageBox.Show("Los cambios que haya realizado se perderán si no los guarda. ¿Está seguro que desea salir?",
                                             "Confirmar",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (Orden.Id != 0)
                {
                    // 1) Recargar desde BD (con Includes)
                    var reloaded = _ordenReparacionService.RecargarEntidad(Orden.Id);

                    // 2) Forzar la notificación de cambios al ViewModel
                    //    Reasignando la propiedad para que se dispare OnPropertyChanged.
                    Orden = reloaded;
                    OnPropertyChanged(nameof(Orden));

                    // 3) Vuelves a asignar las propiedades que muestra tu formulario:
                    ClienteSeleccionado = Orden.Cliente;
                    OnPropertyChanged(nameof(ClienteSeleccionado));

                    VehiculoSeleccionado = Orden.Vehiculo;
                    OnPropertyChanged(nameof(VehiculoSeleccionado));

                    LineasOrden = new ObservableCollection<LineaOrden>(Orden.LineasOrden);
                    OnPropertyChanged(nameof(LineasOrden));
                }
                _callback?.Invoke(false); // Notificar que se canceló la operación
                OnClose?.Invoke(); // Evento para cerrar cualquier lógica adicional asociada

            }

        }

        private void GenerarFactura()
        {
            if (!DatosSonCorrectos()) return;

            if (Orden.FechaSalida is null)
            {
                Orden.FechaSalida = DateTime.Now;
                OnPropertyChanged(nameof(Orden.FechaSalida)); // Notifica el cambio a la interfaz
                Orden = Orden; // Esto asegura que la propiedad Orden se refresque
                OnPropertyChanged(nameof(Orden));
            }
            GuardarOrden(false);
            var tallerConfig = _tallerConfigService.ObtenerConfiguracion();
            var copiaOrden = new OrdenReparacion
            {
                Id = Orden.Id,
                Cliente = ClienteSeleccionado,
                ClienteId = ClienteSeleccionado.Id,
                Descripcion = Orden.Descripcion,
                LineasOrden = Orden.LineasOrden,
                Vehiculo = VehiculoSeleccionado,
                VehiculoId = VehiculoSeleccionado.Id
            };
            var facturaGenerator = new FacturaPdfGenerator(copiaOrden, tallerConfig);

            var directorio = @"C:\Facturas";
            var filePath = $"Factura_{Orden.Id}.pdf";

            facturaGenerator.GenerarFactura(filePath, false);
            var rutaFactura = Path.Combine(directorio, filePath);

            MessageBox.Show($"Factura generada correctamente en {rutaFactura}. Los datos que haya podido modificar se han guardado correctamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);

            _printerService.AbrirPDFEnVisor(rutaFactura);
        }

        private void GenerarPresupuesto()
        {
            if (!DatosSonCorrectos()) return;

            GuardarOrden(false);
            var tallerConfig = _tallerConfigService.ObtenerConfiguracion();
            var copiaOrden = new OrdenReparacion
            {
                Id = Orden.Id,
                Cliente = ClienteSeleccionado,
                ClienteId = ClienteSeleccionado.Id,
                Descripcion = Orden.Descripcion,
                LineasOrden = Orden.LineasOrden,
                Vehiculo = VehiculoSeleccionado,
                VehiculoId = VehiculoSeleccionado.Id
            };
            var facturaGenerator = new FacturaPdfGenerator(copiaOrden, tallerConfig);

            var directorio = @"C:\Presupuestos";
            var filePath = $"Presupuesto_{Orden.Id}.pdf";

            facturaGenerator.GenerarFactura(filePath, true);
            var rutaPresupuesto = Path.Combine(directorio, filePath);

            MessageBox.Show($"Presupuesto generado correctamente en {rutaPresupuesto}. Los datos que haya podido modificar se han guardado correctamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);

            _printerService.AbrirPDFEnVisor(rutaPresupuesto);
        }

        private void GenerarOrdenReparacion()
        {
            if (!DatosSonCorrectos()) return;

            GuardarOrden(false);
            var tallerConfig = _tallerConfigService.ObtenerConfiguracion();
            var copiaOrden = new OrdenReparacion
            {
                Id = Orden.Id,
                Cliente = ClienteSeleccionado,
                ClienteId = ClienteSeleccionado.Id,
                Descripcion = Orden.Descripcion,
                LineasOrden = Orden.LineasOrden,
                Vehiculo = VehiculoSeleccionado,
                VehiculoId = VehiculoSeleccionado.Id
            };
            var facturaGenerator = new FacturaPdfGenerator(copiaOrden, tallerConfig);

            var directorio = @"C:\OrdenesReparacion";
            var filePath = $"OrdenReparacion_{Orden.Id}.pdf";

            facturaGenerator.GenerarOrdenReparacion(filePath);
            var rutaOrdenReparacion = Path.Combine(directorio, filePath);

            MessageBox.Show($"Orden de reparación generada correctamente en {rutaOrdenReparacion}. Los datos que haya podido modificar se han guardado correctamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);

            _printerService.AbrirPDFEnVisor(rutaOrdenReparacion);
        }

        private bool DatosSonCorrectos()
        {
            if (ClienteSeleccionado is null || VehiculoSeleccionado is null)
            {
                MessageBox.Show("Debe seleccionar un cliente y un vehiculo", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            return true;
        }
    }
}
