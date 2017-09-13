using System;
using System.Diagnostics;
using System.Windows.Forms;
using HwidSystem;
namespace bot_supreme
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            h = new Hwid("https://www.bot-supreme.com/APi_/api-panel/webco/", "V1");
        }
        Hwid h = null;
        private void button1_Click(object sender, EventArgs e)
        {
           
            int ret = h.CheckLogin(txtUsername.Text, txtPassword.Text);
            if (ret == 0)
            {
                h.ChechforUpdate();
                MessageBox.Show(h.GetMOTD(), @"Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                Form1 mfrm = new Form1();
                mfrm.Show();
            }
            else if (ret == 1) { MessageBox.Show(@"You account has been banned!", @"Banned", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            else if (ret == 2) { MessageBox.Show(@"Invalid Username or Password", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            else if (ret == 3) { MessageBox.Show(@"An Error has occurred, Please try again later", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            new RegisterForm(h).ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.bot-supreme.com");
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}
