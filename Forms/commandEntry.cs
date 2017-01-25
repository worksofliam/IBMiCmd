using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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

                IBMi.runCommands(commands);
                loadNewCommands();
            }
        }

        public void loadNewCommands()
        {
            richTextBox1.AppendText(Environment.NewLine);
            foreach (string line in IBMi.getOutput())
            {
                richTextBox1.AppendText(Environment.NewLine + "> " + line);
            }
        }
    }
}
