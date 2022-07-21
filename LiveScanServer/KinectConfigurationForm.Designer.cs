
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
            this.components = new System.ComponentModel.Container();
            this.lbDepthRes = new System.Windows.Forms.ListBox();
            this.btApplyCurrentDevice = new System.Windows.Forms.Button();
            this.cbFilterDepthMap = new System.Windows.Forms.CheckBox();
            this.lDepthFilterSizeHeader = new System.Windows.Forms.Label();
            this.lbColorRes = new System.Windows.Forms.ListBox();
            this.lDepthModeHeader = new System.Windows.Forms.Label();
            this.lColorModeHeader = new System.Windows.Forms.Label();
            this.btApplyAll = new System.Windows.Forms.Button();
            this.nDepthFilterSize = new System.Windows.Forms.NumericUpDown();
            this.pInfoRefineCalib = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.ttInfo = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.nDepthFilterSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoRefineCalib)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // lbDepthRes
            // 
            this.lbDepthRes.FormattingEnabled = true;
            this.lbDepthRes.Location = new System.Drawing.Point(12, 31);
            this.lbDepthRes.Name = "lbDepthRes";
            this.lbDepthRes.Size = new System.Drawing.Size(173, 95);
            this.lbDepthRes.TabIndex = 0;
            this.lbDepthRes.SelectedIndexChanged += new System.EventHandler(this.lbDepthRes_SelectedIndexChanged);
            // 
            // btApplyCurrentDevice
            // 
            this.btApplyCurrentDevice.Location = new System.Drawing.Point(265, 192);
            this.btApplyCurrentDevice.Name = "btApplyCurrentDevice";
            this.btApplyCurrentDevice.Size = new System.Drawing.Size(125, 23);
            this.btApplyCurrentDevice.TabIndex = 1;
            this.btApplyCurrentDevice.Text = "Apply to current device";
            this.btApplyCurrentDevice.UseVisualStyleBackColor = true;
            this.btApplyCurrentDevice.Click += new System.EventHandler(this.btApply_Click);
            // 
            // cbFilterDepthMap
            // 
            this.cbFilterDepthMap.AutoSize = true;
            this.cbFilterDepthMap.Location = new System.Drawing.Point(12, 137);
            this.cbFilterDepthMap.Name = "cbFilterDepthMap";
            this.cbFilterDepthMap.Size = new System.Drawing.Size(104, 17);
            this.cbFilterDepthMap.TabIndex = 4;
            this.cbFilterDepthMap.Text = "Filter Depth Map";
            this.cbFilterDepthMap.UseVisualStyleBackColor = true;
            this.cbFilterDepthMap.CheckedChanged += new System.EventHandler(this.cbFilterDepthMap_CheckedChanged);
            // 
            // lDepthFilterSizeHeader
            // 
            this.lDepthFilterSizeHeader.AutoSize = true;
            this.lDepthFilterSizeHeader.Location = new System.Drawing.Point(70, 162);
            this.lDepthFilterSizeHeader.Name = "lDepthFilterSizeHeader";
            this.lDepthFilterSizeHeader.Size = new System.Drawing.Size(107, 13);
            this.lDepthFilterSizeHeader.TabIndex = 6;
            this.lDepthFilterSizeHeader.Text = "Depth Size Filter Size";
            // 
            // lbColorRes
            // 
            this.lbColorRes.FormattingEnabled = true;
            this.lbColorRes.Location = new System.Drawing.Point(208, 31);
            this.lbColorRes.Name = "lbColorRes";
            this.lbColorRes.Size = new System.Drawing.Size(182, 95);
            this.lbColorRes.TabIndex = 8;
            this.lbColorRes.SelectedIndexChanged += new System.EventHandler(this.lbColorRes_SelectedIndexChanged);
            // 
            // lDepthModeHeader
            // 
            this.lDepthModeHeader.AutoSize = true;
            this.lDepthModeHeader.Location = new System.Drawing.Point(9, 12);
            this.lDepthModeHeader.Name = "lDepthModeHeader";
            this.lDepthModeHeader.Size = new System.Drawing.Size(128, 13);
            this.lDepthModeHeader.TabIndex = 9;
            this.lDepthModeHeader.Text = "Depth Camera Resolution";
            // 
            // lColorModeHeader
            // 
            this.lColorModeHeader.AutoSize = true;
            this.lColorModeHeader.Location = new System.Drawing.Point(205, 12);
            this.lColorModeHeader.Name = "lColorModeHeader";
            this.lColorModeHeader.Size = new System.Drawing.Size(123, 13);
            this.lColorModeHeader.TabIndex = 10;
            this.lColorModeHeader.Text = "Color Camera Resolution";
            // 
            // btApplyAll
            // 
            this.btApplyAll.Location = new System.Drawing.Point(139, 192);
            this.btApplyAll.Name = "btApplyAll";
            this.btApplyAll.Size = new System.Drawing.Size(120, 23);
            this.btApplyAll.TabIndex = 11;
            this.btApplyAll.Text = "Apply to all devices";
            this.btApplyAll.UseVisualStyleBackColor = true;
            this.btApplyAll.Click += new System.EventHandler(this.btApplyAll_Click);
            // 
            // nDepthFilterSize
            // 
            this.nDepthFilterSize.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nDepthFilterSize.Location = new System.Drawing.Point(31, 159);
            this.nDepthFilterSize.Maximum = new decimal(new int[] {
            51,
            0,
            0,
            0});
            this.nDepthFilterSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nDepthFilterSize.Name = "nDepthFilterSize";
            this.nDepthFilterSize.Size = new System.Drawing.Size(35, 20);
            this.nDepthFilterSize.TabIndex = 12;
            this.nDepthFilterSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nDepthFilterSize.ValueChanged += new System.EventHandler(this.nDepthFilterSize_ValueChanged);
            // 
            // pInfoRefineCalib
            // 
            this.pInfoRefineCalib.Image = global::KinectServer.Properties.Resources.info_box;
            this.pInfoRefineCalib.Location = new System.Drawing.Point(139, 10);
            this.pInfoRefineCalib.Name = "pInfoRefineCalib";
            this.pInfoRefineCalib.Size = new System.Drawing.Size(16, 16);
            this.pInfoRefineCalib.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoRefineCalib.TabIndex = 25;
            this.pInfoRefineCalib.TabStop = false;
            this.ttInfo.SetToolTip(this.pInfoRefineCalib, "N/WFOV = Near / Wide Field of View. NFOV  is recommended for most capture scenari" +
        "os, as it yields a higher pixel density. The higher the resolution, the higher t" +
        "he performance cost");
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::KinectServer.Properties.Resources.info_box;
            this.pictureBox1.Location = new System.Drawing.Point(330, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 26;
            this.pictureBox1.TabStop = false;
            this.ttInfo.SetToolTip(this.pictureBox1, "4:3 resolutions are better suited for NFOV depth resolutions, as they provide a b" +
        "etter overlap with the depth cam");
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::KinectServer.Properties.Resources.info_box;
            this.pictureBox2.Location = new System.Drawing.Point(116, 136);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(16, 16);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 27;
            this.pictureBox2.TabStop = false;
            this.ttInfo.SetToolTip(this.pictureBox2, "Removes artifacts around the edges of depth maps. Impacts performance, and only a" +
        "pplicable in Pointcloud capture");
            // 
            // ttInfo
            // 
            this.ttInfo.AutomaticDelay = 0;
            // 
            // KinectConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 227);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pInfoRefineCalib);
            this.Controls.Add(this.nDepthFilterSize);
            this.Controls.Add(this.btApplyAll);
            this.Controls.Add(this.lColorModeHeader);
            this.Controls.Add(this.lDepthModeHeader);
            this.Controls.Add(this.lbColorRes);
            this.Controls.Add(this.lDepthFilterSizeHeader);
            this.Controls.Add(this.cbFilterDepthMap);
            this.Controls.Add(this.btApplyCurrentDevice);
            this.Controls.Add(this.lbDepthRes);
            this.Name = "KinectConfigurationForm";
            this.Text = "KinectSettingsForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.KinectSettingsForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.nDepthFilterSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoRefineCalib)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbDepthRes;
        private System.Windows.Forms.Button btApplyCurrentDevice;
        private System.Windows.Forms.CheckBox cbFilterDepthMap;
        private System.Windows.Forms.Label lDepthFilterSizeHeader;
        private System.Windows.Forms.ListBox lbColorRes;
        private System.Windows.Forms.Label lDepthModeHeader;
        private System.Windows.Forms.Label lColorModeHeader;
        private System.Windows.Forms.Button btApplyAll;
        private System.Windows.Forms.NumericUpDown nDepthFilterSize;
        private System.Windows.Forms.PictureBox pInfoRefineCalib;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ToolTip ttInfo;
    }
}