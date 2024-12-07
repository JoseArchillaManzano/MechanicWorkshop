using MechanicWorkshopApp.Data;
using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MechanicWorkshopApp.Views
{
    /// <summary>
    /// Lógica de interacción para VehiculosView.xaml
    /// </summary>
    public partial class VehiculosView : Window
    {
        private readonly VehiculoService _vehiculoService;
        private Cliente _cliente;

        public VehiculosView(Cliente cliente)
        {
            InitializeComponent();

            _cliente = cliente;
            _vehiculoService = new VehiculoService(new TallerContext());

            Title = $"Vehículos del Cliente: {_cliente.Nombre}";

            // Cargar vehículos al iniciar
            CargarVehiculos();
        }

        private void CargarVehiculos()
        {
            dgVehiculos.ItemsSource = _vehiculoService.ObtenerVehiculosPorCliente(_cliente.Id).ToList();
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            var vehiculoForm = new VehiculoForm(new Vehiculo { Cliente = _cliente });
            if (vehiculoForm.ShowDialog() == true)
            {
                CargarVehiculos(); // Refrescar lista
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgVehiculos.SelectedItem is Vehiculo vehiculoSeleccionado)
            {
                var vehiculoForm = new VehiculoForm(vehiculoSeleccionado);
                if (vehiculoForm.ShowDialog() == true)
                {
                    CargarVehiculos(); // Refrescar lista
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un vehículo para editar.",
                                "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgVehiculos.SelectedItem is Vehiculo vehiculoSeleccionado)
            {
                var result = MessageBox.Show("¿Estás seguro de que deseas eliminar este vehículo?",
                                             "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _vehiculoService.EliminarVehiculo(vehiculoSeleccionado.Id);
                    CargarVehiculos(); // Refrescar lista
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un vehículo para eliminar.",
                                "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
