using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IBMiCmd.IBMiTools;

namespace IBMiCmd.Forms
{
    public partial class selectConfig : Form
    {
        public selectConfig()
        {
            InitializeComponent();
        }

        private void ReloadListView()
        {
            listView1.Items.Clear();
            foreach (string System in Config.GetConfigs())
            {
                listView1.Items.Add(new ListViewItem(System, 0));
            }
        }

        private void selectConfig_Load(object sender, EventArgs e)
        {
            ReloadListView();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                Config.SwitchToConfig(listView1.SelectedItems[0].Text);

                if (Main.BindsWindow != null) Main.BindsWindow.cmdBindings_Load();
                IBMi.AddOutput("Switched to " + listView1.SelectedItems[0].Text + ".");
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new newConfig().ShowDialog();
            ReloadListView();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
