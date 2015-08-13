namespace AlgorithmDemo
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
            this.labelnteriorAlgorithm = new System.Windows.Forms.Label();
            this.comboBoxInteriorAlgorithm = new System.Windows.Forms.ComboBox();
            this.trackBarRenderSpeed = new System.Windows.Forms.TrackBar();
            this.labelRenderSpeed = new System.Windows.Forms.Label();
            this.labelStarfield = new System.Windows.Forms.Label();
            this.propertyGridInteriorDriver = new System.Windows.Forms.PropertyGrid();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.labelEndpoint = new System.Windows.Forms.Label();
            this.buttonRestart = new System.Windows.Forms.Button();
            this.labelInteriorBrightness = new System.Windows.Forms.Label();
            this.trackBarInteriorBrightness = new System.Windows.Forms.TrackBar();
            this.buttonReconnect = new System.Windows.Forms.Button();
            this.trackBarExteriorBrightness = new System.Windows.Forms.TrackBar();
            this.labelBrightnessExterior = new System.Windows.Forms.Label();
            this.propertyGridExteriorDriver = new System.Windows.Forms.PropertyGrid();
            this.comboBoxExteriorAlgorithm = new System.Windows.Forms.ComboBox();
            this.labelAlgorithmExterior = new System.Windows.Forms.Label();
            this.groupBoxExterior = new System.Windows.Forms.GroupBox();
            this.buttonNextExteriorAlgorithm = new System.Windows.Forms.Button();
            this.textBoxExteriorAlgorithm = new System.Windows.Forms.TextBox();
            this.checkBoxAmbientExterior = new System.Windows.Forms.CheckBox();
            this.groupBoxInterior = new System.Windows.Forms.GroupBox();
            this.buttonNextInteriorAlgorithm = new System.Windows.Forms.Button();
            this.textBoxInteriorAlgorithm = new System.Windows.Forms.TextBox();
            this.checkBoxInteriorAmbient = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRenderSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarInteriorBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarExteriorBrightness)).BeginInit();
            this.groupBoxExterior.SuspendLayout();
            this.groupBoxInterior.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelnteriorAlgorithm
            // 
            this.labelnteriorAlgorithm.AutoSize = true;
            this.labelnteriorAlgorithm.Location = new System.Drawing.Point(11, 25);
            this.labelnteriorAlgorithm.Name = "labelnteriorAlgorithm";
            this.labelnteriorAlgorithm.Size = new System.Drawing.Size(50, 13);
            this.labelnteriorAlgorithm.TabIndex = 0;
            this.labelnteriorAlgorithm.Text = "Algorithm";
            // 
            // comboBoxInteriorAlgorithm
            // 
            this.comboBoxInteriorAlgorithm.FormattingEnabled = true;
            this.comboBoxInteriorAlgorithm.Location = new System.Drawing.Point(67, 22);
            this.comboBoxInteriorAlgorithm.Name = "comboBoxInteriorAlgorithm";
            this.comboBoxInteriorAlgorithm.Size = new System.Drawing.Size(344, 21);
            this.comboBoxInteriorAlgorithm.TabIndex = 1;
            this.comboBoxInteriorAlgorithm.SelectedIndexChanged += new System.EventHandler(this.comboBoxInteriorAlgorithm_SelectedIndexChanged);
            // 
            // trackBarRenderSpeed
            // 
            this.trackBarRenderSpeed.Location = new System.Drawing.Point(443, 12);
            this.trackBarRenderSpeed.Name = "trackBarRenderSpeed";
            this.trackBarRenderSpeed.Size = new System.Drawing.Size(573, 45);
            this.trackBarRenderSpeed.TabIndex = 2;
            this.trackBarRenderSpeed.Value = 10;
            this.trackBarRenderSpeed.ValueChanged += new System.EventHandler(this.trackBarRenderSpeed_ValueChanged);
            // 
            // labelRenderSpeed
            // 
            this.labelRenderSpeed.AutoSize = true;
            this.labelRenderSpeed.Location = new System.Drawing.Point(361, 16);
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
            this.labelStarfield.Size = new System.Drawing.Size(38, 13);
            this.labelStarfield.TabIndex = 4;
            this.labelStarfield.Text = "Server";
            // 
            // propertyGridInteriorDriver
            // 
            this.propertyGridInteriorDriver.Location = new System.Drawing.Point(14, 109);
            this.propertyGridInteriorDriver.Name = "propertyGridInteriorDriver";
            this.propertyGridInteriorDriver.Size = new System.Drawing.Size(467, 253);
            this.propertyGridInteriorDriver.TabIndex = 6;
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(56, 14);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(164, 20);
            this.textBoxIP.TabIndex = 7;
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(241, 14);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(33, 20);
            this.textBoxPort.TabIndex = 8;
            // 
            // labelEndpoint
            // 
            this.labelEndpoint.AutoSize = true;
            this.labelEndpoint.Location = new System.Drawing.Point(225, 18);
            this.labelEndpoint.Name = "labelEndpoint";
            this.labelEndpoint.Size = new System.Drawing.Size(10, 13);
            this.labelEndpoint.TabIndex = 9;
            this.labelEndpoint.Text = ":";
            // 
            // buttonRestart
            // 
            this.buttonRestart.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonRestart.Location = new System.Drawing.Point(941, 449);
            this.buttonRestart.Name = "buttonRestart";
            this.buttonRestart.Size = new System.Drawing.Size(75, 23);
            this.buttonRestart.TabIndex = 10;
            this.buttonRestart.Text = "Restart";
            this.buttonRestart.UseVisualStyleBackColor = true;
            this.buttonRestart.Click += new System.EventHandler(this.buttonRestart_Click);
            // 
            // labelInteriorBrightness
            // 
            this.labelInteriorBrightness.AutoSize = true;
            this.labelInteriorBrightness.Location = new System.Drawing.Point(11, 64);
            this.labelInteriorBrightness.Name = "labelInteriorBrightness";
            this.labelInteriorBrightness.Size = new System.Drawing.Size(56, 13);
            this.labelInteriorBrightness.TabIndex = 11;
            this.labelInteriorBrightness.Text = "Brightness";
            // 
            // trackBarInteriorBrightness
            // 
            this.trackBarInteriorBrightness.Location = new System.Drawing.Point(73, 55);
            this.trackBarInteriorBrightness.Maximum = 100;
            this.trackBarInteriorBrightness.Name = "trackBarInteriorBrightness";
            this.trackBarInteriorBrightness.Size = new System.Drawing.Size(408, 45);
            this.trackBarInteriorBrightness.TabIndex = 12;
            this.trackBarInteriorBrightness.Value = 100;
            this.trackBarInteriorBrightness.ValueChanged += new System.EventHandler(this.trackBarInteriorBrightness_ValueChanged);
            // 
            // buttonReconnect
            // 
            this.buttonReconnect.Location = new System.Drawing.Point(280, 12);
            this.buttonReconnect.Name = "buttonReconnect";
            this.buttonReconnect.Size = new System.Drawing.Size(75, 23);
            this.buttonReconnect.TabIndex = 13;
            this.buttonReconnect.Text = "Reconnect";
            this.buttonReconnect.UseVisualStyleBackColor = true;
            this.buttonReconnect.Click += new System.EventHandler(this.buttonReconnect_Click);
            // 
            // trackBarExteriorBrightness
            // 
            this.trackBarExteriorBrightness.Location = new System.Drawing.Point(77, 55);
            this.trackBarExteriorBrightness.Maximum = 100;
            this.trackBarExteriorBrightness.Name = "trackBarExteriorBrightness";
            this.trackBarExteriorBrightness.Size = new System.Drawing.Size(408, 45);
            this.trackBarExteriorBrightness.TabIndex = 20;
            this.trackBarExteriorBrightness.Value = 100;
            this.trackBarExteriorBrightness.Scroll += new System.EventHandler(this.trackBarBrightnessExterior_Scroll);
            // 
            // labelBrightnessExterior
            // 
            this.labelBrightnessExterior.AutoSize = true;
            this.labelBrightnessExterior.Location = new System.Drawing.Point(15, 64);
            this.labelBrightnessExterior.Name = "labelBrightnessExterior";
            this.labelBrightnessExterior.Size = new System.Drawing.Size(56, 13);
            this.labelBrightnessExterior.TabIndex = 19;
            this.labelBrightnessExterior.Text = "Brightness";
            // 
            // propertyGridExteriorDriver
            // 
            this.propertyGridExteriorDriver.Location = new System.Drawing.Point(18, 109);
            this.propertyGridExteriorDriver.Name = "propertyGridExteriorDriver";
            this.propertyGridExteriorDriver.Size = new System.Drawing.Size(467, 253);
            this.propertyGridExteriorDriver.TabIndex = 18;
            // 
            // comboBoxExteriorAlgorithm
            // 
            this.comboBoxExteriorAlgorithm.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBoxExteriorAlgorithm.FormattingEnabled = true;
            this.comboBoxExteriorAlgorithm.Location = new System.Drawing.Point(71, 22);
            this.comboBoxExteriorAlgorithm.Name = "comboBoxExteriorAlgorithm";
            this.comboBoxExteriorAlgorithm.Size = new System.Drawing.Size(344, 21);
            this.comboBoxExteriorAlgorithm.TabIndex = 15;
            this.comboBoxExteriorAlgorithm.SelectedIndexChanged += new System.EventHandler(this.comboBoxAlgorithmExterior_SelectedIndexChanged);
            // 
            // labelAlgorithmExterior
            // 
            this.labelAlgorithmExterior.AutoSize = true;
            this.labelAlgorithmExterior.Location = new System.Drawing.Point(15, 25);
            this.labelAlgorithmExterior.Name = "labelAlgorithmExterior";
            this.labelAlgorithmExterior.Size = new System.Drawing.Size(50, 13);
            this.labelAlgorithmExterior.TabIndex = 14;
            this.labelAlgorithmExterior.Text = "Algorithm";
            // 
            // groupBoxExterior
            // 
            this.groupBoxExterior.Controls.Add(this.buttonNextExteriorAlgorithm);
            this.groupBoxExterior.Controls.Add(this.textBoxExteriorAlgorithm);
            this.groupBoxExterior.Controls.Add(this.checkBoxAmbientExterior);
            this.groupBoxExterior.Controls.Add(this.trackBarExteriorBrightness);
            this.groupBoxExterior.Controls.Add(this.propertyGridExteriorDriver);
            this.groupBoxExterior.Controls.Add(this.labelBrightnessExterior);
            this.groupBoxExterior.Controls.Add(this.labelAlgorithmExterior);
            this.groupBoxExterior.Controls.Add(this.comboBoxExteriorAlgorithm);
            this.groupBoxExterior.Location = new System.Drawing.Point(517, 63);
            this.groupBoxExterior.Name = "groupBoxExterior";
            this.groupBoxExterior.Size = new System.Drawing.Size(499, 380);
            this.groupBoxExterior.TabIndex = 21;
            this.groupBoxExterior.TabStop = false;
            this.groupBoxExterior.Text = "Exterior";
            // 
            // buttonNextExteriorAlgorithm
            // 
            this.buttonNextExteriorAlgorithm.Location = new System.Drawing.Point(340, 20);
            this.buttonNextExteriorAlgorithm.Name = "buttonNextExteriorAlgorithm";
            this.buttonNextExteriorAlgorithm.Size = new System.Drawing.Size(75, 23);
            this.buttonNextExteriorAlgorithm.TabIndex = 23;
            this.buttonNextExteriorAlgorithm.Text = "Next";
            this.buttonNextExteriorAlgorithm.UseVisualStyleBackColor = true;
            this.buttonNextExteriorAlgorithm.Visible = false;
            this.buttonNextExteriorAlgorithm.Click += new System.EventHandler(this.buttonNextExteriorAlgorithm_Click);
            // 
            // textBoxExteriorAlgorithm
            // 
            this.textBoxExteriorAlgorithm.Location = new System.Drawing.Point(71, 22);
            this.textBoxExteriorAlgorithm.Name = "textBoxExteriorAlgorithm";
            this.textBoxExteriorAlgorithm.ReadOnly = true;
            this.textBoxExteriorAlgorithm.Size = new System.Drawing.Size(263, 20);
            this.textBoxExteriorAlgorithm.TabIndex = 22;
            this.textBoxExteriorAlgorithm.Visible = false;
            // 
            // checkBoxAmbientExterior
            // 
            this.checkBoxAmbientExterior.AutoSize = true;
            this.checkBoxAmbientExterior.Location = new System.Drawing.Point(421, 24);
            this.checkBoxAmbientExterior.Name = "checkBoxAmbientExterior";
            this.checkBoxAmbientExterior.Size = new System.Drawing.Size(64, 17);
            this.checkBoxAmbientExterior.TabIndex = 21;
            this.checkBoxAmbientExterior.Text = "Ambient";
            this.checkBoxAmbientExterior.UseVisualStyleBackColor = true;
            this.checkBoxAmbientExterior.CheckedChanged += new System.EventHandler(this.checkBoxAmbientExterior_CheckedChanged);
            // 
            // groupBoxInterior
            // 
            this.groupBoxInterior.Controls.Add(this.buttonNextInteriorAlgorithm);
            this.groupBoxInterior.Controls.Add(this.textBoxInteriorAlgorithm);
            this.groupBoxInterior.Controls.Add(this.checkBoxInteriorAmbient);
            this.groupBoxInterior.Controls.Add(this.labelnteriorAlgorithm);
            this.groupBoxInterior.Controls.Add(this.comboBoxInteriorAlgorithm);
            this.groupBoxInterior.Controls.Add(this.propertyGridInteriorDriver);
            this.groupBoxInterior.Controls.Add(this.labelInteriorBrightness);
            this.groupBoxInterior.Controls.Add(this.trackBarInteriorBrightness);
            this.groupBoxInterior.Location = new System.Drawing.Point(15, 63);
            this.groupBoxInterior.Name = "groupBoxInterior";
            this.groupBoxInterior.Size = new System.Drawing.Size(496, 380);
            this.groupBoxInterior.TabIndex = 22;
            this.groupBoxInterior.TabStop = false;
            this.groupBoxInterior.Text = "Interior";
            // 
            // buttonNextInteriorAlgorithm
            // 
            this.buttonNextInteriorAlgorithm.Location = new System.Drawing.Point(336, 20);
            this.buttonNextInteriorAlgorithm.Name = "buttonNextInteriorAlgorithm";
            this.buttonNextInteriorAlgorithm.Size = new System.Drawing.Size(75, 23);
            this.buttonNextInteriorAlgorithm.TabIndex = 25;
            this.buttonNextInteriorAlgorithm.Text = "Next";
            this.buttonNextInteriorAlgorithm.UseVisualStyleBackColor = true;
            this.buttonNextInteriorAlgorithm.Visible = false;
            this.buttonNextInteriorAlgorithm.Click += new System.EventHandler(this.buttonNextInteriorAlgorithm_Click);
            // 
            // textBoxInteriorAlgorithm
            // 
            this.textBoxInteriorAlgorithm.Location = new System.Drawing.Point(67, 22);
            this.textBoxInteriorAlgorithm.Name = "textBoxInteriorAlgorithm";
            this.textBoxInteriorAlgorithm.ReadOnly = true;
            this.textBoxInteriorAlgorithm.Size = new System.Drawing.Size(263, 20);
            this.textBoxInteriorAlgorithm.TabIndex = 24;
            this.textBoxInteriorAlgorithm.Visible = false;
            // 
            // checkBoxInteriorAmbient
            // 
            this.checkBoxInteriorAmbient.AutoSize = true;
            this.checkBoxInteriorAmbient.Location = new System.Drawing.Point(417, 25);
            this.checkBoxInteriorAmbient.Name = "checkBoxInteriorAmbient";
            this.checkBoxInteriorAmbient.Size = new System.Drawing.Size(64, 17);
            this.checkBoxInteriorAmbient.TabIndex = 22;
            this.checkBoxInteriorAmbient.Text = "Ambient";
            this.checkBoxInteriorAmbient.UseVisualStyleBackColor = true;
            this.checkBoxInteriorAmbient.CheckedChanged += new System.EventHandler(this.checkBoxInteriorAmbient_CheckedChanged);
            // 
            // FormDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1034, 484);
            this.Controls.Add(this.buttonReconnect);
            this.Controls.Add(this.trackBarRenderSpeed);
            this.Controls.Add(this.buttonRestart);
            this.Controls.Add(this.labelRenderSpeed);
            this.Controls.Add(this.labelEndpoint);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.textBoxIP);
            this.Controls.Add(this.labelStarfield);
            this.Controls.Add(this.groupBoxExterior);
            this.Controls.Add(this.groupBoxInterior);
            this.Name = "FormDemo";
            this.Text = "Starfield Display Driver";
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRenderSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarInteriorBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarExteriorBrightness)).EndInit();
            this.groupBoxExterior.ResumeLayout(false);
            this.groupBoxExterior.PerformLayout();
            this.groupBoxInterior.ResumeLayout(false);
            this.groupBoxInterior.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelnteriorAlgorithm;
        private System.Windows.Forms.ComboBox comboBoxInteriorAlgorithm;
        private System.Windows.Forms.TrackBar trackBarRenderSpeed;
        private System.Windows.Forms.Label labelRenderSpeed;
        private System.Windows.Forms.Label labelStarfield;
        private System.Windows.Forms.PropertyGrid propertyGridInteriorDriver;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Label labelEndpoint;
        private System.Windows.Forms.Button buttonRestart;
        private System.Windows.Forms.Label labelInteriorBrightness;
        private System.Windows.Forms.TrackBar trackBarInteriorBrightness;
        private System.Windows.Forms.Button buttonReconnect;
        private System.Windows.Forms.TrackBar trackBarExteriorBrightness;
        private System.Windows.Forms.Label labelBrightnessExterior;
        private System.Windows.Forms.PropertyGrid propertyGridExteriorDriver;
        private System.Windows.Forms.ComboBox comboBoxExteriorAlgorithm;
        private System.Windows.Forms.Label labelAlgorithmExterior;
        private System.Windows.Forms.GroupBox groupBoxExterior;
        private System.Windows.Forms.CheckBox checkBoxAmbientExterior;
        private System.Windows.Forms.GroupBox groupBoxInterior;
        private System.Windows.Forms.CheckBox checkBoxInteriorAmbient;
        private System.Windows.Forms.Button buttonNextExteriorAlgorithm;
        private System.Windows.Forms.TextBox textBoxExteriorAlgorithm;
        private System.Windows.Forms.Button buttonNextInteriorAlgorithm;
        private System.Windows.Forms.TextBox textBoxInteriorAlgorithm;
    }
}

