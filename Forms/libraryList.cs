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
		
			if (textBox1.Text.Trim() == "")
			{
				textBox1.Focus();
				return;
			}

			foreach (var item in textBox1.Text.Trim().Split(',')) {
				// Check that each entered item matches the requirement for a library name in QSYS.LIB
				Match match = Regex.Match(item, "^[a-zA-Z]\\w{0,9}}$");
				if (match.Success)
				{
					s += item + ',';
				}
				else {
					// TODO: Notify of syntax error
				}
			}
			// Remove last comma
			s.Remove(s.Length - 1, 1);

			IBMi.setConfig("datalibl", s);

			this.Close();
		}
	}
}
