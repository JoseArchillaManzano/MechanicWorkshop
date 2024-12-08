﻿using MechanicWorkshopApp.Data;
using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.Services;
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
    /// Lógica de interacción para ClienteForm.xaml
    /// </summary>
    public partial class ClienteForm : Window
    {
        private Cliente _cliente;
        private ClienteService _clienteService;

        public ClienteForm(ClienteService clienteService, Cliente cliente = null)
        {
            InitializeComponent();
            _cliente = cliente ?? new Cliente();
            _clienteService = clienteService;
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
        private void GuardarCliente()
        {
            if (_cliente.Id == 0)
            {
                // Nuevo cliente
                _clienteService.AgregarCliente(_cliente);
            }
            else
            {
                // Actualizar cliente existente
                _clienteService.ActualizarCliente(_cliente);
            }
        }

    }
}