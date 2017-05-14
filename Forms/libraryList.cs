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
			textBox1.Text = IBMi.getConfig("datalibl");
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			string s = "";
			foreach (string item in textBox1.Text.Trim().Split(',')) {
                if (IBMiUtilities.isValidQSYSObjectName(item.Trim()))
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
         
            IBMi.setConfig("datalibl", s.Remove(s.Length - 1, 1)); //Remove last comma

            this.Close();
        }
	}
}
