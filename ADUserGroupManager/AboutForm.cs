using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADUserGroupManager
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            this.Text = "Acerca de";
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblInfo = new Label
            {
                Text = "Autor: Ezequiel Banega IA\nSoftware: Active Directory User Management\nDescripción: Es un software de automatización para la creación de usuarios, OUs, grupos y organización en general para uso interno exclusivo de PraxisEMR.",
                AutoSize = true,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            this.Controls.Add(lblInfo);
        }
    }
}