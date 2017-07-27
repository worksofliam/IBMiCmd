namespace IBMiCmd.Forms
{
    partial class dspfEdit
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.screen = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recordFormatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.field_hidden = new System.Windows.Forms.RadioButton();
            this.field_both = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.field_text = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.field_y = new System.Windows.Forms.NumericUpDown();
            this.field_x = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.field_colour = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.field_len = new System.Windows.Forms.NumericUpDown();
            this.field_output = new System.Windows.Forms.RadioButton();
            this.field_input = new System.Windows.Forms.RadioButton();
            this.field_val = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.field_name = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.rec_funcs = new System.Windows.Forms.CheckedListBox();
            this.rcd_name = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rec_pageup = new System.Windows.Forms.CheckBox();
            this.rec_pagedown = new System.Windows.Forms.CheckBox();
            this.field_number = new System.Windows.Forms.CheckBox();
            this.field_dec = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.field_y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.field_x)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.field_len)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.field_dec)).BeginInit();
            this.SuspendLayout();
            // 
            // screen
            // 
            this.screen.BackColor = System.Drawing.Color.Black;
            this.screen.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.screen.ForeColor = System.Drawing.Color.Lime;
            this.screen.Location = new System.Drawing.Point(204, 52);
            this.screen.Name = "screen";
            this.screen.Size = new System.Drawing.Size(717, 456);
            this.screen.TabIndex = 1;
            this.screen.MouseClick += new System.Windows.Forms.MouseEventHandler(this.screen_MouseClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.insertToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1175, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // insertToolStripMenuItem
            // 
            this.insertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recordFormatToolStripMenuItem,
            this.textToolStripMenuItem});
            this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
            this.insertToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.insertToolStripMenuItem.Text = "Insert";
            // 
            // recordFormatToolStripMenuItem
            // 
            this.recordFormatToolStripMenuItem.Name = "recordFormatToolStripMenuItem";
            this.recordFormatToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.recordFormatToolStripMenuItem.Text = "Record Format";
            this.recordFormatToolStripMenuItem.Click += new System.EventHandler(this.recordFormatToolStripMenuItem_Click);
            // 
            // textToolStripMenuItem
            // 
            this.textToolStripMenuItem.Name = "textToolStripMenuItem";
            this.textToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.textToolStripMenuItem.Text = "Field";
            this.textToolStripMenuItem.Click += new System.EventHandler(this.textToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.field_dec);
            this.groupBox1.Controls.Add(this.field_number);
            this.groupBox1.Controls.Add(this.field_hidden);
            this.groupBox1.Controls.Add(this.field_both);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.field_text);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.field_y);
            this.groupBox1.Controls.Add(this.field_x);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.field_colour);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.field_len);
            this.groupBox1.Controls.Add(this.field_output);
            this.groupBox1.Controls.Add(this.field_input);
            this.groupBox1.Controls.Add(this.field_val);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.field_name);
            this.groupBox1.Location = new System.Drawing.Point(927, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(239, 445);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Selected";
            this.groupBox1.Visible = false;
            // 
            // field_hidden
            // 
            this.field_hidden.AutoSize = true;
            this.field_hidden.Location = new System.Drawing.Point(149, 176);
            this.field_hidden.Name = "field_hidden";
            this.field_hidden.Size = new System.Drawing.Size(78, 24);
            this.field_hidden.TabIndex = 17;
            this.field_hidden.TabStop = true;
            this.field_hidden.Text = "Hidden";
            this.field_hidden.UseVisualStyleBackColor = true;
            // 
            // field_both
            // 
            this.field_both.AutoSize = true;
            this.field_both.Location = new System.Drawing.Point(62, 117);
            this.field_both.Name = "field_both";
            this.field_both.Size = new System.Drawing.Size(61, 24);
            this.field_both.TabIndex = 16;
            this.field_both.TabStop = true;
            this.field_both.Text = "Both";
            this.field_both.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(143, 410);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 29);
            this.button1.TabIndex = 15;
            this.button1.Text = "Delete";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // field_text
            // 
            this.field_text.AutoSize = true;
            this.field_text.Location = new System.Drawing.Point(149, 117);
            this.field_text.Name = "field_text";
            this.field_text.Size = new System.Drawing.Size(57, 24);
            this.field_text.TabIndex = 14;
            this.field_text.TabStop = true;
            this.field_text.Text = "Text";
            this.field_text.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(2, 371);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 20);
            this.label6.TabIndex = 12;
            this.label6.Text = "Location";
            // 
            // field_y
            // 
            this.field_y.Location = new System.Drawing.Point(156, 369);
            this.field_y.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.field_y.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.field_y.Name = "field_y";
            this.field_y.Size = new System.Drawing.Size(77, 26);
            this.field_y.TabIndex = 11;
            this.field_y.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // field_x
            // 
            this.field_x.Location = new System.Drawing.Point(73, 369);
            this.field_x.Maximum = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.field_x.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.field_x.Name = "field_x";
            this.field_x.Size = new System.Drawing.Size(77, 26);
            this.field_x.TabIndex = 10;
            this.field_x.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 283);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 20);
            this.label5.TabIndex = 9;
            this.label5.Text = "Colour";
            // 
            // field_colour
            // 
            this.field_colour.FormattingEnabled = true;
            this.field_colour.ItemHeight = 20;
            this.field_colour.Items.AddRange(new object[] {
            "Green",
            "Yellow",
            "Blue",
            "Red",
            "White",
            "Turquoise",
            "Pink"});
            this.field_colour.Location = new System.Drawing.Point(73, 274);
            this.field_colour.Name = "field_colour";
            this.field_colour.Size = new System.Drawing.Size(160, 84);
            this.field_colour.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 211);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Length";
            // 
            // field_len
            // 
            this.field_len.Location = new System.Drawing.Point(73, 209);
            this.field_len.Maximum = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.field_len.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.field_len.Name = "field_len";
            this.field_len.Size = new System.Drawing.Size(160, 26);
            this.field_len.TabIndex = 6;
            this.field_len.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // field_output
            // 
            this.field_output.AutoSize = true;
            this.field_output.Location = new System.Drawing.Point(149, 147);
            this.field_output.Name = "field_output";
            this.field_output.Size = new System.Drawing.Size(76, 24);
            this.field_output.TabIndex = 5;
            this.field_output.TabStop = true;
            this.field_output.Text = "Output";
            this.field_output.UseVisualStyleBackColor = true;
            // 
            // field_input
            // 
            this.field_input.AutoSize = true;
            this.field_input.Location = new System.Drawing.Point(62, 147);
            this.field_input.Name = "field_input";
            this.field_input.Size = new System.Drawing.Size(64, 24);
            this.field_input.TabIndex = 4;
            this.field_input.TabStop = true;
            this.field_input.Text = "Input";
            this.field_input.UseVisualStyleBackColor = true;
            // 
            // field_val
            // 
            this.field_val.Location = new System.Drawing.Point(73, 76);
            this.field_val.Name = "field_val";
            this.field_val.Size = new System.Drawing.Size(160, 26);
            this.field_val.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Value";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Name";
            // 
            // field_name
            // 
            this.field_name.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.field_name.Location = new System.Drawing.Point(73, 35);
            this.field_name.MaxLength = 10;
            this.field_name.Name = "field_name";
            this.field_name.Size = new System.Drawing.Size(160, 26);
            this.field_name.TabIndex = 0;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(927, 52);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(239, 28);
            this.comboBox1.TabIndex = 5;
            this.comboBox1.SelectedValueChanged += new System.EventHandler(this.comboBox1_SelectedValueChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(204, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(710, 25);
            this.tabControl1.TabIndex = 6;
            this.tabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_TabIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(702, 0);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "FORMAT1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rec_pagedown);
            this.groupBox2.Controls.Add(this.rec_pageup);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.rec_funcs);
            this.groupBox2.Controls.Add(this.rcd_name);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Location = new System.Drawing.Point(12, 27);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(186, 481);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Record Format";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(105, 446);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 29);
            this.button2.TabIndex = 4;
            this.button2.Text = "Save";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 89);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(107, 20);
            this.label8.TabIndex = 3;
            this.label8.Text = "Function keys";
            // 
            // rec_funcs
            // 
            this.rec_funcs.ColumnWidth = 50;
            this.rec_funcs.FormattingEnabled = true;
            this.rec_funcs.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24"});
            this.rec_funcs.Location = new System.Drawing.Point(19, 112);
            this.rec_funcs.MultiColumn = true;
            this.rec_funcs.Name = "rec_funcs";
            this.rec_funcs.Size = new System.Drawing.Size(152, 130);
            this.rec_funcs.TabIndex = 2;
            // 
            // rcd_name
            // 
            this.rcd_name.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.rcd_name.Location = new System.Drawing.Point(71, 33);
            this.rcd_name.MaxLength = 10;
            this.rcd_name.Name = "rcd_name";
            this.rcd_name.Size = new System.Drawing.Size(100, 26);
            this.rcd_name.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 36);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 20);
            this.label7.TabIndex = 0;
            this.label7.Text = "Name";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // rec_pageup
            // 
            this.rec_pageup.AutoSize = true;
            this.rec_pageup.Location = new System.Drawing.Point(61, 248);
            this.rec_pageup.Name = "rec_pageup";
            this.rec_pageup.Size = new System.Drawing.Size(90, 24);
            this.rec_pageup.TabIndex = 5;
            this.rec_pageup.Text = "Page Up";
            this.rec_pageup.UseVisualStyleBackColor = true;
            // 
            // rec_pagedown
            // 
            this.rec_pagedown.AutoSize = true;
            this.rec_pagedown.Location = new System.Drawing.Point(61, 277);
            this.rec_pagedown.Name = "rec_pagedown";
            this.rec_pagedown.Size = new System.Drawing.Size(110, 24);
            this.rec_pagedown.TabIndex = 6;
            this.rec_pagedown.Text = "Page Down";
            this.rec_pagedown.UseVisualStyleBackColor = true;
            // 
            // field_number
            // 
            this.field_number.AutoSize = true;
            this.field_number.Location = new System.Drawing.Point(62, 176);
            this.field_number.Name = "field_number";
            this.field_number.Size = new System.Drawing.Size(84, 24);
            this.field_number.TabIndex = 18;
            this.field_number.Text = "Number";
            this.field_number.UseVisualStyleBackColor = true;
            // 
            // field_dec
            // 
            this.field_dec.Location = new System.Drawing.Point(89, 242);
            this.field_dec.Maximum = new decimal(new int[] {
            63,
            0,
            0,
            0});
            this.field_dec.Name = "field_dec";
            this.field_dec.Size = new System.Drawing.Size(144, 26);
            this.field_dec.TabIndex = 19;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 245);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 20);
            this.label1.TabIndex = 20;
            this.label1.Text = "Decimals";
            // 
            // dspfEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1175, 518);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.screen);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "dspfEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Display File Edit";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.field_y)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.field_x)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.field_len)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.field_dec)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel screen;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox field_name;
        private System.Windows.Forms.RadioButton field_output;
        private System.Windows.Forms.RadioButton field_input;
        private System.Windows.Forms.TextBox field_val;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown field_len;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox field_colour;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown field_y;
        private System.Windows.Forms.NumericUpDown field_x;
        private System.Windows.Forms.RadioButton field_text;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ToolStripMenuItem recordFormatToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox rcd_name;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckedListBox rec_funcs;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.RadioButton field_both;
        private System.Windows.Forms.RadioButton field_hidden;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.CheckBox rec_pagedown;
        private System.Windows.Forms.CheckBox rec_pageup;
        private System.Windows.Forms.CheckBox field_number;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown field_dec;
    }
}