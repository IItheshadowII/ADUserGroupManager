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
                // Ruta del ejecutable actual
                string currentExePath = Application.ExecutablePath;
                string currentProcessId = Process.GetCurrentProcess().Id.ToString();

                // Crear un archivo batch para realizar la actualización después de que el proceso actual termine
                string batchPath = Path.Combine(Path.GetTempPath(), "updateApp.bat");

                // Contenido del archivo batch
                string batchContent = $@"
@echo off
echo Esperando a que el proceso original termine...

:loop
timeout /t 2 >nul
tasklist /fi ""PID eq {currentProcessId}"" | find ""{currentProcessId}"" >nul
if not errorlevel 1 goto loop

echo Proceso terminado. Actualizando la aplicación...
copy ""{newExePath}"" ""{currentExePath}"" /Y
if errorlevel 1 goto error

echo Eliminando archivo temporal...
del ""{newExePath}""

echo Iniciando la nueva versión de la aplicación...
start """" ""{currentExePath}""
goto end

:error
echo Error durante la actualización.
pause

:end
del ""%~f0""
";

                // Escribir el archivo batch
                File.WriteAllText(batchPath, batchContent);

                // Ejecutar el archivo batch
                Process.Start(batchPath);

                // Terminar este proceso para permitir la actualización
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error preparando la actualización: " + ex.Message,
                              "Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }
    }
}
