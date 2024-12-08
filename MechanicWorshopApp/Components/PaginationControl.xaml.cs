using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MechanicWorkshopApp.Components
{
    /// <summary>
    /// Lógica de interacción para PaginationControl.xaml
    /// </summary>
    public partial class PaginationControl : UserControl, INotifyPropertyChanged
    {
        public PaginationControl()
        {
            InitializeComponent();
        }

        // DependencyProperty para CurrentPage con TwoWay Binding
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(
                nameof(CurrentPage),
                typeof(int),
                typeof(PaginationControl),
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCurrentPageChanged));

        // DependencyProperty para TotalPages
        public static readonly DependencyProperty TotalPagesProperty =
            DependencyProperty.Register(
                nameof(TotalPages),
                typeof(int),
                typeof(PaginationControl),
                new PropertyMetadata(1, OnTotalPagesChanged));

        // DependencyProperty para PreviousPageCommand
        public static readonly DependencyProperty PreviousPageCommandProperty =
            DependencyProperty.Register(
                nameof(PreviousPageCommand),
                typeof(ICommand),
                typeof(PaginationControl));

        // DependencyProperty para NextPageCommand
        public static readonly DependencyProperty NextPageCommandProperty =
            DependencyProperty.Register(
                nameof(NextPageCommand),
                typeof(ICommand),
                typeof(PaginationControl));

        // Eventos y métodos de notificación de propiedad
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Propiedad CurrentPage
        public int CurrentPage
        {
            get => (int)GetValue(CurrentPageProperty);
            set 
            {
                SetValue(CurrentPageProperty, value);
                OnPropertyChanged(nameof(CurrentPage));
            } 

        }

        // Propiedad TotalPages
        public int TotalPages
        {
            get => (int)GetValue(TotalPagesProperty);
            set => SetValue(TotalPagesProperty, value);
        }

        // Propiedad PreviousPageCommand
        public ICommand PreviousPageCommand
        {
            get => (ICommand)GetValue(PreviousPageCommandProperty);
            set => SetValue(PreviousPageCommandProperty, value);
        }

        // Propiedad NextPageCommand
        public ICommand NextPageCommand
        {
            get => (ICommand)GetValue(NextPageCommandProperty);
            set => SetValue(NextPageCommandProperty, value);
        }

        // Propiedad CanGoPrevious
        public bool CanGoPrevious => CurrentPage > 1;

        // Propiedad CanGoNext
        public bool CanGoNext => CurrentPage < TotalPages;

        // Callback para cambios en CurrentPage
        private static void OnCurrentPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PaginationControl)d;
            control.OnPropertyChanged(nameof(CanGoPrevious));
            control.OnPropertyChanged(nameof(CanGoNext));
        }

        // Callback para cambios en TotalPages
        private static void OnTotalPagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PaginationControl)d;
            control.OnPropertyChanged(nameof(CanGoNext));
        }
    }
}