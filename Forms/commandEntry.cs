using System;
using System.Threading;
using System.Windows.Forms;
using IBMiCmd.IBMiTools;

namespace IBMiCmd
{
    public partial class commandEntry : Form
    {
        public commandEntry()
        {
            InitializeComponent();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Modifiers == Keys.Control)
            {
                string[] commands = textBox1.Lines;
                textBox1.Clear();

                richTextBox1.AppendText(Environment.NewLine);
                foreach (string cmd in commands)
                {
                    richTextBox1.AppendText(Environment.NewLine + cmd);
                }

                for (int i = 0; i < commands.Length; i++)
                {
                    commands[i] = "QUOTE RCMD " + commands[i];
                }

                Thread gothread = new Thread((ThreadStart)delegate { runCommands(commands); });
                gothread.Start();
            }
        }

        public void runCommands(string[] commands)
        {
            IBMi.RunCommands(commands);
            loadNewOutput();
        }

        public void loadNewOutput()
        {
            Invoke((MethodInvoker)delegate
            {
                richTextBox1.AppendText(Environment.NewLine);
                foreach (string line in IBMi.GetOutput())
                {
                    richTextBox1.AppendText(Environment.NewLine + line);
                }
                IBMi.FlushOutput();
            });
        }


    }
}
