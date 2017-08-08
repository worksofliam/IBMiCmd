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

        public void LoadList(String[] List)
        {
            List<ListViewItem> Items = null;
            this.Invoke(new MethodInvoker(delegate ()
            {
                bool Show = (List != null);

                if (Show)
                    Show = List.Length != 0;
                
                if (Show)
                {
                    Items = new List<ListViewItem>();
                    foreach (string Item in List)
                    {
                        Items.Add(new ListViewItem(Item, (Item.StartsWith("%") ? 1 : 0)));
                    }
                }

                if (Show)
                {
                    listView1.Items.Clear();
                    listView1.Items.AddRange(Items.ToArray());
                }

                if (Show && this.Opacity == 0)
                    this.Location = NppFunctions.GetCaretPos();

                this.Opacity = (Show ? 100 : 0);
            }));
        }
    }
}
