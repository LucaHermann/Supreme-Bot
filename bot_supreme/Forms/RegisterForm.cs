using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bot_supreme
{
    public partial class RegisterForm : Form
    {
        private HwidSystem.Hwid hwid = null;
        public RegisterForm(HwidSystem.Hwid h)
        {
            hwid = h;
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtPassword2.Text == null || txtSerial.Text == null || txtPassword.Text == null || txtUsername.Text == null)
            {
                MessageBox.Show("Textfield must'n null", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtPassword.Text != txtPassword2.Text)
            {
                MessageBox.Show("Password do not match", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            hwid.RegisterUser(txtUsername.Text, txtPassword.Text, txtSerial.Text);
        }
    }
}
