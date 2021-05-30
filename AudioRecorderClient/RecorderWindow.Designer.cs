
namespace AudioRecorderClient
{
    partial class RecorderWindow
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
            this.deviceSelectionList = new System.Windows.Forms.ListBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.connectionTextField = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // deviceSelectionList
            // 
            this.deviceSelectionList.FormattingEnabled = true;
            this.deviceSelectionList.Location = new System.Drawing.Point(51, 12);
            this.deviceSelectionList.Name = "deviceSelectionList";
            this.deviceSelectionList.Size = new System.Drawing.Size(190, 95);
            this.deviceSelectionList.TabIndex = 0;
            this.deviceSelectionList.SelectedIndexChanged += new System.EventHandler(this.deviceSelectionList_SelectedIndexChanged);
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(166, 122);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 23);
            this.connectButton.TabIndex = 1;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // connectionTextField
            // 
            this.connectionTextField.Location = new System.Drawing.Point(51, 125);
            this.connectionTextField.Name = "connectionTextField";
            this.connectionTextField.Size = new System.Drawing.Size(109, 20);
            this.connectionTextField.TabIndex = 2;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 143);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(310, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // RecorderWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 165);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.connectionTextField);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.deviceSelectionList);
            this.Name = "RecorderWindow";
            this.Text = "LiveScan3D Audio Recorder Client";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RecorderWindow_FormClosed);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox deviceSelectionList;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.TextBox connectionTextField;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}

