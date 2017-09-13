using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace bot_supreme
{
    public partial class SettingsForm : Form
    {

        
        public SettingsForm(UserPreference _userPreference)
        {
            InitializeComponent();
            textBox1.Text = _userPreference.Proxy;
            SleepSec.Value = _userPreference.SleepCheckout;
            chkBypassCaptcha.Checked = _userPreference.AutoBypassCaptcha;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        public string Proxy { get; set; }
        public bool  AutoBypassCaptcha { get; set; }
        public int SleepCheckout { get; set; }
        private void button1_Click(object sender, EventArgs e)
        {
            Proxy = textBox1.Text;
            SleepCheckout = (int)SleepSec.Value;
            AutoBypassCaptcha = chkBypassCaptcha.Checked;
            DialogResult = DialogResult.OK;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
