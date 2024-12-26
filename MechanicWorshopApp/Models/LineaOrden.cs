using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Models
{
    public class LineaOrden
    {
        public int Id { get; set; }
        public int OrdenReparacionId { get; set; }
        public OrdenReparacion OrdenReparacion { get; set; }

        public string Concepto { get; set; } // Descripción del material o servicio
        public double PrecioUnitario { get; set; } // Precio unitario
        public double Cantidad { get; set; } // Cantidad de materiales o unidades de mano de obra
        public TipoLinea TipoLinea { get; set; } // Distinción entre Material y ManoDeObra

        // Precio total calculado
        public double Total => PrecioUnitario * Cantidad;

        private bool _conceptoSet;
        private bool _precioUnitarioSet;
        private bool _cantidadSet;

        // Implementación de INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Método para forzar la validación
        public void ForzarValidacion()
        {
            _conceptoSet = true;
            OnPropertyChanged(nameof(Concepto));

            _precioUnitarioSet = true;
            OnPropertyChanged(nameof(PrecioUnitario));

            _cantidadSet = true;
            OnPropertyChanged(nameof(Cantidad));
        }

        // Implementación de IDataErrorInfo
        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string result = null;

                switch (columnName)
                {
                    case nameof(Concepto):
                        if (_conceptoSet && string.IsNullOrWhiteSpace(Concepto))
                            result = "El concepto es obligatorio.";
                        break;

                    case nameof(PrecioUnitario):
                        if (_precioUnitarioSet && PrecioUnitario <= 0)
                            result = "El precio unitario debe ser mayor a 0.";
                        break;

                    case nameof(Cantidad):
                        if (_cantidadSet && Cantidad <= 0)
                            result = "La cantidad debe ser mayor a 0.";
                        break;
                }

                return result;
            }
        }
    }
}