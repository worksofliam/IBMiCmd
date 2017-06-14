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
using NppPluginNET;
using System.IO;
using System.Threading;

namespace IBMiCmd.Forms
{
    public partial class openMember : Form
    {
        public openMember()
        {
            InitializeComponent();
            textBox4.Text = IBMi.GetConfig("system");
        }

        private void button1_Click(object sender, EventArgs e)
        {
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
            if (textBox3.Text.Trim() == "")
            {
                MessageBox.Show("Member name is not valid.");
                return;
            }
            
            string resultFile = DownloadMember(textBox1.Text, textBox2.Text, textBox3.Text);

            if (resultFile != "")
            {
                //Open File
                OpenFileReadOnly(resultFile);
                this.Close();
            }
            else
            {
                MessageBox.Show("Unable to download member " + textBox3.Text + ". Please check it exists.");
                if (Main.CommandWindow != null) Main.CommandWindow.loadNewCommands();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string DownloadMember(string Lib, string Obj, string Mbr)
        {
            string filetemp = Path.GetTempFileName();
            List<string> commands = new List<string>();

            Lib = Lib.ToUpper();
            Obj = Obj.ToUpper();
            Mbr = Mbr.ToUpper();

            commands.Add("ASCII");
            commands.Add("cd /QSYS.lib");
            commands.Add("recv " + Lib + ".lib/" + Obj + ".file/" + Mbr + ".mbr \"" + filetemp + "\"");

            if (IBMi.RunCommands(commands.ToArray()) == false)
            {
                return filetemp;
            }
            else
            {
                return "";
            }
        }

        private void OpenFileReadOnly(string Path)
        {
            Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DOOPEN, 0, Path);
            Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_SETREADONLY, 1, 0);
        }
    }
}
