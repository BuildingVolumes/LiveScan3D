namespace KinectServer
{
    partial class SettingsForm
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
            this.lbICPIters = new System.Windows.Forms.Label();
            this.txtICPIters = new System.Windows.Forms.TextBox();
            this.grClient = new System.Windows.Forms.GroupBox();
            this.gbICP = new System.Windows.Forms.GroupBox();
            this.txtRefinIters = new System.Windows.Forms.TextBox();
            this.lbOuterIters = new System.Windows.Forms.Label();
            this.exportGroup = new System.Windows.Forms.GroupBox();
            this.cbCompressionLevel = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lExtrinsics = new System.Windows.Forms.Label();
            this.cbExtrinsicsFormat = new System.Windows.Forms.ComboBox();
            this.rBinaryPly = new System.Windows.Forms.RadioButton();
            this.lbFormat = new System.Windows.Forms.Label();
            this.rAsciiPly = new System.Windows.Forms.RadioButton();
            this.grMarkers = new System.Windows.Forms.GroupBox();
            this.lbX2 = new System.Windows.Forms.Label();
            this.btRemove = new System.Windows.Forms.Button();
            this.txtOrientationZ = new System.Windows.Forms.TextBox();
            this.txtId = new System.Windows.Forms.TextBox();
            this.lbY2 = new System.Windows.Forms.Label();
            this.txtOrientationY = new System.Windows.Forms.TextBox();
            this.lbId = new System.Windows.Forms.Label();
            this.lbZ2 = new System.Windows.Forms.Label();
            this.txtOrientationX = new System.Windows.Forms.TextBox();
            this.btAdd = new System.Windows.Forms.Button();
            this.lbTranslation = new System.Windows.Forms.Label();
            this.lbOrientation = new System.Windows.Forms.Label();
            this.txtTranslationZ = new System.Windows.Forms.TextBox();
            this.txtTranslationX = new System.Windows.Forms.TextBox();
            this.lisMarkers = new System.Windows.Forms.ListBox();
            this.txtTranslationY = new System.Windows.Forms.TextBox();
            this.grBounding = new System.Windows.Forms.GroupBox();
            this.lbMin = new System.Windows.Forms.Label();
            this.txtMaxZ = new System.Windows.Forms.TextBox();
            this.txtMaxY = new System.Windows.Forms.TextBox();
            this.txtMinX = new System.Windows.Forms.TextBox();
            this.txtMaxX = new System.Windows.Forms.TextBox();
            this.txtMinY = new System.Windows.Forms.TextBox();
            this.lbMax = new System.Windows.Forms.Label();
            this.txtMinZ = new System.Windows.Forms.TextBox();
            this.lbZ = new System.Windows.Forms.Label();
            this.lbY = new System.Windows.Forms.Label();
            this.lbX = new System.Windows.Forms.Label();
            this.UpdateClientsBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.grClient.SuspendLayout();
            this.gbICP.SuspendLayout();
            this.exportGroup.SuspendLayout();
            this.grMarkers.SuspendLayout();
            this.grBounding.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbICPIters
            // 
            this.lbICPIters.AutoSize = true;
            this.lbICPIters.Location = new System.Drawing.Point(7, 27);
            this.lbICPIters.Name = "lbICPIters";
            this.lbICPIters.Size = new System.Drawing.Size(86, 13);
            this.lbICPIters.TabIndex = 24;
            this.lbICPIters.Text = "Num of ICP iters:";
            // 
            // txtICPIters
            // 
            this.txtICPIters.Location = new System.Drawing.Point(106, 25);
            this.txtICPIters.Name = "txtICPIters";
            this.txtICPIters.Size = new System.Drawing.Size(38, 20);
            this.txtICPIters.TabIndex = 25;
            this.txtICPIters.TextChanged += new System.EventHandler(this.txtICPIters_TextChanged);
            // 
            // grClient
            // 
            this.grClient.Controls.Add(this.gbICP);
            this.grClient.Controls.Add(this.exportGroup);
            this.grClient.Controls.Add(this.grMarkers);
            this.grClient.Controls.Add(this.grBounding);
            this.grClient.Location = new System.Drawing.Point(8, 8);
            this.grClient.Margin = new System.Windows.Forms.Padding(2);
            this.grClient.Name = "grClient";
            this.grClient.Size = new System.Drawing.Size(632, 251);
            this.grClient.TabIndex = 43;
            this.grClient.TabStop = false;
            this.grClient.Text = "More Settings";
            // 
            // gbICP
            // 
            this.gbICP.Controls.Add(this.txtRefinIters);
            this.gbICP.Controls.Add(this.lbOuterIters);
            this.gbICP.Controls.Add(this.lbICPIters);
            this.gbICP.Controls.Add(this.txtICPIters);
            this.gbICP.Location = new System.Drawing.Point(271, 188);
            this.gbICP.Name = "gbICP";
            this.gbICP.Size = new System.Drawing.Size(353, 54);
            this.gbICP.TabIndex = 60;
            this.gbICP.TabStop = false;
            this.gbICP.Text = "Calibration Refinement";
            // 
            // txtRefinIters
            // 
            this.txtRefinIters.Location = new System.Drawing.Point(294, 25);
            this.txtRefinIters.Name = "txtRefinIters";
            this.txtRefinIters.Size = new System.Drawing.Size(38, 20);
            this.txtRefinIters.TabIndex = 27;
            this.txtRefinIters.TextChanged += new System.EventHandler(this.txtRefinIters_TextChanged);
            // 
            // lbOuterIters
            // 
            this.lbOuterIters.AutoSize = true;
            this.lbOuterIters.Location = new System.Drawing.Point(170, 28);
            this.lbOuterIters.Name = "lbOuterIters";
            this.lbOuterIters.Size = new System.Drawing.Size(118, 13);
            this.lbOuterIters.TabIndex = 26;
            this.lbOuterIters.Text = "Num of refinement iters:";
            // 
            // exportGroup
            // 
            this.exportGroup.Controls.Add(this.cbCompressionLevel);
            this.exportGroup.Controls.Add(this.label2);
            this.exportGroup.Controls.Add(this.lExtrinsics);
            this.exportGroup.Controls.Add(this.cbExtrinsicsFormat);
            this.exportGroup.Controls.Add(this.rBinaryPly);
            this.exportGroup.Controls.Add(this.lbFormat);
            this.exportGroup.Controls.Add(this.rAsciiPly);
            this.exportGroup.Location = new System.Drawing.Point(9, 116);
            this.exportGroup.Name = "exportGroup";
            this.exportGroup.Size = new System.Drawing.Size(249, 126);
            this.exportGroup.TabIndex = 50;
            this.exportGroup.TabStop = false;
            this.exportGroup.Text = "Export";
            // 
            // cbCompressionLevel
            // 
            this.cbCompressionLevel.FormattingEnabled = true;
            this.cbCompressionLevel.Items.AddRange(new object[] {
            "0 (no compression)",
            "1",
            "2 (recommended)",
            "3",
            "5",
            "7",
            "9",
            "11",
            "13",
            "15",
            "17",
            "19"});
            this.cbCompressionLevel.Location = new System.Drawing.Point(109, 100);
            this.cbCompressionLevel.Margin = new System.Windows.Forms.Padding(2);
            this.cbCompressionLevel.Name = "cbCompressionLevel";
            this.cbCompressionLevel.Size = new System.Drawing.Size(114, 21);
            this.cbCompressionLevel.TabIndex = 32;
            this.cbCompressionLevel.SelectedIndexChanged += new System.EventHandler(this.cbCompressionLevel_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 102);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 31;
            this.label2.Text = "Compression Level:";
            // 
            // lExtrinsics
            // 
            this.lExtrinsics.AutoSize = true;
            this.lExtrinsics.Location = new System.Drawing.Point(8, 22);
            this.lExtrinsics.Name = "lExtrinsics";
            this.lExtrinsics.Size = new System.Drawing.Size(51, 13);
            this.lExtrinsics.TabIndex = 35;
            this.lExtrinsics.Text = "Extrinsics";
            // 
            // cbExtrinsicsFormat
            // 
            this.cbExtrinsicsFormat.AllowDrop = true;
            this.cbExtrinsicsFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbExtrinsicsFormat.FormattingEnabled = true;
            this.cbExtrinsicsFormat.Items.AddRange(new object[] {
            "Don\'t Export",
            "Open3D Style"});
            this.cbExtrinsicsFormat.Location = new System.Drawing.Point(61, 19);
            this.cbExtrinsicsFormat.Name = "cbExtrinsicsFormat";
            this.cbExtrinsicsFormat.Size = new System.Drawing.Size(114, 21);
            this.cbExtrinsicsFormat.TabIndex = 33;
            this.cbExtrinsicsFormat.SelectedIndexChanged += new System.EventHandler(this.cbExtrinsicsFormat_SelectedIndexChanged);
            // 
            // rBinaryPly
            // 
            this.rBinaryPly.AutoSize = true;
            this.rBinaryPly.Location = new System.Drawing.Point(109, 72);
            this.rBinaryPly.Name = "rBinaryPly";
            this.rBinaryPly.Size = new System.Drawing.Size(77, 17);
            this.rBinaryPly.TabIndex = 30;
            this.rBinaryPly.TabStop = true;
            this.rBinaryPly.Text = "Binary PLY";
            this.rBinaryPly.UseVisualStyleBackColor = true;
            this.rBinaryPly.CheckedChanged += new System.EventHandler(this.PlyFormat_CheckedChanged);
            // 
            // lbFormat
            // 
            this.lbFormat.AutoSize = true;
            this.lbFormat.Location = new System.Drawing.Point(8, 53);
            this.lbFormat.Name = "lbFormat";
            this.lbFormat.Size = new System.Drawing.Size(114, 13);
            this.lbFormat.TabIndex = 29;
            this.lbFormat.Text = "Pointcloud File Format:";
            // 
            // rAsciiPly
            // 
            this.rAsciiPly.AutoSize = true;
            this.rAsciiPly.Location = new System.Drawing.Point(11, 72);
            this.rAsciiPly.Name = "rAsciiPly";
            this.rAsciiPly.Size = new System.Drawing.Size(75, 17);
            this.rAsciiPly.TabIndex = 28;
            this.rAsciiPly.TabStop = true;
            this.rAsciiPly.Text = "ASCII PLY";
            this.rAsciiPly.UseVisualStyleBackColor = true;
            this.rAsciiPly.CheckedChanged += new System.EventHandler(this.PlyFormat_CheckedChanged);
            // 
            // grMarkers
            // 
            this.grMarkers.Controls.Add(this.lbX2);
            this.grMarkers.Controls.Add(this.btRemove);
            this.grMarkers.Controls.Add(this.txtOrientationZ);
            this.grMarkers.Controls.Add(this.txtId);
            this.grMarkers.Controls.Add(this.lbY2);
            this.grMarkers.Controls.Add(this.txtOrientationY);
            this.grMarkers.Controls.Add(this.lbId);
            this.grMarkers.Controls.Add(this.lbZ2);
            this.grMarkers.Controls.Add(this.txtOrientationX);
            this.grMarkers.Controls.Add(this.btAdd);
            this.grMarkers.Controls.Add(this.lbTranslation);
            this.grMarkers.Controls.Add(this.lbOrientation);
            this.grMarkers.Controls.Add(this.txtTranslationZ);
            this.grMarkers.Controls.Add(this.txtTranslationX);
            this.grMarkers.Controls.Add(this.lisMarkers);
            this.grMarkers.Controls.Add(this.txtTranslationY);
            this.grMarkers.Location = new System.Drawing.Point(271, 19);
            this.grMarkers.Name = "grMarkers";
            this.grMarkers.Size = new System.Drawing.Size(353, 163);
            this.grMarkers.TabIndex = 45;
            this.grMarkers.TabStop = false;
            this.grMarkers.Text = "Calibration Markers";
            // 
            // lbX2
            // 
            this.lbX2.AutoSize = true;
            this.lbX2.Location = new System.Drawing.Point(230, 19);
            this.lbX2.Name = "lbX2";
            this.lbX2.Size = new System.Drawing.Size(14, 13);
            this.lbX2.TabIndex = 49;
            this.lbX2.Text = "X";
            // 
            // btRemove
            // 
            this.btRemove.Location = new System.Drawing.Point(233, 127);
            this.btRemove.Name = "btRemove";
            this.btRemove.Size = new System.Drawing.Size(92, 23);
            this.btRemove.TabIndex = 59;
            this.btRemove.Text = "Remove Marker";
            this.btRemove.UseVisualStyleBackColor = true;
            this.btRemove.Click += new System.EventHandler(this.btRemove_Click);
            // 
            // txtOrientationZ
            // 
            this.txtOrientationZ.Location = new System.Drawing.Point(305, 38);
            this.txtOrientationZ.Name = "txtOrientationZ";
            this.txtOrientationZ.Size = new System.Drawing.Size(38, 20);
            this.txtOrientationZ.TabIndex = 48;
            this.txtOrientationZ.TextChanged += new System.EventHandler(this.txtOrientationZ_TextChanged);
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(217, 93);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(38, 20);
            this.txtId.TabIndex = 58;
            this.txtId.TextChanged += new System.EventHandler(this.txtId_TextChanged);
            // 
            // lbY2
            // 
            this.lbY2.AutoSize = true;
            this.lbY2.Location = new System.Drawing.Point(274, 19);
            this.lbY2.Name = "lbY2";
            this.lbY2.Size = new System.Drawing.Size(14, 13);
            this.lbY2.TabIndex = 50;
            this.lbY2.Text = "Y";
            // 
            // txtOrientationY
            // 
            this.txtOrientationY.Location = new System.Drawing.Point(261, 38);
            this.txtOrientationY.Name = "txtOrientationY";
            this.txtOrientationY.Size = new System.Drawing.Size(38, 20);
            this.txtOrientationY.TabIndex = 47;
            this.txtOrientationY.TextChanged += new System.EventHandler(this.txtOrientationY_TextChanged);
            // 
            // lbId
            // 
            this.lbId.AutoSize = true;
            this.lbId.Location = new System.Drawing.Point(132, 96);
            this.lbId.Name = "lbId";
            this.lbId.Size = new System.Drawing.Size(57, 13);
            this.lbId.TabIndex = 57;
            this.lbId.Text = "Marker ID:";
            // 
            // lbZ2
            // 
            this.lbZ2.AutoSize = true;
            this.lbZ2.Location = new System.Drawing.Point(317, 19);
            this.lbZ2.Name = "lbZ2";
            this.lbZ2.Size = new System.Drawing.Size(14, 13);
            this.lbZ2.TabIndex = 51;
            this.lbZ2.Text = "Z";
            // 
            // txtOrientationX
            // 
            this.txtOrientationX.Location = new System.Drawing.Point(217, 38);
            this.txtOrientationX.Name = "txtOrientationX";
            this.txtOrientationX.Size = new System.Drawing.Size(38, 20);
            this.txtOrientationX.TabIndex = 46;
            this.txtOrientationX.TextChanged += new System.EventHandler(this.txtOrientationX_TextChanged);
            // 
            // btAdd
            // 
            this.btAdd.Location = new System.Drawing.Point(135, 127);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(92, 23);
            this.btAdd.TabIndex = 56;
            this.btAdd.Text = "Add Marker";
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // lbTranslation
            // 
            this.lbTranslation.AutoSize = true;
            this.lbTranslation.Location = new System.Drawing.Point(132, 68);
            this.lbTranslation.Name = "lbTranslation";
            this.lbTranslation.Size = new System.Drawing.Size(62, 13);
            this.lbTranslation.TabIndex = 52;
            this.lbTranslation.Text = "Translation:";
            // 
            // lbOrientation
            // 
            this.lbOrientation.AutoSize = true;
            this.lbOrientation.Location = new System.Drawing.Point(132, 41);
            this.lbOrientation.Name = "lbOrientation";
            this.lbOrientation.Size = new System.Drawing.Size(61, 13);
            this.lbOrientation.TabIndex = 45;
            this.lbOrientation.Text = "Orientation:";
            // 
            // txtTranslationZ
            // 
            this.txtTranslationZ.Location = new System.Drawing.Point(305, 65);
            this.txtTranslationZ.Name = "txtTranslationZ";
            this.txtTranslationZ.Size = new System.Drawing.Size(38, 20);
            this.txtTranslationZ.TabIndex = 55;
            this.txtTranslationZ.TextChanged += new System.EventHandler(this.txtTranslationZ_TextChanged);
            // 
            // txtTranslationX
            // 
            this.txtTranslationX.Location = new System.Drawing.Point(217, 65);
            this.txtTranslationX.Name = "txtTranslationX";
            this.txtTranslationX.Size = new System.Drawing.Size(38, 20);
            this.txtTranslationX.TabIndex = 53;
            this.txtTranslationX.TextChanged += new System.EventHandler(this.txtTranslationX_TextChanged);
            // 
            // lisMarkers
            // 
            this.lisMarkers.FormattingEnabled = true;
            this.lisMarkers.Location = new System.Drawing.Point(6, 41);
            this.lisMarkers.Name = "lisMarkers";
            this.lisMarkers.Size = new System.Drawing.Size(120, 108);
            this.lisMarkers.TabIndex = 43;
            this.lisMarkers.SelectedIndexChanged += new System.EventHandler(this.lisMarkers_SelectedIndexChanged);
            // 
            // txtTranslationY
            // 
            this.txtTranslationY.Location = new System.Drawing.Point(261, 65);
            this.txtTranslationY.Name = "txtTranslationY";
            this.txtTranslationY.Size = new System.Drawing.Size(38, 20);
            this.txtTranslationY.TabIndex = 54;
            this.txtTranslationY.TextChanged += new System.EventHandler(this.txtTranslationY_TextChanged);
            // 
            // grBounding
            // 
            this.grBounding.Controls.Add(this.lbMin);
            this.grBounding.Controls.Add(this.txtMaxZ);
            this.grBounding.Controls.Add(this.txtMaxY);
            this.grBounding.Controls.Add(this.txtMinX);
            this.grBounding.Controls.Add(this.txtMaxX);
            this.grBounding.Controls.Add(this.txtMinY);
            this.grBounding.Controls.Add(this.lbMax);
            this.grBounding.Controls.Add(this.txtMinZ);
            this.grBounding.Controls.Add(this.lbZ);
            this.grBounding.Controls.Add(this.lbY);
            this.grBounding.Controls.Add(this.lbX);
            this.grBounding.Location = new System.Drawing.Point(9, 19);
            this.grBounding.Name = "grBounding";
            this.grBounding.Size = new System.Drawing.Size(249, 91);
            this.grBounding.TabIndex = 46;
            this.grBounding.TabStop = false;
            this.grBounding.Text = "Bounding Box";
            // 
            // lbMin
            // 
            this.lbMin.AutoSize = true;
            this.lbMin.Location = new System.Drawing.Point(8, 38);
            this.lbMin.Name = "lbMin";
            this.lbMin.Size = new System.Drawing.Size(66, 13);
            this.lbMin.TabIndex = 13;
            this.lbMin.Text = "Min Bounds:";
            // 
            // txtMaxZ
            // 
            this.txtMaxZ.Location = new System.Drawing.Point(181, 62);
            this.txtMaxZ.Name = "txtMaxZ";
            this.txtMaxZ.Size = new System.Drawing.Size(38, 20);
            this.txtMaxZ.TabIndex = 23;
            this.txtMaxZ.TextChanged += new System.EventHandler(this.txtMaxZ_TextChanged);
            // 
            // txtMaxY
            // 
            this.txtMaxY.Location = new System.Drawing.Point(137, 62);
            this.txtMaxY.Name = "txtMaxY";
            this.txtMaxY.Size = new System.Drawing.Size(38, 20);
            this.txtMaxY.TabIndex = 22;
            this.txtMaxY.TextChanged += new System.EventHandler(this.txtMaxY_TextChanged);
            // 
            // txtMinX
            // 
            this.txtMinX.Location = new System.Drawing.Point(93, 35);
            this.txtMinX.Name = "txtMinX";
            this.txtMinX.Size = new System.Drawing.Size(38, 20);
            this.txtMinX.TabIndex = 14;
            this.txtMinX.TextChanged += new System.EventHandler(this.txtMinX_TextChanged);
            // 
            // txtMaxX
            // 
            this.txtMaxX.Location = new System.Drawing.Point(93, 62);
            this.txtMaxX.Name = "txtMaxX";
            this.txtMaxX.Size = new System.Drawing.Size(38, 20);
            this.txtMaxX.TabIndex = 21;
            this.txtMaxX.TextChanged += new System.EventHandler(this.txtMaxX_TextChanged);
            // 
            // txtMinY
            // 
            this.txtMinY.Location = new System.Drawing.Point(137, 35);
            this.txtMinY.Name = "txtMinY";
            this.txtMinY.Size = new System.Drawing.Size(38, 20);
            this.txtMinY.TabIndex = 15;
            this.txtMinY.TextChanged += new System.EventHandler(this.txtMinY_TextChanged);
            // 
            // lbMax
            // 
            this.lbMax.AutoSize = true;
            this.lbMax.Location = new System.Drawing.Point(8, 65);
            this.lbMax.Name = "lbMax";
            this.lbMax.Size = new System.Drawing.Size(69, 13);
            this.lbMax.TabIndex = 20;
            this.lbMax.Text = "Max Bounds:";
            // 
            // txtMinZ
            // 
            this.txtMinZ.Location = new System.Drawing.Point(181, 35);
            this.txtMinZ.Name = "txtMinZ";
            this.txtMinZ.Size = new System.Drawing.Size(38, 20);
            this.txtMinZ.TabIndex = 16;
            this.txtMinZ.TextChanged += new System.EventHandler(this.txtMinZ_TextChanged);
            // 
            // lbZ
            // 
            this.lbZ.AutoSize = true;
            this.lbZ.Location = new System.Drawing.Point(193, 16);
            this.lbZ.Name = "lbZ";
            this.lbZ.Size = new System.Drawing.Size(14, 13);
            this.lbZ.TabIndex = 19;
            this.lbZ.Text = "Z";
            // 
            // lbY
            // 
            this.lbY.AutoSize = true;
            this.lbY.Location = new System.Drawing.Point(150, 16);
            this.lbY.Name = "lbY";
            this.lbY.Size = new System.Drawing.Size(14, 13);
            this.lbY.TabIndex = 18;
            this.lbY.Text = "Y";
            // 
            // lbX
            // 
            this.lbX.AutoSize = true;
            this.lbX.Location = new System.Drawing.Point(106, 16);
            this.lbX.Name = "lbX";
            this.lbX.Size = new System.Drawing.Size(14, 13);
            this.lbX.TabIndex = 17;
            this.lbX.Text = "X";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 270);
            this.Controls.Add(this.grClient);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SettingsForm_FormClosed);
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.grClient.ResumeLayout(false);
            this.gbICP.ResumeLayout(false);
            this.gbICP.PerformLayout();
            this.exportGroup.ResumeLayout(false);
            this.exportGroup.PerformLayout();
            this.grMarkers.ResumeLayout(false);
            this.grMarkers.PerformLayout();
            this.grBounding.ResumeLayout(false);
            this.grBounding.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lbICPIters;
        private System.Windows.Forms.TextBox txtICPIters;
        private System.Windows.Forms.GroupBox grClient;
        private System.Windows.Forms.TextBox txtRefinIters;
        private System.Windows.Forms.Label lbOuterIters;
        private System.Windows.Forms.GroupBox grMarkers;
        private System.Windows.Forms.Label lbX2;
        private System.Windows.Forms.Button btRemove;
        private System.Windows.Forms.TextBox txtOrientationZ;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label lbY2;
        private System.Windows.Forms.TextBox txtOrientationY;
        private System.Windows.Forms.Label lbId;
        private System.Windows.Forms.Label lbZ2;
        private System.Windows.Forms.TextBox txtOrientationX;
        private System.Windows.Forms.Button btAdd;
        private System.Windows.Forms.Label lbTranslation;
        private System.Windows.Forms.Label lbOrientation;
        private System.Windows.Forms.TextBox txtTranslationZ;
        private System.Windows.Forms.TextBox txtTranslationX;
        private System.Windows.Forms.ListBox lisMarkers;
        private System.Windows.Forms.TextBox txtTranslationY;
        private System.Windows.Forms.GroupBox grBounding;
        private System.Windows.Forms.Label lbMin;
        private System.Windows.Forms.TextBox txtMaxZ;
        private System.Windows.Forms.TextBox txtMaxY;
        private System.Windows.Forms.TextBox txtMinX;
        private System.Windows.Forms.TextBox txtMaxX;
        private System.Windows.Forms.TextBox txtMinY;
        private System.Windows.Forms.Label lbMax;
        private System.Windows.Forms.TextBox txtMinZ;
        private System.Windows.Forms.Label lbZ;
        private System.Windows.Forms.Label lbY;
        private System.Windows.Forms.Label lbX;
        private System.Windows.Forms.RadioButton rBinaryPly;
        private System.Windows.Forms.Label lbFormat;
        private System.Windows.Forms.RadioButton rAsciiPly;
        private System.Windows.Forms.ComboBox cbCompressionLevel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox exportGroup;
        private System.Windows.Forms.ComboBox cbExtrinsicsFormat;
        private System.Windows.Forms.Label lExtrinsics;
        private System.ComponentModel.BackgroundWorker UpdateClientsBackgroundWorker;
        private System.Windows.Forms.GroupBox gbICP;
    }
}