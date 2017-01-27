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
                string[] data = e.Node.Tag.ToString().Split(',');
                int line, col, pos;

                line = int.Parse(data[0]) - 1;
                col = int.Parse(data[1]) - 1;

                IntPtr curScintilla = PluginBase.GetCurrentScintilla();
                Win32.SendMessage(curScintilla, SciMsg.SCI_ENSUREVISIBLE, line, 0);
                if (line >= 0)
                {
                    pos = (int)Win32.SendMessage(curScintilla, SciMsg.SCI_POSITIONFROMLINE, line, 0);
                    pos += col;
                    Win32.SendMessage(curScintilla, SciMsg.SCI_GOTOPOS, pos, 0);
                    Win32.SendMessage(curScintilla, SciMsg.SCI_GRABFOCUS, 0, 0);
                }
            }
        }

        public void publishErrors()
        {
            Invoke((MethodInvoker)delegate
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
                        curNode = master.Nodes.Add(error.getCode() + ": " + error.getData().Trim() + " (" + error.getLine().ToString() + ")");
                        curNode.Tag = error.getLine().ToString() + ',' + error.getColumn().ToString();
                    }
                }

                if (realErrors == 0)
                {
                    master.Nodes.Add("No errors found.");
                }

                master.Expand();
            });
        }
    }
}
