using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IBMiCmd.Forms
{
    public partial class installRemote : Form
    {
        public installRemote()
        {
            InitializeComponent();
        }

        private void installRemote_Load(object sender, EventArgs e)
        {
            textBox1.Text = "QGPL";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string installLibrary = "";
            if (IBMiUtilities.isValidQSYSObjectName(textBox1.Text.Trim()))
            {
                installLibrary += textBox1.Text.Trim();
            }
            else
            {
                label1.Text += " - Invalid Library Specified";
                label1.Update();
                return;
            }

            IBMiNPPInstaller.installRemoteLib(installLibrary);

            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
