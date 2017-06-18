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
using IBMiCmd.LanguageTools;
using NppPluginNET;
using System.IO;
using System.Threading;

namespace IBMiCmd.Forms
{
    public partial class openMember : Form
    {
        private Boolean isReadonly;
        public openMember()
        {
            InitializeComponent();
            textBox4.Text = IBMi.GetConfig("system");
            checkBox1.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.isReadonly = checkBox1.Checked;
            textBox1.Text = textBox1.Text.ToUpper();
            textBox2.Text = textBox2.Text.ToUpper();
            textBox3.Text = textBox3.Text.ToUpper();

            if (textBox1.Text == "*CURLIB") textBox1.Text = IBMi.GetConfig("curlib");

            if (!IBMiUtilities.IsValidQSYSObjectName(textBox1.Text))
            {
                MessageBox.Show("Library name is not valid.");
                return;
            }
            if (!IBMiUtilities.IsValidQSYSObjectName(textBox2.Text))
            {
                MessageBox.Show("Object name is not valid.");
                return;
            }
            if (!IBMiUtilities.IsValidQSYSObjectName(textBox3.Text))
            {
                MessageBox.Show("Member name is not valid.");
                return;
            }
            
            string resultFile = IBMiUtilities.DownloadMember(textBox1.Text, textBox2.Text, textBox3.Text);
            if (Main.CommandWindow != null) Main.CommandWindow.loadNewCommands();

            if (resultFile != "")
            {
                //Open File
                NppFunctions.OpenFile(resultFile, this.isReadonly);
                if (!this.isReadonly)
                    OpenMembers.AddMember(textBox4.Text, resultFile, textBox1.Text, textBox2.Text, textBox3.Text);

                this.Close();
            }
            else
            {
                MessageBox.Show("Unable to download member " + textBox1.Text + "/" + textBox1.Text + "." + textBox3.Text + ". Please check it exists and that you have access to the remote system.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
