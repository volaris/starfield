namespace Ambient
{
    partial class FormDemo
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
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.labelRenderSpeed = new System.Windows.Forms.Label();
            this.labelStarfield = new System.Windows.Forms.Label();
            this.comboBoxStarfield = new System.Windows.Forms.ComboBox();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.labelEndpoint = new System.Windows.Forms.Label();
            this.labelBrightness = new System.Windows.Forms.Label();
            this.trackBarBrightness = new System.Windows.Forms.TrackBar();
            this.buttonReconnect = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.labelAlgorithm = new System.Windows.Forms.Label();
            this.textBoxAlgorithm = new System.Windows.Forms.TextBox();
            this.listBoxDrivers = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonSequential = new System.Windows.Forms.RadioButton();
            this.radioButtonRandom = new System.Windows.Forms.RadioButton();
            this.radioButtonManual = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBrightness)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.trackBar1.Location = new System.Drawing.Point(94, 421);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(388, 45);
            this.trackBar1.TabIndex = 2;
            this.trackBar1.Value = 10;
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // labelRenderSpeed
            // 
            this.labelRenderSpeed.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelRenderSpeed.AutoSize = true;
            this.labelRenderSpeed.Location = new System.Drawing.Point(12, 430);
            this.labelRenderSpeed.Name = "labelRenderSpeed";
            this.labelRenderSpeed.Size = new System.Drawing.Size(76, 13);
            this.labelRenderSpeed.TabIndex = 3;
            this.labelRenderSpeed.Text = "Render Speed";
            // 
            // labelStarfield
            // 
            this.labelStarfield.AutoSize = true;
            this.labelStarfield.Location = new System.Drawing.Point(12, 16);
            this.labelStarfield.Name = "labelStarfield";
            this.labelStarfield.Size = new System.Drawing.Size(45, 13);
            this.labelStarfield.TabIndex = 4;
            this.labelStarfield.Text = "Starfield";
            // 
            // comboBoxStarfield
            // 
            this.comboBoxStarfield.FormattingEnabled = true;
            this.comboBoxStarfield.Location = new System.Drawing.Point(63, 13);
            this.comboBoxStarfield.Name = "comboBoxStarfield";
            this.comboBoxStarfield.Size = new System.Drawing.Size(193, 21);
            this.comboBoxStarfield.TabIndex = 5;
            this.comboBoxStarfield.SelectedIndexChanged += new System.EventHandler(this.comboBoxStarfield_SelectedIndexChanged);
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(262, 13);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(84, 20);
            this.textBoxIP.TabIndex = 7;
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(368, 13);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(33, 20);
            this.textBoxPort.TabIndex = 8;
            // 
            // labelEndpoint
            // 
            this.labelEndpoint.AutoSize = true;
            this.labelEndpoint.Location = new System.Drawing.Point(352, 17);
            this.labelEndpoint.Name = "labelEndpoint";
            this.labelEndpoint.Size = new System.Drawing.Size(10, 13);
            this.labelEndpoint.TabIndex = 9;
            this.labelEndpoint.Text = ":";
            // 
            // labelBrightness
            // 
            this.labelBrightness.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelBrightness.AutoSize = true;
            this.labelBrightness.Location = new System.Drawing.Point(12, 481);
            this.labelBrightness.Name = "labelBrightness";
            this.labelBrightness.Size = new System.Drawing.Size(56, 13);
            this.labelBrightness.TabIndex = 11;
            this.labelBrightness.Text = "Brightness";
            // 
            // trackBarBrightness
            // 
            this.trackBarBrightness.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.trackBarBrightness.Location = new System.Drawing.Point(74, 472);
            this.trackBarBrightness.Maximum = 100;
            this.trackBarBrightness.Name = "trackBarBrightness";
            this.trackBarBrightness.Size = new System.Drawing.Size(408, 45);
            this.trackBarBrightness.TabIndex = 12;
            this.trackBarBrightness.Value = 100;
            this.trackBarBrightness.ValueChanged += new System.EventHandler(this.trackBarBrightness_ValueChanged);
            // 
            // buttonReconnect
            // 
            this.buttonReconnect.Location = new System.Drawing.Point(407, 11);
            this.buttonReconnect.Name = "buttonReconnect";
            this.buttonReconnect.Size = new System.Drawing.Size(75, 23);
            this.buttonReconnect.TabIndex = 13;
            this.buttonReconnect.Text = "Reconnect";
            this.buttonReconnect.UseVisualStyleBackColor = true;
            this.buttonReconnect.Click += new System.EventHandler(this.buttonReconnect_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonNext.Location = new System.Drawing.Point(407, 393);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(75, 23);
            this.buttonNext.TabIndex = 15;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // labelAlgorithm
            // 
            this.labelAlgorithm.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelAlgorithm.AutoSize = true;
            this.labelAlgorithm.Location = new System.Drawing.Point(12, 397);
            this.labelAlgorithm.Name = "labelAlgorithm";
            this.labelAlgorithm.Size = new System.Drawing.Size(50, 13);
            this.labelAlgorithm.TabIndex = 0;
            this.labelAlgorithm.Text = "Algorithm";
            // 
            // textBoxAlgorithm
            // 
            this.textBoxAlgorithm.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.textBoxAlgorithm.Location = new System.Drawing.Point(63, 395);
            this.textBoxAlgorithm.Name = "textBoxAlgorithm";
            this.textBoxAlgorithm.ReadOnly = true;
            this.textBoxAlgorithm.Size = new System.Drawing.Size(338, 20);
            this.textBoxAlgorithm.TabIndex = 14;
            // 
            // listBoxDrivers
            // 
            this.listBoxDrivers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.listBoxDrivers.FormattingEnabled = true;
            this.listBoxDrivers.Location = new System.Drawing.Point(12, 45);
            this.listBoxDrivers.Name = "listBoxDrivers";
            this.listBoxDrivers.Size = new System.Drawing.Size(470, 277);
            this.listBoxDrivers.TabIndex = 16;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.groupBox1.Controls.Add(this.radioButtonManual);
            this.groupBox1.Controls.Add(this.radioButtonRandom);
            this.groupBox1.Controls.Add(this.radioButtonSequential);
            this.groupBox1.Location = new System.Drawing.Point(12, 343);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(470, 44);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Driver Order";
            // 
            // radioButtonSequential
            // 
            this.radioButtonSequential.AutoSize = true;
            this.radioButtonSequential.Checked = true;
            this.radioButtonSequential.Location = new System.Drawing.Point(6, 19);
            this.radioButtonSequential.Name = "radioButtonSequential";
            this.radioButtonSequential.Size = new System.Drawing.Size(75, 17);
            this.radioButtonSequential.TabIndex = 0;
            this.radioButtonSequential.TabStop = true;
            this.radioButtonSequential.Text = "Sequential";
            this.radioButtonSequential.UseVisualStyleBackColor = true;
            // 
            // radioButtonRandom
            // 
            this.radioButtonRandom.AutoSize = true;
            this.radioButtonRandom.Location = new System.Drawing.Point(87, 19);
            this.radioButtonRandom.Name = "radioButtonRandom";
            this.radioButtonRandom.Size = new System.Drawing.Size(65, 17);
            this.radioButtonRandom.TabIndex = 1;
            this.radioButtonRandom.Text = "Random";
            this.radioButtonRandom.UseVisualStyleBackColor = true;
            // 
            // radioButtonManual
            // 
            this.radioButtonManual.AutoSize = true;
            this.radioButtonManual.Location = new System.Drawing.Point(158, 19);
            this.radioButtonManual.Name = "radioButtonManual";
            this.radioButtonManual.Size = new System.Drawing.Size(60, 17);
            this.radioButtonManual.TabIndex = 2;
            this.radioButtonManual.Text = "Manual";
            this.radioButtonManual.UseVisualStyleBackColor = true;
            // 
            // FormDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 516);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listBoxDrivers);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.textBoxAlgorithm);
            this.Controls.Add(this.buttonReconnect);
            this.Controls.Add(this.trackBarBrightness);
            this.Controls.Add(this.labelBrightness);
            this.Controls.Add(this.labelEndpoint);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.textBoxIP);
            this.Controls.Add(this.comboBoxStarfield);
            this.Controls.Add(this.labelStarfield);
            this.Controls.Add(this.labelRenderSpeed);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.labelAlgorithm);
            this.Name = "FormDemo";
            this.Text = "Starfield Display Driver";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBrightness)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label labelRenderSpeed;
        private System.Windows.Forms.Label labelStarfield;
        private System.Windows.Forms.ComboBox comboBoxStarfield;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Label labelEndpoint;
        private System.Windows.Forms.Label labelBrightness;
        private System.Windows.Forms.TrackBar trackBarBrightness;
        private System.Windows.Forms.Button buttonReconnect;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label labelAlgorithm;
        private System.Windows.Forms.TextBox textBoxAlgorithm;
        private System.Windows.Forms.ListBox listBoxDrivers;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonRandom;
        private System.Windows.Forms.RadioButton radioButtonSequential;
        private System.Windows.Forms.RadioButton radioButtonManual;
    }
}

