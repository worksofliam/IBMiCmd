using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NppPluginNET;

namespace IBMiCmd.Forms.prompts
{
    public partial class clPrompt : Form
    {
        public clPrompt()
        {
            InitializeComponent();
        }

        private Boolean validate()
        {
            Boolean isGood = true;
            string messageOut = "";

            string[] libs = textBox3.Text.Split(' ');

            foreach(string lib in libs)
            {
                if (lib.Length > 10)
                {
                    isGood = false;
                    messageOut = "Library " + lib + " is longer than 10 characters.";
                }
            }

            if (!textBox1.Text.Contains("/"))
            {
                isGood = false;
                messageOut = "Library and program name required.";
            }

            if (checkBox1.Checked)
            {
                if (!textBox4.Text.ToUpper().Contains("*EVENTF"))
                {
                    isGood = false;
                    messageOut = "For errors to show, please use OPTION(*EVENTF) as a command parameter.";
                }
            }

            if (messageOut != "")
            {
                MessageBox.Show(messageOut, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            return isGood;
        }

        private string[] generateCommands()
        {
            List<string> list = new List<string>();

            if (textBox3.Text.Trim() != "") list.Add("CHGLIBL LIBL(" + textBox3.Text.ToUpper() + ")");
            list.Add("CRTSRCPF FILE(QTEMP/IBMICMDSRC) RCDLEN(112)");
            list.Add("CPYFRMSTMF FROMSTMF('" + textBox2.Text + "') TOMBR('/QSYS.lib/QTEMP.lib/IBMICMDSRC.file/THECL.mbr') MBROPT(*ADD)");
            list.Add("CRTBNDCL PGM(" + textBox1.Text.ToUpper() + ") SRCFILE(QTEMP/IBMICMDSRC) SRCMBR(THECL) " + textBox4.Text);
            list.Add("DLTOBJ OBJ(QTEMP/IBMICMDSRC) OBJTYPE(*FILE)");

            if (checkBox1.Checked)
            {
                list.Add("ERRORS " + textBox1.Text.Replace('/', ' '));
            }

            return list.ToArray();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> bindings;
            if (validate())
            {
                IBMi.setConfig("CLBind", string.Join("|", generateCommands()));
                bindings = IBMi.getConfig("binds").Split('|').ToList();
                bindings.Add("CLBind");
                IBMi.setConfig("binds", string.Join("|", bindings.ToArray()));

                if (Main.bindsWindow != null)
                {
                    Main.bindsWindow.cmdBindings_Load();
                    Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMSHOW, 0, Main.bindsWindow.Handle);
                }

                this.Close();
            }
        }
    }
}
