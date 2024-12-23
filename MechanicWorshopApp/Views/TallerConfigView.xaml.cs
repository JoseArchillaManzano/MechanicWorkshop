using MechanicWorkshopApp.ViewModels;
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
    /// Lógica de interacción para TallerConfigView.xaml
    /// </summary>
    public partial class TallerConfigView : Window
    {
        public TallerConfigView(TallerConfigViewModel tallerConfigViewModel)
        {
            InitializeComponent();
            DataContext = tallerConfigViewModel;
        }
    }
}
