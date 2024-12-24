using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MechanicWorkshopApp.Services
{
    public class PrinterService
    {
        public void AbrirPDFEnVisor(string rutaArchivo)
        {
            try
            {
                if (File.Exists(rutaArchivo))
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = rutaArchivo,
                            UseShellExecute = true
                        }
                    };
                    process.Start();
                }
                else
                {
                    MessageBox.Show("El archivo no se encontró.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al intentar abrir el archivo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
