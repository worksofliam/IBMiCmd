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
            field_name.TextChanged += field_save_Click;
            field_val.TextChanged += field_save_Click;
            field_input.CheckedChanged += field_save_Click;
            field_output.CheckedChanged += field_save_Click;
            field_text.CheckedChanged += field_save_Click;
            field_len.ValueChanged += field_save_Click;
            field_colour.SelectedIndexChanged += field_save_Click;

            field_x.ValueChanged += field_save_Click;
            field_y.ValueChanged += field_save_Click;
        }

        private Control CurrentlySelectedField;
        private void label_MouseClick(object sender, MouseEventArgs e)
        {
            CurrentlySelectedField = null;

            Control controlItem = (Control)sender;
            FieldInfo fieldInfo = (FieldInfo)controlItem.Tag;
            
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

            this.CurrentlySelectedField = controlItem;
        }

        public void AddLabel(FieldInfo.TextType Type, Point location)
        {
            Label text = new Label();
            FieldInfo fieldInfo = new FieldInfo();
            fieldInfo.Name = Type.ToString().ToUpper() + screen.Controls.Count.ToString();
            fieldInfo.Length = Type.ToString().Length;
            fieldInfo.Value = Type.ToString();
            fieldInfo.Position = new Point(1, 1);
            fieldInfo.Type = Type;

            text.AutoSize = true;
            text.Name = fieldInfo.Name;
            text.Text = fieldInfo.Value;
            text.Tag = fieldInfo;
            text.Location = DSPFtoUILoc(fieldInfo.Position);
            text.MouseClick += label_MouseClick;

            screen.Controls.Add(text);
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

        public static Point DSPFtoUILoc(Point point)
        {
            int x = point.X - 1, y = point.Y - 1;

            x = x * 8;
            y = y * 19;

            return new Point(x, y);
        }

        private void inputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddLabel(FieldInfo.TextType.Input, new Point(1, 1));
        }

        private void outputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddLabel(FieldInfo.TextType.Output, new Point(1, 1));
        }

        private void textToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddLabel(FieldInfo.TextType.Text, new Point(1, 1));
        }

        private void field_save_Click(object sender, EventArgs e)
        {
            if (CurrentlySelectedField == null) return;

            FieldInfo fieldInfo = (FieldInfo)CurrentlySelectedField.Tag;

            fieldInfo.Name = field_name.Text;
            fieldInfo.Length = Convert.ToInt32(field_len.Value);
            fieldInfo.Value = field_val.Text;
            fieldInfo.Position = new Point(Convert.ToInt32(field_x.Value), Convert.ToInt32(field_y.Value));

            if (field_input.Checked)
                fieldInfo.Type = FieldInfo.TextType.Input;
            if (field_output.Checked)
                fieldInfo.Type = FieldInfo.TextType.Output;
            if (field_text.Checked)
                fieldInfo.Type = FieldInfo.TextType.Text;

            field_len.Enabled = (fieldInfo.Type != FieldInfo.TextType.Text);

            if (fieldInfo.Type == FieldInfo.TextType.Text)
            {
                if (fieldInfo.Value.Length == 0) fieldInfo.Value = "-";

                fieldInfo.Length = fieldInfo.Value.Length;
                field_len.Value = fieldInfo.Length;
            }
            else
            {
                if (fieldInfo.Value.Length > fieldInfo.Length)
                {
                    fieldInfo.Value = fieldInfo.Value.Substring(0, fieldInfo.Length);
                    field_val.Text = fieldInfo.Value;
                }
            }

            if (field_colour.SelectedIndex >= 0) {
                fieldInfo.Colour = field_colour.SelectedItems[0].ToString();
            }

            CurrentlySelectedField.Name = fieldInfo.Name;
            CurrentlySelectedField.Location = DSPFtoUILoc(fieldInfo.Position);
            CurrentlySelectedField.ForeColor = FieldInfo.TextToColor(fieldInfo.Colour);
            if (fieldInfo.Value.Trim() == "")
            {
                CurrentlySelectedField.Text = fieldInfo.Value.PadRight(fieldInfo.Length, '_');
            }
            else
            {
                CurrentlySelectedField.Text = fieldInfo.Value.PadRight(fieldInfo.Length);
            }
            if (fieldInfo.Type == FieldInfo.TextType.Input)
            {
                CurrentlySelectedField.Font = new Font(CurrentlySelectedField.Font, FontStyle.Underline);
            }
            else
            {
                CurrentlySelectedField.Font = new Font(CurrentlySelectedField.Font, FontStyle.Regular);
            }
        }
    }
}
