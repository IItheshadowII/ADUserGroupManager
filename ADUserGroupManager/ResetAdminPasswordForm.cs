using System;
using System.Windows.Forms;

namespace ADUserGroupManager
{
    public partial class ResetAdminPasswordForm : Form
    {
        private ActiveDirectoryManager adManager;

        public ResetAdminPasswordForm()
        {
            InitializeComponent();
            adManager = new ActiveDirectoryManager(null);
        }

        // Este método se ejecutará cuando se haga clic en el botón de reset
        private void btnReset_Click(object sender, EventArgs e)
        {
            string serverName = txtResetServerName.Text.Trim();

            if (string.IsNullOrEmpty(serverName))
            {
                MessageBox.Show("Por favor, ingrese el nombre del servidor.",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"¿Está seguro que desea restablecer la contraseña del administrador local en {serverName}?",
                "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.No)
                return;

            try
            {
                // Generar una nueva contraseña
                string newPassword = adManager.GeneratePassword();

                // Resetear la contraseña
                adManager.ResetLocalAdminPassword(serverName, newPassword);

                // Mostrar la nueva contraseña en una etiqueta
                lblResult.Text = $"La contraseña del administrador local en {serverName} ha sido restablecida.\n\n" +
                                $"Usuario: Administrator\nNueva contraseña: {newPassword}\n\n" +
                                "Por favor, guarde esta contraseña en un lugar seguro.";

                // Opcionalmente, copiar al portapapeles
                Clipboard.SetText(newPassword);

                MessageBox.Show($"Contraseña restablecida con éxito. La nueva contraseña ha sido copiada al portapapeles.",
                              "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al restablecer la contraseña: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}