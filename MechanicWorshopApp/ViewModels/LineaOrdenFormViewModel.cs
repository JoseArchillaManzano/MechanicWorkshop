using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MechanicWorkshopApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MechanicWorkshopApp.ViewModels
{
    public partial class LineaOrdenFormViewModel : ObservableObject
    {
        private readonly LineaOrden _lineaOrden;
        private readonly Action<bool> _onClose;
        private readonly TallerConfig _configTaller;

        [ObservableProperty]
        private string concepto;

        [ObservableProperty]
        private double cantidad;

        [ObservableProperty]
        private double precio;

        private TipoLinea tipo;

        // Propiedad calculada para el Total
        public double Total => Cantidad * Precio;

        public TipoLinea Tipo
        {
            get => tipo;
            set
            {
                if (SetProperty(ref tipo, value))
                {
                    if (tipo == TipoLinea.ManoDeObra && string.IsNullOrEmpty(Concepto))
                    {
                        Concepto = "Mano de obra";
                        Precio = _configTaller.HoraManoObra;
                    }
                    else if (tipo != TipoLinea.ManoDeObra)
                    {
                        Concepto = string.Empty;
                        Precio = 0;
                    }
                }
            }
        }

        public ObservableCollection<TipoLinea> TiposLinea { get; } = new ObservableCollection<TipoLinea>(
            Enum.GetValues(typeof(TipoLinea)).Cast<TipoLinea>()
        );

        public IRelayCommand GuardarCommand { get; }
        public IRelayCommand CancelarCommand { get; }

        public LineaOrdenFormViewModel(LineaOrden lineaOrden, TallerConfig configTaller, Action<bool> onClose )
        {
            _lineaOrden = lineaOrden;
            _onClose = onClose;
            _configTaller = configTaller;
            // Inicializar propiedades con valores de la línea de orden
            Concepto = lineaOrden.Concepto;
            Cantidad = lineaOrden.Cantidad;
            Precio = lineaOrden.PrecioUnitario;
            Tipo = lineaOrden.TipoLinea;

            GuardarCommand = new RelayCommand(Guardar);
            CancelarCommand = new RelayCommand(Cancelar);

            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Cantidad) || args.PropertyName == nameof(Precio))
                {
                    OnPropertyChanged(nameof(Total));
                }
            };
        }

        private void Guardar()
        {
            if (ValidarFormulario())
            {
                // Actualizar los valores en la línea de orden
                _lineaOrden.Concepto = Concepto;
                _lineaOrden.Cantidad = Cantidad;
                _lineaOrden.PrecioUnitario = Precio;
                _lineaOrden.TipoLinea = Tipo;

                _onClose?.Invoke(true); // Notificar que se ha guardado correctamente
                CloseWindow();
            }
            else
            {
                MessageBox.Show("Por favor, rellene todos los campos antes de guardar.", "Errores", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancelar()
        {
            _onClose?.Invoke(false); // Notificar que se ha cancelado
            CloseWindow();
        }

        private bool ValidarFormulario()
        {
            _lineaOrden.ForzarValidacion();
            return !string.IsNullOrWhiteSpace(Concepto) &&
                   Cantidad > 0 &&
                   Precio >= 0;
        }

        private void CloseWindow()
        {
            if (Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this) is Window window)
            {
                window.Close();
            }
        }
    }
}
