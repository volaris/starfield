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
            this.labelAlgorithm = new System.Windows.Forms.Label();
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
            this.textBoxAlgorithm = new System.Windows.Forms.TextBox();
            this.buttonNext = new System.Windows.Forms.Button();
            this.checkBoxSoundResponsive = new System.Windows.Forms.CheckBox();
            this.checkBoxAmbientInteractive = new System.Windows.Forms.CheckBox();
            this.buttonReload = new System.Windows.Forms.Button();
            this.checkBoxInteractive = new System.Windows.Forms.CheckBox();
            this.checkBoxAmbient = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBrightness)).BeginInit();
            this.SuspendLayout();
            // 
            // labelAlgorithm
            // 
            this.labelAlgorithm.AutoSize = true;
            this.labelAlgorithm.Location = new System.Drawing.Point(24, 83);
            this.labelAlgorithm.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelAlgorithm.Name = "labelAlgorithm";
            this.labelAlgorithm.Size = new System.Drawing.Size(102, 25);
            this.labelAlgorithm.TabIndex = 0;
            this.labelAlgorithm.Text = "Algorithm";
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar1.Location = new System.Drawing.Point(188, 129);
            this.trackBar1.Margin = new System.Windows.Forms.Padding(6);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(776, 90);
            this.trackBar1.TabIndex = 2;
            this.trackBar1.Value = 10;
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // labelRenderSpeed
            // 
            this.labelRenderSpeed.AutoSize = true;
            this.labelRenderSpeed.Location = new System.Drawing.Point(24, 146);
            this.labelRenderSpeed.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelRenderSpeed.Name = "labelRenderSpeed";
            this.labelRenderSpeed.Size = new System.Drawing.Size(150, 25);
            this.labelRenderSpeed.TabIndex = 3;
            this.labelRenderSpeed.Text = "Render Speed";
            // 
            // labelStarfield
            // 
            this.labelStarfield.AutoSize = true;
            this.labelStarfield.Location = new System.Drawing.Point(24, 31);
            this.labelStarfield.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelStarfield.Name = "labelStarfield";
            this.labelStarfield.Size = new System.Drawing.Size(91, 25);
            this.labelStarfield.TabIndex = 4;
            this.labelStarfield.Text = "Starfield";
            // 
            // comboBoxStarfield
            // 
            this.comboBoxStarfield.FormattingEnabled = true;
            this.comboBoxStarfield.Location = new System.Drawing.Point(126, 25);
            this.comboBoxStarfield.Margin = new System.Windows.Forms.Padding(6);
            this.comboBoxStarfield.Name = "comboBoxStarfield";
            this.comboBoxStarfield.Size = new System.Drawing.Size(382, 33);
            this.comboBoxStarfield.TabIndex = 5;
            this.comboBoxStarfield.SelectedIndexChanged += new System.EventHandler(this.comboBoxStarfield_SelectedIndexChanged);
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(524, 25);
            this.textBoxIP.Margin = new System.Windows.Forms.Padding(6);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(164, 31);
            this.textBoxIP.TabIndex = 7;
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(736, 25);
            this.textBoxPort.Margin = new System.Windows.Forms.Padding(6);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(62, 31);
            this.textBoxPort.TabIndex = 8;
            // 
            // labelEndpoint
            // 
            this.labelEndpoint.AutoSize = true;
            this.labelEndpoint.Location = new System.Drawing.Point(704, 33);
            this.labelEndpoint.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelEndpoint.Name = "labelEndpoint";
            this.labelEndpoint.Size = new System.Drawing.Size(18, 25);
            this.labelEndpoint.TabIndex = 9;
            this.labelEndpoint.Text = ":";
            // 
            // labelBrightness
            // 
            this.labelBrightness.AutoSize = true;
            this.labelBrightness.Location = new System.Drawing.Point(24, 244);
            this.labelBrightness.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelBrightness.Name = "labelBrightness";
            this.labelBrightness.Size = new System.Drawing.Size(114, 25);
            this.labelBrightness.TabIndex = 11;
            this.labelBrightness.Text = "Brightness";
            // 
            // trackBarBrightness
            // 
            this.trackBarBrightness.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarBrightness.Location = new System.Drawing.Point(148, 227);
            this.trackBarBrightness.Margin = new System.Windows.Forms.Padding(6);
            this.trackBarBrightness.Maximum = 100;
            this.trackBarBrightness.Name = "trackBarBrightness";
            this.trackBarBrightness.Size = new System.Drawing.Size(816, 90);
            this.trackBarBrightness.TabIndex = 12;
            this.trackBarBrightness.Value = 100;
            this.trackBarBrightness.ValueChanged += new System.EventHandler(this.trackBarBrightness_ValueChanged);
            // 
            // buttonReconnect
            // 
            this.buttonReconnect.Location = new System.Drawing.Point(814, 21);
            this.buttonReconnect.Margin = new System.Windows.Forms.Padding(6);
            this.buttonReconnect.Name = "buttonReconnect";
            this.buttonReconnect.Size = new System.Drawing.Size(150, 44);
            this.buttonReconnect.TabIndex = 13;
            this.buttonReconnect.Text = "Reconnect";
            this.buttonReconnect.UseVisualStyleBackColor = true;
            this.buttonReconnect.Click += new System.EventHandler(this.buttonReconnect_Click);
            // 
            // textBoxAlgorithm
            // 
            this.textBoxAlgorithm.Location = new System.Drawing.Point(126, 79);
            this.textBoxAlgorithm.Margin = new System.Windows.Forms.Padding(6);
            this.textBoxAlgorithm.Name = "textBoxAlgorithm";
            this.textBoxAlgorithm.ReadOnly = true;
            this.textBoxAlgorithm.Size = new System.Drawing.Size(672, 31);
            this.textBoxAlgorithm.TabIndex = 14;
            // 
            // buttonNext
            // 
            this.buttonNext.Location = new System.Drawing.Point(814, 75);
            this.buttonNext.Margin = new System.Windows.Forms.Padding(6);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(150, 44);
            this.buttonNext.TabIndex = 15;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // checkBoxSoundResponsive
            // 
            this.checkBoxSoundResponsive.AutoSize = true;
            this.checkBoxSoundResponsive.Location = new System.Drawing.Point(29, 304);
            this.checkBoxSoundResponsive.Name = "checkBoxSoundResponsive";
            this.checkBoxSoundResponsive.Size = new System.Drawing.Size(225, 29);
            this.checkBoxSoundResponsive.TabIndex = 16;
            this.checkBoxSoundResponsive.Text = "Sound Responsive";
            this.checkBoxSoundResponsive.UseVisualStyleBackColor = true;
            // 
            // checkBoxAmbientInteractive
            // 
            this.checkBoxAmbientInteractive.AutoSize = true;
            this.checkBoxAmbientInteractive.Location = new System.Drawing.Point(29, 339);
            this.checkBoxAmbientInteractive.Name = "checkBoxAmbientInteractive";
            this.checkBoxAmbientInteractive.Size = new System.Drawing.Size(227, 29);
            this.checkBoxAmbientInteractive.TabIndex = 17;
            this.checkBoxAmbientInteractive.Text = "Ambient Interactive";
            this.checkBoxAmbientInteractive.UseVisualStyleBackColor = true;
            // 
            // buttonReload
            // 
            this.buttonReload.Location = new System.Drawing.Point(442, 304);
            this.buttonReload.Name = "buttonReload";
            this.buttonReload.Size = new System.Drawing.Size(180, 64);
            this.buttonReload.TabIndex = 18;
            this.buttonReload.Text = "Reload Drivers";
            this.buttonReload.UseVisualStyleBackColor = true;
            this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
            // 
            // checkBoxInteractive
            // 
            this.checkBoxInteractive.AutoSize = true;
            this.checkBoxInteractive.Location = new System.Drawing.Point(281, 304);
            this.checkBoxInteractive.Name = "checkBoxInteractive";
            this.checkBoxInteractive.Size = new System.Drawing.Size(143, 29);
            this.checkBoxInteractive.TabIndex = 19;
            this.checkBoxInteractive.Text = "Interactive";
            this.checkBoxInteractive.UseVisualStyleBackColor = true;
            // 
            // checkBoxAmbient
            // 
            this.checkBoxAmbient.AutoSize = true;
            this.checkBoxAmbient.Checked = true;
            this.checkBoxAmbient.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAmbient.Location = new System.Drawing.Point(281, 339);
            this.checkBoxAmbient.Name = "checkBoxAmbient";
            this.checkBoxAmbient.Size = new System.Drawing.Size(122, 29);
            this.checkBoxAmbient.TabIndex = 20;
            this.checkBoxAmbient.Text = "Ambient";
            this.checkBoxAmbient.UseVisualStyleBackColor = true;
            // 
            // FormDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(988, 396);
            this.Controls.Add(this.checkBoxAmbient);
            this.Controls.Add(this.checkBoxInteractive);
            this.Controls.Add(this.buttonReload);
            this.Controls.Add(this.checkBoxAmbientInteractive);
            this.Controls.Add(this.checkBoxSoundResponsive);
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
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "FormDemo";
            this.Text = "Starfield Display Driver";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBrightness)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelAlgorithm;
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
        private System.Windows.Forms.TextBox textBoxAlgorithm;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.CheckBox checkBoxSoundResponsive;
        private System.Windows.Forms.CheckBox checkBoxAmbientInteractive;
        private System.Windows.Forms.Button buttonReload;
        private System.Windows.Forms.CheckBox checkBoxInteractive;
        private System.Windows.Forms.CheckBox checkBoxAmbient;
    }
}

