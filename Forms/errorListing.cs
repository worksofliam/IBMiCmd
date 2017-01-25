using NppPluginNET;
using System;
using System.Windows.Forms;

namespace IBMiCmd.Forms
{
    public partial class errorListing : Form
    {
        private TreeNode master;

        public errorListing()
        {
            InitializeComponent();
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag == null) { }
            else if (e.Node.Tag.ToString() == "CHG")
            {
                new programChoose().ShowDialog();
                publishErrors();
            }
            else
            {
                int line;
                if (!int.TryParse(e.Node.Tag.ToString(), out line)) return;
                IntPtr curScintilla = PluginBase.GetCurrentScintilla();
                Win32.SendMessage(curScintilla, SciMsg.SCI_ENSUREVISIBLE, line - 1, 0);
                Win32.SendMessage(curScintilla, SciMsg.SCI_GOTOLINE, line - 1, 0);
                Win32.SendMessage(curScintilla, SciMsg.SCI_GRABFOCUS, 0, 0);
            }
        }

        public void publishErrors()
        {
            if (master == null)
            {
                master = treeView1.Nodes.Add(ErrorHandle.doName());
            }
            else
            {
                master.Text = ErrorHandle.doName();
                master.Nodes.Clear();
            }

            TreeNode curNode;
            int realErrors = 0;
            foreach (lineError error in ErrorHandle.getErrors())
            {
                if (error.getSev() > 0)
                {
                    realErrors += 1;
                    curNode = master.Nodes.Add(error.getData().Trim() + " (" + error.getLine().Trim() + ")");
                    curNode.Tag = error.getLine().Trim();
                }
            }

            if (realErrors == 0)
            {
                master.Nodes.Add("No errors found.");
            }

            master.Expand();
        }
    }
}
