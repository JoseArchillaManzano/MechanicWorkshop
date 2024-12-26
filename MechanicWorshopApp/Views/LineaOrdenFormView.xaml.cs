using MechanicWorkshopApp.ViewModels;

using System.Windows;

namespace MechanicWorkshopApp.Views
{
    /// <summary>
    /// Lógica de interacción para LineaOrdenView.xaml
    /// </summary>
    public partial class LineaOrdenFormView : Window
    {
        public LineaOrdenFormView(LineaOrdenFormViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
