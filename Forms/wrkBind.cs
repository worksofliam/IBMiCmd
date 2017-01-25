using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IBMiCmd.Forms
{
    public partial class wrkBind : Form
    {
        public wrkBind()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "")
            {
                MessageBox.Show("Binding name must not be blank.");
            }
            else if (textBox1.Text.Trim().Contains(" "))
            {
                MessageBox.Show("Binding name must not contain any spaces.");
            }
            else
            {
                cmdBindings.editingBind = textBox1.Text.Trim();
                cmdBindings.editingBindCmds = richTextBox1.Lines;
                cmdBindings.editingCanceled = false;
                this.Close();
            }
        }

        private void wrkBind_Load(object sender, EventArgs e)
        {
            textBox1.Text = cmdBindings.editingBind;
            if (cmdBindings.editingBindCmds != null) richTextBox1.Lines = cmdBindings.editingBindCmds;
        }
    }
}
