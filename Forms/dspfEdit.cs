using IBMiCmd.LanguageTools;
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

    public partial class dspfEdit : Form
    {
        public dspfEdit()
        {
            InitializeComponent();
        }

        private Control CurrentlySelectedField;
        private void label_MouseClick(object sender, MouseEventArgs e)
        {
            Control controlItem = (Control)sender;
            FieldInfo fieldInfo = (FieldInfo)controlItem.Tag;
            
            this.CurrentlySelectedField = controlItem;
            
            groupBox1.Visible = true;
            groupBox1.Text = fieldInfo.Name;

            field_name.Text = fieldInfo.Name;
            field_val.Text = fieldInfo.Value;
            field_input.Checked = fieldInfo.Type == FieldInfo.TextType.Input;
            field_output.Checked = fieldInfo.Type == FieldInfo.TextType.Output;
            field_text.Checked = fieldInfo.Type == FieldInfo.TextType.Text;

            field_len.Enabled = !field_text.Checked;
            field_len.Value = fieldInfo.Length;

            field_colour.SelectedIndex = field_colour.Items.IndexOf(fieldInfo.Colour);

            field_x.Value = fieldInfo.Position.X;
            field_y.Value = fieldInfo.Position.Y;
        }

        public void AddLabel(FieldInfo.TextType Type, Point location)
        {
            Label text = new Label();
            FieldInfo fieldInfo = new FieldInfo();
            fieldInfo.Name = "TEXT" + screen.Controls.Count.ToString();
            fieldInfo.Length = 9;
            fieldInfo.Value = "Text here";
            fieldInfo.Position = new Point(1, 1);
            fieldInfo.Type = Type;

            text.Name = fieldInfo.Name;
            text.Text = fieldInfo.Value;
            text.Tag = fieldInfo;
            text.Location = DSPFtoUILoc(fieldInfo.Position);
            text.MouseClick += label_MouseClick;
            //if (Input) text.Font = new Font(text.Font, FontStyle.Underline);

            screen.Controls.Add(text);
        }

        public static Point DSPFtoUILoc(Point point)
        {
            int x = point.X - 1, y = point.Y - 1;

            x = x * 8;
            y = y * 19;

            return new Point(x, y);
        }

        private void inputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddLabel(FieldInfo.TextType.Output, new Point(1, 1));
        }

        private void outputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddLabel(FieldInfo.TextType.Output, new Point(1, 1));
        }

        private void field_save_Click(object sender, EventArgs e)
        {
            FieldInfo fieldInfo = (FieldInfo)CurrentlySelectedField.Tag;

            fieldInfo.Name = field_name.Text;
            fieldInfo.Length = Convert.ToInt32(field_len.Value);
            fieldInfo.Value = field_val.Text.PadRight(fieldInfo.Length);
            fieldInfo.Position = new Point(Convert.ToInt32(field_x.Value), Convert.ToInt32(field_y.Value));

            field_len.Enabled = !field_text.Checked;

            if (field_input.Checked)
                fieldInfo.Type = FieldInfo.TextType.Input;
            if (field_output.Checked)
                fieldInfo.Type = FieldInfo.TextType.Output;
            if (field_text.Checked)
                fieldInfo.Type = FieldInfo.TextType.Text;

            if (fieldInfo.Type != FieldInfo.TextType.Text)
            {
                if (field_len.Value < field_val.Text.Length)
                {
                    field_val.Text = field_val.Text.Substring(0, Convert.ToInt32(field_len.Value));
                }
            }

            if (field_colour.SelectedIndex >= 0) {
                fieldInfo.Colour = field_colour.SelectedItems[0].ToString();
            }

            if (fieldInfo.Type == FieldInfo.TextType.Output) {
                if (fieldInfo.Value.Trim() == "")
                {
                    fieldInfo.Value = "".PadRight(fieldInfo.Length, 'O');
                }
            }

            CurrentlySelectedField.Name = fieldInfo.Name;
            CurrentlySelectedField.Text = fieldInfo.Value;
            CurrentlySelectedField.Location = DSPFtoUILoc(fieldInfo.Position);
            CurrentlySelectedField.ForeColor = FieldInfo.TextToColor(fieldInfo.Colour);
            if (fieldInfo.Type == FieldInfo.TextType.Input)
            {
                CurrentlySelectedField.Font = new Font(CurrentlySelectedField.Font, FontStyle.Underline);
            }
            else
            {
                CurrentlySelectedField.Font = new Font(CurrentlySelectedField.Font, FontStyle.Regular);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CurrentlySelectedField.Dispose();
            groupBox1.Visible = false;
        }

        private void screen_MouseClick(object sender, MouseEventArgs e)
        {
            CurrentlySelectedField = null;
            groupBox1.Visible = false;
        }
    }
}
