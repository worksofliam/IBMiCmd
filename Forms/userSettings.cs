using System;
using System.Windows.Forms;
using IBMiCmd.IBMiTools;

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
            textBox1.Text = IBMi.GetConfig("system");
            textBox2.Text = IBMi.GetConfig("username");
            textBox3.Text = IBMi.GetConfig("password");
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

            IBMi.SetConfig("system", textBox1.Text);
            IBMi.SetConfig("username", textBox2.Text);
            IBMi.SetConfig("password", IBMi.Base64Encode(textBox3.Text));

            this.Close();
        }
    }
}
