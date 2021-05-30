
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
            // KinectConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 269);
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
    }
}