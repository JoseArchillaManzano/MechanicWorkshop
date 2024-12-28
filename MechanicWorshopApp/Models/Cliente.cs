using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Models
{
    public class Cliente : IDataErrorInfo, INotifyPropertyChanged
    {
        public int Id { get; set; }

        // Campos privados y banderas para rastrear interacción
        private string _nombre;
        private bool _nombreSet;

        private string _dniCif;
        private bool _dniCifSet;

        private string _direccion;
        private bool _direccionSet;

        private string _municipio;
        private bool _municipioSet;

        private string _provincia;
        private bool _provinciaSet;

        private string _codigoPostal;
        private bool _codigoPostalSet;

        private string _telefono;
        private bool _telefonoSet;

        // Propiedades con setters modificados
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

        public string DNI_CIF
        {
            get => _dniCif;
            set
            {
                if (_dniCif != value)
                {
                    _dniCif = value;
                    _dniCifSet = true;
                    OnPropertyChanged(nameof(DNI_CIF));
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

        public string CodigoPostal
        {
            get => _codigoPostal;
            set
            {
                if (_codigoPostal != value)
                {
                    _codigoPostal = value;
                    _codigoPostalSet = true;
                    OnPropertyChanged(nameof(CodigoPostal));
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

        // Propiedades que no requieren validación
        public string Municipio
        {
            get => _municipio;
            set
            {
                if (_municipio != value)
                {
                    _municipio = value;
                    _municipioSet = true;
                    OnPropertyChanged(nameof(Municipio));
                }
            }
        }
        public string Provincia
        {
            get => _provincia;
            set
            {
                if (_provincia != value)
                {
                    _provincia = value;
                    _provinciaSet = true;
                    OnPropertyChanged(nameof(Provincia));
                }
            }
        }
        public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();

        public ICollection<OrdenReparacion> OrdenesReparacion { get; set; } = new HashSet<OrdenReparacion>();

        // Implementación de INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Método para forzar la validación
        public void ForzarValidacion()
        {
            _nombreSet = true;
            OnPropertyChanged(nameof(Nombre));

            _dniCifSet = true;
            OnPropertyChanged(nameof(DNI_CIF));

            _direccionSet = true;
            OnPropertyChanged(nameof(Direccion));

            _codigoPostalSet = true;
            OnPropertyChanged(nameof(CodigoPostal));

            _telefonoSet = true;
            OnPropertyChanged(nameof(Telefono));

            _municipioSet = true;
            OnPropertyChanged(nameof(Municipio));

            _provinciaSet = true;
            OnPropertyChanged(nameof(Provincia));

            // Si tienes más propiedades con validación, agrégalas aquí
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

                    case nameof(DNI_CIF):
                        if (_dniCifSet)
                        {
                            if (string.IsNullOrWhiteSpace(DNI_CIF) || DNI_CIF.Length < 9)
                                result = "El DNI/CIF debe tener al menos 9 caracteres.";
                        }
                        break;

                    case nameof(Direccion):
                        if (_direccionSet && string.IsNullOrWhiteSpace(Direccion))
                            result = "La dirección es obligatoria.";
                        break;

                    case nameof(CodigoPostal):
                        if (_codigoPostalSet)
                        {
                            if (string.IsNullOrWhiteSpace(CodigoPostal) || CodigoPostal.Length != 5 || !int.TryParse(CodigoPostal, out _))
                                result = "El código postal debe tener 5 dígitos numéricos.";
                        }
                        break;

                    case nameof(Telefono):
                        if (_telefonoSet)
                        {
                            if (string.IsNullOrWhiteSpace(Telefono) || !long.TryParse(Telefono, out _) || Telefono.Length < 9 || Telefono.Length > 15)
                                result = "El teléfono debe ser un número válido entre 9 y 15 dígitos.";
                        }
                        break;
                    case nameof(Municipio):
                        if (_municipioSet && string.IsNullOrWhiteSpace(Municipio))
                            result = "El municipio es obligatorio";
                        break;
                    case nameof(Provincia):
                        if (_provinciaSet && string.IsNullOrWhiteSpace(Provincia))
                            result = "La provincia es obligatoria";
                        break;
                }

                return result;
            }
        }
    }
}