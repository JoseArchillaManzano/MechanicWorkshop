﻿using MechanicWorkshopApp.Data;
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
        public void Initialize(ClientesFormViewModel viewModel)
        {
            DataContext = viewModel;
            // Suscribirse al evento de cierre del formulario desde el ViewModel
            viewModel.OnClose += () =>
            {
                DialogResult = true;
                Close();
            };
        }

    }
}