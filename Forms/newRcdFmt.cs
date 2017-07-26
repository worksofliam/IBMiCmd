using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IBMiCmd.Forms
{
    public partial class newRcdFmt : Form
    {
        public newRcdFmt()
        {
            InitializeComponent();
        }

        public string name = "";

        private void button1_Click(object sender, EventArgs e)
        {
            this.name = rcd_name.Text;
            this.Close();
        }
    }
}
