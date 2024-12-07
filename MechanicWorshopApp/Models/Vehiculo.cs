using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Models
{
    public class Vehiculo : IDataErrorInfo, INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public string? Observaciones { get; set; }

        // Campos privados y banderas para rastrear interacción
        private string _marca;
        private bool _marcaSet;

        private string _modelo;
        private bool _modeloSet;

        private string _matricula;
        private bool _matriculaSet;

        private string _motor;
        private bool _motorSet;

        private string _bastidor;
        private bool _bastidorSet;

        private int _kilometraje;
        private bool _kilometrajeSet;

        // Propiedades con setters modificados
        public string Marca
        {
            get => _marca;
            set
            {
                if (_marca != value)
                {
                    _marca = value;
                    _marcaSet = true;
                    OnPropertyChanged(nameof(Marca));
                }
            }
        }

        public string Modelo
        {
            get => _modelo;
            set
            {
                if (_modelo != value)
                {
                    _modelo = value;
                    _modeloSet = true;
                    OnPropertyChanged(nameof(Modelo));
                }
            }
        }

        public string Matricula
        {
            get => _matricula;
            set
            {
                if (_matricula != value)
                {
                    _matricula = value;
                    _matriculaSet = true;
                    OnPropertyChanged(nameof(Matricula));
                }
            }
        }

        public string Motor
        {
            get => _motor;
            set
            {
                if (_motor != value)
                {
                    _motor = value;
                    _motorSet = true;
                    OnPropertyChanged(nameof(Motor));
                }
            }
        }

        public string Bastidor
        {
            get => _bastidor;
            set
            {
                if (_bastidor != value)
                {
                    _bastidor = value;
                    _bastidorSet = true;
                    OnPropertyChanged(nameof(Bastidor));
                }
            }
        }

        public int Kilometraje
        {
            get => _kilometraje;
            set
            {
                if (_kilometraje != value)
                {
                    _kilometraje = value;
                    _kilometrajeSet = true;
                    OnPropertyChanged(nameof(Kilometraje));
                }
            }
        }

        // Implementación de INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Método para forzar la validación
        public void ForzarValidacion()
        {
            _marcaSet = true;
            OnPropertyChanged(nameof(Marca));

            _modeloSet = true;
            OnPropertyChanged(nameof(Modelo));

            _matriculaSet = true;
            OnPropertyChanged(nameof(Matricula));

            _motorSet = true;
            OnPropertyChanged(nameof(Motor));

            _bastidorSet = true;
            OnPropertyChanged(nameof(Bastidor));

            _kilometrajeSet = true;
            OnPropertyChanged(nameof(Kilometraje));
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
                    case nameof(Marca):
                        if (_marcaSet && string.IsNullOrWhiteSpace(Marca))
                            result = "La marca es obligatoria.";
                        break;

                    case nameof(Modelo):
                        if (_modeloSet && string.IsNullOrWhiteSpace(Modelo))
                            result = "El modelo es obligatorio.";
                        break;

                    case nameof(Matricula):
                        if (_matriculaSet)
                        {
                            if (string.IsNullOrWhiteSpace(Matricula))
                                result = "La matrícula es obligatoria.";
                            else if (Matricula.Length < 4 || Matricula.Length > 10)
                                result = "La matrícula debe tener entre 4 y 10 caracteres.";
                        }
                        break;

                    case nameof(Motor):
                        if (_motorSet && string.IsNullOrWhiteSpace(Motor))
                            result = "El tipo de motor es obligatorio.";
                        break;

                    case nameof(Bastidor):
                        if (_bastidorSet && string.IsNullOrWhiteSpace(Bastidor))
                            result = "El número de bastidor es obligatorio.";
                        break;

                    case nameof(Kilometraje):
                        if (_kilometrajeSet && Kilometraje < 0)
                            result = "El kilometraje debe ser un número positivo.";
                        break;
                }

                return result;
            }
        }
    }
}
