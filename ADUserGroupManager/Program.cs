// Program.cs
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
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }

        private static void RealizarProcesoDeActualizacion(string newExePath)
        {
            try
            {
                Thread.Sleep(2000);
                string currentExePath = Application.ExecutablePath;

                if (File.Exists(currentExePath))
                {
                    File.Delete(currentExePath);
                }

                File.Copy(newExePath, currentExePath);
                File.Delete(newExePath);

                Process.Start(currentExePath);
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
