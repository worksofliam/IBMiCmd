using NppPluginNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace IBMiCmd.Forms
{
    public partial class rpgFileConvert : Form
    {
        public rpgFileConvert()
        {
            InitializeComponent();
        }
        private string _curFile;

        private void rpgFileConvert_Load(object sender, EventArgs e)
        {
            this._curFile = GetCurrentFileName();
            string[] lines = File.ReadAllLines(this._curFile);
            string[] newLines = new string[lines.Length];

            string curLine = "";
            for(int i = 0; i < lines.Length; i++)
            {
                curLine = RPGTools.getFree(lines[i]);
                newLines[i] = (curLine != "" ? curLine : lines[i]);
            }

            this.Text = "RPG Conversion - " + this._curFile;

            //Place original code into richTextBox1
            richTextBox1.Lines = lines;

            //Generate new code and place into richTextBox2
            richTextBox2.Lines = newLines;
        }

        private void acceptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            File.WriteAllLines(this._curFile, richTextBox2.Lines);
            RefreshWindow(this._curFile);
            this.Close();
        }

        private void declineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        public static string GetCurrentFileName()
        {
            var sb = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_GETFULLCURRENTPATH, 0, sb);
            return sb.ToString();
        }

        public static void RefreshWindow(string path)
        {
            Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_RELOADFILE, 0, path);
        }
    }
}
