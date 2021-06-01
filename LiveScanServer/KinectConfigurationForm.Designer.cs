
namespace KinectServer
{
    partial class KinectConfigurationForm
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
            this.lDepthModeListBox = new System.Windows.Forms.ListBox();
            this.btUpdate = new System.Windows.Forms.Button();
            this.lbDepthModeDetaulsLabel = new System.Windows.Forms.Label();
            this.kinectIDLabel = new System.Windows.Forms.Label();
            this.cbFilterDepthMap = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbFilterDepthMapSize = new System.Windows.Forms.MaskedTextBox();
            this.SuspendLayout();
            // 
            // lDepthModeListBox
            // 
            this.lDepthModeListBox.FormattingEnabled = true;
            this.lDepthModeListBox.Location = new System.Drawing.Point(12, 28);
            this.lDepthModeListBox.Name = "lDepthModeListBox";
            this.lDepthModeListBox.Size = new System.Drawing.Size(178, 95);
            this.lDepthModeListBox.TabIndex = 0;
            this.lDepthModeListBox.SelectedIndexChanged += new System.EventHandler(this.ListBox1_SelectedIndexChanged);
            // 
            // btUpdate
            // 
            this.btUpdate.Location = new System.Drawing.Point(226, 234);
            this.btUpdate.Name = "btUpdate";
            this.btUpdate.Size = new System.Drawing.Size(120, 23);
            this.btUpdate.TabIndex = 1;
            this.btUpdate.Text = "Update";
            this.btUpdate.UseVisualStyleBackColor = true;
            this.btUpdate.Click += new System.EventHandler(this.UpdateButton_Click);
            // 
            // lbDepthModeDetaulsLabel
            // 
            this.lbDepthModeDetaulsLabel.Location = new System.Drawing.Point(196, 28);
            this.lbDepthModeDetaulsLabel.Name = "lbDepthModeDetaulsLabel";
            this.lbDepthModeDetaulsLabel.Size = new System.Drawing.Size(150, 95);
            this.lbDepthModeDetaulsLabel.TabIndex = 2;
            this.lbDepthModeDetaulsLabel.Text = "label1";
            // 
            // kinectIDLabel
            // 
            this.kinectIDLabel.AutoSize = true;
            this.kinectIDLabel.Location = new System.Drawing.Point(13, 9);
            this.kinectIDLabel.Name = "kinectIDLabel";
            this.kinectIDLabel.Size = new System.Drawing.Size(38, 13);
            this.kinectIDLabel.TabIndex = 3;
            this.kinectIDLabel.Text = "label1";
            // 
            // cbFilterDepthMap
            // 
            this.cbFilterDepthMap.AutoSize = true;
            this.cbFilterDepthMap.Location = new System.Drawing.Point(12, 148);
            this.cbFilterDepthMap.Name = "cbFilterDepthMap";
            this.cbFilterDepthMap.Size = new System.Drawing.Size(104, 17);
            this.cbFilterDepthMap.TabIndex = 4;
            this.cbFilterDepthMap.Text = "Filter Depth Map";
            this.cbFilterDepthMap.UseVisualStyleBackColor = true;
            this.cbFilterDepthMap.CheckedChanged += new System.EventHandler(this.cbFilterDepthMap_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(58, 178);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Depth Size Filter Size";
            // 
            // tbFilterDepthMapSize
            // 
            this.tbFilterDepthMapSize.Location = new System.Drawing.Point(12, 175);
            this.tbFilterDepthMapSize.Mask = "00";
            this.tbFilterDepthMapSize.Name = "tbFilterDepthMapSize";
            this.tbFilterDepthMapSize.Size = new System.Drawing.Size(39, 20);
            this.tbFilterDepthMapSize.TabIndex = 7;
            this.tbFilterDepthMapSize.ValidatingType = typeof(int);
            this.tbFilterDepthMapSize.TextChanged += new System.EventHandler(this.tbFilterDepthMapSize_TextChanged);
            this.tbFilterDepthMapSize.Leave += new System.EventHandler(this.tbFilterDepthMapSize_TextChanged);
            this.tbFilterDepthMapSize.Validating += new System.ComponentModel.CancelEventHandler(this.tbFilterDepthMapSize_Validating);
            // 
            // KinectConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 269);
            this.Controls.Add(this.tbFilterDepthMapSize);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbFilterDepthMap);
            this.Controls.Add(this.kinectIDLabel);
            this.Controls.Add(this.lbDepthModeDetaulsLabel);
            this.Controls.Add(this.btUpdate);
            this.Controls.Add(this.lDepthModeListBox);
            this.Name = "KinectConfigurationForm";
            this.Text = "KinectSettingsForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.KinectSettingsForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lDepthModeListBox;
        private System.Windows.Forms.Button btUpdate;
        private System.Windows.Forms.Label lbDepthModeDetaulsLabel;
        private System.Windows.Forms.Label kinectIDLabel;
        private System.Windows.Forms.CheckBox cbFilterDepthMap;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MaskedTextBox tbFilterDepthMapSize;
    }
}