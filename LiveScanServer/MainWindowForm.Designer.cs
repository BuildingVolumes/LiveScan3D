namespace LiveScanServer
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
            this.txtSeqName = new System.Windows.Forms.TextBox();
            this.btRefineCalib = new System.Windows.Forms.Button();
            this.btSettings = new System.Windows.Forms.Button();
            this.lbSeqName = new System.Windows.Forms.Label();
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
            this.Visible = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Configuration = new System.Windows.Forms.DataGridViewButtonColumn();
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
            this.tlLiveView = new System.Windows.Forms.TableLayoutPanel();
            this.tlStatusInfo = new System.Windows.Forms.TableLayoutPanel();
            this.lStateIndicator = new System.Windows.Forms.Label();
            this.pStatusIndicator = new System.Windows.Forms.PictureBox();
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
            this.tlLiveView.SuspendLayout();
            this.tlStatusInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pStatusIndicator)).BeginInit();
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
            this.btCalibrate.Location = new System.Drawing.Point(7, 28);
            this.btCalibrate.Margin = new System.Windows.Forms.Padding(4);
            this.btCalibrate.Name = "btCalibrate";
            this.btCalibrate.Size = new System.Drawing.Size(127, 28);
            this.btCalibrate.TabIndex = 2;
            this.btCalibrate.Text = "Calibrate";
            this.btCalibrate.UseVisualStyleBackColor = true;
            this.btCalibrate.Click += new System.EventHandler(this.btCalibrate_Click);
            // 
            // txtSeqName
            // 
            this.txtSeqName.Location = new System.Drawing.Point(128, 32);
            this.txtSeqName.Margin = new System.Windows.Forms.Padding(4);
            this.txtSeqName.MaxLength = 40;
            this.txtSeqName.Name = "txtSeqName";
            this.txtSeqName.Size = new System.Drawing.Size(384, 22);
            this.txtSeqName.TabIndex = 7;
            this.txtSeqName.Text = "Capture";
            // 
            // btRefineCalib
            // 
            this.btRefineCalib.Location = new System.Drawing.Point(187, 28);
            this.btRefineCalib.Margin = new System.Windows.Forms.Padding(4);
            this.btRefineCalib.Name = "btRefineCalib";
            this.btRefineCalib.Size = new System.Drawing.Size(149, 28);
            this.btRefineCalib.TabIndex = 11;
            this.btRefineCalib.Text = "Refine Calibration";
            this.btRefineCalib.UseVisualStyleBackColor = true;
            this.btRefineCalib.Click += new System.EventHandler(this.btRefineCalib_Click);
            // 
            // btSettings
            // 
            this.btSettings.Location = new System.Drawing.Point(389, 191);
            this.btSettings.Margin = new System.Windows.Forms.Padding(4);
            this.btSettings.Name = "btSettings";
            this.btSettings.Size = new System.Drawing.Size(127, 28);
            this.btSettings.TabIndex = 13;
            this.btSettings.Text = "More Settings";
            this.btSettings.UseVisualStyleBackColor = true;
            this.btSettings.Click += new System.EventHandler(this.btSettings_Click);
            // 
            // lbSeqName
            // 
            this.lbSeqName.AutoSize = true;
            this.lbSeqName.Location = new System.Drawing.Point(3, 36);
            this.lbSeqName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbSeqName.Name = "lbSeqName";
            this.lbSeqName.Size = new System.Drawing.Size(112, 16);
            this.lbSeqName.TabIndex = 14;
            this.lbSeqName.Text = "Sequence Name:";
            // 
            // lClientsHeader
            // 
            this.lClientsHeader.AutoSize = true;
            this.lClientsHeader.Dock = System.Windows.Forms.DockStyle.Left;
            this.lClientsHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lClientsHeader.Location = new System.Drawing.Point(0, 0);
            this.lClientsHeader.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lClientsHeader.Name = "lClientsHeader";
            this.lClientsHeader.Size = new System.Drawing.Size(60, 18);
            this.lClientsHeader.TabIndex = 17;
            this.lClientsHeader.Text = "Clients";
            // 
            // lCalibrationHeader
            // 
            this.lCalibrationHeader.AutoSize = true;
            this.lCalibrationHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lCalibrationHeader.Location = new System.Drawing.Point(0, 0);
            this.lCalibrationHeader.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lCalibrationHeader.Name = "lCalibrationHeader";
            this.lCalibrationHeader.Size = new System.Drawing.Size(89, 18);
            this.lCalibrationHeader.TabIndex = 18;
            this.lCalibrationHeader.Text = "Calibration";
            // 
            // lSettingsHeader
            // 
            this.lSettingsHeader.AutoSize = true;
            this.lSettingsHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lSettingsHeader.Location = new System.Drawing.Point(0, 0);
            this.lSettingsHeader.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lSettingsHeader.Name = "lSettingsHeader";
            this.lSettingsHeader.Size = new System.Drawing.Size(69, 18);
            this.lSettingsHeader.TabIndex = 19;
            this.lSettingsHeader.Text = "Settings";
            // 
            // lCaptureHeader
            // 
            this.lCaptureHeader.AutoSize = true;
            this.lCaptureHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lCaptureHeader.Location = new System.Drawing.Point(3, 0);
            this.lCaptureHeader.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lCaptureHeader.Name = "lCaptureHeader";
            this.lCaptureHeader.Size = new System.Drawing.Size(67, 18);
            this.lCaptureHeader.TabIndex = 20;
            this.lCaptureHeader.Text = "Capture";
            // 
            // rExportRaw
            // 
            this.rExportRaw.Checked = true;
            this.rExportRaw.Location = new System.Drawing.Point(11, 4);
            this.rExportRaw.Margin = new System.Windows.Forms.Padding(4);
            this.rExportRaw.Name = "rExportRaw";
            this.rExportRaw.Size = new System.Drawing.Size(112, 21);
            this.rExportRaw.TabIndex = 21;
            this.rExportRaw.TabStop = true;
            this.rExportRaw.Text = "Raw Frames";
            this.rExportRaw.UseVisualStyleBackColor = true;
            this.rExportRaw.Click += new System.EventHandler(this.rExportRaw_Clicked);
            // 
            // rExportPointclouds
            // 
            this.rExportPointclouds.Location = new System.Drawing.Point(264, 4);
            this.rExportPointclouds.Margin = new System.Windows.Forms.Padding(4);
            this.rExportPointclouds.Name = "rExportPointclouds";
            this.rExportPointclouds.Size = new System.Drawing.Size(107, 21);
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
            this.SyncState,
            this.Visible,
            this.Configuration});
            this.gvClients.Location = new System.Drawing.Point(0, 33);
            this.gvClients.Margin = new System.Windows.Forms.Padding(4);
            this.gvClients.MultiSelect = false;
            this.gvClients.Name = "gvClients";
            this.gvClients.RowHeadersVisible = false;
            this.gvClients.RowHeadersWidth = 62;
            this.gvClients.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvClients.Size = new System.Drawing.Size(515, 202);
            this.gvClients.TabIndex = 25;
            this.gvClients.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvClients_CellContentClick);
            this.gvClients.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvClients_CellEndEdit);
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
            this.Column2.HeaderText = "Name";
            this.Column2.MinimumWidth = 8;
            this.Column2.Name = "Column2";
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column2.ToolTipText = "Given nickname of the device, if not set shows Serial Number";
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
            this.Column4.Resizable = System.Windows.Forms.DataGridViewTriState.True;
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
            // Visible
            // 
            this.Visible.HeaderText = "👁️";
            this.Visible.MinimumWidth = 6;
            this.Visible.Name = "Visible";
            this.Visible.ToolTipText = "Hide/Show the clients pointcloud in the live preview";
            this.Visible.Width = 27;
            // 
            // Configuration
            // 
            this.Configuration.HeaderText = "";
            this.Configuration.MinimumWidth = 6;
            this.Configuration.Name = "Configuration";
            this.Configuration.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Configuration.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Configuration.Width = 25;
            // 
            // glLiveView
            // 
            this.glLiveView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(0)))), ((int)(((byte)(25)))));
            this.glLiveView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glLiveView.Location = new System.Drawing.Point(5, 36);
            this.glLiveView.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.glLiveView.Name = "glLiveView";
            this.glLiveView.Size = new System.Drawing.Size(846, 804);
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
            this.lFPS.BackColor = System.Drawing.SystemColors.Control;
            this.lFPS.Dock = System.Windows.Forms.DockStyle.Right;
            this.lFPS.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lFPS.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lFPS.Location = new System.Drawing.Point(747, 0);
            this.lFPS.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lFPS.Name = "lFPS";
            this.lFPS.Size = new System.Drawing.Size(97, 22);
            this.lFPS.TabIndex = 27;
            this.lFPS.Text = "30 FPS";
            this.lFPS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chMergeScans
            // 
            this.chMergeScans.AutoSize = true;
            this.chMergeScans.Location = new System.Drawing.Point(280, 30);
            this.chMergeScans.Margin = new System.Windows.Forms.Padding(4);
            this.chMergeScans.Name = "chMergeScans";
            this.chMergeScans.Size = new System.Drawing.Size(109, 20);
            this.chMergeScans.TabIndex = 30;
            this.chMergeScans.Text = "Merge Scans";
            this.chMergeScans.UseVisualStyleBackColor = true;
            this.chMergeScans.CheckedChanged += new System.EventHandler(this.chMergeScans_CheckedChanged);
            // 
            // lTempSyncHeader
            // 
            this.lTempSyncHeader.AutoSize = true;
            this.lTempSyncHeader.Location = new System.Drawing.Point(3, 34);
            this.lTempSyncHeader.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lTempSyncHeader.Name = "lTempSyncHeader";
            this.lTempSyncHeader.Size = new System.Drawing.Size(102, 16);
            this.lTempSyncHeader.TabIndex = 31;
            this.lTempSyncHeader.Text = "Temporal Sync:";
            // 
            // chHardwareSync
            // 
            this.chHardwareSync.Location = new System.Drawing.Point(121, 33);
            this.chHardwareSync.Margin = new System.Windows.Forms.Padding(4);
            this.chHardwareSync.Name = "chHardwareSync";
            this.chHardwareSync.Size = new System.Drawing.Size(133, 21);
            this.chHardwareSync.TabIndex = 32;
            this.chHardwareSync.Text = "Hardware Sync";
            this.chHardwareSync.UseVisualStyleBackColor = true;
            this.chHardwareSync.Click += new System.EventHandler(this.chHardwareSync_Clicked);
            // 
            // chNetworkSync
            // 
            this.chNetworkSync.Location = new System.Drawing.Point(295, 34);
            this.chNetworkSync.Margin = new System.Windows.Forms.Padding(4);
            this.chNetworkSync.Name = "chNetworkSync";
            this.chNetworkSync.Size = new System.Drawing.Size(125, 21);
            this.chNetworkSync.TabIndex = 33;
            this.chNetworkSync.Text = "Network Sync";
            this.chNetworkSync.UseVisualStyleBackColor = true;
            this.chNetworkSync.Click += new System.EventHandler(this.chNetworkSync_Clicked);
            // 
            // lExposure
            // 
            this.lExposure.AutoSize = true;
            this.lExposure.Location = new System.Drawing.Point(3, 70);
            this.lExposure.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lExposure.Name = "lExposure";
            this.lExposure.Size = new System.Drawing.Size(67, 16);
            this.lExposure.TabIndex = 36;
            this.lExposure.Text = "Exposure:";
            // 
            // rExposureAuto
            // 
            this.rExposureAuto.AutoSize = true;
            this.rExposureAuto.Location = new System.Drawing.Point(13, 6);
            this.rExposureAuto.Margin = new System.Windows.Forms.Padding(4);
            this.rExposureAuto.Name = "rExposureAuto";
            this.rExposureAuto.Size = new System.Drawing.Size(55, 20);
            this.rExposureAuto.TabIndex = 37;
            this.rExposureAuto.TabStop = true;
            this.rExposureAuto.Text = "Auto";
            this.rExposureAuto.UseVisualStyleBackColor = true;
            this.rExposureAuto.Click += new System.EventHandler(this.rExposureAuto_Clicked);
            // 
            // rExposureManual
            // 
            this.rExposureManual.AutoSize = true;
            this.rExposureManual.Location = new System.Drawing.Point(87, 6);
            this.rExposureManual.Margin = new System.Windows.Forms.Padding(4);
            this.rExposureManual.Name = "rExposureManual";
            this.rExposureManual.Size = new System.Drawing.Size(72, 20);
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
            this.trManualExposure.Location = new System.Drawing.Point(311, 63);
            this.trManualExposure.Margin = new System.Windows.Forms.Padding(4);
            this.trManualExposure.Maximum = -5;
            this.trManualExposure.Minimum = -11;
            this.trManualExposure.Name = "trManualExposure";
            this.trManualExposure.Size = new System.Drawing.Size(205, 56);
            this.trManualExposure.TabIndex = 39;
            this.trManualExposure.Value = -5;
            this.trManualExposure.Scroll += new System.EventHandler(this.trManualExposure_Scroll);
            // 
            // lPerformance
            // 
            this.lPerformance.AutoSize = true;
            this.lPerformance.Location = new System.Drawing.Point(4, 159);
            this.lPerformance.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lPerformance.Name = "lPerformance";
            this.lPerformance.Size = new System.Drawing.Size(87, 16);
            this.lPerformance.TabIndex = 41;
            this.lPerformance.Text = "Performance:";
            // 
            // chEnablePreview
            // 
            this.chEnablePreview.AutoSize = true;
            this.chEnablePreview.Location = new System.Drawing.Point(103, 158);
            this.chEnablePreview.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chEnablePreview.Name = "chEnablePreview";
            this.chEnablePreview.Size = new System.Drawing.Size(244, 20);
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
            this.pInfoClientPreview.Location = new System.Drawing.Point(361, 158);
            this.pInfoClientPreview.Margin = new System.Windows.Forms.Padding(4);
            this.pInfoClientPreview.Name = "pInfoClientPreview";
            this.pInfoClientPreview.Size = new System.Drawing.Size(20, 18);
            this.pInfoClientPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoClientPreview.TabIndex = 43;
            this.pInfoClientPreview.TabStop = false;
            this.tooltips.SetToolTip(this.pInfoClientPreview, "When disabled, clients will not show a preview picture during capture. Might incr" +
        "ease capture performance");
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(248, 34);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(20, 18);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 34;
            this.pictureBox1.TabStop = false;
            this.tooltips.SetToolTip(this.pictureBox1, "Before activating, make sure that all Kinects are connected to the server and the" +
        " sync cables are set up in the right order");
            // 
            // PInfoExposure
            // 
            this.PInfoExposure.Location = new System.Drawing.Point(76, 70);
            this.PInfoExposure.Margin = new System.Windows.Forms.Padding(4);
            this.PInfoExposure.Name = "PInfoExposure";
            this.PInfoExposure.Size = new System.Drawing.Size(20, 18);
            this.PInfoExposure.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PInfoExposure.TabIndex = 40;
            this.PInfoExposure.TabStop = false;
            this.tooltips.SetToolTip(this.PInfoExposure, "Sets the exposure on all cameras. When Hardware Sync is enabled, manual mode is r" +
        "equired");
            // 
            // pInfoNetworkSync
            // 
            this.pInfoNetworkSync.Location = new System.Drawing.Point(413, 34);
            this.pInfoNetworkSync.Margin = new System.Windows.Forms.Padding(4);
            this.pInfoNetworkSync.Name = "pInfoNetworkSync";
            this.pInfoNetworkSync.Size = new System.Drawing.Size(20, 18);
            this.pInfoNetworkSync.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoNetworkSync.TabIndex = 35;
            this.pInfoNetworkSync.TabStop = false;
            this.tooltips.SetToolTip(this.pInfoNetworkSync, "No special hardware configuration needed. Might give more consistent frame timing" +
        "s between the devices, but reduces the framerate");
            // 
            // pInfoRefineCalib
            // 
            this.pInfoRefineCalib.Location = new System.Drawing.Point(340, 32);
            this.pInfoRefineCalib.Margin = new System.Windows.Forms.Padding(4);
            this.pInfoRefineCalib.Name = "pInfoRefineCalib";
            this.pInfoRefineCalib.Size = new System.Drawing.Size(20, 18);
            this.pInfoRefineCalib.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoRefineCalib.TabIndex = 24;
            this.pInfoRefineCalib.TabStop = false;
            this.tooltips.SetToolTip(this.pInfoRefineCalib, "Refines the calibration with an ICP algorithm. The cameras field of view needs to" +
        " be overlapping");
            // 
            // pInfoCalibrate
            // 
            this.pInfoCalibrate.Location = new System.Drawing.Point(137, 33);
            this.pInfoCalibrate.Margin = new System.Windows.Forms.Padding(4);
            this.pInfoCalibrate.Name = "pInfoCalibrate";
            this.pInfoCalibrate.Size = new System.Drawing.Size(20, 18);
            this.pInfoCalibrate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoCalibrate.TabIndex = 23;
            this.pInfoCalibrate.TabStop = false;
            this.tooltips.SetToolTip(this.pInfoCalibrate, "Calibrate all cameras spatially. At least one marker needs to be visible to all c" +
        "ameras");
            // 
            // pInfoRawFrames
            // 
            this.pInfoRawFrames.Location = new System.Drawing.Point(123, 5);
            this.pInfoRawFrames.Margin = new System.Windows.Forms.Padding(4);
            this.pInfoRawFrames.Name = "pInfoRawFrames";
            this.pInfoRawFrames.Size = new System.Drawing.Size(20, 18);
            this.pInfoRawFrames.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoRawFrames.TabIndex = 28;
            this.pInfoRawFrames.TabStop = false;
            this.tooltips.SetToolTip(this.pInfoRawFrames, "Save recording as color (.jpg) and depth (.tiff) frames. Best capture performance" +
        " and maximum quality, but requires postprocessing");
            // 
            // pInfoPointclouds
            // 
            this.pInfoPointclouds.Location = new System.Drawing.Point(371, 4);
            this.pInfoPointclouds.Margin = new System.Windows.Forms.Padding(4);
            this.pInfoPointclouds.Name = "pInfoPointclouds";
            this.pInfoPointclouds.Size = new System.Drawing.Size(20, 18);
            this.pInfoPointclouds.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoPointclouds.TabIndex = 29;
            this.pInfoPointclouds.TabStop = false;
            this.tooltips.SetToolTip(this.pInfoPointclouds, resources.GetString("pInfoPointclouds.ToolTip"));
            // 
            // pInfoWhiteBalance
            // 
            this.pInfoWhiteBalance.Location = new System.Drawing.Point(108, 113);
            this.pInfoWhiteBalance.Margin = new System.Windows.Forms.Padding(4);
            this.pInfoWhiteBalance.Name = "pInfoWhiteBalance";
            this.pInfoWhiteBalance.Size = new System.Drawing.Size(20, 18);
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
            this.gbExposure.Location = new System.Drawing.Point(137, 60);
            this.gbExposure.Margin = new System.Windows.Forms.Padding(4);
            this.gbExposure.Name = "gbExposure";
            this.gbExposure.Size = new System.Drawing.Size(171, 33);
            this.gbExposure.TabIndex = 44;
            // 
            // gbFrameExport
            // 
            this.gbFrameExport.Controls.Add(this.rExportRaw);
            this.gbFrameExport.Controls.Add(this.rExportPointclouds);
            this.gbFrameExport.Controls.Add(this.pInfoRawFrames);
            this.gbFrameExport.Controls.Add(this.pInfoPointclouds);
            this.gbFrameExport.Controls.Add(this.chMergeScans);
            this.gbFrameExport.Location = new System.Drawing.Point(4, 64);
            this.gbFrameExport.Margin = new System.Windows.Forms.Padding(4);
            this.gbFrameExport.Name = "gbFrameExport";
            this.gbFrameExport.Size = new System.Drawing.Size(403, 55);
            this.gbFrameExport.TabIndex = 45;
            // 
            // tlMainPanel
            // 
            this.tlMainPanel.ColumnCount = 2;
            this.tlMainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 533F));
            this.tlMainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlMainPanel.Controls.Add(this.pnLiveView, 1, 0);
            this.tlMainPanel.Controls.Add(this.tlControls, 0, 0);
            this.tlMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlMainPanel.Location = new System.Drawing.Point(0, 0);
            this.tlMainPanel.Margin = new System.Windows.Forms.Padding(4);
            this.tlMainPanel.Name = "tlMainPanel";
            this.tlMainPanel.RowCount = 1;
            this.tlMainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlMainPanel.Size = new System.Drawing.Size(1397, 854);
            this.tlMainPanel.TabIndex = 46;
            // 
            // pnLiveView
            // 
            this.pnLiveView.Controls.Add(this.tlLiveView);
            this.pnLiveView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnLiveView.Location = new System.Drawing.Point(537, 4);
            this.pnLiveView.Margin = new System.Windows.Forms.Padding(4);
            this.pnLiveView.Name = "pnLiveView";
            this.pnLiveView.Size = new System.Drawing.Size(856, 846);
            this.pnLiveView.TabIndex = 0;
            // 
            // tlLiveView
            // 
            this.tlLiveView.ColumnCount = 1;
            this.tlLiveView.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlLiveView.Controls.Add(this.tlStatusInfo, 0, 0);
            this.tlLiveView.Controls.Add(this.glLiveView, 0, 1);
            this.tlLiveView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlLiveView.Location = new System.Drawing.Point(0, 0);
            this.tlLiveView.Margin = new System.Windows.Forms.Padding(4);
            this.tlLiveView.Name = "tlLiveView";
            this.tlLiveView.RowCount = 2;
            this.tlLiveView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlLiveView.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlLiveView.Size = new System.Drawing.Size(856, 846);
            this.tlLiveView.TabIndex = 27;
            // 
            // tlStatusInfo
            // 
            this.tlStatusInfo.BackColor = System.Drawing.SystemColors.Control;
            this.tlStatusInfo.ColumnCount = 3;
            this.tlStatusInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tlStatusInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlStatusInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 133F));
            this.tlStatusInfo.Controls.Add(this.lStateIndicator, 1, 0);
            this.tlStatusInfo.Controls.Add(this.lFPS, 2, 0);
            this.tlStatusInfo.Controls.Add(this.pStatusIndicator, 0, 0);
            this.tlStatusInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlStatusInfo.Location = new System.Drawing.Point(4, 4);
            this.tlStatusInfo.Margin = new System.Windows.Forms.Padding(4);
            this.tlStatusInfo.Name = "tlStatusInfo";
            this.tlStatusInfo.RowCount = 1;
            this.tlStatusInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlStatusInfo.Size = new System.Drawing.Size(848, 22);
            this.tlStatusInfo.TabIndex = 27;
            // 
            // lStateIndicator
            // 
            this.lStateIndicator.BackColor = System.Drawing.SystemColors.Control;
            this.lStateIndicator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lStateIndicator.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lStateIndicator.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.lStateIndicator.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lStateIndicator.Location = new System.Drawing.Point(31, 0);
            this.lStateIndicator.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lStateIndicator.Name = "lStateIndicator";
            this.lStateIndicator.Size = new System.Drawing.Size(680, 22);
            this.lStateIndicator.TabIndex = 28;
            this.lStateIndicator.Text = "State...";
            this.lStateIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pStatusIndicator
            // 
            this.pStatusIndicator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pStatusIndicator.Image = global::LiveScanServer.Properties.Resources.Loading_Animation;
            this.pStatusIndicator.InitialImage = global::LiveScanServer.Properties.Resources.Loading_Animation;
            this.pStatusIndicator.Location = new System.Drawing.Point(4, 4);
            this.pStatusIndicator.Margin = new System.Windows.Forms.Padding(4);
            this.pStatusIndicator.Name = "pStatusIndicator";
            this.pStatusIndicator.Size = new System.Drawing.Size(19, 14);
            this.pStatusIndicator.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pStatusIndicator.TabIndex = 29;
            this.pStatusIndicator.TabStop = false;
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
            this.tlControls.Location = new System.Drawing.Point(4, 4);
            this.tlControls.Margin = new System.Windows.Forms.Padding(4);
            this.tlControls.Name = "tlControls";
            this.tlControls.RowCount = 6;
            this.tlControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 246F));
            this.tlControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tlControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 231F));
            this.tlControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 135F));
            this.tlControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tlControls.Size = new System.Drawing.Size(525, 846);
            this.tlControls.TabIndex = 1;
            // 
            // pnClients
            // 
            this.pnClients.Controls.Add(this.lClientsHeader);
            this.pnClients.Controls.Add(this.gvClients);
            this.pnClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnClients.Location = new System.Drawing.Point(4, 4);
            this.pnClients.Margin = new System.Windows.Forms.Padding(4);
            this.pnClients.Name = "pnClients";
            this.pnClients.Size = new System.Drawing.Size(517, 238);
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
            this.pnSettings.Location = new System.Drawing.Point(4, 324);
            this.pnSettings.Margin = new System.Windows.Forms.Padding(4);
            this.pnSettings.Name = "pnSettings";
            this.pnSettings.Size = new System.Drawing.Size(517, 223);
            this.pnSettings.TabIndex = 1;
            // 
            // gbWhiteBalance
            // 
            this.gbWhiteBalance.Controls.Add(this.rWhiteBalanceManual);
            this.gbWhiteBalance.Controls.Add(this.rWhiteBalanceAuto);
            this.gbWhiteBalance.Location = new System.Drawing.Point(137, 102);
            this.gbWhiteBalance.Margin = new System.Windows.Forms.Padding(4);
            this.gbWhiteBalance.Name = "gbWhiteBalance";
            this.gbWhiteBalance.Size = new System.Drawing.Size(171, 33);
            this.gbWhiteBalance.TabIndex = 48;
            // 
            // rWhiteBalanceManual
            // 
            this.rWhiteBalanceManual.AutoSize = true;
            this.rWhiteBalanceManual.Location = new System.Drawing.Point(87, 6);
            this.rWhiteBalanceManual.Margin = new System.Windows.Forms.Padding(4);
            this.rWhiteBalanceManual.Name = "rWhiteBalanceManual";
            this.rWhiteBalanceManual.Size = new System.Drawing.Size(72, 20);
            this.rWhiteBalanceManual.TabIndex = 38;
            this.rWhiteBalanceManual.TabStop = true;
            this.rWhiteBalanceManual.Text = "Manual";
            this.rWhiteBalanceManual.UseVisualStyleBackColor = true;
            this.rWhiteBalanceManual.CheckedChanged += new System.EventHandler(this.rWhiteBalanceManual_CheckedChanged);
            // 
            // rWhiteBalanceAuto
            // 
            this.rWhiteBalanceAuto.AutoSize = true;
            this.rWhiteBalanceAuto.Location = new System.Drawing.Point(13, 6);
            this.rWhiteBalanceAuto.Margin = new System.Windows.Forms.Padding(4);
            this.rWhiteBalanceAuto.Name = "rWhiteBalanceAuto";
            this.rWhiteBalanceAuto.Size = new System.Drawing.Size(55, 20);
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
            this.trWhiteBalance.Location = new System.Drawing.Point(311, 101);
            this.trWhiteBalance.Margin = new System.Windows.Forms.Padding(4);
            this.trWhiteBalance.Maximum = 18;
            this.trWhiteBalance.Minimum = 5;
            this.trWhiteBalance.Name = "trWhiteBalance";
            this.trWhiteBalance.Size = new System.Drawing.Size(205, 56);
            this.trWhiteBalance.TabIndex = 46;
            this.trWhiteBalance.Value = 8;
            this.trWhiteBalance.Scroll += new System.EventHandler(this.tbWhiteBalance_Scroll);
            // 
            // lWhiteBalance
            // 
            this.lWhiteBalance.AutoSize = true;
            this.lWhiteBalance.Location = new System.Drawing.Point(3, 113);
            this.lWhiteBalance.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lWhiteBalance.Name = "lWhiteBalance";
            this.lWhiteBalance.Size = new System.Drawing.Size(97, 16);
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
            this.pnCalibration.Location = new System.Drawing.Point(4, 250);
            this.pnCalibration.Margin = new System.Windows.Forms.Padding(4);
            this.pnCalibration.Name = "pnCalibration";
            this.pnCalibration.Size = new System.Drawing.Size(517, 66);
            this.pnCalibration.TabIndex = 5;
            // 
            // btRecord
            // 
            this.btRecord.AutoSize = true;
            this.btRecord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btRecord.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btRecord.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btRecord.Location = new System.Drawing.Point(4, 788);
            this.btRecord.Margin = new System.Windows.Forms.Padding(4);
            this.btRecord.MinimumSize = new System.Drawing.Size(0, 37);
            this.btRecord.Name = "btRecord";
            this.btRecord.Size = new System.Drawing.Size(517, 54);
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
            this.pnCapture.Location = new System.Drawing.Point(4, 653);
            this.pnCapture.Margin = new System.Windows.Forms.Padding(4);
            this.pnCapture.Name = "pnCapture";
            this.pnCapture.Size = new System.Drawing.Size(517, 127);
            this.pnCapture.TabIndex = 2;
            // 
            // MainWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1397, 854);
            this.Controls.Add(this.tlMainPanel);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(549, 803);
            this.Name = "MainWindowForm";
            this.Text = "LiveScanServer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.MainWindowForm_Load);
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
            this.tlLiveView.ResumeLayout(false);
            this.tlStatusInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pStatusIndicator)).EndInit();
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

        }

        #endregion
        private System.Windows.Forms.Button btCalibrate;
        private System.Windows.Forms.Button btRecord;
        private System.Windows.Forms.TextBox txtSeqName;
        private System.Windows.Forms.Button btRefineCalib;
        private System.Windows.Forms.Button btSettings;
        private System.Windows.Forms.Label lbSeqName;
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
        private System.Windows.Forms.Label lStateIndicator;
        private System.Windows.Forms.TableLayoutPanel tlLiveView;
        private System.Windows.Forms.TableLayoutPanel tlStatusInfo;
        private System.Windows.Forms.PictureBox pStatusIndicator;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn SyncState;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Visible;
        private System.Windows.Forms.DataGridViewButtonColumn Configuration;
    }
}

