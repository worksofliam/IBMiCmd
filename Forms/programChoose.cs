using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace IBMiCmd.Forms
{
    public partial class programChoose : Form
    {
        public programChoose()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ErrorHandle.getErrors(textBox1.Text, textBox2.Text);

            this.Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab) textBox2.Focus();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab) button1.Focus();
        }
    }
}
