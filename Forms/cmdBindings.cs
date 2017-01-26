using NppPluginNET;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace IBMiCmd.Forms
{
    public partial class cmdBindings : Form
    {
        public cmdBindings()
        {
            InitializeComponent();
        }

        public static string editingBind = "";
        public static string[] editingBindCmds;
        public static bool editingCanceled = true;

        private static List<string> bindings;

        public void cmdBindings_Load()
        {
            TreeNode curNode;

            bindings = new List<string>();

            foreach (string bind in IBMi.getConfig("binds").Split('|'))
            {
                bindings.Add(bind.Trim());
            }
            foreach(string bind in bindings)
            {
                curNode = treeView1.Nodes.Add(bind);
                curNode.Tag = IBMi.getConfig(bind);
            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string[] cmds;
            string[] data;

            string[] errDsp = null;

            if (e.Node.Tag.ToString().Trim() != "")
            {
                cmds = e.Node.Tag.ToString().Split('|');

                for(int i = 0; i < cmds.Length; i++)
                {
                    cmds[i] = cmds[i].Trim();
                    data = cmds[i].Split(' ');

                    switch (data[0].ToUpper())
                    {
                        case "ERRORS":
                            errDsp = new string[2];
                            errDsp[0] = data[1];
                            errDsp[1] = data[2];
                            cmds[i] = "";
                            break;

                        default:
                            cmds[i] = "QUOTE RCMD " + cmds[i];
                            break;
                    }
                }

                Thread gothread = new Thread((ThreadStart)delegate { runCommands(cmds, errDsp); });
                gothread.Start();
            }
        }

        public void runCommands(string[] commands, string[] errDsp)
        {
            IBMi.runCommands(commands);
            if (Main.commandWindow != null) Main.commandWindow.loadNewCommands();

            if (errDsp != null)
            {
                ErrorHandle.getErrors(errDsp[0], errDsp[1]);
                if (Main.errorWindow != null)
                {
                    Main.errorWindow.publishErrors();
                    Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMSHOW, 0, Main.errorWindow.Handle);
                }
            }
        }

        private void wrkwithBind(object sender, System.EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                editingBind = treeView1.SelectedNode.Text;
                editingBindCmds = IBMi.getConfig(editingBind).Split('|');

                wrkWithBind();

                treeView1.SelectedNode.Tag = string.Join("|", editingBindCmds);
            }
            else
            {
                MessageBox.Show("Please select a binding to edit.");
            }
        }

        private void addBind(object sender, System.EventArgs e)
        {
            editingBind = "";
            editingBindCmds = null;
            wrkWithBind();
        }

        public void wrkWithBind()
        {
            TreeNode curNode;

            editingCanceled = true;
            new wrkBind().ShowDialog();
            if (editingCanceled == false)
            {
                IBMi.setConfig(editingBind, string.Join("|", editingBindCmds));
                
                if (bindings.Contains(editingBind))
                {
                    //Handled in wrkwithBind
                }
                else
                {
                    curNode = treeView1.Nodes.Add(editingBind);
                    curNode.Tag = string.Join("|", editingBindCmds);

                    bindings.Add(editingBind);
                    IBMi.setConfig("binds", string.Join("|", bindings.ToArray()));
                }
            }
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (treeView1.SelectedNode != null)
                {
                    var confirmResult = MessageBox.Show("Are you sure to delete this binding?",
                                     "Delete binding",
                                     MessageBoxButtons.YesNo);

                    if (confirmResult == DialogResult.Yes)
                    {
                        bindings.Remove(treeView1.SelectedNode.Text);
                        IBMi.setConfig("binds", string.Join("|", bindings.ToArray()));
                        IBMi.remConfig(treeView1.SelectedNode.Text);
                        treeView1.Nodes.Remove(treeView1.SelectedNode);
                    }
                }
            }
        }
    }
}
