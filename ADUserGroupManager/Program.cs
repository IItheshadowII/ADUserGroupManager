using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADUserGroupManager
{
    static class Program
    {
        private const string UpdateArg = "/update";

        [STAThread]
        static void Main(string[] args)
        {
            // Si la app se inició con el argumento "/update"
            if (args.Length > 0 && args[0] == UpdateArg)
            {
                if (args.Length > 1)
                {
                    RealizarProcesoDeActualizacion(args[1]);
                }
                else
                {
                    MessageBox.Show("No se recibió la ruta del nuevo ejecutable para actualizar.",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    Environment.Exit(1);
                }
            }
            else
            {
                // Inicio normal de la aplicación
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }

        /// <summary>
        /// Realiza el reemplazo del .exe actual por la nueva versión
        /// y relanza la aplicación actualizada.
        /// </summary>
        /// <param name="newExePath">Ruta del nuevo ejecutable descargado</param>
        private static void RealizarProcesoDeActualizacion(string newExePath)
        {
            try
            {
                // Dar un tiempo para asegurarnos de que la primera instancia se haya cerrado
                Thread.Sleep(2000);

                // Ruta del ejecutable actual (esta segunda instancia se ejecuta en otro proceso)
                string currentExePath = Application.ExecutablePath;

                // Copiar el nuevo .exe sobre el actual (sobrescritura habilitada con true)
                File.Copy(newExePath, currentExePath, true);

                // Borrar el .exe temporal
                File.Delete(newExePath);

                // Iniciar la nueva versión
                Process.Start(currentExePath);

                // Terminar este proceso
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error durante la actualización: " + ex.Message,
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }
    }
}
