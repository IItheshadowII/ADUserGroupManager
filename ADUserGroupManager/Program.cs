using System;
using System.Windows.Forms; // Esto es lo que falta


namespace ADUserGroupManager
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1()); // Asegúrate de que esto inicie tu Form1
        }
    }
}
