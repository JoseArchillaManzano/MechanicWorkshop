using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MechanicWorkshopApp.Configuration;
using MechanicWorkshopApp.Data;
using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.Services;
using MechanicWorkshopApp.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;

namespace MechanicWorkshopApp.ViewModels
{
    public partial class ClientesViewModel :  ObservableObject
    {
        private readonly ClienteService _clienteService;

        [ObservableProperty]
        private int currentPage = 1; // Valor inicial

        [ObservableProperty]
        private int totalPages;

        [ObservableProperty]
        private ObservableCollection<Cliente> clientes;

        [ObservableProperty]
        private int _pageSize = AppSettings.PageSize;

        [ObservableProperty]
        private string searchQuery = string.Empty;

        [ObservableProperty]
        private ObservableCollection<int> pageSizes; // Tamaños de página disponibles

        private readonly System.Timers.Timer _debounceTimer;

        public ClientesViewModel(ClienteService clienteService)
        {
            _clienteService = clienteService;

            _debounceTimer = new System.Timers.Timer(500); // 300 ms de retraso
            _debounceTimer.AutoReset = false; // Solo se dispara una vez
            _debounceTimer.Elapsed += (s, e) =>
            {
                // Actualizar clientes en el hilo de la interfaz
                App.Current.Dispatcher.Invoke(UpdateClientes);
            };
            // Inicializar comandos
            NextPageCommand = new RelayCommand(ExecuteNextPage, CanExecuteNextPage);
            PreviousPageCommand = new RelayCommand(ExecutePreviousPage, CanExecutePreviousPage);
            PageSizes = new ObservableCollection<int>(AppSettings.AvailablePageSizes);
            // Inicializar datos
            UpdateClientes();
        }

        public RelayCommand NextPageCommand { get; }
        public RelayCommand PreviousPageCommand { get; }

        private void ExecuteNextPage()
        {
            if (CurrentPage < TotalPages) // Usa los campos internos
            {
                CurrentPage++;
                UpdateClientes();
            }
        }

        private bool CanExecuteNextPage()
        {
            return CurrentPage < TotalPages; // Usa los campos internos
        }

        private void ExecutePreviousPage()
        {
            if (CurrentPage > 1) // Usa los campos internos
            {
                CurrentPage--;
                UpdateClientes();
            }
        }

        private bool CanExecutePreviousPage()
        {
            return CurrentPage > 1; // Usa los campos internos
        }

        partial void OnSearchQueryChanged(string value)
        {
            CurrentPage = 1; // Reinicia a la primera página
                             // Reiniciar el temporizador
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        public void UpdateClientes()
        {
            // Obtener clientes paginados
            var result = _clienteService.GetClientesPaginated(CurrentPage, PageSize, SearchQuery); // Usa el campo interno

            // Actualizar propiedades
            Clientes = new ObservableCollection<Cliente>(result.Items);
            TotalPages = result.TotalPages;

            // Notificar cambios
            OnPropertyChanged(nameof(CurrentPage));
            OnPropertyChanged(nameof(TotalPages));
            // Actualizar estados de los botones
            NextPageCommand.NotifyCanExecuteChanged();
            PreviousPageCommand.NotifyCanExecuteChanged();
        }

        public void ChangePageSize(int newSize)
        {
            PageSize = newSize;
            CurrentPage = 1; // Reiniciar a la primera página
            UpdateClientes();
        }

        private void DebounceTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Ejecutar actualización en el hilo de la interfaz
            App.Current.Dispatcher.Invoke(UpdateClientes);
        }
    }
}