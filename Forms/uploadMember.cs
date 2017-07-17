using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using NppPluginNET;
using IBMiCmd.LanguageTools;
using IBMiCmd.IBMiTools;

namespace IBMiCmd.Forms
{
    public partial class uploadMember : Form
    {
        private bool _CloseOnShow;
        private OpenMember _Member;
        public uploadMember()
        {
            InitializeComponent();
            StringBuilder path = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_GETFULLCURRENTPATH, 0, path);

            if (!OpenMembers.Contains(path.ToString()))
            {
                MessageBox.Show("Unable to save this file to a member. You are only able to save to member which has been opened in this same Notepad++ session.");
                this._CloseOnShow = true; return;
            }

            this._Member = OpenMembers.GetMember(path.ToString());

            if (this._Member.GetSystemName() != IBMi.GetConfig("system"))
            {
                MessageBox.Show("You can only save this member when you are connected to " + this._Member.GetSystemName() + ".");
                this._CloseOnShow = true; return;
            }

            textBox1.Text = this._Member.GetLibrary();
            textBox2.Text = this._Member.GetObject();
            textBox3.Text = this._Member.GetMember();
            textBox4.Text = this._Member.GetSystemName();
            textBox5.Text = Path.GetFileName(this._Member.GetLocalFile());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Please note that saving to the member will overwrite any changes that you have not picked up since you last opened the member.", "Notice", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);

            if (result == DialogResult.OK)
            {
                bool UploadResult = IBMiUtilities.UploadMember(this._Member.GetLocalFile(), this._Member.GetLibrary(), this._Member.GetObject(), this._Member.GetMember());

                if (UploadResult)
                {
                    this.Close();
                }
                else
                {
                    if (Main.CommandWindow != null) Main.CommandWindow.loadNewOutput();
                    MessageBox.Show("Failed to save " + this._Member.GetMember() + ". Please check the Command Entry output.");
                }
            }
        }

        private void uploadMember_Shown(object sender, EventArgs e)
        {
            if (this._CloseOnShow) this.Close();
        }
    }
}
