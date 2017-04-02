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

            string name = input.Substring(7, 16).Trim();
            string len = "";
            string type = "";
            string decimals = "";
            string keywords = input.Substring(44).Trim();
            string output = "";
            string field = "";

            string field1 = "", field2 = "", result = "", opcode = "";

            switch(Char.ToUpper(chars[6]))
            {
                case 'D':
                    len = input.Substring(33, 7).Trim();
                    type = input.Substring(40, 1).Trim();
                    decimals = input.Substring(41, 3).Trim();
                    field = input.Substring(24, 2).Trim().ToUpper();

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
                        case "":
                            if (len != "")
                            {
                                if (decimals == "")
                                {
                                    if (keywords.ToUpper().Contains("VARYING"))
                                    {
                                        type = "Varchar";
                                    }
                                    else
                                    {
                                        type = "Char";
                                    }
                                    type += "(" + len + ")";
                                }
                                else
                                {
                                    type = "Zoned" + "(" + len + ":" + decimals + ")";
                                }
                            }
                            break;
                    }

                    switch (field)
                    {
                        case "S":
                            output = "Dcl-S " + name + " " + type + " " + keywords + ';';
                            break;
                        case "DS":
                        case "PR":
                        case "PI":
                            if (name == "") name = "*N";
                            output = "Dcl-" + field + " " + name + " " + type + " " + keywords + ";";
                            break;
                        case "":
                            if (name == "") name = "*N";
                            output = "  Dcl-Parm " + name + " " + type + " " + keywords + ';';
                            break;
                    }
                    break;

                case 'P':
                    switch(Char.ToUpper(chars[24]))
                    {
                        case 'B':
                            output = "Dcl-Proc " + name + " " + keywords + ";";
                            break;
                        case 'E':
                            output = "End-Proc;";
                            break;
                    }
                    break;

                case 'C':
                    int spaces = 0;
                    field1 = input.Substring(12, 14).Trim();
                    opcode = input.Substring(26, 10).Trim().ToUpper();
                    field2 = input.Substring(36, 14).Trim();
                    result = input.Substring(50, 14).Trim();

                    switch (opcode)
                    {
                        case "ADD":
                            output = result + " = " + field1 + " + " + field2 + ";";
                            break;
                        case "BEGSR":
                            output = opcode + " " + field1 + ";";
                            break;
                        case "CAT":
                            if (field2.Contains(":"))
                            {
                                spaces = int.Parse(field2.Split(':')[1]);
                                field2 = field2.Split(':')[0].Trim();
                            }
                            output = result + " = " + field1 + "+ '" + "".PadLeft(spaces) + "' + " + field2 + ";";
                            break;
                        case "CHAIN":
                            output = opcode + " " + field1 + " " + field2 + " " + result + ";";
                            break;
                    }
                    break;

                default:
                    MessageBox.Show("Specification not supported by converter.", "Conversion error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }

            return "".PadLeft(8) + output;
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
