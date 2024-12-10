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
    /// Lógica de interacción para VehiculoForm.xaml
    /// </summary>
    public partial class VehiculoForm : Window
    {

        public VehiculoForm()
        {
            InitializeComponent();
        }

        public void Initialize(VehiculoFormViewModel viewModel)
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