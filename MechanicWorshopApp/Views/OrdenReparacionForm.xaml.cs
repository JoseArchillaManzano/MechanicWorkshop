using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.ViewModels;
using System.Windows;

namespace MechanicWorkshopApp.Views
{
    /// <summary>
    /// Lógica de interacción para OrdenReparacionForm.xaml
    /// </summary>
    public partial class OrdenReparacionForm : Window
    {
        private readonly OrdenReparacionFormViewModel _viewModel;
        public OrdenReparacionForm(OrdenReparacionFormViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        public void Initialize(OrdenReparacion orden)
        {
            _viewModel.Initialize(orden);
            _viewModel.OnClose += () =>
            {
                DialogResult = true;
                Close();
            };
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show(
                "Los cambios que haya realizado se perderán si no los guarda. ¿Está seguro que desea salir?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                // Cancela el cierre de la ventana
                e.Cancel = true;
            }
        }
    }
}
