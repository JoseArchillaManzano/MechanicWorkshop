using MechanicWorkshopApp.Data;
using MechanicWorkshopApp.Models;
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
        public ClientesView()
        {
            InitializeComponent();
            CargarClientes();
        }

        private void CargarClientes()
        {
            using (var context = new TallerContext())
            {
                var clientes = context.Clientes.ToList();
                dgClientes.ItemsSource = clientes;
            }
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            var form = new ClienteForm();
            if (form.ShowDialog() == true)
            {
                CargarClientes(); // Recarga la lista después de guardar
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var clienteSeleccionado = dgClientes.SelectedItem as Cliente;
            if (clienteSeleccionado != null)
            {
                var form = new ClienteForm(clienteSeleccionado);
                if (form.ShowDialog() == true)
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
                    using (var context = new TallerContext())
                    {
                        context.Clientes.Remove(clienteSeleccionado);
                        context.SaveChanges();
                    }
                    CargarClientes(); // Actualiza la lista
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un cliente para eliminar.");
            }
        }
    }
}
