using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts.Wpf;
using LiveCharts;
using MechanicWorkshopApp.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace MechanicWorkshopApp.ViewModels
{
    public partial class MetricasViewModel : ObservableObject
    {
        private readonly ClienteService _clienteService;
        private readonly VehiculoService _vehiculoService;
        private readonly OrdenReparacionService _ordenService;

        [ObservableProperty]
        private int totalClientes;

        [ObservableProperty]
        private int totalVehiculos;

        [ObservableProperty]
        private int totalOrdenesActivas;

        [ObservableProperty]
        private SeriesCollection graficoOrdenesPorMes;

        [ObservableProperty]
        private SeriesCollection graficoIngresosMensuales;

        [ObservableProperty]
        private string[] meses;

        [ObservableProperty]
        private Func<double, string> formatoEje;

        [ObservableProperty]
        private IEnumerable<string> añosDisponibles;

        [ObservableProperty]
        private string añoSeleccionado;

        [ObservableProperty]
        private int totalOrdenesCerradas;

        public MetricasViewModel(
            ClienteService clienteService,
            VehiculoService vehiculoService,
            OrdenReparacionService ordenService)
        {
            _clienteService = clienteService;
            _vehiculoService = vehiculoService;
            _ordenService = ordenService;

            FormatoEje = value => value.ToString("N0"); // Para mostrar los valores enteros en los ejes
            CargarMetricas();
        }

        private void CargarMetricas()
        {
            // Métricas clave
            TotalClientes = _clienteService.ObtenerTotalClientes();
            TotalVehiculos = _vehiculoService.ObtenerTotalVehiculos();
            TotalOrdenesActivas = _ordenService.ObtenerOrdenesActivas();
            TotalOrdenesCerradas = _ordenService.ObtenerOrdenesCerradas();

            // Años y meses cargados en colecciones locales
            AñosDisponibles = new ObservableCollection<string>(_ordenService.ObtenerAñosDisponibles().ToList());
            AñoSeleccionado = AñosDisponibles.LastOrDefault() ?? DateTime.Now.Year.ToString();

            GenerarMeses(); // Generar la lista de meses
            // Cargar gráficos
            CargarGraficoOrdenesPorMes();
            CargarGraficoIngresosMensuales();
        }

        private void CargarGraficoOrdenesPorMes()
        {
            if (int.TryParse(AñoSeleccionado, out int año))
            {
                var datosOrdenes = _ordenService.ObtenerOrdenesPorMes(año);

                // Mapear los datos en base a los meses definidos
                var valoresOrdenes = Meses.Select(mes =>
                    datosOrdenes.ContainsKey(mes) ? datosOrdenes[mes] : 0
                ).ToList();

                GraficoOrdenesPorMes = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Órdenes",
                        Values = new ChartValues<int>(valoresOrdenes)
                    }
                };
            }
        }

        private void CargarGraficoIngresosMensuales()
        {
            if (int.TryParse(AñoSeleccionado, out int año))
            {
                var datosIngresos = _ordenService.ObtenerIngresosPorMes(año);
                var datosIngresosManoObra = _ordenService.ObtenerIngresosPorManoDeObra(año);

                // Mapear los datos en base a los meses definidos
                var valoresIngresos = Meses.Select(mes =>
                    datosIngresos.ContainsKey(mes) ? datosIngresos[mes] : 0.0
                ).ToList();

                var valoresManoObra = Meses.Select(mes =>
                    datosIngresosManoObra.ContainsKey(mes) ? datosIngresosManoObra[mes] : 0.0
                ).ToList();

                GraficoIngresosMensuales = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Ingresos",
                        Values = new ChartValues<double>(valoresIngresos),
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize = 10
                    },
                    new LineSeries
                    {
                        Title = "Mano de Obra",
                        Values = new ChartValues<double>(valoresManoObra),
                        StrokeThickness = 2,
                        PointGeometry = DefaultGeometries.Square,
                        PointGeometrySize = 10
                    }
                };
            }
        }

        partial void OnAñoSeleccionadoChanged(string value)
        {
            if (int.TryParse(value, out int año))
            {
                // Generar meses y recargar gráficos
                GenerarMeses();
                CargarGraficoOrdenesPorMes();
                CargarGraficoIngresosMensuales();
            }
        }

        private void GenerarMeses()
        {
            // Lista de meses en español
            Meses = new[]
            {
        "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
        "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
    };
        }
    }
}

