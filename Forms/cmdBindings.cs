using NppPluginNET;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using IBMiCmd.LanguageTools;
using IBMiCmd.IBMiTools;

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

            foreach (string bind in IBMi.GetConfig("binds").Split('|'))
            {
                bindings.Add(bind.Trim());
            }
            treeView1.Nodes.Clear();
            foreach (string bind in bindings)
            {
                curNode = treeView1.Nodes.Add(bind);
                curNode.Tag = IBMi.GetConfig(bind);
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
                    cmds[i] = replaceVars(cmds[i].Trim());
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
            IBMi.RunCommands(commands);
            if (Main.CommandWindow != null) Main.CommandWindow.loadNewCommands();

            if (errDsp != null)
            {
                ErrorHandle.getErrors(errDsp[0], errDsp[1]);
                if (Main.ErrorWindow != null)
                {
                    Main.ErrorWindow.publishErrors();
                    Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMSHOW, 0, Main.ErrorWindow.Handle);
                }
            }
        }
        
        private static string replaceVars(string cmd)
        {
            StringBuilder path = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_GETFILENAME, 0, path);
            string[] name;
            if (path.ToString().Contains("."))
            {
                name = path.ToString().Split('.');
            }
            else
            {
                name = new string[2];
                name[0] = path.ToString();
                name[1] = "";
            }

            cmd = cmd.Replace("%file%", name[0]);
            cmd = cmd.Replace("%ext%", name[1]);

            return cmd;
        }

        private void wrkwithBind(object sender, System.EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                editingBind = treeView1.SelectedNode.Text;
                editingBindCmds = IBMi.GetConfig(editingBind).Split('|');

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
                IBMi.SetConfig(editingBind, string.Join("|", editingBindCmds));
                
                if (bindings.Contains(editingBind))
                {
                    //Handled in wrkwithBind
                }
                else
                {
                    curNode = treeView1.Nodes.Add(editingBind);
                    curNode.Tag = string.Join("|", editingBindCmds);

                    bindings.Add(editingBind);
                    IBMi.SetConfig("binds", string.Join("|", bindings.ToArray()));
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
                        IBMi.SetConfig("binds", string.Join("|", bindings.ToArray()));
                        IBMi.RemConfig(treeView1.SelectedNode.Text);
                        treeView1.Nodes.Remove(treeView1.SelectedNode);
                    }
                }
            }
        }
    }
}
