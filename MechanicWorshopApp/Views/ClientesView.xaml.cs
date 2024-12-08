using MechanicWorkshopApp.Configuration;
using MechanicWorkshopApp.Data;
using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.Services;
using MechanicWorkshopApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
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
    /// Lógica de interacción para ClientesView.xaml
    /// </summary>
    public partial class ClientesView : Window
    {
        private readonly ClienteService _clienteService;
        private int _pageSize = AppSettings.PageSize; // Tamaño de página por defecto

        public int CurrentPage { get; set; } = 1; // Página inicial
        public int TotalClientes { get; set; } // Total de clientes

        private readonly ClientesViewModel _viewModel;
        private readonly Func<ClienteForm> _clienteFormFactory;

        public ClientesView(ClientesViewModel viewModel, ClienteService clienteService, Func<ClienteForm> clienteFormFactory)
        {
            InitializeComponent();

            _clienteService = clienteService; // Instancia del servicio
            _clienteFormFactory = clienteFormFactory;
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void CargarTotalClientes()
        {
            TotalClientes = _clienteService.ObtenerTotalClientes();
        }

        private void CargarClientes()
        {
            // Llamar al servicio para obtener los clientes de la página actual
            var clientes = _clienteService.ObtenerClientesPaginados(CurrentPage, _pageSize);
            dgClientes.ItemsSource = clientes;
        }

        private void PaginationControl_PageChanged(object sender, RoutedEventArgs e)
        {
            // Cambiar de página y recargar los clientes
            CargarClientes();
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            var clienteForm = _clienteFormFactory();
            if (clienteForm.ShowDialog() == true)
            {
                //CargarTotalClientes(); // Actualiza el total de clientes
                //CargarClientes(); // Recarga la lista después de guardar
                _viewModel.UpdateClientes();
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var clienteSeleccionado = dgClientes.SelectedItem as Cliente;
            if (clienteSeleccionado != null)
            {
                var clienteForm = _clienteFormFactory();
                clienteForm.DataContext = clienteSeleccionado;
                if (clienteForm.ShowDialog() == true)
                {
                    CargarClientes(); // Recarga la lista después de editar
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un cliente para editar.");
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var clienteSeleccionado = dgClientes.SelectedItem as Cliente;
            if (clienteSeleccionado != null)
            {
                var result = MessageBox.Show("¿Estás seguro de que deseas eliminar este cliente?",
                                             "Confirmar Eliminación", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    _clienteService.EliminarCliente(clienteSeleccionado.Id);
                    CargarTotalClientes(); // Actualiza el total de clientes
                    CargarClientes(); // Recarga la lista
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un cliente para eliminar.");
            }
        }

        private void BtnMostrarVehiculos_Click(object sender, RoutedEventArgs e)
        {
            var clienteSeleccionado = ((FrameworkElement)sender).DataContext as Cliente;

            if (clienteSeleccionado != null)
            {
                // Obtiene el proveedor de servicios
                var serviceProvider = ((App)Application.Current).Services;

                // Resuelve la ventana VehiculosView desde el contenedor
                var vehiculosView = serviceProvider.GetRequiredService<VehiculosView>();

                // Establece el cliente seleccionado como contexto o parámetro
                vehiculosView.DataContext = clienteSeleccionado;

                // Muestra la ventana
                vehiculosView.ShowDialog();
            }
            else
            {
                MessageBox.Show("Error al intentar cargar los vehículos. Por favor, inténtalo de nuevo.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnPageSizeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && DataContext is ClientesViewModel viewModel)
            {
                int newPageSize = (int)e.AddedItems[0];
                viewModel.ChangePageSize(newPageSize);
            }
        }
    }
}
