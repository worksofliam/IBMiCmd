using NppPluginNET;
using System;
using System.Windows.Forms;
using IBMiCmd.LanguageTools;
using IBMiCmd.IBMiTools;
using System.IO;

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
                int line, col;

                line = int.Parse(data[0]) - 1;
                col = int.Parse(data[1]) - 1;

                onSelectError(e.Node.Parent.Text, line, col);
            }
        }

        private void onSelectError(string File, int Line, int Col)
        {
            string[] OpenFiles = NppFunctions.GetOpenFiles();
            string[] files = new string[2];

            //Compare by file name and extension first
            files[0] = Path.GetFileName(File);
            foreach(string OpenFile in OpenFiles)
            {
                files[1] = Path.GetFileName(OpenFile);

                if (files[0] == files[1])
                {
                    SwitchToFile(OpenFile, Line, Col);
                    return;
                }
            }

            //Compare just by file name afterwards
            files[0] = Path.GetFileNameWithoutExtension(File);
            foreach (string OpenFile in OpenFiles)
            {
                files[1] = Path.GetFileNameWithoutExtension(OpenFile);

                if (files[0] == files[1])
                {
                    SwitchToFile(OpenFile, Line, Col);
                    return;
                }
            }

            MessageBox.Show("Unable to open error. Please open the source manually first and then try again.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void SwitchToFile(string name, int line, int col)
        {
            int pos = 0;
            IntPtr curScintilla = PluginBase.nppData._nppHandle;
            Win32.SendMessage(curScintilla, NppMsg.NPPM_SWITCHTOFILE, 0, name);

            curScintilla = PluginBase.GetCurrentScintilla();
            Win32.SendMessage(curScintilla, SciMsg.SCI_ENSUREVISIBLE, line, 0);
            if (line >= 0)
            {
                pos = (int)Win32.SendMessage(curScintilla, SciMsg.SCI_POSITIONFROMLINE, line, 0);
                pos += col;
                Win32.SendMessage(curScintilla, SciMsg.SCI_GOTOPOS, pos, 0);
                Win32.SendMessage(curScintilla, SciMsg.SCI_GRABFOCUS, 0, 0);
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
                    foreach (LineError error in ErrorHandle.getErrors(fileid))
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
                toolStripStatusLabel2.Text = IBMi.GetConfig("system") + ":" + ErrorHandle.doName();
                toolStripStatusLabel3.Text = DateTime.Now.ToString("h:mm:ss tt");
            });
        }
    }
}
