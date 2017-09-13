using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace bot_supreme
{
    public partial class ProductForm : Form
    {
        public ProductForm()
        {
            InitializeComponent();
            cbCategory.SelectedIndex = 0;
            cbSize.SelectedIndex = 0;
            cbSize2.SelectedIndex = 0;
        }

        private void ProductForm_Load(object sender, EventArgs e)
        {

        }
        public  string PSize { get; private set; }
        public string PCategory { get; private set; }
        public string PColor { get; private set; }
        public string PName { get; private set; }
        public string P2SnSize { get; private set; }
        private void button1_Click(object sender, EventArgs e)
        {
            PName = txtName.Text;
            PCategory = cbCategory.SelectedItem.ToString();
            PSize = cbSize.SelectedItem.ToString();
            PColor = txtColor.Text;
            P2SnSize = cbSize2.SelectedItem.ToString();
            this.DialogResult = DialogResult.OK;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void cbSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbSize2.Enabled = cbSize.SelectedIndex != 0;
            cbSize2.SelectedIndex = cbSize.SelectedIndex;
        }

        private void button2_Click(object sender, EventArgs e)
        {
          
        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCategory.Text == "shoes")
            {
                cbSize.Items.Clear();
                cbSize2.Items.Clear();
                cbSize.Items.AddRange(new object[] { "-", "US 6 / UK 5", "US 7 / UK 6", "US 8 / UK 7", "US 9 / UK 8", "US 10 / UK 9", "US 11 / UK 10", "US 12 / UK 11", "US 13 / UK 12"});
                cbSize2.Items.AddRange(new object[] { "-", "US 6 / UK 5", "US 7 / UK 6", "US 8 / UK 7", "US 9 / UK 8", "US 10 / UK 9", "US 11 / UK 10", "US 12 / UK 11", "US 13 / UK 12" });
                cbSize.SelectedIndex = 0;
                cbSize2.SelectedIndex = 0;
            }
            else if (cbCategory.Text == "pants")
            {
                cbSize.Items.Clear();
                cbSize2.Items.Clear();
                cbSize.Items.AddRange(new object[] { "-", "30", "32", "34", "36" });
                cbSize2.Items.AddRange(new object[] { "-", "30", "32", "34", "36" });
                cbSize.SelectedIndex = 0;
                cbSize2.SelectedIndex = 0;
            }
            else if (cbCategory.Text == "hats")
            {
                cbSize.Items.Clear();
                cbSize2.Items.Clear();
                cbSize.Items.AddRange(new object[] { "-", "7 1/8", "7 1/4", "7 3/8", "7 1/2", "7 5/8", "7 3/4", "S/M", "M/L", "L/XL" });
                cbSize2.Items.AddRange(new object[] { "-", "7 1/8", "7 1/4", "7 3/8", "7 1/2", "7 5/8", "7 3/4" ,"S/M", "M/L", "L/XL" });
                cbSize.SelectedIndex = 0;
                cbSize2.SelectedIndex = 0;
            }
            else
            {
            
                    cbSize.Items.Clear();
                    cbSize2.Items.Clear();
                    cbSize.Items.AddRange(new object[] { "-","Small", "Medium", "Large","XLarge" });
                    cbSize2.Items.AddRange(new object[] { "-", "Small", "Medium", "Large", "XLarge" });
                cbSize.SelectedIndex = 0;
                cbSize2.SelectedIndex = 0;

            }
        }

        private void cbSize2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
