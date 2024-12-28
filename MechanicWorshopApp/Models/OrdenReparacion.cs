using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Models
{
    public class OrdenReparacion
    {
        public int Id { get; set; }
        public DateTime FechaEntrada { get; set; } = DateTime.UtcNow;
        public DateTime? FechaSalida { get; set; } // Nullable para permitir órdenes sin fecha de salida
        public string Descripcion { get; set; } = string.Empty;

        // Relación con cliente
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        // Relación con vehículo
        public int VehiculoId { get; set; }
        public Vehiculo Vehiculo { get; set; }

        // Relación con líneas de orden
        public ICollection<LineaOrden> LineasOrden { get; set; } = new List<LineaOrden>();

        private bool _descripcionSet;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Método para forzar la validación
        public void ForzarValidacion()
        {
            _descripcionSet = true;
            OnPropertyChanged(nameof(Descripcion));
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
                    case nameof(Descripcion):
                        if (_descripcionSet && string.IsNullOrWhiteSpace(Descripcion))
                            result = "Debe indicar el trabajo a realizar";
                        break;
                }

                return result;
            }
        }
    }
}