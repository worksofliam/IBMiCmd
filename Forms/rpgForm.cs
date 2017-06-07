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

            fixedLine = getLine(curFileLine);

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
        
        private static string getLine(int line)
        {
            IntPtr curScintilla = PluginBase.GetCurrentScintilla();
            int lineLength = (int)Win32.SendMessage(curScintilla, SciMsg.SCI_LINELENGTH, line, 0);
            StringBuilder sb = new StringBuilder(lineLength);
            Win32.SendMessage(curScintilla, SciMsg.SCI_GETLINE, line, sb);

            line = (int)Win32.SendMessage(curScintilla, SciMsg.SCI_POSITIONFROMLINE, line, 0);
            lineLength--;
            Win32.SendMessage(curScintilla, SciMsg.SCI_SETSELECTION, line, line+lineLength);

            return sb.ToString().Substring(0, lineLength);
        }

        private static void setLine(string value)
        {
            //Hopefully is still selected?
            IntPtr curScintilla = PluginBase.GetCurrentScintilla();
            Win32.SendMessage(curScintilla, SciMsg.SCI_REPLACESEL, 0, value);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            setLine(textBox2.Text);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
