using IBMiCmd.LanguageTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IBMiCmd.Forms
{
    public partial class IntelliSenseWindow : Form
    {
        public IntelliSenseWindow()
        {
            InitializeComponent();
            this.Visible = false;
        }

        private string currentKey = "";

        public void SetKey(string Key)
        {
            currentKey = Key;
        }

        public void LoadList(ListViewItem[] List)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                bool Show = (List != null);

                if (Show)
                    Show = List.Length != 0;

                if (Show)
                {
                    listView1.Items.Clear();
                    listView1.Items.AddRange(List);
                }

                Point newLoc = NppFunctions.GetCaretPos();
                if (Show && (this.Opacity == 0 || newLoc.Y > this.Location.Y))
                    this.Location = newLoc;

                if (Show)
                    NppFunctions.CancelNPPAutoC();

                this.Opacity = (Show ? 100 : 0);
            }));
        }

        public void HideWindow()
        {
            this.Opacity = 0;
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count == 1) {
                string value = listView1.SelectedItems[0].Text;
                if (value.Contains(" "))
                    value = value.Substring(0, value.IndexOf(' '));

                value = value.Substring(currentKey.Length);
                //NppFunctions.AppendText(value);
                HideWindow();
            }

        }
    }
}
