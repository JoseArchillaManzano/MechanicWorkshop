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
    /// Lógica de interacción para ClienteSearchDialog.xaml
    /// </summary>
    public partial class ClienteSearchDialog : Window
    {
        private readonly ClienteService _clienteService;
        private List<Cliente> _clientes; // Lista completa de clientes
        public Cliente ClienteSeleccionado { get; private set; } // Cliente seleccionado

        public ClienteSearchDialog(ClienteService clienteService)
        {
            InitializeComponent();
            _clienteService = clienteService;

            // Cargar clientes al inicio
            CargarClientes();
        }

        private void CargarClientes()
        {
            _clientes = _clienteService.ObtenerClientes().ToList();
            dgClientes.ItemsSource = _clientes;
        }

        private void TxtBuscar_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string filtro = txtBuscar.Text.ToLower();

            // Filtrar clientes según el texto ingresado
            var clientesFiltrados = _clientes
                .Where(c => c.Nombre.ToLower().Contains(filtro) || c.DNI_CIF.ToLower().Contains(filtro))
                .ToList();

            dgClientes.ItemsSource = clientesFiltrados;
        }

        private void DgClientes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Actualizar cliente seleccionado
            ClienteSeleccionado = dgClientes.SelectedItem as Cliente;
        }

        private void BtnSeleccionar_Click(object sender, RoutedEventArgs e)
        {
            if (ClienteSeleccionado == null)
            {
                MessageBox.Show("Por favor, selecciona un cliente.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Cierra el diálogo con éxito
            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            // Cierra el diálogo sin seleccionar un cliente
            DialogResult = false;
            Close();
        }
    }
}