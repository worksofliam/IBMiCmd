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
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.field_text = new System.Windows.Forms.RadioButton();
            this.field_save = new System.Windows.Forms.Button();
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
            this.screen.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.field_y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.field_x)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.field_len)).BeginInit();
            this.SuspendLayout();
            // 
            // screen
            // 
            this.screen.BackColor = System.Drawing.Color.Black;
            this.screen.Controls.Add(this.label1);
            this.screen.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.screen.ForeColor = System.Drawing.Color.Lime;
            this.screen.Location = new System.Drawing.Point(12, 42);
            this.screen.Name = "screen";
            this.screen.Size = new System.Drawing.Size(640, 456);
            this.screen.TabIndex = 1;
            this.screen.MouseClick += new System.Windows.Forms.MouseEventHandler(this.screen_MouseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(639, 456);
            this.label1.TabIndex = 0;
            this.label1.Text = "1234567890123456789012345678901234567890123456789012345678901234567890\r\n2\r\n3\r\n4\r\n" +
    "5\r\n6\r\n7\r\n8\r\n9\r\n0\r\n1\r\n2\r\n3\r\n4\r\n5\r\n6\r\n7\r\n8\r\n9\r\n0\r\n1\r\n2\r\n3\r\n4";
            this.label1.Visible = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.insertToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(909, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // insertToolStripMenuItem
            // 
            this.insertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textToolStripMenuItem});
            this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
            this.insertToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.insertToolStripMenuItem.Text = "Insert";
            // 
            // textToolStripMenuItem
            // 
            this.textToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.inputToolStripMenuItem,
            this.outputToolStripMenuItem});
            this.textToolStripMenuItem.Name = "textToolStripMenuItem";
            this.textToolStripMenuItem.Size = new System.Drawing.Size(95, 22);
            this.textToolStripMenuItem.Text = "Text";
            // 
            // inputToolStripMenuItem
            // 
            this.inputToolStripMenuItem.Name = "inputToolStripMenuItem";
            this.inputToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.inputToolStripMenuItem.Text = "Input";
            this.inputToolStripMenuItem.Click += new System.EventHandler(this.inputToolStripMenuItem_Click);
            // 
            // outputToolStripMenuItem
            // 
            this.outputToolStripMenuItem.Name = "outputToolStripMenuItem";
            this.outputToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.outputToolStripMenuItem.Text = "Output";
            this.outputToolStripMenuItem.Click += new System.EventHandler(this.outputToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.field_text);
            this.groupBox1.Controls.Add(this.field_save);
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
            this.groupBox1.Location = new System.Drawing.Point(658, 42);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(239, 455);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Selected";
            this.groupBox1.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 334);
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
            this.field_text.Location = new System.Drawing.Point(168, 117);
            this.field_text.Name = "field_text";
            this.field_text.Size = new System.Drawing.Size(57, 24);
            this.field_text.TabIndex = 14;
            this.field_text.TabStop = true;
            this.field_text.Text = "Text";
            this.field_text.UseVisualStyleBackColor = true;
            // 
            // field_save
            // 
            this.field_save.Location = new System.Drawing.Point(109, 334);
            this.field_save.Name = "field_save";
            this.field_save.Size = new System.Drawing.Size(124, 29);
            this.field_save.TabIndex = 13;
            this.field_save.Text = "Update / Save";
            this.field_save.UseVisualStyleBackColor = true;
            this.field_save.Click += new System.EventHandler(this.field_save_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(2, 295);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 20);
            this.label6.TabIndex = 12;
            this.label6.Text = "Location";
            // 
            // field_y
            // 
            this.field_y.Location = new System.Drawing.Point(156, 293);
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
            this.field_x.Location = new System.Drawing.Point(73, 293);
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
            this.label5.Location = new System.Drawing.Point(11, 207);
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
            "Red"});
            this.field_colour.Location = new System.Drawing.Point(73, 198);
            this.field_colour.Name = "field_colour";
            this.field_colour.Size = new System.Drawing.Size(160, 84);
            this.field_colour.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 156);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Length";
            // 
            // field_len
            // 
            this.field_len.Location = new System.Drawing.Point(73, 154);
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
            this.field_output.Location = new System.Drawing.Point(85, 117);
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
            this.field_input.Location = new System.Drawing.Point(15, 117);
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
            this.field_name.Location = new System.Drawing.Point(73, 35);
            this.field_name.MaxLength = 10;
            this.field_name.Name = "field_name";
            this.field_name.Size = new System.Drawing.Size(160, 26);
            this.field_name.TabIndex = 0;
            // 
            // dspfEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(909, 510);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.screen);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "dspfEdit";
            this.Text = "Display File Edit";
            this.screen.ResumeLayout(false);
            this.screen.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.field_y)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.field_x)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.field_len)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel screen;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inputToolStripMenuItem;
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
        private System.Windows.Forms.ToolStripMenuItem outputToolStripMenuItem;
        private System.Windows.Forms.Button field_save;
        private System.Windows.Forms.RadioButton field_text;
        private System.Windows.Forms.Button button1;
    }
}