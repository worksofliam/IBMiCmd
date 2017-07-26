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
        private int fieldCounter = 0;
        public dspfEdit(Dictionary<String, RecordInfo> Formats = null)
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

            Boolean loadNew = false;
            if (Formats == null)
            {
                loadNew = true;
            }
            else
            {
                if (Formats.Count == 0)
                {
                    loadNew = true;
                }
                else
                {
                    tabControl1.TabPages.Clear();
                    foreach (string Format in Formats.Keys)
                    {
                        tabControl1.TabPages.Add(Format);
                    }
                    rcd_name.Text = Formats.Keys.ToArray()[0];
                    tabControl1.SelectedIndex = 0;

                    RecordFormats = Formats;
                    LoadFormat(rcd_name.Text);
                }
            }

            if (loadNew)
            {
                this.CurrentRecordFormat = tabControl1.SelectedTab.Text;
                rcd_name.Text = this.CurrentRecordFormat;
            }
        }

        #region onClicks

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
            field_both.Checked = fieldInfo.Type == FieldInfo.TextType.Both;
            field_hidden.Checked = fieldInfo.Type == FieldInfo.TextType.Hidden;

            field_len.Enabled = !field_text.Checked;
            field_len.Value = fieldInfo.Length;

            field_colour.SelectedIndex = field_colour.Items.IndexOf(fieldInfo.Colour);

            field_x.Enabled = (fieldInfo.Type != FieldInfo.TextType.Hidden);
            field_y.Enabled = (fieldInfo.Type != FieldInfo.TextType.Hidden);

            if (fieldInfo.Type != FieldInfo.TextType.Hidden)
            {
                field_x.Value = fieldInfo.Position.X;
                field_y.Value = fieldInfo.Position.Y;
            }

            int index = comboBox1.Items.IndexOf(fieldInfo.Name);
            if (index >= 0)
                comboBox1.SelectedIndex = index;

            this.CurrentlySelectedField = controlItem;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            int index = comboBox1.Items.IndexOf(CurrentlySelectedField.Name);
            if (index >= 0)
                comboBox1.Items.RemoveAt(index);

            CurrentlySelectedField.Dispose();
            groupBox1.Visible = false;
        }

        private void screen_MouseClick(object sender, MouseEventArgs e)
        {
            comboBox1.SelectedIndex = -1;
            CurrentlySelectedField = null;
            groupBox1.Visible = false;
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            int index = comboBox1.SelectedIndex;
            if (index >= 0)
            {
                if (screen.Controls[comboBox1.Items[index].ToString()] != null)
                {
                    if (CurrentlySelectedField != null)
                    {
                        if (CurrentlySelectedField.Name != comboBox1.Items[index].ToString())
                        {
                            label_MouseClick(screen.Controls[comboBox1.Items[index].ToString()], null);
                        }
                    }
                    else
                    {
                        label_MouseClick(screen.Controls[comboBox1.Items[index].ToString()], null);
                    }
                }
            }
        }

        #endregion

        public static Point DSPFtoUILoc(Point point)
        {
            int x = point.X - 1, y = point.Y - 1;

            x = x * 8;
            y = y * 19;

            return new Point(x, y);
        }

        #region LabelAdding
        public void AddLabel(FieldInfo.TextType Type, Point location, FieldInfo PreInfo = null)
        {
            Label text = new Label();
            FieldInfo fieldInfo;

            if (PreInfo == null)
            {
                fieldCounter++;
                fieldInfo = new FieldInfo();
                fieldInfo.Name = Type.ToString().ToUpper() + fieldCounter.ToString();
                fieldInfo.Length = Type.ToString().Length;
                fieldInfo.Value = Type.ToString();
                fieldInfo.Position = location;
                fieldInfo.Type = Type;
            }
            else
            {
                fieldInfo = PreInfo;
            }

            text.AutoSize = true;
            text.Name = fieldInfo.Name;
            text.Text = fieldInfo.Value;
            text.Tag = fieldInfo;
            text.Location = DSPFtoUILoc(fieldInfo.Position);
            text.Visible = (fieldInfo.Type != FieldInfo.TextType.Hidden);

            text.ForeColor = FieldInfo.TextToColor(fieldInfo.Colour);
            if (fieldInfo.Value.Trim() == "")
            {
                text.Text = fieldInfo.Value.PadRight(fieldInfo.Length, '_');
            }
            else
            {
                text.Text = fieldInfo.Value.PadRight(fieldInfo.Length);
            }

            if (fieldInfo.Type == FieldInfo.TextType.Input)
            {
                text.Font = new Font(screen.Font, FontStyle.Underline);
            }
            else
            {
                text.Font = new Font(screen.Font, FontStyle.Regular);
            }

            text.MouseClick += label_MouseClick;

            screen.Controls.Add(text);
            comboBox1.Items.Add(fieldInfo.Name);
        }
        
        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddLabel(FieldInfo.TextType.Text, new Point(1, 1));
        }
        #endregion

        private void field_save_Click(object sender, EventArgs e)
        {
            if (CurrentlySelectedField == null) return;
            FieldInfo fieldInfo = (FieldInfo)CurrentlySelectedField.Tag;

            if (field_name.Text.Trim() == "") field_name.Text = "FIELD";

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
            if (field_both.Checked)
                fieldInfo.Type = FieldInfo.TextType.Both;
            if (field_hidden.Checked)
                fieldInfo.Type = FieldInfo.TextType.Hidden;

            field_len.Enabled = (fieldInfo.Type != FieldInfo.TextType.Text);

            field_x.Enabled = (fieldInfo.Type != FieldInfo.TextType.Hidden);
            field_y.Enabled = (fieldInfo.Type != FieldInfo.TextType.Hidden);

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

            int index = comboBox1.FindStringExact(CurrentlySelectedField.Name);
            if (index >= 0)
                comboBox1.Items[index] = fieldInfo.Name;

            CurrentlySelectedField.Name = fieldInfo.Name;
            CurrentlySelectedField.Location = DSPFtoUILoc(fieldInfo.Position);
            CurrentlySelectedField.ForeColor = FieldInfo.TextToColor(fieldInfo.Colour);
            CurrentlySelectedField.Visible = (fieldInfo.Type != FieldInfo.TextType.Hidden);
            if (fieldInfo.Value.Trim() == "")
            {
                CurrentlySelectedField.Text = fieldInfo.Value.PadRight(fieldInfo.Length, '_');
            }
            else
            {
                CurrentlySelectedField.Text = fieldInfo.Value.PadRight(fieldInfo.Length);
            }
            if (fieldInfo.Type == FieldInfo.TextType.Input || fieldInfo.Type == FieldInfo.TextType.Both)
            {
                CurrentlySelectedField.Font = new Font(CurrentlySelectedField.Font, FontStyle.Underline);
            }
            else
            {
                CurrentlySelectedField.Font = new Font(CurrentlySelectedField.Font, FontStyle.Regular);
            }
        }
        
        #region Record Formats
        private Dictionary<string, RecordInfo> RecordFormats = new Dictionary<string, RecordInfo>();
        private string CurrentRecordFormat = "";

        private void recordFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newRcdFmt newrcdfmt = new newRcdFmt();

            newrcdfmt.ShowDialog();
            if (newrcdfmt.name.Trim() != "")
            {
                tabControl1.TabPages.Add(new TabPage(newrcdfmt.name));
            }
        }
        
        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            //Saving old tab
            TabPage current = (sender as TabControl).SelectedTab;
            string RcdFmtName = this.CurrentRecordFormat;

            SaveFormat(RcdFmtName);

            screen.Controls.Clear();
        }

        private void tabControl1_TabIndexChanged(object sender, TabControlEventArgs e)
        {
            //Loading new tab
            groupBox1.Visible = false;
            string RcdFmtName = tabControl1.SelectedTab.Text;
            LoadFormat(RcdFmtName);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Handle rename
            if (this.CurrentRecordFormat != rcd_name.Text)
            {
                if (RecordFormats.ContainsKey(this.CurrentRecordFormat))
                {
                    RecordFormats.Remove(this.CurrentRecordFormat);
                }
                tabControl1.SelectedTab.Text = rcd_name.Text;
            }

            SaveFormat(rcd_name.Text);
            this.CurrentRecordFormat = rcd_name.Text;
        }

        private void LoadFormat(String RcdFmtName)
        {
            this.CurrentRecordFormat = RcdFmtName;

            rcd_name.Text = this.CurrentRecordFormat;

            if (!RecordFormats.ContainsKey(RcdFmtName))
                RecordFormats.Add(RcdFmtName, new RecordInfo(RcdFmtName));

            comboBox1.Items.Clear();
            foreach (FieldInfo field in RecordFormats[RcdFmtName].Fields)
            {
                AddLabel(field.Type, field.Position, field);
            }

            for (int i = 0; i < 24; i++)
            {
                rec_funcs.SetItemChecked(i, RecordFormats[RcdFmtName].FunctionKeys[i]);
            }
        }

        private void SaveFormat(string RcdFmtName)
        {
            List<FieldInfo> RecordFields = new List<FieldInfo>();

            foreach (Control field in screen.Controls)
            {
                if (field.Tag is FieldInfo)
                {
                    RecordFields.Add(field.Tag as FieldInfo);
                }
            }

            if (!RecordFormats.ContainsKey(RcdFmtName))
                RecordFormats.Add(RcdFmtName, new RecordInfo(RcdFmtName));

            RecordFormats[RcdFmtName].Fields = RecordFields.ToArray();
            
            for (int i = 0; i < 24; i++) 
            {
                RecordFormats[RcdFmtName].FunctionKeys[i] = rec_funcs.GetItemChecked(i);
            }

            RecordFormats[RcdFmtName].Name = RcdFmtName;
        }
        #endregion
    }
}
