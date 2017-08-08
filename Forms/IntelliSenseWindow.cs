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

                if (Show && this.Opacity == 0)
                    this.Location = NppFunctions.GetCaretPos();

                this.Opacity = (Show ? 100 : 0);
            }));
        }

        public void HideWindow()
        {
            this.Opacity = 0;
        }
    }
}
