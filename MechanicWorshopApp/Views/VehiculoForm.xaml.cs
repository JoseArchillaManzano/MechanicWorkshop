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
    /// Lógica de interacción para VehiculoForm.xaml
    /// </summary>
    public partial class VehiculoForm : Window
    {
        private readonly VehiculoService _vehiculoService;
        private Vehiculo _vehiculo;

        private readonly bool _clienteFijo;

        public VehiculoForm(Vehiculo vehiculo, bool clienteFijo = true)
        {
            InitializeComponent();
            DataContext = vehiculo;

            _clienteFijo = clienteFijo;

            ConfigurarFormulario();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Forzar la validación
            _vehiculo.ForzarValidacion();

            // Verificar errores antes de guardar
            if (Validation.GetHasError(txtMarca) ||
                Validation.GetHasError(txtModelo) ||
                Validation.GetHasError(txtMatricula) ||
                Validation.GetHasError(txtMotor) ||
                Validation.GetHasError(txtBastidor) ||
                Validation.GetHasError(txtKilometraje))
            {
                MessageBox.Show("Por favor, corrige los errores antes de guardar.",
                                "Errores de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            GuardarVehiculo();

            MessageBox.Show("Vehículo guardado correctamente.",
                            "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Cancela la operación
            Close();
        }

        private void GuardarVehiculo()
        {
            if (_vehiculo.Id == 0)
            {
                _vehiculoService.AgregarVehiculo(_vehiculo);
            }
            else
            {
                _vehiculoService.ActualizarVehiculo(_vehiculo);
            }
        }

        private void ConfigurarFormulario()
        {
            if (_clienteFijo)
            {
                // Mostrar el cliente como texto fijo
                txtCliente.IsReadOnly = true;
                btnSeleccionarCliente.Visibility = Visibility.Collapsed; // Botón para buscar clientes
            }
            else
            {
                // Habilitar selección de cliente
                txtCliente.IsReadOnly = false;
                btnSeleccionarCliente.Visibility = Visibility.Visible; // Botón para abrir el filtro
            }
        }

        private void BtnSeleccionarCliente_Click(object sender, RoutedEventArgs e)
        {
            var clienteService = new ClienteService(new TallerContext()); // Asegúrate de tener acceso al servicio
            var clienteDialog = new ClienteSearchDialog(clienteService);

            if (clienteDialog.ShowDialog() == true)
            {
                var clienteSeleccionado = clienteDialog.ClienteSeleccionado;

                if (clienteSeleccionado != null)
                {
                    _vehiculo.Cliente = clienteSeleccionado;
                    txtCliente.Text = clienteSeleccionado.Nombre; // Actualizar visualmente
                }
            }
        }
    }
}