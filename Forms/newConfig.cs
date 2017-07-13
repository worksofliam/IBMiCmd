using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IBMiCmd.IBMiTools;

namespace IBMiCmd.Forms
{
    public partial class newConfig : Form
    {
        public newConfig()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text.Trim();

            if (textBox1.Text != "")
            {
                Config.SwitchToConfig(textBox1.Text);
                this.Close();
            }
            else
            {
                MessageBox.Show("You must enter a host name to continue with configuration creation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
