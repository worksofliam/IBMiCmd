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
using IBMiCmd.IBMiTools;

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
            this.Text = "Library List for " + IBMi.GetConfig("system");

			string[] libl = IBMi.GetConfig("datalibl").Split(',');
            foreach(string lib in libl)
            {
                listBox1.Items.Add(lib);
            }

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

            //Add a default library
            if (listBox1.Items.Count == 0)
                listBox1.Items.Add("SYSTOOLS");

            string s = "";
			foreach (string item in listBox1.Items) {
                if (IBMiUtilities.IsValidQSYSObjectName(item.Trim()))
                {
                    s += item.Trim() + ',';
                }
                else
                {
                    label2.Text = "Invalid library: " + item.Trim();
                    label2.Update();
                    return;
                }
            }

            if (!IBMiUtilities.IsValidQSYSObjectName(textBox2.Text.Trim())) 
            {
                label2.Text = "Invalid current library.";
                label2.Update();
                return;
            }

            string origLibl = IBMi.GetConfig("datalibl");
            string origCur = IBMi.GetConfig("curlib");

            IBMi.SetConfig("datalibl", s.Remove(s.Length - 1, 1)); //Remove last comma
            IBMi.SetConfig("curlib", textBox2.Text.Trim()); //Remove last comma

            Boolean hasFailed = IBMi.RunCommands(new string[0]);

            if (hasFailed)
            {
                IBMi.SetConfig("datalibl", origLibl);
                IBMi.SetConfig("curlib", origCur);

                MessageBox.Show("Library list contains invalid libraries.", "Library list", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (Main.CommandWindow != null) Main.CommandWindow.loadNewOutput();
            }
            else
            {
                this.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                if (!listBox1.Items.Contains(textBox1.Text))
                {
                    listBox1.Items.Add(textBox1.Text);
                    textBox1.Text = "";
                }
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index >= 0)
            {
                switch (e.KeyCode)
                {
                    case Keys.Delete:
                        listBox1.Items.RemoveAt(index);
                        break;
                    case Keys.Down:
                        MoveItem(1);
                        break;
                    case Keys.Up:
                        MoveItem(-1);
                        break;
                }
            }
        }

        public void MoveItem(int direction)
        {
            // Checking selected item
            if (listBox1.SelectedItem == null || listBox1.SelectedIndex < 0)
                return; // No selected item - nothing to do

            // Calculate new index using move direction
            int newIndex = listBox1.SelectedIndex + direction;

            // Checking bounds of the range
            if (newIndex < 0 || newIndex >= listBox1.Items.Count)
                return; // Index out of range - nothing to do

            object selected = listBox1.SelectedItem;

            // Removing removable element
            listBox1.Items.Remove(selected);
            // Insert it in new position
            listBox1.Items.Insert(newIndex, selected);
            // Restore selection
            if (direction == 1) listBox1.SetSelected(newIndex-1, true);
        }
    }
}
