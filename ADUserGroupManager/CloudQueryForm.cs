using System;
using System.DirectoryServices;
using System.Linq;
using System.Windows.Forms;

namespace ADUserGroupManager
{
    public partial class CloudQueryForm : Form
    {
        private ActiveDirectoryManager adManager;

        public CloudQueryForm()
        {
            InitializeComponent();
            adManager = new ActiveDirectoryManager(UpdateInterface);
        }

        private void UpdateInterface(string message)
        {
            // Implementa la lógica de actualización de la interfaz si es necesario
        }

        private DirectoryEntry GetCloudDirectoryEntry(string cloudName)
        {
            // Utiliza adManager para obtener el base DN y construir el path de la OU
            string ouPath = $"LDAP://OU=PROD_{cloudName},OU=Clinic,{adManager.GetDomainBaseDN()}";
            return new DirectoryEntry(ouPath);
        }

        private int GetNumberOfUsers(string cloudName)
        {
            // Utiliza adManager para obtener el base DN y construir el path de la OU
            string ouPath = $"LDAP://OU=PROD_{cloudName},OU=Clinic,{adManager.GetDomainBaseDN()}";
            using (DirectoryEntry ouEntry = new DirectoryEntry(ouPath))
            {
                return ouEntry.Children.Cast<DirectoryEntry>().Count(entry => entry.SchemaClassName == "user");
            }
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                string cloudName = txtCloudName.Text.Trim();
                DirectoryEntry ouEntry = GetCloudDirectoryEntry(cloudName);

                // Obtener los detalles del creador desde el atributo 'description'
                string createdBy = ouEntry.Properties["description"].Value?.ToString() ?? "No se pudo resolver el propietario";

                string createdDate = ouEntry.Properties["whenCreated"].Value?.ToString() ?? "Desconocido";
                string lastReboot = ouEntry.Properties["whenChanged"].Value?.ToString() ?? "Desconocido";
                int numberOfUsers = GetNumberOfUsers(cloudName);

                txtResult.Text = $@"
Cloud Name: {cloudName}
Creado: {createdDate}
Creado por: {createdBy}
Último reinicio: {lastReboot}
Usuarios creados: {numberOfUsers}
Ubicación OU: {ouEntry.Path}
";
            }
            catch (Exception ex)
            {
                txtResult.Text = $"Error retrieving information for cloud {txtCloudName.Text}: {ex.Message}";
            }
        }
    }
}
