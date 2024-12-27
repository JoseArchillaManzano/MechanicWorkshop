using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Models
{
    public class TallerConfig : IDataErrorInfo, INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _nombre;
        private bool _nombreSet;

        private string _cif;
        private bool _cifSet;

        private string _direccion;
        private bool _direccionSet;

        private string _telefono;
        private bool _telefonoSet;

        private string _registroIndustrial;
        private bool _registroIndustrialSet;

        private double _horaManoObra;
        private bool _horaManoObraSet;

        private int _iva;
        private bool _ivaSet;

        // Propiedades
        public string Nombre
        {
            get => _nombre;
            set
            {
                if (_nombre != value)
                {
                    _nombre = value;
                    _nombreSet = true;
                    OnPropertyChanged(nameof(Nombre));
                }
            }
        }

        public string CIF
        {
            get => _cif;
            set
            {
                if (_cif != value)
                {
                    _cif = value;
                    _cifSet = true;
                    OnPropertyChanged(nameof(CIF));
                }
            }
        }

        public string Direccion
        {
            get => _direccion;
            set
            {
                if (_direccion != value)
                {
                    _direccion = value;
                    _direccionSet = true;
                    OnPropertyChanged(nameof(Direccion));
                }
            }
        }

        public string Telefono
        {
            get => _telefono;
            set
            {
                if (_telefono != value)
                {
                    _telefono = value;
                    _telefonoSet = true;
                    OnPropertyChanged(nameof(Telefono));
                }
            }
        }

        public string RegistroIndustrial
        {
            get => _registroIndustrial;
            set
            {
                if (_registroIndustrial != value)
                {
                    _registroIndustrial = value;
                    _registroIndustrialSet = true;
                    OnPropertyChanged(nameof(RegistroIndustrial));
                }
            }
        }

        public double HoraManoObra
        {
            get => _horaManoObra;
            set
            {
                if (_horaManoObra != value)
                {
                    _horaManoObra = value;
                    _horaManoObraSet = true;
                    OnPropertyChanged(nameof(HoraManoObra));
                }
            }
        }

        public int IVA
        {
            get => _iva;
            set
            {
                if (_iva != value)
                {
                    _iva = value;
                    _ivaSet = true;
                    OnPropertyChanged(nameof(IVA));
                }
            }
        }

        // Implementación de INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                    case nameof(Nombre):
                        if (_nombreSet && string.IsNullOrWhiteSpace(Nombre))
                            result = "El nombre es obligatorio.";
                        break;

                    case nameof(CIF):
                        if (_cifSet && (string.IsNullOrWhiteSpace(CIF) || CIF.Length < 9))
                            result = "El CIF debe tener al menos 9 caracteres.";
                        break;

                    case nameof(Direccion):
                        if (_direccionSet && string.IsNullOrWhiteSpace(Direccion))
                            result = "La dirección es obligatoria.";
                        break;

                    case nameof(Telefono):
                        if (_telefonoSet)
                        {
                            if (string.IsNullOrWhiteSpace(Telefono))
                                result = "El teléfono es obligatorio.";
                            else if (!long.TryParse(Telefono, out _) ||Telefono.Length < 9 || Telefono.Length > 15)
                                result = "El teléfono debe ser un número válido entre 9 y 15 dígitos.";
                        }
                        break;

                    case nameof(RegistroIndustrial):
                        if (_registroIndustrialSet && string.IsNullOrWhiteSpace(RegistroIndustrial))
                            result = "El registro industrial es obligatorio.";
                        break;

                    case nameof(HoraManoObra):
                        if (_horaManoObraSet && HoraManoObra <= 0)
                            result = "La hora de mano de obra debe ser un valor positivo.";
                        break;

                    case nameof(IVA):
                        if (_ivaSet && (IVA < 0 || IVA > 100))
                            result = "El IVA debe estar entre 0 y 100.";
                        break;
                }

                return result;
            }
        }

        // Método para forzar la validación
        public void ForzarValidacion()
        {
            _nombreSet = true;
            OnPropertyChanged(nameof(Nombre));

            _cifSet = true;
            OnPropertyChanged(nameof(CIF));

            _direccionSet = true;
            OnPropertyChanged(nameof(Direccion));

            _telefonoSet = true;
            OnPropertyChanged(nameof(Telefono));

            _registroIndustrialSet = true;
            OnPropertyChanged(nameof(RegistroIndustrial));

            _horaManoObraSet = true;
            OnPropertyChanged(nameof(HoraManoObra));

            _ivaSet = true;
            OnPropertyChanged(nameof(IVA));
        }
    }
}
