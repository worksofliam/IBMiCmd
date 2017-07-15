using System;
using System.Text;
using System.Windows.Forms;
using NppPluginNET;
using IBMiCmd.LanguageTools;

namespace IBMiCmd.Forms
{
    public partial class rpgForm : Form
    {
        public rpgForm()
        {
            InitializeComponent();
        }

        public static int curFileLine;

        

        private void rpgForm_Load(object sender, EventArgs e)
        {
            string freeOut = "", fixedLine = "";
            
            fixedLine = NppFunctions.GetLine(NppFunctions.GetLineNumber());

            freeOut = RPGFree.getFree(fixedLine);
            if (freeOut != "")
            {
                textBox1.Text = fixedLine;
                textBox2.Text = freeOut;
            }
            else
            {
                this.Close();
                MessageBox.Show("Unable to convert line.");
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NppFunctions.SetLine(textBox2.Text);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
