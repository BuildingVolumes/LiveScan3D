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
            this.lbHardwareSync = new System.Windows.Forms.Label();
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
            this.lbMerge.Location = new System.Drawing.Point(5, 45);
            this.lbMerge.Name = "lbMerge";
            this.lbMerge.Size = new System.Drawing.Size(69, 13);
            this.lbMerge.TabIndex = 22;
            this.lbMerge.Text = "Scan saving:";
            // 
            // chMerge
            // 
            this.chMerge.AutoSize = true;
            this.chMerge.Location = new System.Drawing.Point(104, 45);
            this.chMerge.Name = "chMerge";
            this.chMerge.Size = new System.Drawing.Size(86, 17);
            this.chMerge.TabIndex = 23;
            this.chMerge.Text = "merge scans";
            this.chMerge.UseVisualStyleBackColor = true;
            this.chMerge.CheckedChanged += new System.EventHandler(this.chMerge_CheckedChanged);
            // 
            // lbICPIters
            // 
            this.lbICPIters.AutoSize = true;
            this.lbICPIters.Location = new System.Drawing.Point(5, 22);
            this.lbICPIters.Name = "lbICPIters";
            this.lbICPIters.Size = new System.Drawing.Size(86, 13);
            this.lbICPIters.TabIndex = 24;
            this.lbICPIters.Text = "Num of ICP iters:";
            // 
            // txtICPIters
            // 
            this.txtICPIters.Location = new System.Drawing.Point(104, 20);
            this.txtICPIters.Name = "txtICPIters";
            this.txtICPIters.Size = new System.Drawing.Size(38, 20);
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
            this.grClient.Location = new System.Drawing.Point(8, 8);
            this.grClient.Margin = new System.Windows.Forms.Padding(2);
            this.grClient.Name = "grClient";
            this.grClient.Size = new System.Drawing.Size(632, 446);
            this.grClient.TabIndex = 43;
            this.grClient.TabStop = false;
            this.grClient.Text = "KinectClient settings";
            // 
            // gpPerformance
            // 
            this.gpPerformance.Controls.Add(this.cbEnablePreview);
            this.gpPerformance.Location = new System.Drawing.Point(9, 126);
            this.gpPerformance.Margin = new System.Windows.Forms.Padding(2);
            this.gpPerformance.Name = "gpPerformance";
            this.gpPerformance.Padding = new System.Windows.Forms.Padding(2);
            this.gpPerformance.Size = new System.Drawing.Size(249, 42);
            this.gpPerformance.TabIndex = 53;
            this.gpPerformance.TabStop = false;
            this.gpPerformance.Text = "Performance";
            // 
            // cbEnablePreview
            // 
            this.cbEnablePreview.AutoSize = true;
            this.cbEnablePreview.Location = new System.Drawing.Point(11, 17);
            this.cbEnablePreview.Margin = new System.Windows.Forms.Padding(2);
            this.cbEnablePreview.Name = "cbEnablePreview";
            this.cbEnablePreview.Size = new System.Drawing.Size(171, 17);
            this.cbEnablePreview.TabIndex = 0;
            this.cbEnablePreview.Text = "Enable Preview during capture";
            this.cbEnablePreview.UseVisualStyleBackColor = true;
            this.cbEnablePreview.CheckedChanged += new System.EventHandler(this.cbEnablePreview_CheckedChanged);
            // 
            // btApplyAllSettings
            // 
            this.btApplyAllSettings.Location = new System.Drawing.Point(474, 416);
            this.btApplyAllSettings.Name = "btApplyAllSettings";
            this.btApplyAllSettings.Size = new System.Drawing.Size(144, 23);
            this.btApplyAllSettings.TabIndex = 51;
            this.btApplyAllSettings.Text = "Apply settings to clients";
            this.btApplyAllSettings.UseVisualStyleBackColor = true;
            this.btApplyAllSettings.Click += new System.EventHandler(this.btApplyAllSettings_Click);
            // 
            // lApplyWarning
            // 
            this.lApplyWarning.ForeColor = System.Drawing.Color.OrangeRed;
            this.lApplyWarning.Location = new System.Drawing.Point(202, 421);
            this.lApplyWarning.Name = "lApplyWarning";
            this.lApplyWarning.Size = new System.Drawing.Size(269, 13);
            this.lApplyWarning.TabIndex = 52;
            this.lApplyWarning.Text = "Changed settings are not yet applied!";
            this.lApplyWarning.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // grExposure
            // 
            this.grExposure.Controls.Add(this.lbManualExposure);
            this.grExposure.Controls.Add(this.chAutoExposureEnabled);
            this.grExposure.Controls.Add(this.trManualExposure);
            this.grExposure.Location = new System.Drawing.Point(9, 178);
            this.grExposure.Name = "grExposure";
            this.grExposure.Size = new System.Drawing.Size(249, 119);
            this.grExposure.TabIndex = 49;
            this.grExposure.TabStop = false;
            this.grExposure.Text = "Exposure Settings";
            // 
            // lbManualExposure
            // 
            this.lbManualExposure.AutoSize = true;
            this.lbManualExposure.Location = new System.Drawing.Point(79, 52);
            this.lbManualExposure.Name = "lbManualExposure";
            this.lbManualExposure.Size = new System.Drawing.Size(91, 13);
            this.lbManualExposure.TabIndex = 7;
            this.lbManualExposure.Text = "Manual exposure:";
            // 
            // chAutoExposureEnabled
            // 
            this.chAutoExposureEnabled.AutoSize = true;
            this.chAutoExposureEnabled.Checked = true;
            this.chAutoExposureEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chAutoExposureEnabled.Location = new System.Drawing.Point(11, 20);
            this.chAutoExposureEnabled.Name = "chAutoExposureEnabled";
            this.chAutoExposureEnabled.Size = new System.Drawing.Size(135, 17);
            this.chAutoExposureEnabled.TabIndex = 6;
            this.chAutoExposureEnabled.Text = "Auto exposure enabled";
            this.chAutoExposureEnabled.UseVisualStyleBackColor = true;
            this.chAutoExposureEnabled.CheckedChanged += new System.EventHandler(this.chAutoExposureEnabled_CheckedChanged);
            // 
            // trManualExposure
            // 
            this.trManualExposure.Enabled = false;
            this.trManualExposure.LargeChange = 1;
            this.trManualExposure.Location = new System.Drawing.Point(6, 62);
            this.trManualExposure.Maximum = -5;
            this.trManualExposure.Minimum = -11;
            this.trManualExposure.Name = "trManualExposure";
            this.trManualExposure.Size = new System.Drawing.Size(237, 45);
            this.trManualExposure.TabIndex = 5;
            this.trManualExposure.Value = -8;
            this.trManualExposure.Scroll += new System.EventHandler(this.trManualExposure_Scroll);
            // 
            // grTempSync
            // 
            this.grTempSync.Controls.Add(this.grNetworkSync);
            this.grTempSync.Controls.Add(this.grHardwareSync);
            this.grTempSync.Location = new System.Drawing.Point(271, 211);
            this.grTempSync.Name = "grTempSync";
            this.grTempSync.Size = new System.Drawing.Size(355, 194);
            this.grTempSync.TabIndex = 48;
            this.grTempSync.TabStop = false;
            this.grTempSync.Text = "Temporal sync";
            // 
            // grNetworkSync
            // 
            this.grNetworkSync.Controls.Add(this.chNetworkSync);
            this.grNetworkSync.Controls.Add(this.lbNetworkSync);
            this.grNetworkSync.Location = new System.Drawing.Point(10, 99);
            this.grNetworkSync.Name = "grNetworkSync";
            this.grNetworkSync.Size = new System.Drawing.Size(331, 86);
            this.grNetworkSync.TabIndex = 9;
            this.grNetworkSync.TabStop = false;
            this.grNetworkSync.Text = "Network Sync";
            // 
            // chNetworkSync
            // 
            this.chNetworkSync.AutoSize = true;
            this.chNetworkSync.Location = new System.Drawing.Point(11, 19);
            this.chNetworkSync.Name = "chNetworkSync";
            this.chNetworkSync.Size = new System.Drawing.Size(65, 17);
            this.chNetworkSync.TabIndex = 5;
            this.chNetworkSync.Text = "Enabled";
            this.chNetworkSync.UseVisualStyleBackColor = true;
            this.chNetworkSync.CheckedChanged += new System.EventHandler(this.chNetworkSync_CheckedChanged);
            // 
            // lbNetworkSync
            // 
            this.lbNetworkSync.Location = new System.Drawing.Point(9, 39);
            this.lbNetworkSync.Name = "lbNetworkSync";
            this.lbNetworkSync.Size = new System.Drawing.Size(316, 41);
            this.lbNetworkSync.TabIndex = 6;
            this.lbNetworkSync.Text = "No special hardware configuration needed. Might give more consistent frame timing" +
    "s between the devices, but reduces the framerate a lot.";
            // 
            // grHardwareSync
            // 
            this.grHardwareSync.Controls.Add(this.chHardwareSync);
            this.grHardwareSync.Controls.Add(this.lbHardwareSync);
            this.grHardwareSync.Location = new System.Drawing.Point(10, 19);
            this.grHardwareSync.Name = "grHardwareSync";
            this.grHardwareSync.Size = new System.Drawing.Size(337, 74);
            this.grHardwareSync.TabIndex = 8;
            this.grHardwareSync.TabStop = false;
            this.grHardwareSync.Text = "Hardware sync";
            // 
            // chHardwareSync
            // 
            this.chHardwareSync.AutoSize = true;
            this.chHardwareSync.Location = new System.Drawing.Point(11, 19);
            this.chHardwareSync.Name = "chHardwareSync";
            this.chHardwareSync.Size = new System.Drawing.Size(65, 17);
            this.chHardwareSync.TabIndex = 3;
            this.chHardwareSync.Text = "Enabled";
            this.chHardwareSync.UseVisualStyleBackColor = true;
            this.chHardwareSync.CheckedChanged += new System.EventHandler(this.chHardwareSync_CheckedChanged);
            // 
            // lbHardwareSync
            // 
            this.lbHardwareSync.Location = new System.Drawing.Point(8, 39);
            this.lbHardwareSync.Name = "lbHardwareSync";
            this.lbHardwareSync.Size = new System.Drawing.Size(323, 29);
            this.lbHardwareSync.TabIndex = 2;
            this.lbHardwareSync.Text = "Before activating, make sure that all Kinects are connected to the server and pro" +
    "perly connected via the sync cables.";
            // 
            // exportGroup
            // 
            this.exportGroup.Controls.Add(this.rExportRawFrames);
            this.exportGroup.Controls.Add(this.rExportPointcloud);
            this.exportGroup.Controls.Add(this.lExtrinsics);
            this.exportGroup.Controls.Add(this.cbExtrinsicsFormat);
            this.exportGroup.Location = new System.Drawing.Point(9, 303);
            this.exportGroup.Name = "exportGroup";
            this.exportGroup.Size = new System.Drawing.Size(249, 102);
            this.exportGroup.TabIndex = 50;
            this.exportGroup.TabStop = false;
            this.exportGroup.Text = "Export";
            // 
            // rExportRawFrames
            // 
            this.rExportRawFrames.AutoSize = true;
            this.rExportRawFrames.Location = new System.Drawing.Point(6, 46);
            this.rExportRawFrames.Name = "rExportRawFrames";
            this.rExportRawFrames.Size = new System.Drawing.Size(203, 17);
            this.rExportRawFrames.TabIndex = 37;
            this.rExportRawFrames.Text = "Color/Depth Frames (Stored on client)";
            this.rExportRawFrames.UseVisualStyleBackColor = true;
            // 
            // rExportPointcloud
            // 
            this.rExportPointcloud.AutoSize = true;
            this.rExportPointcloud.Checked = true;
            this.rExportPointcloud.Location = new System.Drawing.Point(6, 23);
            this.rExportPointcloud.Name = "rExportPointcloud";
            this.rExportPointcloud.Size = new System.Drawing.Size(164, 17);
            this.rExportPointcloud.TabIndex = 36;
            this.rExportPointcloud.TabStop = true;
            this.rExportPointcloud.Text = "Pointcloud (Stored on Server)";
            this.rExportPointcloud.UseVisualStyleBackColor = true;
            this.rExportPointcloud.CheckedChanged += new System.EventHandler(this.rExportPointcloud_CheckedChanged);
            // 
            // lExtrinsics
            // 
            this.lExtrinsics.AutoSize = true;
            this.lExtrinsics.Location = new System.Drawing.Point(3, 68);
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
            this.cbExtrinsicsFormat.Location = new System.Drawing.Point(56, 65);
            this.cbExtrinsicsFormat.Name = "cbExtrinsicsFormat";
            this.cbExtrinsicsFormat.Size = new System.Drawing.Size(114, 21);
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
            this.grMarkers.Location = new System.Drawing.Point(271, 19);
            this.grMarkers.Name = "grMarkers";
            this.grMarkers.Size = new System.Drawing.Size(353, 185);
            this.grMarkers.TabIndex = 45;
            this.grMarkers.TabStop = false;
            this.grMarkers.Text = "Calibration markers";
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
            this.btRemove.Text = "Remove marker";
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
            this.lbId.Size = new System.Drawing.Size(54, 13);
            this.lbId.TabIndex = 57;
            this.lbId.Text = "Marker id:";
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
            this.btAdd.Text = "Add marker";
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
            this.grBounding.Text = "Bounding box";
            // 
            // lbMin
            // 
            this.lbMin.AutoSize = true;
            this.lbMin.Location = new System.Drawing.Point(8, 38);
            this.lbMin.Name = "lbMin";
            this.lbMin.Size = new System.Drawing.Size(65, 13);
            this.lbMin.TabIndex = 13;
            this.lbMin.Text = "Min bounds:";
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
            this.lbMax.Size = new System.Drawing.Size(68, 13);
            this.lbMax.TabIndex = 20;
            this.lbMax.Text = "Max bounds:";
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
            this.grServer.Location = new System.Drawing.Point(8, 459);
            this.grServer.Name = "grServer";
            this.grServer.Size = new System.Drawing.Size(323, 114);
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
            this.cbCompressionLevel.Location = new System.Drawing.Point(103, 88);
            this.cbCompressionLevel.Margin = new System.Windows.Forms.Padding(2);
            this.cbCompressionLevel.Name = "cbCompressionLevel";
            this.cbCompressionLevel.Size = new System.Drawing.Size(114, 21);
            this.cbCompressionLevel.TabIndex = 32;
            this.cbCompressionLevel.SelectedIndexChanged += new System.EventHandler(this.cbCompressionLevel_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 90);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 31;
            this.label2.Text = "Compression level:";
            // 
            // rBinaryPly
            // 
            this.rBinaryPly.AutoSize = true;
            this.rBinaryPly.Location = new System.Drawing.Point(185, 68);
            this.rBinaryPly.Name = "rBinaryPly";
            this.rBinaryPly.Size = new System.Drawing.Size(76, 17);
            this.rBinaryPly.TabIndex = 30;
            this.rBinaryPly.TabStop = true;
            this.rBinaryPly.Text = "binary PLY";
            this.rBinaryPly.UseVisualStyleBackColor = true;
            this.rBinaryPly.CheckedChanged += new System.EventHandler(this.PlyFormat_CheckedChanged);
            // 
            // lbFormat
            // 
            this.lbFormat.AutoSize = true;
            this.lbFormat.Location = new System.Drawing.Point(5, 69);
            this.lbFormat.Name = "lbFormat";
            this.lbFormat.Size = new System.Drawing.Size(58, 13);
            this.lbFormat.TabIndex = 29;
            this.lbFormat.Text = "File format:";
            // 
            // rAsciiPly
            // 
            this.rAsciiPly.AutoSize = true;
            this.rAsciiPly.Location = new System.Drawing.Point(104, 68);
            this.rAsciiPly.Name = "rAsciiPly";
            this.rAsciiPly.Size = new System.Drawing.Size(75, 17);
            this.rAsciiPly.TabIndex = 28;
            this.rAsciiPly.TabStop = true;
            this.rAsciiPly.Text = "ASCII PLY";
            this.rAsciiPly.UseVisualStyleBackColor = true;
            this.rAsciiPly.CheckedChanged += new System.EventHandler(this.PlyFormat_CheckedChanged);
            // 
            // txtRefinIters
            // 
            this.txtRefinIters.Location = new System.Drawing.Point(272, 20);
            this.txtRefinIters.Name = "txtRefinIters";
            this.txtRefinIters.Size = new System.Drawing.Size(38, 20);
            this.txtRefinIters.TabIndex = 27;
            this.txtRefinIters.TextChanged += new System.EventHandler(this.txtRefinIters_TextChanged);
            // 
            // lbOuterIters
            // 
            this.lbOuterIters.AutoSize = true;
            this.lbOuterIters.Location = new System.Drawing.Point(148, 23);
            this.lbOuterIters.Name = "lbOuterIters";
            this.lbOuterIters.Size = new System.Drawing.Size(118, 13);
            this.lbOuterIters.TabIndex = 26;
            this.lbOuterIters.Text = "Num of refinement iters:";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 586);
            this.Controls.Add(this.grServer);
            this.Controls.Add(this.grClient);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
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
        private System.Windows.Forms.Label lbHardwareSync;
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