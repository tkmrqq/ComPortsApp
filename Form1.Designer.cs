namespace ComPortsApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            textBox1 = new TextBox();
            listBox1 = new ListBox();
            comboBoxPorts = new ComboBox();
            comboBoxParity = new ComboBox();
            label1 = new Label();
            comboBoxGroupNumber = new ComboBox();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(12, 12);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(180, 23);
            textBox1.TabIndex = 0;
            textBox1.KeyDown += textBox1_KeyDown;
            // 
            // listBox1
            // 
            listBox1.BackColor = SystemColors.Window;
            listBox1.BorderStyle = BorderStyle.FixedSingle;
            listBox1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 204);
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(518, 12);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(270, 422);
            listBox1.TabIndex = 1;
            // 
            // comboBoxPorts
            // 
            comboBoxPorts.FormattingEnabled = true;
            comboBoxPorts.Location = new Point(225, 12);
            comboBoxPorts.Name = "comboBoxPorts";
            comboBoxPorts.Size = new Size(121, 23);
            comboBoxPorts.TabIndex = 2;
            comboBoxPorts.SelectedIndexChanged += comboBoxPorts_SelectedIndexChanged;
            // 
            // comboBoxParity
            // 
            comboBoxParity.FormattingEnabled = true;
            comboBoxParity.Location = new Point(355, 12);
            comboBoxParity.Name = "comboBoxParity";
            comboBoxParity.Size = new Size(121, 23);
            comboBoxParity.TabIndex = 3;
            comboBoxParity.SelectedIndexChanged += comboBoxParity_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label1.Location = new Point(12, 385);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 4;
            label1.Text = "label1";
            // 
            // comboBoxGroupNumber
            // 
            comboBoxGroupNumber.FormattingEnabled = true;
            comboBoxGroupNumber.Location = new Point(225, 41);
            comboBoxGroupNumber.Name = "comboBoxGroupNumber";
            comboBoxGroupNumber.Size = new Size(121, 23);
            comboBoxGroupNumber.TabIndex = 5;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(comboBoxGroupNumber);
            Controls.Add(label1);
            Controls.Add(comboBoxParity);
            Controls.Add(comboBoxPorts);
            Controls.Add(listBox1);
            Controls.Add(textBox1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private ListBox listBox1;
        private ComboBox comboBoxPorts;
        private ComboBox comboBoxParity;
        private Label label1;
        private ComboBox comboBoxGroupNumber;
    }
}
