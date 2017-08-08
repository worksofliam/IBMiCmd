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
                programChoose select = new programChoose();

                select.ShowDialog();
                if (select._GotErrors) 
                    publishErrors();
            }
            else
            {
                string[] data = e.Node.Tag.ToString().Split(',');
                int line, col;

                line = int.Parse(data[0]) - 1;
                col = int.Parse(data[1]);
                if (col > 0) col--;

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

                IBMi.AddOutput(files[0] + " == " + files[1]);
                if (files[0] == files[1])
                {
                    NppFunctions.SwitchToFile(OpenFile, Line, Col);
                    return;
                }
            }

            //Compare just by file name afterwards
            files[0] = Path.GetFileNameWithoutExtension(File);
            foreach (string OpenFile in OpenFiles)
            {
                files[1] = Path.GetFileNameWithoutExtension(OpenFile);

                IBMi.AddOutput(files[0] + " == " + files[1]);
                if (files[0] == files[1])
                {
                    NppFunctions.SwitchToFile(OpenFile, Line, Col);
                    return;
                }
            }

            //Compare just by file name & try MEMBER name
            files[0] = GetQSYSMemberName(File);
            foreach (string OpenFile in OpenFiles)
            {
                files[1] = Path.GetFileNameWithoutExtension(OpenFile);

                IBMi.AddOutput(files[0] + " == " + files[1]);
                if (files[0] == files[1])
                {
                    NppFunctions.SwitchToFile(OpenFile, Line, Col);
                    return;
                }
            }

            if (Main.CommandWindow != null) Main.CommandWindow.loadNewOutput();

            MessageBox.Show("Unable to open error. Please open the source manually first and then try again.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static string GetQSYSMemberName(string Input)
        {
            if (Input.Contains("(") && Input.Contains(")"))
            {
                Input = Input.Substring(Input.IndexOf('(') + 1);
                Input = Input.Substring(0, Input.IndexOf(')'));
            }
            return Input;
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
