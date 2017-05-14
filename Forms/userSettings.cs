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
    public partial class userSettings : Form
    {
        public userSettings()
        {
            InitializeComponent();
        }

        private void userSettings_Load(object sender, EventArgs e)
        {
            textBox1.Text = IBMi.getConfig("system");
            textBox2.Text = IBMi.getConfig("username");
            textBox3.Text = IBMi.getConfig("password");
            textBox4.Text = IBMi.getConfig("relicdir");
            textBox5.Text = IBMi.getConfig("reliclib");
		}

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "")
            {
                textBox1.Focus();
                return;
            }
            if (textBox2.Text.Trim() == "")
            {
                textBox2.Focus();
                return;
            }
            if (textBox3.Text.Trim() == "")
            {
                textBox3.Focus();
                return;
            }

            IBMi.setConfig("system", textBox1.Text);
            IBMi.setConfig("username", textBox2.Text);
            IBMi.setConfig("password", textBox3.Text);
            IBMi.setConfig("relicdir", textBox4.Text);
            IBMi.setConfig("reliclib", textBox5.Text);

			this.Close();
        }
	}
}
