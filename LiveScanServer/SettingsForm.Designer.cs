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
            this.lbMerge = new System.Windows.Forms.Label();
            this.chMerge = new System.Windows.Forms.CheckBox();
            this.lbICPIters = new System.Windows.Forms.Label();
            this.txtICPIters = new System.Windows.Forms.TextBox();
            this.grClient = new System.Windows.Forms.GroupBox();
            this.gpPerformance = new System.Windows.Forms.GroupBox();
            this.cbEnablePreview = new System.Windows.Forms.CheckBox();
            this.btApplyAllSettings = new System.Windows.Forms.Button();
            this.lApplyWarning = new System.Windows.Forms.Label();
            this.grExposure = new System.Windows.Forms.GroupBox();
            this.lbManualExposure = new System.Windows.Forms.Label();
            this.chAutoExposureEnabled = new System.Windows.Forms.CheckBox();
            this.trManualExposure = new System.Windows.Forms.TrackBar();
            this.grTempSync = new System.Windows.Forms.GroupBox();
            this.grNetworkSync = new System.Windows.Forms.GroupBox();
            this.chNetworkSync = new System.Windows.Forms.CheckBox();
            this.lbNetworkSync = new System.Windows.Forms.Label();
            this.grHardwareSync = new System.Windows.Forms.GroupBox();
            this.chHardwareSync = new System.Windows.Forms.CheckBox();
            this.lbTempSync = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.exportGroup = new System.Windows.Forms.GroupBox();
            this.rExportRawFrames = new System.Windows.Forms.RadioButton();
            this.rExportPointcloud = new System.Windows.Forms.RadioButton();
            this.lExtrinsics = new System.Windows.Forms.Label();
            this.cbExtrinsicsFormat = new System.Windows.Forms.ComboBox();
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
            this.grServer = new System.Windows.Forms.GroupBox();
            this.cbCompressionLevel = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.rBinaryPly = new System.Windows.Forms.RadioButton();
            this.lbFormat = new System.Windows.Forms.Label();
            this.rAsciiPly = new System.Windows.Forms.RadioButton();
            this.txtRefinIters = new System.Windows.Forms.TextBox();
            this.lbOuterIters = new System.Windows.Forms.Label();
            this.UpdateClientsBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.grClient.SuspendLayout();
            this.gpPerformance.SuspendLayout();
            this.grExposure.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trManualExposure)).BeginInit();
            this.grTempSync.SuspendLayout();
            this.grNetworkSync.SuspendLayout();
            this.grHardwareSync.SuspendLayout();
            this.exportGroup.SuspendLayout();
            this.grMarkers.SuspendLayout();
            this.grBounding.SuspendLayout();
            this.grServer.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbMerge
            // 
            this.lbMerge.AutoSize = true;
            this.lbMerge.Location = new System.Drawing.Point(8, 69);
            this.lbMerge.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbMerge.Name = "lbMerge";
            this.lbMerge.Size = new System.Drawing.Size(99, 20);
            this.lbMerge.TabIndex = 22;
            this.lbMerge.Text = "Scan saving:";
            // 
            // chMerge
            // 
            this.chMerge.AutoSize = true;
            this.chMerge.Location = new System.Drawing.Point(156, 69);
            this.chMerge.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chMerge.Name = "chMerge";
            this.chMerge.Size = new System.Drawing.Size(126, 24);
            this.chMerge.TabIndex = 23;
            this.chMerge.Text = "merge scans";
            this.chMerge.UseVisualStyleBackColor = true;
            this.chMerge.CheckedChanged += new System.EventHandler(this.chMerge_CheckedChanged);
            // 
            // lbICPIters
            // 
            this.lbICPIters.AutoSize = true;
            this.lbICPIters.Location = new System.Drawing.Point(8, 34);
            this.lbICPIters.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbICPIters.Name = "lbICPIters";
            this.lbICPIters.Size = new System.Drawing.Size(128, 20);
            this.lbICPIters.TabIndex = 24;
            this.lbICPIters.Text = "Num of ICP iters:";
            // 
            // txtICPIters
            // 
            this.txtICPIters.Location = new System.Drawing.Point(156, 31);
            this.txtICPIters.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtICPIters.Name = "txtICPIters";
            this.txtICPIters.Size = new System.Drawing.Size(55, 26);
            this.txtICPIters.TabIndex = 25;
            this.txtICPIters.TextChanged += new System.EventHandler(this.txtICPIters_TextChanged);
            // 
            // grClient
            // 
            this.grClient.Controls.Add(this.gpPerformance);
            this.grClient.Controls.Add(this.btApplyAllSettings);
            this.grClient.Controls.Add(this.lApplyWarning);
            this.grClient.Controls.Add(this.grExposure);
            this.grClient.Controls.Add(this.grTempSync);
            this.grClient.Controls.Add(this.exportGroup);
            this.grClient.Controls.Add(this.grMarkers);
            this.grClient.Controls.Add(this.grBounding);
            this.grClient.Location = new System.Drawing.Point(12, 12);
            this.grClient.Name = "grClient";
            this.grClient.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grClient.Size = new System.Drawing.Size(948, 829);
            this.grClient.TabIndex = 43;
            this.grClient.TabStop = false;
            this.grClient.Text = "KinectClient settings";
            // 
            // gpPerformance
            // 
            this.gpPerformance.Controls.Add(this.cbEnablePreview);
            this.gpPerformance.Location = new System.Drawing.Point(9, 122);
            this.gpPerformance.Name = "gpPerformance";
            this.gpPerformance.Size = new System.Drawing.Size(249, 46);
            this.gpPerformance.TabIndex = 53;
            this.gpPerformance.TabStop = false;
            this.gpPerformance.Text = "Performance";
            // 
            // cbEnablePreview
            // 
            this.cbEnablePreview.AutoSize = true;
            this.cbEnablePreview.Location = new System.Drawing.Point(6, 20);
            this.cbEnablePreview.Name = "cbEnablePreview";
            this.cbEnablePreview.Size = new System.Drawing.Size(171, 17);
            this.cbEnablePreview.TabIndex = 0;
            this.cbEnablePreview.Text = "Enable Preview during capture";
            this.cbEnablePreview.UseVisualStyleBackColor = true;
            this.cbEnablePreview.CheckedChanged += new System.EventHandler(this.cbEnablePreview_CheckedChanged);
            // 
            // btApplyAllSettings
            // 
            this.btApplyAllSettings.Location = new System.Drawing.Point(723, 778);
            this.btApplyAllSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btApplyAllSettings.Name = "btApplyAllSettings";
            this.btApplyAllSettings.Size = new System.Drawing.Size(216, 35);
            this.btApplyAllSettings.TabIndex = 51;
            this.btApplyAllSettings.Text = "Apply settings to clients";
            this.btApplyAllSettings.UseVisualStyleBackColor = true;
            this.btApplyAllSettings.Click += new System.EventHandler(this.btApplyAllSettings_Click);
            // 
            // lApplyWarning
            // 
            this.lApplyWarning.ForeColor = System.Drawing.Color.OrangeRed;
            this.lApplyWarning.Location = new System.Drawing.Point(318, 786);
            this.lApplyWarning.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lApplyWarning.Name = "lApplyWarning";
            this.lApplyWarning.Size = new System.Drawing.Size(404, 20);
            this.lApplyWarning.TabIndex = 52;
            this.lApplyWarning.Text = "Changed settings are not yet applied!";
            this.lApplyWarning.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // grExposure
            // 
            this.grExposure.Controls.Add(this.lbManualExposure);
            this.grExposure.Controls.Add(this.chAutoExposureEnabled);
            this.grExposure.Controls.Add(this.trManualExposure);
            this.grExposure.Location = new System.Drawing.Point(14, 435);
            this.grExposure.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grExposure.Name = "grExposure";
            this.grExposure.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grExposure.Size = new System.Drawing.Size(374, 183);
            this.grExposure.TabIndex = 49;
            this.grExposure.TabStop = false;
            this.grExposure.Text = "Exposure Settings";
            // 
            // lbManualExposure
            // 
            this.lbManualExposure.AutoSize = true;
            this.lbManualExposure.Location = new System.Drawing.Point(118, 80);
            this.lbManualExposure.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbManualExposure.Name = "lbManualExposure";
            this.lbManualExposure.Size = new System.Drawing.Size(134, 20);
            this.lbManualExposure.TabIndex = 7;
            this.lbManualExposure.Text = "Manual exposure:";
            // 
            // chAutoExposureEnabled
            // 
            this.chAutoExposureEnabled.AutoSize = true;
            this.chAutoExposureEnabled.Checked = true;
            this.chAutoExposureEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chAutoExposureEnabled.Location = new System.Drawing.Point(16, 31);
            this.chAutoExposureEnabled.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chAutoExposureEnabled.Name = "chAutoExposureEnabled";
            this.chAutoExposureEnabled.Size = new System.Drawing.Size(199, 24);
            this.chAutoExposureEnabled.TabIndex = 6;
            this.chAutoExposureEnabled.Text = "Auto exposure enabled";
            this.chAutoExposureEnabled.UseVisualStyleBackColor = true;
            this.chAutoExposureEnabled.CheckedChanged += new System.EventHandler(this.chAutoExposureEnabled_CheckedChanged);
            // 
            // trManualExposure
            // 
            this.trManualExposure.Enabled = false;
            this.trManualExposure.LargeChange = 1;
            this.trManualExposure.Location = new System.Drawing.Point(9, 95);
            this.trManualExposure.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.trManualExposure.Maximum = -5;
            this.trManualExposure.Minimum = -11;
            this.trManualExposure.Name = "trManualExposure";
            this.trManualExposure.Size = new System.Drawing.Size(356, 69);
            this.trManualExposure.TabIndex = 5;
            this.trManualExposure.Value = -8;
            this.trManualExposure.Scroll += new System.EventHandler(this.trManualExposure_Scroll);
            // 
            // grTempSync
            // 
            this.grTempSync.Controls.Add(this.grNetworkSync);
            this.grTempSync.Controls.Add(this.grHardwareSync);
            this.grTempSync.Controls.Add(this.label1);
            this.grTempSync.Location = new System.Drawing.Point(406, 325);
            this.grTempSync.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grTempSync.Name = "grTempSync";
            this.grTempSync.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grTempSync.Size = new System.Drawing.Size(532, 298);
            this.grTempSync.TabIndex = 48;
            this.grTempSync.TabStop = false;
            this.grTempSync.Text = "Temporal sync";
            // 
            // grNetworkSync
            // 
            this.grNetworkSync.Controls.Add(this.chNetworkSync);
            this.grNetworkSync.Controls.Add(this.lbNetworkSync);
            this.grNetworkSync.Location = new System.Drawing.Point(15, 152);
            this.grNetworkSync.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grNetworkSync.Name = "grNetworkSync";
            this.grNetworkSync.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grNetworkSync.Size = new System.Drawing.Size(496, 132);
            this.grNetworkSync.TabIndex = 9;
            this.grNetworkSync.TabStop = false;
            this.grNetworkSync.Text = "Network Sync";
            // 
            // chNetworkSync
            // 
            this.chNetworkSync.AutoSize = true;
            this.chNetworkSync.Location = new System.Drawing.Point(16, 29);
            this.chNetworkSync.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chNetworkSync.Name = "chNetworkSync";
            this.chNetworkSync.Size = new System.Drawing.Size(94, 24);
            this.chNetworkSync.TabIndex = 5;
            this.chNetworkSync.Text = "Enabled";
            this.chNetworkSync.UseVisualStyleBackColor = true;
            this.chNetworkSync.CheckedChanged += new System.EventHandler(this.chNetworkSync_CheckedChanged);
            // 
            // lbNetworkSync
            // 
            this.lbNetworkSync.Location = new System.Drawing.Point(14, 60);
            this.lbNetworkSync.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbNetworkSync.Name = "lbNetworkSync";
            this.lbNetworkSync.Size = new System.Drawing.Size(474, 63);
            this.lbNetworkSync.TabIndex = 6;
            this.lbNetworkSync.Text = "No special hardware configuration needed. Might give more consistent frame timing" +
    "s between the devices, but reduces the framerate a lot.";
            // 
            // grHardwareSync
            // 
            this.grHardwareSync.Controls.Add(this.chHardwareSync);
            this.grHardwareSync.Controls.Add(this.lbTempSync);
            this.grHardwareSync.Location = new System.Drawing.Point(15, 29);
            this.grHardwareSync.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grHardwareSync.Name = "grHardwareSync";
            this.grHardwareSync.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grHardwareSync.Size = new System.Drawing.Size(506, 114);
            this.grHardwareSync.TabIndex = 8;
            this.grHardwareSync.TabStop = false;
            this.grHardwareSync.Text = "Hardware sync";
            // 
            // chHardwareSync
            // 
            this.chHardwareSync.AutoSize = true;
            this.chHardwareSync.Location = new System.Drawing.Point(16, 29);
            this.chHardwareSync.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chHardwareSync.Name = "chHardwareSync";
            this.chHardwareSync.Size = new System.Drawing.Size(94, 24);
            this.chHardwareSync.TabIndex = 3;
            this.chHardwareSync.Text = "Enabled";
            this.chHardwareSync.UseVisualStyleBackColor = true;
            this.chHardwareSync.CheckedChanged += new System.EventHandler(this.chHardwareSync_CheckedChanged);
            // 
            // lbTempSync
            // 
            this.lbTempSync.Location = new System.Drawing.Point(12, 60);
            this.lbTempSync.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbTempSync.Name = "lbTempSync";
            this.lbTempSync.Size = new System.Drawing.Size(484, 45);
            this.lbTempSync.TabIndex = 2;
            this.lbTempSync.Text = "Before activating, make sure that all Kinects are connected to the server and pro" +
    "perly connected via the sync cables.";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 63);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.MaximumSize = new System.Drawing.Size(0, 92);
            this.label1.MinimumSize = new System.Drawing.Size(0, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 46);
            this.label1.TabIndex = 1;
            this.label1.Text = "Notice: All devices need to be connected  to the server and via sync cabel before" +
    " enabeling! First Device in Server List will become the master";
            // 
            // exportGroup
            // 
            this.exportGroup.Controls.Add(this.rExportRawFrames);
            this.exportGroup.Controls.Add(this.rExportPointcloud);
            this.exportGroup.Controls.Add(this.lExtrinsics);
            this.exportGroup.Controls.Add(this.cbExtrinsicsFormat);
            this.exportGroup.Location = new System.Drawing.Point(408, 632);
            this.exportGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.exportGroup.Name = "exportGroup";
            this.exportGroup.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.exportGroup.Size = new System.Drawing.Size(532, 135);
            this.exportGroup.TabIndex = 50;
            this.exportGroup.TabStop = false;
            this.exportGroup.Text = "Export";
            // 
            // rExportRawFrames
            // 
            this.rExportRawFrames.AutoSize = true;
            this.rExportRawFrames.Location = new System.Drawing.Point(9, 71);
            this.rExportRawFrames.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rExportRawFrames.Name = "rExportRawFrames";
            this.rExportRawFrames.Size = new System.Drawing.Size(302, 24);
            this.rExportRawFrames.TabIndex = 37;
            this.rExportRawFrames.Text = "Color/Depth Frames (Stored on client)";
            this.rExportRawFrames.UseVisualStyleBackColor = true;
            // 
            // rExportPointcloud
            // 
            this.rExportPointcloud.AutoSize = true;
            this.rExportPointcloud.Checked = true;
            this.rExportPointcloud.Location = new System.Drawing.Point(9, 35);
            this.rExportPointcloud.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rExportPointcloud.Name = "rExportPointcloud";
            this.rExportPointcloud.Size = new System.Drawing.Size(242, 24);
            this.rExportPointcloud.TabIndex = 36;
            this.rExportPointcloud.TabStop = true;
            this.rExportPointcloud.Text = "Pointcloud (Stored on Server)";
            this.rExportPointcloud.UseVisualStyleBackColor = true;
            this.rExportPointcloud.CheckedChanged += new System.EventHandler(this.rExportPointcloud_CheckedChanged);
            // 
            // lExtrinsics
            // 
            this.lExtrinsics.AutoSize = true;
            this.lExtrinsics.Location = new System.Drawing.Point(4, 105);
            this.lExtrinsics.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lExtrinsics.Name = "lExtrinsics";
            this.lExtrinsics.Size = new System.Drawing.Size(76, 20);
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
            this.cbExtrinsicsFormat.Location = new System.Drawing.Point(84, 100);
            this.cbExtrinsicsFormat.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbExtrinsicsFormat.Name = "cbExtrinsicsFormat";
            this.cbExtrinsicsFormat.Size = new System.Drawing.Size(169, 28);
            this.cbExtrinsicsFormat.TabIndex = 33;
            this.cbExtrinsicsFormat.SelectedIndexChanged += new System.EventHandler(this.cbExtrinsicsFormat_SelectedIndexChanged);
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
            this.grMarkers.Location = new System.Drawing.Point(406, 29);
            this.grMarkers.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grMarkers.Name = "grMarkers";
            this.grMarkers.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grMarkers.Size = new System.Drawing.Size(530, 285);
            this.grMarkers.TabIndex = 45;
            this.grMarkers.TabStop = false;
            this.grMarkers.Text = "Calibration markers";
            // 
            // lbX2
            // 
            this.lbX2.AutoSize = true;
            this.lbX2.Location = new System.Drawing.Point(345, 29);
            this.lbX2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbX2.Name = "lbX2";
            this.lbX2.Size = new System.Drawing.Size(20, 20);
            this.lbX2.TabIndex = 49;
            this.lbX2.Text = "X";
            // 
            // btRemove
            // 
            this.btRemove.Location = new System.Drawing.Point(350, 195);
            this.btRemove.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btRemove.Name = "btRemove";
            this.btRemove.Size = new System.Drawing.Size(138, 35);
            this.btRemove.TabIndex = 59;
            this.btRemove.Text = "Remove marker";
            this.btRemove.UseVisualStyleBackColor = true;
            this.btRemove.Click += new System.EventHandler(this.btRemove_Click);
            // 
            // txtOrientationZ
            // 
            this.txtOrientationZ.Location = new System.Drawing.Point(458, 58);
            this.txtOrientationZ.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtOrientationZ.Name = "txtOrientationZ";
            this.txtOrientationZ.Size = new System.Drawing.Size(55, 26);
            this.txtOrientationZ.TabIndex = 48;
            this.txtOrientationZ.TextChanged += new System.EventHandler(this.txtOrientationZ_TextChanged);
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(326, 143);
            this.txtId.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(55, 26);
            this.txtId.TabIndex = 58;
            this.txtId.TextChanged += new System.EventHandler(this.txtId_TextChanged);
            // 
            // lbY2
            // 
            this.lbY2.AutoSize = true;
            this.lbY2.Location = new System.Drawing.Point(411, 29);
            this.lbY2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbY2.Name = "lbY2";
            this.lbY2.Size = new System.Drawing.Size(20, 20);
            this.lbY2.TabIndex = 50;
            this.lbY2.Text = "Y";
            // 
            // txtOrientationY
            // 
            this.txtOrientationY.Location = new System.Drawing.Point(392, 58);
            this.txtOrientationY.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtOrientationY.Name = "txtOrientationY";
            this.txtOrientationY.Size = new System.Drawing.Size(55, 26);
            this.txtOrientationY.TabIndex = 47;
            this.txtOrientationY.TextChanged += new System.EventHandler(this.txtOrientationY_TextChanged);
            // 
            // lbId
            // 
            this.lbId.AutoSize = true;
            this.lbId.Location = new System.Drawing.Point(198, 148);
            this.lbId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbId.Name = "lbId";
            this.lbId.Size = new System.Drawing.Size(78, 20);
            this.lbId.TabIndex = 57;
            this.lbId.Text = "Marker id:";
            // 
            // lbZ2
            // 
            this.lbZ2.AutoSize = true;
            this.lbZ2.Location = new System.Drawing.Point(476, 29);
            this.lbZ2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbZ2.Name = "lbZ2";
            this.lbZ2.Size = new System.Drawing.Size(19, 20);
            this.lbZ2.TabIndex = 51;
            this.lbZ2.Text = "Z";
            // 
            // txtOrientationX
            // 
            this.txtOrientationX.Location = new System.Drawing.Point(326, 58);
            this.txtOrientationX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtOrientationX.Name = "txtOrientationX";
            this.txtOrientationX.Size = new System.Drawing.Size(55, 26);
            this.txtOrientationX.TabIndex = 46;
            this.txtOrientationX.TextChanged += new System.EventHandler(this.txtOrientationX_TextChanged);
            // 
            // btAdd
            // 
            this.btAdd.Location = new System.Drawing.Point(202, 195);
            this.btAdd.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(138, 35);
            this.btAdd.TabIndex = 56;
            this.btAdd.Text = "Add marker";
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // lbTranslation
            // 
            this.lbTranslation.AutoSize = true;
            this.lbTranslation.Location = new System.Drawing.Point(198, 105);
            this.lbTranslation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbTranslation.Name = "lbTranslation";
            this.lbTranslation.Size = new System.Drawing.Size(91, 20);
            this.lbTranslation.TabIndex = 52;
            this.lbTranslation.Text = "Translation:";
            // 
            // lbOrientation
            // 
            this.lbOrientation.AutoSize = true;
            this.lbOrientation.Location = new System.Drawing.Point(198, 63);
            this.lbOrientation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbOrientation.Name = "lbOrientation";
            this.lbOrientation.Size = new System.Drawing.Size(91, 20);
            this.lbOrientation.TabIndex = 45;
            this.lbOrientation.Text = "Orientation:";
            // 
            // txtTranslationZ
            // 
            this.txtTranslationZ.Location = new System.Drawing.Point(458, 100);
            this.txtTranslationZ.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtTranslationZ.Name = "txtTranslationZ";
            this.txtTranslationZ.Size = new System.Drawing.Size(55, 26);
            this.txtTranslationZ.TabIndex = 55;
            this.txtTranslationZ.TextChanged += new System.EventHandler(this.txtTranslationZ_TextChanged);
            // 
            // txtTranslationX
            // 
            this.txtTranslationX.Location = new System.Drawing.Point(326, 100);
            this.txtTranslationX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtTranslationX.Name = "txtTranslationX";
            this.txtTranslationX.Size = new System.Drawing.Size(55, 26);
            this.txtTranslationX.TabIndex = 53;
            this.txtTranslationX.TextChanged += new System.EventHandler(this.txtTranslationX_TextChanged);
            // 
            // lisMarkers
            // 
            this.lisMarkers.FormattingEnabled = true;
            this.lisMarkers.ItemHeight = 20;
            this.lisMarkers.Location = new System.Drawing.Point(9, 63);
            this.lisMarkers.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lisMarkers.Name = "lisMarkers";
            this.lisMarkers.Size = new System.Drawing.Size(178, 164);
            this.lisMarkers.TabIndex = 43;
            this.lisMarkers.SelectedIndexChanged += new System.EventHandler(this.lisMarkers_SelectedIndexChanged);
            // 
            // txtTranslationY
            // 
            this.txtTranslationY.Location = new System.Drawing.Point(392, 100);
            this.txtTranslationY.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtTranslationY.Name = "txtTranslationY";
            this.txtTranslationY.Size = new System.Drawing.Size(55, 26);
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
            this.grBounding.Location = new System.Drawing.Point(14, 29);
            this.grBounding.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grBounding.Name = "grBounding";
            this.grBounding.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grBounding.Size = new System.Drawing.Size(374, 140);
            this.grBounding.TabIndex = 46;
            this.grBounding.TabStop = false;
            this.grBounding.Text = "Bounding box";
            // 
            // lbMin
            // 
            this.lbMin.AutoSize = true;
            this.lbMin.Location = new System.Drawing.Point(12, 58);
            this.lbMin.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbMin.Name = "lbMin";
            this.lbMin.Size = new System.Drawing.Size(95, 20);
            this.lbMin.TabIndex = 13;
            this.lbMin.Text = "Min bounds:";
            // 
            // txtMaxZ
            // 
            this.txtMaxZ.Location = new System.Drawing.Point(272, 95);
            this.txtMaxZ.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtMaxZ.Name = "txtMaxZ";
            this.txtMaxZ.Size = new System.Drawing.Size(55, 26);
            this.txtMaxZ.TabIndex = 23;
            this.txtMaxZ.TextChanged += new System.EventHandler(this.txtMaxZ_TextChanged);
            // 
            // txtMaxY
            // 
            this.txtMaxY.Location = new System.Drawing.Point(206, 95);
            this.txtMaxY.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtMaxY.Name = "txtMaxY";
            this.txtMaxY.Size = new System.Drawing.Size(55, 26);
            this.txtMaxY.TabIndex = 22;
            this.txtMaxY.TextChanged += new System.EventHandler(this.txtMaxY_TextChanged);
            // 
            // txtMinX
            // 
            this.txtMinX.Location = new System.Drawing.Point(140, 54);
            this.txtMinX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtMinX.Name = "txtMinX";
            this.txtMinX.Size = new System.Drawing.Size(55, 26);
            this.txtMinX.TabIndex = 14;
            this.txtMinX.TextChanged += new System.EventHandler(this.txtMinX_TextChanged);
            // 
            // txtMaxX
            // 
            this.txtMaxX.Location = new System.Drawing.Point(140, 95);
            this.txtMaxX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtMaxX.Name = "txtMaxX";
            this.txtMaxX.Size = new System.Drawing.Size(55, 26);
            this.txtMaxX.TabIndex = 21;
            this.txtMaxX.TextChanged += new System.EventHandler(this.txtMaxX_TextChanged);
            // 
            // txtMinY
            // 
            this.txtMinY.Location = new System.Drawing.Point(206, 54);
            this.txtMinY.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtMinY.Name = "txtMinY";
            this.txtMinY.Size = new System.Drawing.Size(55, 26);
            this.txtMinY.TabIndex = 15;
            this.txtMinY.TextChanged += new System.EventHandler(this.txtMinY_TextChanged);
            // 
            // lbMax
            // 
            this.lbMax.AutoSize = true;
            this.lbMax.Location = new System.Drawing.Point(12, 100);
            this.lbMax.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbMax.Name = "lbMax";
            this.lbMax.Size = new System.Drawing.Size(99, 20);
            this.lbMax.TabIndex = 20;
            this.lbMax.Text = "Max bounds:";
            // 
            // txtMinZ
            // 
            this.txtMinZ.Location = new System.Drawing.Point(272, 54);
            this.txtMinZ.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtMinZ.Name = "txtMinZ";
            this.txtMinZ.Size = new System.Drawing.Size(55, 26);
            this.txtMinZ.TabIndex = 16;
            this.txtMinZ.TextChanged += new System.EventHandler(this.txtMinZ_TextChanged);
            // 
            // lbZ
            // 
            this.lbZ.AutoSize = true;
            this.lbZ.Location = new System.Drawing.Point(290, 25);
            this.lbZ.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbZ.Name = "lbZ";
            this.lbZ.Size = new System.Drawing.Size(19, 20);
            this.lbZ.TabIndex = 19;
            this.lbZ.Text = "Z";
            // 
            // lbY
            // 
            this.lbY.AutoSize = true;
            this.lbY.Location = new System.Drawing.Point(225, 25);
            this.lbY.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbY.Name = "lbY";
            this.lbY.Size = new System.Drawing.Size(20, 20);
            this.lbY.TabIndex = 18;
            this.lbY.Text = "Y";
            // 
            // lbX
            // 
            this.lbX.AutoSize = true;
            this.lbX.Location = new System.Drawing.Point(159, 25);
            this.lbX.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbX.Name = "lbX";
            this.lbX.Size = new System.Drawing.Size(20, 20);
            this.lbX.TabIndex = 17;
            this.lbX.Text = "X";
            // 
            // grServer
            // 
            this.grServer.Controls.Add(this.cbCompressionLevel);
            this.grServer.Controls.Add(this.label2);
            this.grServer.Controls.Add(this.rBinaryPly);
            this.grServer.Controls.Add(this.lbFormat);
            this.grServer.Controls.Add(this.rAsciiPly);
            this.grServer.Controls.Add(this.txtRefinIters);
            this.grServer.Controls.Add(this.lbOuterIters);
            this.grServer.Controls.Add(this.lbMerge);
            this.grServer.Controls.Add(this.txtICPIters);
            this.grServer.Controls.Add(this.chMerge);
            this.grServer.Controls.Add(this.lbICPIters);
            this.grServer.Location = new System.Drawing.Point(18, 857);
            this.grServer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grServer.Name = "grServer";
            this.grServer.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grServer.Size = new System.Drawing.Size(484, 175);
            this.grServer.TabIndex = 44;
            this.grServer.TabStop = false;
            this.grServer.Text = "KinectServer settings";
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
            this.cbCompressionLevel.Location = new System.Drawing.Point(154, 135);
            this.cbCompressionLevel.Name = "cbCompressionLevel";
            this.cbCompressionLevel.Size = new System.Drawing.Size(169, 28);
            this.cbCompressionLevel.TabIndex = 32;
            this.cbCompressionLevel.SelectedIndexChanged += new System.EventHandler(this.cbCompressionLevel_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 138);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 20);
            this.label2.TabIndex = 31;
            this.label2.Text = "Compression level:";
            // 
            // rBinaryPly
            // 
            this.rBinaryPly.AutoSize = true;
            this.rBinaryPly.Location = new System.Drawing.Point(278, 105);
            this.rBinaryPly.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rBinaryPly.Name = "rBinaryPly";
            this.rBinaryPly.Size = new System.Drawing.Size(110, 24);
            this.rBinaryPly.TabIndex = 30;
            this.rBinaryPly.TabStop = true;
            this.rBinaryPly.Text = "binary PLY";
            this.rBinaryPly.UseVisualStyleBackColor = true;
            this.rBinaryPly.CheckedChanged += new System.EventHandler(this.PlyFormat_CheckedChanged);
            // 
            // lbFormat
            // 
            this.lbFormat.AutoSize = true;
            this.lbFormat.Location = new System.Drawing.Point(8, 106);
            this.lbFormat.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbFormat.Name = "lbFormat";
            this.lbFormat.Size = new System.Drawing.Size(88, 20);
            this.lbFormat.TabIndex = 29;
            this.lbFormat.Text = "File format:";
            // 
            // rAsciiPly
            // 
            this.rAsciiPly.AutoSize = true;
            this.rAsciiPly.Location = new System.Drawing.Point(156, 105);
            this.rAsciiPly.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rAsciiPly.Name = "rAsciiPly";
            this.rAsciiPly.Size = new System.Drawing.Size(111, 24);
            this.rAsciiPly.TabIndex = 28;
            this.rAsciiPly.TabStop = true;
            this.rAsciiPly.Text = "ASCII PLY";
            this.rAsciiPly.UseVisualStyleBackColor = true;
            this.rAsciiPly.CheckedChanged += new System.EventHandler(this.PlyFormat_CheckedChanged);
            // 
            // txtRefinIters
            // 
            this.txtRefinIters.Location = new System.Drawing.Point(408, 31);
            this.txtRefinIters.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtRefinIters.Name = "txtRefinIters";
            this.txtRefinIters.Size = new System.Drawing.Size(55, 26);
            this.txtRefinIters.TabIndex = 27;
            this.txtRefinIters.TextChanged += new System.EventHandler(this.txtRefinIters_TextChanged);
            // 
            // lbOuterIters
            // 
            this.lbOuterIters.AutoSize = true;
            this.lbOuterIters.Location = new System.Drawing.Point(222, 35);
            this.lbOuterIters.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbOuterIters.Name = "lbOuterIters";
            this.lbOuterIters.Size = new System.Drawing.Size(178, 20);
            this.lbOuterIters.TabIndex = 26;
            this.lbOuterIters.Text = "Num of refinement iters:";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 1051);
            this.Controls.Add(this.grServer);
            this.Controls.Add(this.grClient);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SettingsForm_FormClosed);
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.grClient.ResumeLayout(false);
            this.gpPerformance.ResumeLayout(false);
            this.gpPerformance.PerformLayout();
            this.grExposure.ResumeLayout(false);
            this.grExposure.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trManualExposure)).EndInit();
            this.grTempSync.ResumeLayout(false);
            this.grNetworkSync.ResumeLayout(false);
            this.grNetworkSync.PerformLayout();
            this.grHardwareSync.ResumeLayout(false);
            this.grHardwareSync.PerformLayout();
            this.exportGroup.ResumeLayout(false);
            this.exportGroup.PerformLayout();
            this.grMarkers.ResumeLayout(false);
            this.grMarkers.PerformLayout();
            this.grBounding.ResumeLayout(false);
            this.grBounding.PerformLayout();
            this.grServer.ResumeLayout(false);
            this.grServer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbMerge;
        private System.Windows.Forms.CheckBox chMerge;
        private System.Windows.Forms.Label lbICPIters;
        private System.Windows.Forms.TextBox txtICPIters;
        private System.Windows.Forms.GroupBox grClient;
        private System.Windows.Forms.GroupBox grServer;
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
        private System.Windows.Forms.GroupBox grTempSync;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbTempSync;
        private System.Windows.Forms.TrackBar trManualExposure;
        private System.Windows.Forms.GroupBox grExposure;
        private System.Windows.Forms.Label lbManualExposure;
        private System.Windows.Forms.CheckBox chAutoExposureEnabled;
        private System.Windows.Forms.GroupBox exportGroup;
        private System.Windows.Forms.ComboBox cbExtrinsicsFormat;
        private System.Windows.Forms.Label lExtrinsics;
        private System.Windows.Forms.RadioButton rExportRawFrames;
        private System.Windows.Forms.RadioButton rExportPointcloud;
        private System.Windows.Forms.Button btApplyAllSettings;
        private System.Windows.Forms.Label lApplyWarning;
        private System.Windows.Forms.Label lbNetworkSync;
        private System.Windows.Forms.CheckBox chNetworkSync;
        private System.Windows.Forms.GroupBox grHardwareSync;
        private System.Windows.Forms.GroupBox grNetworkSync;
        private System.Windows.Forms.CheckBox chHardwareSync;
        private System.ComponentModel.BackgroundWorker UpdateClientsBackgroundWorker;
        private System.Windows.Forms.GroupBox gpPerformance;
        private System.Windows.Forms.CheckBox cbEnablePreview;
    }
}