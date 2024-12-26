﻿using MechanicWorkshopApp.Data;
using MechanicWorkshopApp.Models;
using MechanicWorkshopApp.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicWorkshopApp.Services
{
    public class OrdenReparacionService
    {
        private readonly TallerContext _context;

        public OrdenReparacionService(TallerContext context)
        {
            _context = context;
        }

        public PagedResult<OrdenReparacion> ObtenerOrdenesPaginadas(int page, int pageSize, string? searchQuery = null)
        {
            var query = _context.OrdenesReparacion
                                .Include(o => o.Cliente)
                                .Include(o => o.Vehiculo)
                                .Include(o => o.LineasOrden)
                                .AsQueryable();

            // Aplicar búsqueda si se proporciona un query
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(o =>
                    o.Cliente.Nombre.Contains(searchQuery) ||
                    o.Vehiculo.Matricula.Contains(searchQuery));
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var items = query
                .OrderByDescending(o => o.FechaEntrada)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<OrdenReparacion>(items, totalItems, totalPages, pageSize);
        }

        public OrdenReparacion ObtenerOrdenPorId(int id)
        {
            return _context.OrdenesReparacion
                           .Include(o => o.Cliente)
                           .Include(o => o.Vehiculo)
                           .Include(o => o.LineasOrden)
                           .FirstOrDefault(o => o.Id == id);
        }

        public void CrearOrden(OrdenReparacion orden)
        {
            if (orden.Cliente != null && _context.Entry(orden.Cliente).State != EntityState.Detached)
            {
                _context.Entry(orden.Cliente).State = EntityState.Detached;
            }

            // Si el vehículo está rastreado, desvincularlo
            if (orden.Vehiculo != null && _context.Entry(orden.Vehiculo).State != EntityState.Detached)
            {
                _context.Entry(orden.Vehiculo).State = EntityState.Detached;
            }

            // Adjuntar cliente y vehículo al contexto actual
            if (orden.Cliente != null)
            {
                _context.Attach(orden.Cliente);
            }

            if (orden.Vehiculo != null)
            {
                _context.Attach(orden.Vehiculo);
            }

            AdjustDatesToUtc(orden);

            _context.OrdenesReparacion.Add(orden);
            _context.SaveChanges();
        }

        public void ActualizarOrden(OrdenReparacion orden)
        {
            AdjustDatesToUtc(orden);
            _context.OrdenesReparacion.Update(orden);
            _context.SaveChanges();
        }

        public void EliminarOrden(int id)
        {
            var orden = _context.OrdenesReparacion.Find(id);
            if (orden != null)
            {
                _context.OrdenesReparacion.Remove(orden);
                _context.SaveChanges();
            }
        }

        public void ActualizarFechaSalida(int ordenId, DateTime fechaSalida)
        {
            var orden = _context.OrdenesReparacion.Find(ordenId);
            if (orden != null)
            {
                orden.FechaSalida = fechaSalida;
                _context.SaveChanges();
            }
        }

        private void AdjustDatesToUtc(OrdenReparacion orden)
        {
            if (orden.FechaEntrada.Kind == DateTimeKind.Local || orden.FechaEntrada.Kind == DateTimeKind.Unspecified)
            {
                orden.FechaEntrada = DateTime.SpecifyKind(orden.FechaEntrada, DateTimeKind.Local).ToUniversalTime();
            }

            // Normalizar FechaSalida
            if (orden.FechaSalida.HasValue)
            {
                var fechaSalida = orden.FechaSalida.Value.AddHours(1);
                if (fechaSalida.Kind == DateTimeKind.Local || fechaSalida.Kind == DateTimeKind.Unspecified)
                {
                    orden.FechaSalida = DateTime.SpecifyKind(fechaSalida, DateTimeKind.Unspecified).ToUniversalTime();
                }
            }
        }

        public int ObtenerOrdenesActivas()
        {
            return _context.OrdenesReparacion.Count(or => or.FechaSalida == null);
        }

        public int ObtenerOrdenesCerradas()
        {
            return _context.OrdenesReparacion.Count(or => or.FechaSalida != null);
        }

        public Dictionary<string, int> ObtenerOrdenesPorMes(int año)
        {
            var datos = _context.OrdenesReparacion
                .Where(o => o.FechaEntrada != null && o.FechaEntrada.Year == año) // Filtrar por año y asegurar que la fecha no sea nula
                .GroupBy(o => o.FechaEntrada.Month) // Agrupar solo por el mes
                .Select(g => new
                {
                    Mes = g.Key, // Mes como número
                    Cantidad = g.Count() // Contar las órdenes en ese mes
                })
                .OrderBy(d => d.Mes) // Ordenar por el número del mes
                .ToList();

            // Convertir a diccionario con el nombre del mes como clave
            return datos.ToDictionary(
                d => ObtenerNombreMes(d.Mes), // Nombre del mes
                d => d.Cantidad); // Cantidad de órdenes
        }

        public Dictionary<string, double> ObtenerIngresosPorMes(int año)
        {
            var datos = _context.OrdenesReparacion
                .Where(o => o.FechaSalida != null && o.FechaSalida.Value.Year == año) // Filtrar por año
                .GroupBy(o => o.FechaSalida.Value.Month) // Agrupar por mes
                .Select(g => new
                {
                    Mes = g.Key, // Mes como clave
                    TotalIngresos = g.Sum(o => o.LineasOrden.Sum(l => (double)(l.Cantidad * l.PrecioUnitario)))
                })
                .OrderBy(d => d.Mes) // Ordenar por mes
                .ToList();

            // Convertir a diccionario con el formato "Mes"
            return datos.ToDictionary(
                d => ObtenerNombreMes(d.Mes), // Convertir número de mes a nombre
                d => d.TotalIngresos);
        }

        private string ObtenerNombreMes(int mes)
        {
            return mes switch
            {
                1 => "Enero",
                2 => "Febrero",
                3 => "Marzo",
                4 => "Abril",
                5 => "Mayo",
                6 => "Junio",
                7 => "Julio",
                8 => "Agosto",
                9 => "Septiembre",
                10 => "Octubre",
                11 => "Noviembre",
                12 => "Diciembre",
                _ => "Desconocido"
            };
        }

        public IEnumerable<string> ObtenerAñosDisponibles()
        {
            // Lógica para obtener los años disponibles
            return _context.OrdenesReparacion.Select(o => o.FechaEntrada.Year.ToString()).Distinct().ToList();
        }

        public Dictionary<string, double> ObtenerIngresosPorManoDeObra(int año)
        {
            string tipoManoDeObra = TipoLinea.ManoDeObra.ToString();

            var lineas = _context.LineasOrden
                .Where(l => l.TipoLinea.ToString() == tipoManoDeObra
                            && l.OrdenReparacion.FechaSalida != null
                            && l.OrdenReparacion.FechaSalida.Value.Year == año) // Filtrar por año
                .Select(l => new
                {
                    l.PrecioUnitario,
                    l.Cantidad,
                    FechaSalida = l.OrdenReparacion.FechaSalida
                })
                .ToList();

            // Agrupar por mes
            return lineas
                .GroupBy(l => l.FechaSalida.Value.Month) // Agrupar por número de mes
                .Select(g => new
                {
                    Mes = g.Key, // Mes como clave
                    TotalIngresos = g.Sum(l => (double)(l.PrecioUnitario * l.Cantidad))
                })
                .OrderBy(d => d.Mes) // Ordenar por mes
                .ToDictionary(
                    d => ObtenerNombreMes(d.Mes), // Convertir número de mes a nombre
                    d => d.TotalIngresos);
        }
    }
}