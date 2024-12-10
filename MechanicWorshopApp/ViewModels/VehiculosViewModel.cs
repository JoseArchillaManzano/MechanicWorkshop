﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MechanicWorkshopApp.Configuration;
using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.Services;
using MechanicWorkshopApp.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MechanicWorkshopApp.ViewModels
{
    public partial class VehiculosViewModel : ObservableObject
    {
        private readonly VehiculoService _vehiculoService;
        private readonly Func<VehiculoForm> _vehiculoFormFactory;
        private  int _clienteId;

        [ObservableProperty]
        private int currentPage = 1;

        [ObservableProperty]
        private int totalPages;

        [ObservableProperty]
        private ObservableCollection<Vehiculo> vehiculos;

        [ObservableProperty]
        private string searchQuery = string.Empty;

        [ObservableProperty]
        private int pageSize = AppSettings.PageSize;

        [ObservableProperty]
        private Vehiculo selectedVehiculo;

        public RelayCommand NextPageCommand { get; }
        public RelayCommand PreviousPageCommand { get; }

        public ICommand AgregarVehiculoCommand { get; }
        public ICommand EditarVehiculoCommand { get; }
        public ICommand EliminarVehiculoCommand { get; }

        public VehiculosViewModel(VehiculoService vehiculoService, Func<VehiculoForm> vehiculoFormFactory)
        {
            _vehiculoService = vehiculoService;
            _vehiculoFormFactory = vehiculoFormFactory;
            // Configurar comandos
            NextPageCommand = new RelayCommand(ExecuteNextPage, CanExecuteNextPage);
            PreviousPageCommand = new RelayCommand(ExecutePreviousPage, CanExecutePreviousPage);

            // Inicializar comandos
            AgregarVehiculoCommand = new RelayCommand(ExecuteAgregarVehiculo);
            EditarVehiculoCommand = new RelayCommand(ExecuteEditarVehiculo);
            EliminarVehiculoCommand = new RelayCommand(ExecuteEliminarVehiculo);

            // Cargar datos iniciales
            UpdateVehiculos();
        }
        public void Initialize(int clienteId)
        {
            _clienteId = clienteId;
            UpdateVehiculos();
        }

        private void ExecuteNextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                UpdateVehiculos();
            }
        }

        private bool CanExecuteNextPage() => CurrentPage < TotalPages;

        private void ExecutePreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                UpdateVehiculos();
            }
        }

        private bool CanExecutePreviousPage() => CurrentPage > 1;

        public void UpdateVehiculos()
        {
            var result = _vehiculoService.ObtenerVehiculosPaginadosPorCliente(
                _clienteId, CurrentPage, PageSize, SearchQuery);

            Vehiculos = new ObservableCollection<Vehiculo>(result.Items);
            TotalPages = result.TotalPages;

            // Actualizar estados de los comandos
            NextPageCommand.NotifyCanExecuteChanged();
            PreviousPageCommand.NotifyCanExecuteChanged();
        }

        private void ExecuteAgregarVehiculo()
        {
            var vehiculoForm = _vehiculoFormFactory();
            var viewModel = new VehiculoFormViewModel(
                new Vehiculo { ClienteId = _clienteId },
                _vehiculoService,
                result =>
                {
                    if (result) UpdateVehiculos();
                }
            );

            vehiculoForm.Initialize(viewModel);

            vehiculoForm.ShowDialog();
        }

        private void ExecuteEditarVehiculo()
        {
            if (SelectedVehiculo != null)
            {
                var vehiculoForm = _vehiculoFormFactory();
                var viewModel = new VehiculoFormViewModel(
                    SelectedVehiculo,
                    _vehiculoService,
                    result =>
                    {
                        if (result) UpdateVehiculos();
                    }
                );

                vehiculoForm.Initialize(viewModel);

                vehiculoForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Debe seleccionar un vehículo para editar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void ExecuteEliminarVehiculo()
        {
            if (SelectedVehiculo != null)
            {
                var result = System.Windows.MessageBox.Show("¿Estás seguro de que deseas eliminar este vehículo?",
                    "Confirmar Eliminación", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);

                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    _vehiculoService.EliminarVehiculo(SelectedVehiculo.Id);
                    UpdateVehiculos();
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar un vehículo para eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        partial void OnSelectedVehiculoChanged(Vehiculo value)
        {
            ((RelayCommand)EditarVehiculoCommand).NotifyCanExecuteChanged();
            ((RelayCommand)EliminarVehiculoCommand).NotifyCanExecuteChanged();
        }
    }
}