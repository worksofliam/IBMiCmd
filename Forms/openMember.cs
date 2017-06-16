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
            if (textBox3.Text.Trim() == "")
            {
                MessageBox.Show("Member name is not valid.");
                return;
            }
            
            string resultFile = DownloadMember(textBox1.Text, textBox2.Text, textBox3.Text);

            if (resultFile != "")
            {
                //Open File
                OpenFile(resultFile, this.isReadonly);
                if (!this.isReadonly)
                    OpenMembers.AddMember(resultFile, textBox1.Text, textBox2.Text, textBox3.Text);

                this.Close();
            }
            else
            {
                if (Main.CommandWindow != null) Main.CommandWindow.loadNewCommands();
                MessageBox.Show("Unable to download member " + textBox3.Text + ". Please check it exists and that you have access to the remote system.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string DownloadMember(string Lib, string Obj, string Mbr)
        {
            string filetemp = Path.GetTempPath() + Mbr + "." + Obj;
            List<string> commands = new List<string>();

            if (!File.Exists(filetemp)) File.Create(filetemp).Close();

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

        private static void OpenFile(string Path, Boolean ReadOnly)
        {
            Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DOOPEN, 0, Path);
            if (ReadOnly) Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_SETREADONLY, 1, 0);
        }
    }
}
