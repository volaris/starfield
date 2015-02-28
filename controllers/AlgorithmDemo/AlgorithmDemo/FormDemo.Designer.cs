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
            this.labelAlgorithm = new System.Windows.Forms.Label();
            this.comboBoxAlgorithm = new System.Windows.Forms.ComboBox();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.labelRenderSpeed = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelAlgorithm
            // 
            this.labelAlgorithm.AutoSize = true;
            this.labelAlgorithm.Location = new System.Drawing.Point(12, 15);
            this.labelAlgorithm.Name = "labelAlgorithm";
            this.labelAlgorithm.Size = new System.Drawing.Size(50, 13);
            this.labelAlgorithm.TabIndex = 0;
            this.labelAlgorithm.Text = "Algorithm";
            // 
            // comboBoxAlgorithm
            // 
            this.comboBoxAlgorithm.FormattingEnabled = true;
            this.comboBoxAlgorithm.Location = new System.Drawing.Point(68, 12);
            this.comboBoxAlgorithm.Name = "comboBoxAlgorithm";
            this.comboBoxAlgorithm.Size = new System.Drawing.Size(277, 21);
            this.comboBoxAlgorithm.TabIndex = 1;
            this.comboBoxAlgorithm.SelectedIndexChanged += new System.EventHandler(this.comboBoxAlgorithm_SelectedIndexChanged);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(94, 39);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(251, 45);
            this.trackBar1.TabIndex = 2;
            // 
            // labelRenderSpeed
            // 
            this.labelRenderSpeed.AutoSize = true;
            this.labelRenderSpeed.Location = new System.Drawing.Point(12, 48);
            this.labelRenderSpeed.Name = "labelRenderSpeed";
            this.labelRenderSpeed.Size = new System.Drawing.Size(76, 13);
            this.labelRenderSpeed.TabIndex = 3;
            this.labelRenderSpeed.Text = "Render Speed";
            // 
            // FormDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 204);
            this.Controls.Add(this.labelRenderSpeed);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.comboBoxAlgorithm);
            this.Controls.Add(this.labelAlgorithm);
            this.Name = "FormDemo";
            this.Text = "Starfield Display Driver";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelAlgorithm;
        private System.Windows.Forms.ComboBox comboBoxAlgorithm;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label labelRenderSpeed;
    }
}

