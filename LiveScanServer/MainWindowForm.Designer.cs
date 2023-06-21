namespace KinectServer
{
    partial class MainWindowForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindowForm));
            this.btCalibrate = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.txtSeqName = new System.Windows.Forms.TextBox();
            this.btRefineCalib = new System.Windows.Forms.Button();
            this.OpenGLWorker = new System.ComponentModel.BackgroundWorker();
            this.btSettings = new System.Windows.Forms.Button();
            this.lbSeqName = new System.Windows.Forms.Label();
            this.btKinectSettingsOpenButton = new System.Windows.Forms.Button();
            this.lClientsHeader = new System.Windows.Forms.Label();
            this.lCalibrationHeader = new System.Windows.Forms.Label();
            this.lSettingsHeader = new System.Windows.Forms.Label();
            this.lCaptureHeader = new System.Windows.Forms.Label();
            this.rExportRaw = new System.Windows.Forms.RadioButton();
            this.rExportPointclouds = new System.Windows.Forms.RadioButton();
            this.gvClients = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SyncState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.glLiveView = new OpenTK.GLControl();
            this.lFPS = new System.Windows.Forms.Label();
            this.chMergeScans = new System.Windows.Forms.CheckBox();
            this.lTempSyncHeader = new System.Windows.Forms.Label();
            this.chHardwareSync = new System.Windows.Forms.CheckBox();
            this.chNetworkSync = new System.Windows.Forms.CheckBox();
            this.lExposure = new System.Windows.Forms.Label();
            this.rExposureAuto = new System.Windows.Forms.RadioButton();
            this.rExposureManual = new System.Windows.Forms.RadioButton();
            this.trManualExposure = new System.Windows.Forms.TrackBar();
            this.lPerformance = new System.Windows.Forms.Label();
            this.chEnablePreview = new System.Windows.Forms.CheckBox();
            this.tooltips = new System.Windows.Forms.ToolTip(this.components);
            this.pInfoClientPreview = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.PInfoExposure = new System.Windows.Forms.PictureBox();
            this.pInfoNetworkSync = new System.Windows.Forms.PictureBox();
            this.pInfoRefineCalib = new System.Windows.Forms.PictureBox();
            this.pInfoCalibrate = new System.Windows.Forms.PictureBox();
            this.pInfoRawFrames = new System.Windows.Forms.PictureBox();
            this.pInfoPointclouds = new System.Windows.Forms.PictureBox();
            this.pInfoWhiteBalance = new System.Windows.Forms.PictureBox();
            this.gbExposure = new System.Windows.Forms.Panel();
            this.gbFrameExport = new System.Windows.Forms.Panel();
            this.tlMainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.pnLiveView = new System.Windows.Forms.Panel();
            this.tlControls = new System.Windows.Forms.TableLayoutPanel();
            this.pnClients = new System.Windows.Forms.Panel();
            this.pnSettings = new System.Windows.Forms.Panel();
            this.gbWhiteBalance = new System.Windows.Forms.Panel();
            this.rWhiteBalanceManual = new System.Windows.Forms.RadioButton();
            this.rWhiteBalanceAuto = new System.Windows.Forms.RadioButton();
            this.trWhiteBalance = new System.Windows.Forms.TrackBar();
            this.lWhiteBalance = new System.Windows.Forms.Label();
            this.pnCalibration = new System.Windows.Forms.Panel();
            this.btRecord = new System.Windows.Forms.Button();
            this.pnCapture = new System.Windows.Forms.Panel();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvClients)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trManualExposure)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoClientPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PInfoExposure)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoNetworkSync)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoRefineCalib)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoCalibrate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoRawFrames)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoPointclouds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoWhiteBalance)).BeginInit();
            this.gbExposure.SuspendLayout();
            this.gbFrameExport.SuspendLayout();
            this.tlMainPanel.SuspendLayout();
            this.pnLiveView.SuspendLayout();
            this.tlControls.SuspendLayout();
            this.pnClients.SuspendLayout();
            this.pnSettings.SuspendLayout();
            this.gbWhiteBalance.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trWhiteBalance)).BeginInit();
            this.pnCalibration.SuspendLayout();
            this.pnCapture.SuspendLayout();
            this.SuspendLayout();
            // 
            // btCalibrate
            // 
            this.btCalibrate.Location = new System.Drawing.Point(5, 23);
            this.btCalibrate.Name = "btCalibrate";
            this.btCalibrate.Size = new System.Drawing.Size(95, 23);
            this.btCalibrate.TabIndex = 2;
            this.btCalibrate.Text = "Calibrate";
            this.btCalibrate.UseVisualStyleBackColor = true;
            this.btCalibrate.Click += new System.EventHandler(this.btCalibrate_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 672);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1048, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // txtSeqName
            // 
            this.txtSeqName.Location = new System.Drawing.Point(96, 26);
            this.txtSeqName.MaxLength = 40;
            this.txtSeqName.Name = "txtSeqName";
            this.txtSeqName.Size = new System.Drawing.Size(289, 20);
            this.txtSeqName.TabIndex = 7;
            this.txtSeqName.Text = "Capture";
            // 
            // btRefineCalib
            // 
            this.btRefineCalib.Location = new System.Drawing.Point(140, 23);
            this.btRefineCalib.Name = "btRefineCalib";
            this.btRefineCalib.Size = new System.Drawing.Size(112, 23);
            this.btRefineCalib.TabIndex = 11;
            this.btRefineCalib.Text = "Refine Calibration";
            this.btRefineCalib.UseVisualStyleBackColor = true;
            this.btRefineCalib.Click += new System.EventHandler(this.btRefineCalib_Click);
            // 
            // OpenGLWorker
            // 
            this.OpenGLWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.OpenGLWorker_DoWork);
            this.OpenGLWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.OpenGLWorker_RunWorkerCompleted);
            // 
            // btSettings
            // 
            this.btSettings.Location = new System.Drawing.Point(292, 155);
            this.btSettings.Name = "btSettings";
            this.btSettings.Size = new System.Drawing.Size(95, 23);
            this.btSettings.TabIndex = 13;
            this.btSettings.Text = "More Settings";
            this.btSettings.UseVisualStyleBackColor = true;
            this.btSettings.Click += new System.EventHandler(this.btSettings_Click);
            // 
            // lbSeqName
            // 
            this.lbSeqName.AutoSize = true;
            this.lbSeqName.Location = new System.Drawing.Point(2, 29);
            this.lbSeqName.Name = "lbSeqName";
            this.lbSeqName.Size = new System.Drawing.Size(90, 13);
            this.lbSeqName.TabIndex = 14;
            this.lbSeqName.Text = "Sequence Name:";
            // 
            // btKinectSettingsOpenButton
            // 
            this.btKinectSettingsOpenButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btKinectSettingsOpenButton.Location = new System.Drawing.Point(3, 197);
            this.btKinectSettingsOpenButton.Name = "btKinectSettingsOpenButton";
            this.btKinectSettingsOpenButton.Size = new System.Drawing.Size(382, 23);
            this.btKinectSettingsOpenButton.TabIndex = 15;
            this.btKinectSettingsOpenButton.Text = "Individual Client Configuration";
            this.btKinectSettingsOpenButton.UseVisualStyleBackColor = true;
            this.btKinectSettingsOpenButton.Click += new System.EventHandler(this.btKinectSettingsOpenButton_Click);
            // 
            // lClientsHeader
            // 
            this.lClientsHeader.AutoSize = true;
            this.lClientsHeader.Dock = System.Windows.Forms.DockStyle.Left;
            this.lClientsHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lClientsHeader.Location = new System.Drawing.Point(0, 0);
            this.lClientsHeader.Name = "lClientsHeader";
            this.lClientsHeader.Size = new System.Drawing.Size(51, 15);
            this.lClientsHeader.TabIndex = 17;
            this.lClientsHeader.Text = "Clients";
            // 
            // lCalibrationHeader
            // 
            this.lCalibrationHeader.AutoSize = true;
            this.lCalibrationHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lCalibrationHeader.Location = new System.Drawing.Point(0, 0);
            this.lCalibrationHeader.Name = "lCalibrationHeader";
            this.lCalibrationHeader.Size = new System.Drawing.Size(77, 15);
            this.lCalibrationHeader.TabIndex = 18;
            this.lCalibrationHeader.Text = "Calibration";
            // 
            // lSettingsHeader
            // 
            this.lSettingsHeader.AutoSize = true;
            this.lSettingsHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lSettingsHeader.Location = new System.Drawing.Point(0, 0);
            this.lSettingsHeader.Name = "lSettingsHeader";
            this.lSettingsHeader.Size = new System.Drawing.Size(59, 15);
            this.lSettingsHeader.TabIndex = 19;
            this.lSettingsHeader.Text = "Settings";
            // 
            // lCaptureHeader
            // 
            this.lCaptureHeader.AutoSize = true;
            this.lCaptureHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lCaptureHeader.Location = new System.Drawing.Point(2, 0);
            this.lCaptureHeader.Name = "lCaptureHeader";
            this.lCaptureHeader.Size = new System.Drawing.Size(57, 15);
            this.lCaptureHeader.TabIndex = 20;
            this.lCaptureHeader.Text = "Capture";
            // 
            // rExportRaw
            // 
            this.rExportRaw.Checked = true;
            this.rExportRaw.Location = new System.Drawing.Point(8, 3);
            this.rExportRaw.Name = "rExportRaw";
            this.rExportRaw.Size = new System.Drawing.Size(84, 17);
            this.rExportRaw.TabIndex = 21;
            this.rExportRaw.TabStop = true;
            this.rExportRaw.Text = "Raw Frames";
            this.rExportRaw.UseVisualStyleBackColor = true;
            this.rExportRaw.Click += new System.EventHandler(this.rExportRaw_Clicked);
            // 
            // rExportPointclouds
            // 
            this.rExportPointclouds.Location = new System.Drawing.Point(198, 3);
            this.rExportPointclouds.Name = "rExportPointclouds";
            this.rExportPointclouds.Size = new System.Drawing.Size(80, 17);
            this.rExportPointclouds.TabIndex = 22;
            this.rExportPointclouds.Text = "Pointclouds";
            this.rExportPointclouds.UseVisualStyleBackColor = true;
            this.rExportPointclouds.Click += new System.EventHandler(this.rExportPointclouds_Clicked);
            // 
            // gvClients
            // 
            this.gvClients.AllowUserToAddRows = false;
            this.gvClients.AllowUserToDeleteRows = false;
            this.gvClients.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gvClients.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.gvClients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvClients.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.SyncState});
            this.gvClients.Location = new System.Drawing.Point(0, 27);
            this.gvClients.MultiSelect = false;
            this.gvClients.Name = "gvClients";
            this.gvClients.ReadOnly = true;
            this.gvClients.RowHeadersWidth = 62;
            this.gvClients.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvClients.Size = new System.Drawing.Size(386, 164);
            this.gvClients.TabIndex = 25;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Column1.HeaderText = "ID";
            this.Column1.MinimumWidth = 8;
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column1.ToolTipText = "The internal Livescan ID";
            this.Column1.Width = 24;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Column2.HeaderText = "Serial Number";
            this.Column2.MinimumWidth = 8;
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column2.ToolTipText = "Device Serial Number, assigned by manufacturer";
            this.Column2.Width = 90;
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Column3.HeaderText = "IP";
            this.Column3.MinimumWidth = 8;
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column3.ToolTipText = "The devices IP adress in the local network";
            this.Column3.Width = 90;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Column4.HeaderText = "Calibrated";
            this.Column4.MinimumWidth = 8;
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column4.ToolTipText = "Is this device spatially calibrated?";
            this.Column4.Width = 60;
            // 
            // SyncState
            // 
            this.SyncState.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.SyncState.HeaderText = "Sync State";
            this.SyncState.MinimumWidth = 8;
            this.SyncState.Name = "SyncState";
            this.SyncState.ReadOnly = true;
            this.SyncState.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.SyncState.ToolTipText = "The current temporal synchronisation state";
            // 
            // glLiveView
            // 
            this.glLiveView.BackColor = System.Drawing.Color.Black;
            this.glLiveView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glLiveView.Location = new System.Drawing.Point(0, 0);
            this.glLiveView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.glLiveView.Name = "glLiveView";
            this.glLiveView.Size = new System.Drawing.Size(642, 666);
            this.glLiveView.TabIndex = 26;
            this.glLiveView.VSync = false;
            this.glLiveView.Load += new System.EventHandler(this.glLiveView_Load);
            this.glLiveView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glLiveView_KeyDown);
            this.glLiveView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glLiveView_MouseDown);
            this.glLiveView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glLiveView_MouseMove);
            this.glLiveView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glLiveView_MouseUp);
            this.glLiveView.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glLiveView_Scroll);
            // 
            // lFPS
            // 
            this.lFPS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lFPS.BackColor = System.Drawing.Color.Black;
            this.lFPS.ForeColor = System.Drawing.SystemColors.Control;
            this.lFPS.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lFPS.Location = new System.Drawing.Point(596, 0);
            this.lFPS.Name = "lFPS";
            this.lFPS.Size = new System.Drawing.Size(46, 23);
            this.lFPS.TabIndex = 27;
            this.lFPS.Text = "30 FPS";
            this.lFPS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chMergeScans
            // 
            this.chMergeScans.AutoSize = true;
            this.chMergeScans.Location = new System.Drawing.Point(210, 24);
            this.chMergeScans.Name = "chMergeScans";
            this.chMergeScans.Size = new System.Drawing.Size(89, 17);
            this.chMergeScans.TabIndex = 30;
            this.chMergeScans.Text = "Merge Scans";
            this.chMergeScans.UseVisualStyleBackColor = true;
            this.chMergeScans.CheckedChanged += new System.EventHandler(this.chMergeScans_CheckedChanged);
            // 
            // lTempSyncHeader
            // 
            this.lTempSyncHeader.AutoSize = true;
            this.lTempSyncHeader.Location = new System.Drawing.Point(2, 28);
            this.lTempSyncHeader.Name = "lTempSyncHeader";
            this.lTempSyncHeader.Size = new System.Drawing.Size(81, 13);
            this.lTempSyncHeader.TabIndex = 31;
            this.lTempSyncHeader.Text = "Temporal Sync:";
            // 
            // chHardwareSync
            // 
            this.chHardwareSync.Location = new System.Drawing.Point(91, 27);
            this.chHardwareSync.Name = "chHardwareSync";
            this.chHardwareSync.Size = new System.Drawing.Size(100, 17);
            this.chHardwareSync.TabIndex = 32;
            this.chHardwareSync.Text = "Hardware Sync";
            this.chHardwareSync.UseVisualStyleBackColor = true;
            this.chHardwareSync.Click += new System.EventHandler(this.chHardwareSync_Clicked);
            // 
            // chNetworkSync
            // 
            this.chNetworkSync.Location = new System.Drawing.Point(221, 28);
            this.chNetworkSync.Name = "chNetworkSync";
            this.chNetworkSync.Size = new System.Drawing.Size(94, 17);
            this.chNetworkSync.TabIndex = 33;
            this.chNetworkSync.Text = "Network Sync";
            this.chNetworkSync.UseVisualStyleBackColor = true;
            this.chNetworkSync.Click += new System.EventHandler(this.chNetworkSync_Clicked);
            // 
            // lExposure
            // 
            this.lExposure.AutoSize = true;
            this.lExposure.Location = new System.Drawing.Point(2, 57);
            this.lExposure.Name = "lExposure";
            this.lExposure.Size = new System.Drawing.Size(54, 13);
            this.lExposure.TabIndex = 36;
            this.lExposure.Text = "Exposure:";
            // 
            // rExposureAuto
            // 
            this.rExposureAuto.AutoSize = true;
            this.rExposureAuto.Location = new System.Drawing.Point(10, 5);
            this.rExposureAuto.Name = "rExposureAuto";
            this.rExposureAuto.Size = new System.Drawing.Size(47, 17);
            this.rExposureAuto.TabIndex = 37;
            this.rExposureAuto.TabStop = true;
            this.rExposureAuto.Text = "Auto";
            this.rExposureAuto.UseVisualStyleBackColor = true;
            this.rExposureAuto.Click += new System.EventHandler(this.rExposureAuto_Clicked);
            // 
            // rExposureManual
            // 
            this.rExposureManual.AutoSize = true;
            this.rExposureManual.Location = new System.Drawing.Point(65, 5);
            this.rExposureManual.Name = "rExposureManual";
            this.rExposureManual.Size = new System.Drawing.Size(60, 17);
            this.rExposureManual.TabIndex = 38;
            this.rExposureManual.TabStop = true;
            this.rExposureManual.Text = "Manual";
            this.rExposureManual.UseVisualStyleBackColor = true;
            this.rExposureManual.Click += new System.EventHandler(this.rExposureManual_Clicked);
            // 
            // trManualExposure
            // 
            this.trManualExposure.Enabled = false;
            this.trManualExposure.LargeChange = 1;
            this.trManualExposure.Location = new System.Drawing.Point(233, 51);
            this.trManualExposure.Maximum = -5;
            this.trManualExposure.Minimum = -11;
            this.trManualExposure.Name = "trManualExposure";
            this.trManualExposure.Size = new System.Drawing.Size(154, 45);
            this.trManualExposure.TabIndex = 39;
            this.trManualExposure.Value = -5;
            this.trManualExposure.Scroll += new System.EventHandler(this.trManualExposure_Scroll);
            // 
            // lPerformance
            // 
            this.lPerformance.AutoSize = true;
            this.lPerformance.Location = new System.Drawing.Point(3, 129);
            this.lPerformance.Name = "lPerformance";
            this.lPerformance.Size = new System.Drawing.Size(70, 13);
            this.lPerformance.TabIndex = 41;
            this.lPerformance.Text = "Performance:";
            // 
            // chEnablePreview
            // 
            this.chEnablePreview.AutoSize = true;
            this.chEnablePreview.Location = new System.Drawing.Point(77, 128);
            this.chEnablePreview.Margin = new System.Windows.Forms.Padding(2);
            this.chEnablePreview.Name = "chEnablePreview";
            this.chEnablePreview.Size = new System.Drawing.Size(198, 17);
            this.chEnablePreview.TabIndex = 42;
            this.chEnablePreview.Text = "Enable client preview during capture";
            this.chEnablePreview.UseVisualStyleBackColor = true;
            this.chEnablePreview.CheckedChanged += new System.EventHandler(this.cbEnablePreview_Clicked);
            // 
            // tooltips
            // 
            this.tooltips.AutomaticDelay = 0;
            this.tooltips.AutoPopDelay = 0;
            this.tooltips.InitialDelay = 0;
            this.tooltips.IsBalloon = true;
            this.tooltips.ReshowDelay = 0;
            // 
            // pInfoClientPreview
            // 
            this.pInfoClientPreview.Image = global::KinectServer.Properties.Resources.info_box;
            this.pInfoClientPreview.Location = new System.Drawing.Point(271, 128);
            this.pInfoClientPreview.Name = "pInfoClientPreview";
            this.pInfoClientPreview.Size = new System.Drawing.Size(15, 15);
            this.pInfoClientPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoClientPreview.TabIndex = 43;
            this.pInfoClientPreview.TabStop = false;
            this.tooltips.SetToolTip(this.pInfoClientPreview, "When disabled, clients will not show a preview picture during capture. Might incr" +
        "ease capture performance");
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::KinectServer.Properties.Resources.info_box;
            this.pictureBox1.Location = new System.Drawing.Point(186, 28);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(15, 15);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 34;
            this.pictureBox1.TabStop = false;
            this.tooltips.SetToolTip(this.pictureBox1, "Before activating, make sure that all Kinects are connected to the server and the" +
        " sync cables are set up in the right order");
            // 
            // PInfoExposure
            // 
            this.PInfoExposure.Image = global::KinectServer.Properties.Resources.info_box;
            this.PInfoExposure.Location = new System.Drawing.Point(57, 57);
            this.PInfoExposure.Name = "PInfoExposure";
            this.PInfoExposure.Size = new System.Drawing.Size(15, 15);
            this.PInfoExposure.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PInfoExposure.TabIndex = 40;
            this.PInfoExposure.TabStop = false;
            this.tooltips.SetToolTip(this.PInfoExposure, "Sets the exposure on all cameras. When Hardware Sync is enabled, manual mode is r" +
        "equired");
            // 
            // pInfoNetworkSync
            // 
            this.pInfoNetworkSync.Image = global::KinectServer.Properties.Resources.info_box;
            this.pInfoNetworkSync.Location = new System.Drawing.Point(310, 28);
            this.pInfoNetworkSync.Name = "pInfoNetworkSync";
            this.pInfoNetworkSync.Size = new System.Drawing.Size(15, 15);
            this.pInfoNetworkSync.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoNetworkSync.TabIndex = 35;
            this.pInfoNetworkSync.TabStop = false;
            this.tooltips.SetToolTip(this.pInfoNetworkSync, "No special hardware configuration needed. Might give more consistent frame timing" +
        "s between the devices, but reduces the framerate");
            // 
            // pInfoRefineCalib
            // 
            this.pInfoRefineCalib.Image = global::KinectServer.Properties.Resources.info_box;
            this.pInfoRefineCalib.Location = new System.Drawing.Point(255, 26);
            this.pInfoRefineCalib.Name = "pInfoRefineCalib";
            this.pInfoRefineCalib.Size = new System.Drawing.Size(15, 15);
            this.pInfoRefineCalib.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoRefineCalib.TabIndex = 24;
            this.pInfoRefineCalib.TabStop = false;
            this.tooltips.SetToolTip(this.pInfoRefineCalib, "Refines the calibration with an ICP algorithm. The cameras field of view needs to" +
        " be overlapping");
            // 
            // pInfoCalibrate
            // 
            this.pInfoCalibrate.Image = global::KinectServer.Properties.Resources.info_box;
            this.pInfoCalibrate.Location = new System.Drawing.Point(103, 27);
            this.pInfoCalibrate.Name = "pInfoCalibrate";
            this.pInfoCalibrate.Size = new System.Drawing.Size(15, 15);
            this.pInfoCalibrate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoCalibrate.TabIndex = 23;
            this.pInfoCalibrate.TabStop = false;
            this.tooltips.SetToolTip(this.pInfoCalibrate, "Calibrate all cameras spatially. At least one marker needs to be visible to all c" +
        "ameras");
            // 
            // pInfoRawFrames
            // 
            this.pInfoRawFrames.Image = global::KinectServer.Properties.Resources.info_box;
            this.pInfoRawFrames.Location = new System.Drawing.Point(92, 4);
            this.pInfoRawFrames.Name = "pInfoRawFrames";
            this.pInfoRawFrames.Size = new System.Drawing.Size(15, 15);
            this.pInfoRawFrames.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoRawFrames.TabIndex = 28;
            this.pInfoRawFrames.TabStop = false;
            this.tooltips.SetToolTip(this.pInfoRawFrames, "Save recording as color (.jpg) and depth (.tiff) frames. Best capture performance" +
        " and maximum quality, but requires postprocessing");
            // 
            // pInfoPointclouds
            // 
            this.pInfoPointclouds.Image = global::KinectServer.Properties.Resources.info_box;
            this.pInfoPointclouds.Location = new System.Drawing.Point(278, 3);
            this.pInfoPointclouds.Name = "pInfoPointclouds";
            this.pInfoPointclouds.Size = new System.Drawing.Size(15, 15);
            this.pInfoPointclouds.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoPointclouds.TabIndex = 29;
            this.pInfoPointclouds.TabStop = false;
            this.tooltips.SetToolTip(this.pInfoPointclouds, resources.GetString("pInfoPointclouds.ToolTip"));
            // 
            // pInfoWhiteBalance
            // 
            this.pInfoWhiteBalance.Image = global::KinectServer.Properties.Resources.info_box;
            this.pInfoWhiteBalance.Location = new System.Drawing.Point(81, 92);
            this.pInfoWhiteBalance.Name = "pInfoWhiteBalance";
            this.pInfoWhiteBalance.Size = new System.Drawing.Size(15, 15);
            this.pInfoWhiteBalance.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoWhiteBalance.TabIndex = 47;
            this.pInfoWhiteBalance.TabStop = false;
            this.tooltips.SetToolTip(this.pInfoWhiteBalance, "Sets the exposure on all cameras. When Hardware Sync is enabled, manual mode is r" +
        "equired");
            // 
            // gbExposure
            // 
            this.gbExposure.Controls.Add(this.rExposureManual);
            this.gbExposure.Controls.Add(this.rExposureAuto);
            this.gbExposure.Location = new System.Drawing.Point(103, 49);
            this.gbExposure.Name = "gbExposure";
            this.gbExposure.Size = new System.Drawing.Size(128, 27);
            this.gbExposure.TabIndex = 44;
            // 
            // gbFrameExport
            // 
            this.gbFrameExport.Controls.Add(this.rExportRaw);
            this.gbFrameExport.Controls.Add(this.rExportPointclouds);
            this.gbFrameExport.Controls.Add(this.pInfoRawFrames);
            this.gbFrameExport.Controls.Add(this.pInfoPointclouds);
            this.gbFrameExport.Controls.Add(this.chMergeScans);
            this.gbFrameExport.Location = new System.Drawing.Point(3, 52);
            this.gbFrameExport.Name = "gbFrameExport";
            this.gbFrameExport.Size = new System.Drawing.Size(302, 45);
            this.gbFrameExport.TabIndex = 45;
            // 
            // tlMainPanel
            // 
            this.tlMainPanel.ColumnCount = 2;
            this.tlMainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 400F));
            this.tlMainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlMainPanel.Controls.Add(this.pnLiveView, 1, 0);
            this.tlMainPanel.Controls.Add(this.tlControls, 0, 0);
            this.tlMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlMainPanel.Location = new System.Drawing.Point(0, 0);
            this.tlMainPanel.Name = "tlMainPanel";
            this.tlMainPanel.RowCount = 1;
            this.tlMainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlMainPanel.Size = new System.Drawing.Size(1048, 672);
            this.tlMainPanel.TabIndex = 46;
            // 
            // pnLiveView
            // 
            this.pnLiveView.Controls.Add(this.lFPS);
            this.pnLiveView.Controls.Add(this.glLiveView);
            this.pnLiveView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnLiveView.Location = new System.Drawing.Point(403, 3);
            this.pnLiveView.Name = "pnLiveView";
            this.pnLiveView.Size = new System.Drawing.Size(642, 666);
            this.pnLiveView.TabIndex = 0;
            // 
            // tlControls
            // 
            this.tlControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlControls.ColumnCount = 1;
            this.tlControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlControls.Controls.Add(this.pnClients, 0, 0);
            this.tlControls.Controls.Add(this.pnSettings, 0, 2);
            this.tlControls.Controls.Add(this.pnCalibration, 0, 1);
            this.tlControls.Controls.Add(this.btRecord, 0, 5);
            this.tlControls.Controls.Add(this.pnCapture, 0, 4);
            this.tlControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlControls.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tlControls.Location = new System.Drawing.Point(3, 3);
            this.tlControls.Name = "tlControls";
            this.tlControls.RowCount = 6;
            this.tlControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 235F));
            this.tlControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 188F));
            this.tlControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tlControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tlControls.Size = new System.Drawing.Size(394, 666);
            this.tlControls.TabIndex = 1;
            // 
            // pnClients
            // 
            this.pnClients.Controls.Add(this.lClientsHeader);
            this.pnClients.Controls.Add(this.gvClients);
            this.pnClients.Controls.Add(this.btKinectSettingsOpenButton);
            this.pnClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnClients.Location = new System.Drawing.Point(3, 3);
            this.pnClients.Name = "pnClients";
            this.pnClients.Size = new System.Drawing.Size(388, 229);
            this.pnClients.TabIndex = 0;
            // 
            // pnSettings
            // 
            this.pnSettings.Controls.Add(this.gbWhiteBalance);
            this.pnSettings.Controls.Add(this.pInfoWhiteBalance);
            this.pnSettings.Controls.Add(this.trWhiteBalance);
            this.pnSettings.Controls.Add(this.lWhiteBalance);
            this.pnSettings.Controls.Add(this.gbExposure);
            this.pnSettings.Controls.Add(this.pInfoClientPreview);
            this.pnSettings.Controls.Add(this.btSettings);
            this.pnSettings.Controls.Add(this.chEnablePreview);
            this.pnSettings.Controls.Add(this.lSettingsHeader);
            this.pnSettings.Controls.Add(this.pictureBox1);
            this.pnSettings.Controls.Add(this.lPerformance);
            this.pnSettings.Controls.Add(this.PInfoExposure);
            this.pnSettings.Controls.Add(this.trManualExposure);
            this.pnSettings.Controls.Add(this.lTempSyncHeader);
            this.pnSettings.Controls.Add(this.lExposure);
            this.pnSettings.Controls.Add(this.chHardwareSync);
            this.pnSettings.Controls.Add(this.pInfoNetworkSync);
            this.pnSettings.Controls.Add(this.chNetworkSync);
            this.pnSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnSettings.Location = new System.Drawing.Point(3, 298);
            this.pnSettings.Name = "pnSettings";
            this.pnSettings.Size = new System.Drawing.Size(388, 182);
            this.pnSettings.TabIndex = 1;
            // 
            // gbWhiteBalance
            // 
            this.gbWhiteBalance.Controls.Add(this.rWhiteBalanceManual);
            this.gbWhiteBalance.Controls.Add(this.rWhiteBalanceAuto);
            this.gbWhiteBalance.Location = new System.Drawing.Point(103, 83);
            this.gbWhiteBalance.Name = "gbWhiteBalance";
            this.gbWhiteBalance.Size = new System.Drawing.Size(128, 27);
            this.gbWhiteBalance.TabIndex = 48;
            // 
            // rWhiteBalanceManual
            // 
            this.rWhiteBalanceManual.AutoSize = true;
            this.rWhiteBalanceManual.Location = new System.Drawing.Point(65, 5);
            this.rWhiteBalanceManual.Name = "rWhiteBalanceManual";
            this.rWhiteBalanceManual.Size = new System.Drawing.Size(60, 17);
            this.rWhiteBalanceManual.TabIndex = 38;
            this.rWhiteBalanceManual.TabStop = true;
            this.rWhiteBalanceManual.Text = "Manual";
            this.rWhiteBalanceManual.UseVisualStyleBackColor = true;
            this.rWhiteBalanceManual.CheckedChanged += new System.EventHandler(this.rWhiteBalanceManual_CheckedChanged);
            // 
            // rWhiteBalanceAuto
            // 
            this.rWhiteBalanceAuto.AutoSize = true;
            this.rWhiteBalanceAuto.Location = new System.Drawing.Point(10, 5);
            this.rWhiteBalanceAuto.Name = "rWhiteBalanceAuto";
            this.rWhiteBalanceAuto.Size = new System.Drawing.Size(47, 17);
            this.rWhiteBalanceAuto.TabIndex = 37;
            this.rWhiteBalanceAuto.TabStop = true;
            this.rWhiteBalanceAuto.Text = "Auto";
            this.rWhiteBalanceAuto.UseVisualStyleBackColor = true;
            this.rWhiteBalanceAuto.CheckedChanged += new System.EventHandler(this.rWhiteBalanceAuto_CheckedChanged);
            // 
            // trWhiteBalance
            // 
            this.trWhiteBalance.Enabled = false;
            this.trWhiteBalance.LargeChange = 4;
            this.trWhiteBalance.Location = new System.Drawing.Point(233, 82);
            this.trWhiteBalance.Maximum = 18;
            this.trWhiteBalance.Minimum = 5;
            this.trWhiteBalance.Name = "trWhiteBalance";
            this.trWhiteBalance.Size = new System.Drawing.Size(154, 45);
            this.trWhiteBalance.TabIndex = 46;
            this.trWhiteBalance.Value = 8;
            this.trWhiteBalance.Scroll += new System.EventHandler(this.tbWhiteBalance_Scroll);
            // 
            // lWhiteBalance
            // 
            this.lWhiteBalance.AutoSize = true;
            this.lWhiteBalance.Location = new System.Drawing.Point(2, 92);
            this.lWhiteBalance.Name = "lWhiteBalance";
            this.lWhiteBalance.Size = new System.Drawing.Size(80, 13);
            this.lWhiteBalance.TabIndex = 45;
            this.lWhiteBalance.Text = "White Balance:";
            // 
            // pnCalibration
            // 
            this.pnCalibration.Controls.Add(this.lCalibrationHeader);
            this.pnCalibration.Controls.Add(this.btRefineCalib);
            this.pnCalibration.Controls.Add(this.btCalibrate);
            this.pnCalibration.Controls.Add(this.pInfoRefineCalib);
            this.pnCalibration.Controls.Add(this.pInfoCalibrate);
            this.pnCalibration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnCalibration.Location = new System.Drawing.Point(3, 238);
            this.pnCalibration.Name = "pnCalibration";
            this.pnCalibration.Size = new System.Drawing.Size(388, 54);
            this.pnCalibration.TabIndex = 5;
            // 
            // btRecord
            // 
            this.btRecord.AutoSize = true;
            this.btRecord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btRecord.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btRecord.Image = global::KinectServer.Properties.Resources.recording;
            this.btRecord.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btRecord.Location = new System.Drawing.Point(3, 619);
            this.btRecord.MinimumSize = new System.Drawing.Size(0, 30);
            this.btRecord.Name = "btRecord";
            this.btRecord.Size = new System.Drawing.Size(388, 44);
            this.btRecord.TabIndex = 4;
            this.btRecord.Text = "  Start Capture";
            this.btRecord.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btRecord.UseVisualStyleBackColor = true;
            this.btRecord.Click += new System.EventHandler(this.btRecord_Click);
            // 
            // pnCapture
            // 
            this.pnCapture.Controls.Add(this.lCaptureHeader);
            this.pnCapture.Controls.Add(this.gbFrameExport);
            this.pnCapture.Controls.Add(this.txtSeqName);
            this.pnCapture.Controls.Add(this.lbSeqName);
            this.pnCapture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnCapture.Location = new System.Drawing.Point(3, 509);
            this.pnCapture.Name = "pnCapture";
            this.pnCapture.Size = new System.Drawing.Size(388, 104);
            this.pnCapture.TabIndex = 2;
            // 
            // MainWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1048, 694);
            this.Controls.Add(this.tlMainPanel);
            this.Controls.Add(this.statusStrip1);
            this.MinimumSize = new System.Drawing.Size(417, 662);
            this.Name = "MainWindowForm";
            this.Text = "LiveScanServer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.MainWindowForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvClients)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trManualExposure)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoClientPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PInfoExposure)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoNetworkSync)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoRefineCalib)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoCalibrate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoRawFrames)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoPointclouds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoWhiteBalance)).EndInit();
            this.gbExposure.ResumeLayout(false);
            this.gbExposure.PerformLayout();
            this.gbFrameExport.ResumeLayout(false);
            this.gbFrameExport.PerformLayout();
            this.tlMainPanel.ResumeLayout(false);
            this.pnLiveView.ResumeLayout(false);
            this.tlControls.ResumeLayout(false);
            this.tlControls.PerformLayout();
            this.pnClients.ResumeLayout(false);
            this.pnClients.PerformLayout();
            this.pnSettings.ResumeLayout(false);
            this.pnSettings.PerformLayout();
            this.gbWhiteBalance.ResumeLayout(false);
            this.gbWhiteBalance.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trWhiteBalance)).EndInit();
            this.pnCalibration.ResumeLayout(false);
            this.pnCalibration.PerformLayout();
            this.pnCapture.ResumeLayout(false);
            this.pnCapture.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btCalibrate;
        private System.Windows.Forms.Button btRecord;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TextBox txtSeqName;
        private System.Windows.Forms.Button btRefineCalib;
        private System.ComponentModel.BackgroundWorker OpenGLWorker;
        private System.Windows.Forms.Button btSettings;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.Label lbSeqName;
        private System.Windows.Forms.Button btKinectSettingsOpenButton;
        private System.Windows.Forms.Label lClientsHeader;
        private System.Windows.Forms.Label lCalibrationHeader;
        private System.Windows.Forms.Label lSettingsHeader;
        private System.Windows.Forms.Label lCaptureHeader;
        private System.Windows.Forms.RadioButton rExportRaw;
        private System.Windows.Forms.RadioButton rExportPointclouds;
        private System.Windows.Forms.PictureBox pInfoCalibrate;
        private System.Windows.Forms.PictureBox pInfoRefineCalib;
        private System.Windows.Forms.DataGridView gvClients;
        private OpenTK.GLControl glLiveView;
        private System.Windows.Forms.Label lFPS;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn SyncState;
        private System.Windows.Forms.PictureBox pInfoRawFrames;
        private System.Windows.Forms.PictureBox pInfoPointclouds;
        private System.Windows.Forms.CheckBox chMergeScans;
        private System.Windows.Forms.Label lTempSyncHeader;
        private System.Windows.Forms.CheckBox chHardwareSync;
        private System.Windows.Forms.CheckBox chNetworkSync;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pInfoNetworkSync;
        private System.Windows.Forms.Label lExposure;
        private System.Windows.Forms.RadioButton rExposureAuto;
        private System.Windows.Forms.RadioButton rExposureManual;
        private System.Windows.Forms.TrackBar trManualExposure;
        private System.Windows.Forms.PictureBox PInfoExposure;
        private System.Windows.Forms.Label lPerformance;
        private System.Windows.Forms.CheckBox chEnablePreview;
        private System.Windows.Forms.ToolTip tooltips;
        private System.Windows.Forms.PictureBox pInfoClientPreview;
        private System.Windows.Forms.Panel gbExposure;
        private System.Windows.Forms.Panel gbFrameExport;
        private System.Windows.Forms.TableLayoutPanel tlMainPanel;
        private System.Windows.Forms.Panel pnLiveView;
        private System.Windows.Forms.TableLayoutPanel tlControls;
        private System.Windows.Forms.Panel pnClients;
        private System.Windows.Forms.Panel pnSettings;
        private System.Windows.Forms.Panel pnCapture;
        private System.Windows.Forms.Panel pnCalibration;
        private System.Windows.Forms.Panel gbWhiteBalance;
        private System.Windows.Forms.RadioButton rWhiteBalanceManual;
        private System.Windows.Forms.RadioButton rWhiteBalanceAuto;
        private System.Windows.Forms.PictureBox pInfoWhiteBalance;
        private System.Windows.Forms.TrackBar trWhiteBalance;
        private System.Windows.Forms.Label lWhiteBalance;
    }
}

