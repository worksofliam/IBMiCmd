using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using IBMiCmd.IBMiTools;
using System.Drawing;

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
            ListViewItem item;
            foreach (string System in Config.GetConfigs())
            {
                string configLoc = Main.SystemsDirectory + System + ".cfg";
                item = listView1.Items.Add(new ListViewItem(System, 0));

                if (IBMi._ConfigFile == configLoc)
                    item.ForeColor = Color.Blue;
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
                if (Main.MemberListWindow != null) Main.MemberListWindow.UpdateSystemName();

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

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (listView1.SelectedItems.Count == 1)
                {
                    var confirmResult = MessageBox.Show("Are you sure to delete this configuration?",
                                     "Delete system configuration",
                                     MessageBoxButtons.YesNo);

                    if (confirmResult == DialogResult.Yes)
                    {
                        ListViewItem item = listView1.SelectedItems[0];
                        string configLoc = Main.SystemsDirectory + item.Text + ".cfg";
                        if (IBMi._ConfigFile != configLoc)
                        {
                            File.Delete(configLoc);
                            listView1.Items.Remove(item);
                        }
                        else
                        {
                            MessageBox.Show("Cannot delete config which is currently selected.");
                        }
                    }
                }
            }
        }
    }
}
