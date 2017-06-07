using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IBMiCmd.Forms
{
	public partial class libraryList : Form
	{
		public libraryList()
		{
			InitializeComponent();
		}

		private void libraryList_Load(object sender, EventArgs e)
		{
			textBox1.Text = IBMi.GetConfig("datalibl");
            textBox2.Text = IBMi.GetConfig("curlib");
        }

		private void button1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void button2_Click(object sender, EventArgs e)
		{
            label2.Text = "";
            label2.Update();

            string s = "";
			foreach (string item in textBox1.Text.Trim().Split(',')) {
                if (IBMiUtilities.IsValidQSYSObjectName(item.Trim()))
                {
                    s += item.Trim() + ',';
                }
                else
                {
                    label2.Text = "Invalid Library List Syntax. Valid syntax is < LIB,LIB,LIB >";
                    label2.Update();
                    return;
                }
            }

            if (IBMiUtilities.IsValidQSYSObjectName(textBox2.Text.Trim())) {
                IBMi.SetConfig("curlib", textBox2.Text.Trim()); //Remove last comma
            }
            else
            {
                label2.Text = "Invalid Current Library Syntax. Valid syntax is < LIB >";
                label2.Update();
                return;
            }

            IBMi.SetConfig("datalibl", s.Remove(s.Length - 1, 1)); //Remove last comma

            this.Close();
        }
    }
}
