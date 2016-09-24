namespace ControllerConfigGenerator
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
            this.propertyGridDriver = new System.Windows.Forms.PropertyGrid();
            this.buttonGenerate = new System.Windows.Forms.Button();
            this.listBoxDrivers = new System.Windows.Forms.ListBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.propertyGridAdded = new System.Windows.Forms.PropertyGrid();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelAlgorithm
            // 
            this.labelAlgorithm.AutoSize = true;
            this.labelAlgorithm.Location = new System.Drawing.Point(9, 382);
            this.labelAlgorithm.Name = "labelAlgorithm";
            this.labelAlgorithm.Size = new System.Drawing.Size(50, 13);
            this.labelAlgorithm.TabIndex = 0;
            this.labelAlgorithm.Text = "Algorithm";
            // 
            // comboBoxAlgorithm
            // 
            this.comboBoxAlgorithm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxAlgorithm.FormattingEnabled = true;
            this.comboBoxAlgorithm.Location = new System.Drawing.Point(68, 382);
            this.comboBoxAlgorithm.Name = "comboBoxAlgorithm";
            this.comboBoxAlgorithm.Size = new System.Drawing.Size(414, 21);
            this.comboBoxAlgorithm.TabIndex = 1;
            this.comboBoxAlgorithm.SelectedIndexChanged += new System.EventHandler(this.comboBoxAlgorithm_SelectedIndexChanged);
            // 
            // propertyGridDriver
            // 
            this.propertyGridDriver.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGridDriver.Location = new System.Drawing.Point(15, 409);
            this.propertyGridDriver.Name = "propertyGridDriver";
            this.propertyGridDriver.Size = new System.Drawing.Size(467, 331);
            this.propertyGridDriver.TabIndex = 6;
            // 
            // buttonGenerate
            // 
            this.buttonGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonGenerate.Location = new System.Drawing.Point(407, 746);
            this.buttonGenerate.Name = "buttonGenerate";
            this.buttonGenerate.Size = new System.Drawing.Size(75, 23);
            this.buttonGenerate.TabIndex = 10;
            this.buttonGenerate.Text = "Generate";
            this.buttonGenerate.UseVisualStyleBackColor = true;
            this.buttonGenerate.Click += new System.EventHandler(this.buttonGenerate_Click);
            // 
            // listBoxDrivers
            // 
            this.listBoxDrivers.FormattingEnabled = true;
            this.listBoxDrivers.Location = new System.Drawing.Point(15, 42);
            this.listBoxDrivers.Name = "listBoxDrivers";
            this.listBoxDrivers.Size = new System.Drawing.Size(467, 121);
            this.listBoxDrivers.TabIndex = 11;
            this.listBoxDrivers.SelectedIndexChanged += new System.EventHandler(this.listBoxDrivers_SelectedIndexChanged);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAdd.Location = new System.Drawing.Point(326, 746);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 12;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 747);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Name";
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxName.Location = new System.Drawing.Point(57, 746);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(263, 20);
            this.textBoxName.TabIndex = 14;
            // 
            // propertyGridAdded
            // 
            this.propertyGridAdded.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGridAdded.Location = new System.Drawing.Point(12, 169);
            this.propertyGridAdded.Name = "propertyGridAdded";
            this.propertyGridAdded.Size = new System.Drawing.Size(467, 203);
            this.propertyGridAdded.TabIndex = 15;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Location = new System.Drawing.Point(404, 169);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(75, 23);
            this.buttonRemove.TabIndex = 16;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(13, 13);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(75, 23);
            this.buttonLoad.TabIndex = 17;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // FormDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 781);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.propertyGridAdded);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.listBoxDrivers);
            this.Controls.Add(this.buttonGenerate);
            this.Controls.Add(this.propertyGridDriver);
            this.Controls.Add(this.comboBoxAlgorithm);
            this.Controls.Add(this.labelAlgorithm);
            this.Name = "FormDemo";
            this.Text = "Starfield Display Driver";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelAlgorithm;
        private System.Windows.Forms.ComboBox comboBoxAlgorithm;
        private System.Windows.Forms.PropertyGrid propertyGridDriver;
        private System.Windows.Forms.Button buttonGenerate;
        private System.Windows.Forms.ListBox listBoxDrivers;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.PropertyGrid propertyGridAdded;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonLoad;
    }
}

