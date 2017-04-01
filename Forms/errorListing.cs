using NppPluginNET;
using System;
using System.Windows.Forms;

namespace IBMiCmd.Forms
{
    public partial class errorListing : Form
    {
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
                int totalErrors = 0;
                TreeNode curNode;

                treeView1.Nodes.Clear(); //Clear out the nodes

                //Add the node that allows object change
                curNode = new TreeNode("Double click to change object");
                curNode.Tag = "CHG";
                curNode.ImageIndex = 0;
                curNode.SelectedImageIndex = 0;
                treeView1.Nodes.Add(curNode);

                //Add the errors
                TreeNode curErr;
                foreach (int fileid in ErrorHandle.getFileIDs())
                {
                    curNode = new TreeNode(ErrorHandle.getFilePath(fileid));
                    foreach (lineError error in ErrorHandle.getErrors(fileid))
                    {
                        if (error.getSev() >= 20)
                        {
                            totalErrors += 1;
                            curErr = curNode.Nodes.Add(error.getCode() + ": " + error.getData().Trim() + " (" + error.getLine().ToString() + ")");
                            curErr.Tag = error.getLine().ToString() + ',' + error.getColumn().ToString();
                            curErr.ImageIndex = 2;
                            curErr.SelectedImageIndex = 2;
                        }
                    }

                    //Only add a node if there is something to display
                    if (curNode.Nodes.Count > 0)
                    {
                        curNode.ImageIndex = 1;
                        curNode.SelectedImageIndex = 1;
                        treeView1.Nodes.Add(curNode);
                    }
                }

                toolStripStatusLabel1.Text = "Total errors: " + totalErrors.ToString();
                toolStripStatusLabel2.Text = ErrorHandle.doName();
                toolStripStatusLabel3.Text = DateTime.Now.ToString("h:mm:ss tt");
            });
        }
    }
}
