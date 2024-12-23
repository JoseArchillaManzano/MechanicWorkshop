using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace MechanicWorkshopApp.ViewModels
{
    public partial class TallerConfigViewModel : ObservableObject
    {
        private readonly TallerConfigService _tallerConfigService;
        [ObservableProperty]
        public TallerConfig configuracion;

        public IRelayCommand GuardarCommand { get; }
        public IRelayCommand CancelarCommand { get; }

        public TallerConfigViewModel(TallerConfigService service)
        {
            _tallerConfigService = service;
            //Configuracion = _service.ObtenerConfiguracion() ?? new TallerConfig();

            GuardarCommand = new RelayCommand(Guardar);
            CancelarCommand = new RelayCommand(Cancelar);

            CargarConfiguracion();
        }

        private void CargarConfiguracion()
        {
            var configuracionExistente = _tallerConfigService.ObtenerConfiguracion();
            if (configuracionExistente != null)
            {
                Configuracion = configuracionExistente;
            }
            else
            {
                // Si no existe, crea una nueva configuración vacía
                Configuracion = new TallerConfig();
            }
        }

        private void Guardar()
        {
            _tallerConfigService.ActualizarConfiguracion(Configuracion);
            MessageBox.Show("Configuración guardada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this)?.Close();
        }

        private void Cancelar()
        {
            Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this)?.Close();
        }
    }
}
