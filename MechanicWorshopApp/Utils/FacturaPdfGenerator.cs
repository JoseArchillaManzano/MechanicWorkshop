﻿using MechanicWorkshopApp.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace MechanicWorkshopApp.Utils
{
    public class FacturaPdfGenerator
    {
        private readonly OrdenReparacion _ordenReparacion;
        private readonly TallerConfig _tallerConfig;

        public FacturaPdfGenerator(OrdenReparacion ordenReparacion, TallerConfig tallerConfig)
        {
            _ordenReparacion = ordenReparacion;
            _tallerConfig = tallerConfig;
        }

        public void GenerarOrdenReparacion(string filePath)
        {
            var directorio = @"C:\OrdenesReparacion";
            if (!Directory.Exists(directorio))
            {
                Directory.CreateDirectory(directorio);
            }

            var rutaFactura = Path.Combine(directorio, filePath);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Element(e => ComposeHeaderOrdenReparacion(e, _ordenReparacion.Id));
                    page.Content().Element(e => ComposeTrabajoContenido(e));
                    page.Footer().Element(ComposeFooter);
                });
            }).GeneratePdf(rutaFactura);
        }

        public void GenerarFactura(string filePath, bool isBudget)
        {
            var directorio = isBudget ? @"C:\Presupuestos" : @"C:\Facturas";
            if (!Directory.Exists(directorio))
            {
                Directory.CreateDirectory(directorio);
            }

            var rutaFactura = Path.Combine(directorio, filePath);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Element(e => ComposeHeader(e, _ordenReparacion.Id, isBudget));
                    page.Content().Element(e => ComposeMateriales(e));
                    if (!isBudget) 
                    {
                        page.Footer().Element(ComposeFooter);
                    }
                });
            }).GeneratePdf(rutaFactura);
        }
        private void ComposeMateriales(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().PaddingTop(15).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2); 
                        columns.RelativeColumn(1); 
                        columns.RelativeColumn(1); 
                        columns.RelativeColumn(1); 
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Concepto").Style(TextStyle.Default.Bold());
                        header.Cell().Text("Cantidad").Style(TextStyle.Default.Bold());
                        header.Cell().Text("Precio U.").Style(TextStyle.Default.Bold());
                        header.Cell().Text("Subtotal").Style(TextStyle.Default.Bold());

                        header.Cell().ColumnSpan(4).PaddingTop(5).PaddingBottom(5)
                            .BorderBottom(1).BorderColor(Colors.Grey.Lighten1);
                    });

                    // Filas de la tabla
                    foreach (var linea in _ordenReparacion.LineasOrden)
                    {
                        table.Cell().Text(linea.Concepto);
                        table.Cell().Text(Math.Round(linea.Cantidad,2).ToString());
                        table.Cell().Text($"{linea.PrecioUnitario:C}");
                        table.Cell().Text($"{(linea.Cantidad * linea.PrecioUnitario):C}");
                    }
                });

                column.Spacing(15);


                column.Item().PaddingTop(15).Element(e => ComposeTotales(e));
            });
        }

        private void ComposeHeader(IContainer container, int ordenId, bool isBudget)
        {

            container.Column(column =>
            {
                column.Item().Text(_tallerConfig.Nombre).Style(TextStyle.Default.FontSize(20).Bold());
                column.Item().PaddingBottom(3).Text($"CIF/NIF: {_tallerConfig.CIF}").Style(TextStyle.Default.FontSize(10));
                column.Item().PaddingBottom(3).Text($"Dirección:{_tallerConfig.Direccion}").Style(TextStyle.Default.FontSize(10));
                column.Item().PaddingBottom(3).Text($"Teléfono: {_tallerConfig.Telefono}").Style(TextStyle.Default.FontSize(10));
                column.Item().PaddingBottom(3).Text($"Registro Industrial: {_tallerConfig.RegistroIndustrial}").Style(TextStyle.Default.FontSize(10));

                column.Spacing(5);

                column.Item().Text($"Emitida el: {DateTime.Now:dd/MM/yyyy}")
                    .Style(TextStyle.Default.FontSize(12))
                    .AlignCenter();

                column.Item().PaddingTop(7).PaddingBottom(7).Element(e =>
                {
                    string name = isBudget ? "Presupuesto" : "Factura";
                    e.Text($"{name} Nº {_ordenReparacion.Id}")
                     .Style(TextStyle.Default.FontSize(16).Bold())
                     .Underline();
                });

                column.Item().Element(ComposeInformacionClienteVehiculo);

                column.Item().Text("LISTADO MATERIALES Y MANO DE OBRA").Style(TextStyle.Default.FontSize(14).Bold()).Underline();

                column.Spacing(15);
            });
        }

        private void ComposeHeaderOrdenReparacion(IContainer container, int ordenId)
        {

            container.Column(column =>
            {
                column.Item().Text(_tallerConfig.Nombre).Style(TextStyle.Default.FontSize(20).Bold());
                column.Item().PaddingBottom(3).Text($"CIF/NIF: {_tallerConfig.CIF}").Style(TextStyle.Default.FontSize(10));
                column.Item().PaddingBottom(3).Text($"Dirección:{_tallerConfig.Direccion}").Style(TextStyle.Default.FontSize(10));
                column.Item().PaddingBottom(3).Text($"Teléfono: {_tallerConfig.Telefono}").Style(TextStyle.Default.FontSize(10));
                column.Item().PaddingBottom(3).Text($"Registro Industrial: {_tallerConfig.RegistroIndustrial}").Style(TextStyle.Default.FontSize(10));

                column.Spacing(5);

                column.Item().Text($"Emitida el: {DateTime.Now:dd/MM/yyyy}")
                    .Style(TextStyle.Default.FontSize(12))
                    .AlignCenter();

                column.Item().PaddingTop(7).PaddingBottom(7).Element(e =>
                {
                    e.Text($"Orden de Reparación Nº {_ordenReparacion.Id}")
                     .Style(TextStyle.Default.FontSize(16).Bold())
                     .Underline();
                });

                column.Item().Element(ComposeInformacionClienteVehiculo);

                column.Spacing(10);

                column.Item().Element(e =>
                {
                    e.PaddingBottom(1) // Aplica el PaddingBottom al contenedor
                     .Text("Trabajo a realizar según indicación del cliente:")
                     .Style(TextStyle.Default.FontSize(14).Bold())
                     .Underline();
                });
            });
        }

        private void ComposeTrabajoContenido(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().PaddingTop(15).Text(string.IsNullOrEmpty(_ordenReparacion.Descripcion)
                    ? "No hay observaciones registradas."
                    : _ordenReparacion.Descripcion).Style(TextStyle.Default.FontSize(12));
            });
        }


        private void ComposeFooter(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().PaddingTop(20).Text("Conforme Taller: ____________________________")
                    .Style(TextStyle.Default.FontSize(10)).AlignLeft();

                column.Item().PaddingTop(20).Text("Conforme Cliente: ___________________________")
                    .Style(TextStyle.Default.FontSize(10)).AlignLeft();

                
                column.Item().PaddingTop(20).Text(" ").Style(TextStyle.Default.FontSize(10));
            });
        }

        private void ComposeInformacionClienteVehiculo(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Border(1).Padding(5).Column(clienteColumn =>
                {
                    clienteColumn.Item().Text("Información del Cliente").Style(TextStyle.Default.Bold());
                    clienteColumn.Item().Text($"Nombre: {_ordenReparacion.Cliente.Nombre}");
                    clienteColumn.Item().Text($"Teléfono: {_ordenReparacion.Cliente.Telefono}");
                    clienteColumn.Item().Text($"Dirección: {_ordenReparacion.Cliente.Direccion}");
                    clienteColumn.Item().Text($"Código Postal: {_ordenReparacion.Cliente.CodigoPostal}");
                    clienteColumn.Item().Text($"Municipio: {_ordenReparacion.Cliente.Municipio}");
                    clienteColumn.Item().Text($"Provincia: {_ordenReparacion.Cliente.Provincia}");
                });

                row.Spacing(10);

                row.RelativeItem().Border(1).Padding(5).Column(vehiculoColumn =>
                {
                    vehiculoColumn.Item().Text("Información del Vehículo").Style(TextStyle.Default.Bold());
                    vehiculoColumn.Item().Text($"Matrícula: {_ordenReparacion.Vehiculo.Matricula}");
                    vehiculoColumn.Item().Text($"Marca: {_ordenReparacion.Vehiculo.Marca}");
                    vehiculoColumn.Item().Text($"Modelo: {_ordenReparacion.Vehiculo.Modelo}");
                    vehiculoColumn.Item().Text($"Bastidor: {_ordenReparacion.Vehiculo.Bastidor}");
                });
            });
        }
        private void ComposeTotales(IContainer container)
        {
            var materialesTotal = _ordenReparacion.LineasOrden
                .Where(l => l.TipoLinea == TipoLinea.Material)
                .Sum(l => l.Cantidad * l.PrecioUnitario);

            var manoDeObraTotal = _ordenReparacion.LineasOrden
                .Where(l => l.TipoLinea == TipoLinea.ManoDeObra)
                .Sum(l => l.Cantidad * l.PrecioUnitario);

            var baseImponible = materialesTotal + manoDeObraTotal;
            var iva = baseImponible * ((double) _tallerConfig.IVA /100);
            var totalGeneral = baseImponible + iva;

            container.Column(column =>
            {
                column.Item().EnsureSpace().Column(totalesColumn =>
                {
                    totalesColumn.Item().Text("Totales").Style(TextStyle.Default.Size(16).Bold());
                    totalesColumn.Item().PaddingTop(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten1);

                    totalesColumn.Item().Row(row =>
                    {
                        row.RelativeItem().Border(1).Padding(5).Column(col =>
                        {
                            col.Item().Text("Materiales").Style(TextStyle.Default.Bold());
                            col.Item().Text($"{materialesTotal:C}");
                        });

                        row.RelativeItem().Border(1).Padding(5).Column(col =>
                        {
                            col.Item().Text("Mano de Obra").Style(TextStyle.Default.Bold());
                            col.Item().Text($"{manoDeObraTotal:C}");
                        });

                        row.RelativeItem().Border(1).Padding(5).Column(col =>
                        {
                            col.Item().Text("Base Imponible").Style(TextStyle.Default.Bold());
                            col.Item().Text($"{baseImponible:C}");
                        });

                        row.RelativeItem().Border(1).Padding(5).Column(col =>
                        {
                            col.Item().Text($"IVA ({_tallerConfig.IVA}%)").Style(TextStyle.Default.Bold());
                            col.Item().Text($"{iva:C}");
                        });

                        row.RelativeItem().Border(1).Padding(5).Column(col =>
                        {
                            col.Item().Text("Total General").Style(TextStyle.Default.Bold());
                            col.Item().Text($"{totalGeneral:C}");
                        });
                    });
                });
            });
        }
    }
}
