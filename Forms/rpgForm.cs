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

namespace IBMiCmd.Forms
{
    public partial class rpgForm : Form
    {
        public rpgForm()
        {
            InitializeComponent();
        }

        public static int curFileLine;

        private static string getFree(string input)
        {
            input = ' ' + input.PadRight(80);
            char[] chars = input.ToCharArray();

            string name = "";
            string len = "";
            string type = "";
            string decimals = "";
            string keywords = "";
            string output = "";

            if (Char.ToUpper(chars[6]) == 'D')
            {
                name = input.Substring(7, 16).Trim();
                len = input.Substring(33, 7).Trim();
                type = input.Substring(40, 1);
                decimals = input.Substring(41, 3).Trim();
                keywords = input.Substring(44).Trim();

                switch (type.ToUpper())
                {
                    case "A":
                        if (keywords.ToUpper().Contains("VARYING"))
                        {
                            type = "Varchar";
                        }
                        else
                        {
                            type = "Char";
                        }
                        type += "(" + len + ")";
                        break;
                    case "B":
                        type = "Bindec" + "(" + len + ")";
                        break;
                    case "C":
                        type = "Ucs2" + "(" + len + ")";
                        break;
                    case "D":
                        type = "Date";
                        break;
                    case "F":
                        type = "Float" + "(" + len + ")";
                        break;
                    case "G":
                        if (keywords.ToUpper().Contains("VARYING"))
                        {
                            type = "Graph";
                        }
                        else
                        {
                            type = "Vargraph";
                        }
                        type += "(" + len + ")";
                        break;
                    case "I":
                        type = "Int" + "(" + len + ")";
                        break;
                    case "N":
                        type = "Ind";
                        break;
                    case "P":
                        type = "Packed" + "(" + len + ":" + decimals + ")";
                        break;
                    case "S":
                        type = "Zoned" + "(" + len + ":" + decimals + ")";
                        break;
                    case "T":
                        type = "Time";
                        break;
                    case "U":
                        type = "Uns" + "(" + len + ")";
                        break;
                    case "Z":
                        type = "Timestamp";
                        break;
                    case "*":
                        type = "Pointer";
                        break;

                    default:
                        MessageBox.Show("A data-type is required to convert.", "Conversion error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return "";
                }

                output = "Dcl-S " + name + " " + type + " " + keywords + ';';
            }
            else
            {
                MessageBox.Show("Definition specification is required to convert.", "Conversion error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return output;
        }

        private void rpgForm_Load(object sender, EventArgs e)
        {
            string freeOut = "", fixedLine = "";

            fixedLine = getLine(curFileLine);

            freeOut = getFree(fixedLine);
            if (freeOut != "")
            {
                textBox1.Text = fixedLine;
                textBox2.Text = freeOut;
            }
            else
            {
                this.Close();
            }
            
        }
        
        private static string getLine(int line)
        {
            IntPtr curScintilla = PluginBase.GetCurrentScintilla();
            int lineLength = (int)Win32.SendMessage(curScintilla, SciMsg.SCI_LINELENGTH, line, 0);
            StringBuilder sb = new StringBuilder(lineLength);
            Win32.SendMessage(curScintilla, SciMsg.SCI_GETLINE, line, sb);
            return sb.ToString().Substring(0, lineLength);
        }

        private static void setLine(int line, int value)
        {

        }
    }
}
