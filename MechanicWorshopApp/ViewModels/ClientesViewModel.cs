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
        private ObservableCollection<int> pageSizes; // Tamaños de página disponibles


        public ClientesViewModel(ClienteService clienteService)
        {
            _clienteService = clienteService;

            // Inicializar comandos
            NextPageCommand = new RelayCommand(ExecuteNextPage, CanExecuteNextPage);
            PreviousPageCommand = new RelayCommand(ExecutePreviousPage, CanExecutePreviousPage);
            PageSizes = new ObservableCollection<int>(AppSettings.AvailablePageSizes);
            // Inicializar datos
            currentPage = 1; // Usa el campo generado directamente
            UpdateClientes();
        }

        public RelayCommand NextPageCommand { get; }
        public RelayCommand PreviousPageCommand { get; }

        private void ExecuteNextPage()
        {
            if (currentPage < totalPages) // Usa los campos internos
            {
                currentPage++;
                UpdateClientes();
            }
        }

        private bool CanExecuteNextPage()
        {
            return currentPage < totalPages; // Usa los campos internos
        }

        private void ExecutePreviousPage()
        {
            if (currentPage > 1) // Usa los campos internos
            {
                currentPage--;
                UpdateClientes();
            }
        }

        private bool CanExecutePreviousPage()
        {
            return currentPage > 1; // Usa los campos internos
        }

        public void UpdateClientes()
        {
            // Obtener clientes paginados
            var result = _clienteService.GetClientesPaginated(currentPage, _pageSize); // Usa el campo interno

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
    }
}