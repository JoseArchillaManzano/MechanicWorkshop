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
    /// Lógica de interacción para ClienteForm.xaml
    /// </summary>
    public partial class ClienteForm : Window
    {
        private Cliente _cliente;

        public ClienteForm(Cliente cliente = null)
        {
            InitializeComponent();
            _cliente = cliente ?? new Cliente();
            DataContext = _cliente; // Vinculamos el DataContext al cliente
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Forzar la validación en todos los campos
            _cliente.ForzarValidacion();

            // Verificar si hay errores antes de guardar
            if (Validation.GetHasError(txtNombre) ||
                Validation.GetHasError(txtDNI_CIF) ||
                Validation.GetHasError(txtDireccion) ||
                Validation.GetHasError(txtCodigoPostal) ||
                Validation.GetHasError(txtTelefono))
            {
                
                MessageBox.Show("Por favor, corrige los errores antes de guardar.", "Errores de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            GuardarCliente();

            MessageBox.Show("Cliente guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; // Cancela la operación
            this.Close();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Tag = true; // Marca el campo como interactuado
            }
        }

        private void GuardarCliente()
        {
            using (var context = new TallerContext())
            {
                _cliente.Nombre = txtNombre.Text;
                _cliente.DNI_CIF = txtDNI_CIF.Text;
                _cliente.Direccion = txtDireccion.Text;
                _cliente.CodigoPostal = txtCodigoPostal.Text;
                _cliente.Municipio = txtMunicipio.Text;
                _cliente.Provincia = txtProvincia.Text;
                _cliente.Telefono = txtTelefono.Text;

                if (_cliente.Id == 0)
                {
                    context.Clientes.Add(_cliente);
                }
                else
                {
                    context.Clientes.Update(_cliente);
                }

                context.SaveChanges();
            }
        }
    }
}