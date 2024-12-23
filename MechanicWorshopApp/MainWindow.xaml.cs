using MechanicWorkshopApp.Data;
using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.Services;
using MechanicWorkshopApp.ViewModels;
using MechanicWorkshopApp.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MechanicWorkshopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();            
        }
        private void BtnClientes_Click(object sender, RoutedEventArgs e)
        {
            var serviceProvider = ((App)Application.Current).Services;

            // Resuelve la ventana desde el contenedor
            var clientesView = serviceProvider.GetRequiredService<ClientesView>();
            clientesView.Show();
        }

        private void BtnOrdenes_Click(object sender, RoutedEventArgs e)
        {
            var serviceProvider = ((App)Application.Current).Services;

            // Resuelve la ventana desde el contenedor
            var ordenesView = serviceProvider.GetRequiredService<OrdenReparacionView>();
            ordenesView.Show();
        }

        private void BtnConfiguracion_Click(object sender, RoutedEventArgs e)
        {
            var serviceProvider = ((App)Application.Current).Services;

            // Resuelve la ventana desde el contenedor
            var ordenesView = serviceProvider.GetRequiredService<TallerConfigView>();
            ordenesView.Show();
        }

    }
}