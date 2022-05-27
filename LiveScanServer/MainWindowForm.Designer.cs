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
            this.btRecord = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.recordingWorker = new System.ComponentModel.BackgroundWorker();
            this.txtSeqName = new System.Windows.Forms.TextBox();
            this.btRefineCalib = new System.Windows.Forms.Button();
            this.OpenGLWorker = new System.ComponentModel.BackgroundWorker();
            this.savingWorker = new System.ComponentModel.BackgroundWorker();
            this.updateWorker = new System.ComponentModel.BackgroundWorker();
            this.btSettings = new System.Windows.Forms.Button();
            this.refineWorker = new System.ComponentModel.BackgroundWorker();
            this.lbSeqName = new System.Windows.Forms.Label();
            this.btKinectSettingsOpenButton = new System.Windows.Forms.Button();
            this.syncWorker = new System.ComponentModel.BackgroundWorker();
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
            this.pInfoPointclouds = new System.Windows.Forms.PictureBox();
            this.pInfoRawFrames = new System.Windows.Forms.PictureBox();
            this.pInfoRefineCalib = new System.Windows.Forms.PictureBox();
            this.pInfoCalibrate = new System.Windows.Forms.PictureBox();
            this.lTempSyncHeader = new System.Windows.Forms.Label();
            this.chHardwareSync = new System.Windows.Forms.CheckBox();
            this.chNetworkSync = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pInfoNetworkSync = new System.Windows.Forms.PictureBox();
            this.lExposure = new System.Windows.Forms.Label();
            this.rExposureAuto = new System.Windows.Forms.RadioButton();
            this.rExposureManual = new System.Windows.Forms.RadioButton();
            this.trManualExposure = new System.Windows.Forms.TrackBar();
            this.PInfoExposure = new System.Windows.Forms.PictureBox();
            this.lPerformance = new System.Windows.Forms.Label();
            this.cbEnablePreview = new System.Windows.Forms.CheckBox();
            this.tooltips = new System.Windows.Forms.ToolTip(this.components);
            this.pInfoClientPreview = new System.Windows.Forms.PictureBox();
            this.gbExposure = new System.Windows.Forms.Panel();
            this.gbFrameExport = new System.Windows.Forms.Panel();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvClients)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoPointclouds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoRawFrames)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoRefineCalib)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoCalibrate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoNetworkSync)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trManualExposure)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PInfoExposure)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoClientPreview)).BeginInit();
            this.gbExposure.SuspendLayout();
            this.gbFrameExport.SuspendLayout();
            this.SuspendLayout();
            // 
            // btCalibrate
            // 
            this.btCalibrate.Location = new System.Drawing.Point(9, 266);
            this.btCalibrate.Name = "btCalibrate";
            this.btCalibrate.Size = new System.Drawing.Size(95, 23);
            this.btCalibrate.TabIndex = 2;
            this.btCalibrate.Text = "Calibrate";
            this.btCalibrate.UseVisualStyleBackColor = true;
            this.btCalibrate.Click += new System.EventHandler(this.btCalibrate_Click);
            // 
            // btRecord
            // 
            this.btRecord.AutoSize = true;
            this.btRecord.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btRecord.Location = new System.Drawing.Point(109, 684);
            this.btRecord.Name = "btRecord";
            this.btRecord.Size = new System.Drawing.Size(140, 28);
            this.btRecord.TabIndex = 4;
            this.btRecord.Text = "Start Capture";
            this.btRecord.UseVisualStyleBackColor = true;
            this.btRecord.Click += new System.EventHandler(this.btRecord_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 724);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1226, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // recordingWorker
            // 
            this.recordingWorker.WorkerSupportsCancellation = true;
            this.recordingWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.recordingWorker_DoWork);
            this.recordingWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.recordingWorker_RunWorkerCompleted);
            // 
            // txtSeqName
            // 
            this.txtSeqName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSeqName.Location = new System.Drawing.Point(102, 603);
            this.txtSeqName.MaxLength = 40;
            this.txtSeqName.Name = "txtSeqName";
            this.txtSeqName.Size = new System.Drawing.Size(203, 20);
            this.txtSeqName.TabIndex = 7;
            this.txtSeqName.Text = "Capture";
            // 
            // btRefineCalib
            // 
            this.btRefineCalib.Location = new System.Drawing.Point(144, 266);
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
            // savingWorker
            // 
            this.savingWorker.WorkerSupportsCancellation = true;
            this.savingWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.savingWorker_DoWork);
            this.savingWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.savingWorker_RunWorkerCompleted);
            // 
            // updateWorker
            // 
            this.updateWorker.WorkerSupportsCancellation = true;
            this.updateWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.updateWorker_DoWork);
            this.updateWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.updateWorker_RunWorkerCompleted);
            // 
            // btSettings
            // 
            this.btSettings.Location = new System.Drawing.Point(296, 456);
            this.btSettings.Name = "btSettings";
            this.btSettings.Size = new System.Drawing.Size(95, 23);
            this.btSettings.TabIndex = 13;
            this.btSettings.Text = "More Settings";
            this.btSettings.UseVisualStyleBackColor = true;
            this.btSettings.Click += new System.EventHandler(this.btSettings_Click);
            // 
            // refineWorker
            // 
            this.refineWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.refineWorker_DoWork);
            this.refineWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.refineWorker_RunWorkerCompleted);
            // 
            // lbSeqName
            // 
            this.lbSeqName.AutoSize = true;
            this.lbSeqName.Location = new System.Drawing.Point(8, 606);
            this.lbSeqName.Name = "lbSeqName";
            this.lbSeqName.Size = new System.Drawing.Size(88, 13);
            this.lbSeqName.TabIndex = 14;
            this.lbSeqName.Text = "Sequence name:";
            // 
            // btKinectSettingsOpenButton
            // 
            this.btKinectSettingsOpenButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btKinectSettingsOpenButton.Location = new System.Drawing.Point(9, 203);
            this.btKinectSettingsOpenButton.Name = "btKinectSettingsOpenButton";
            this.btKinectSettingsOpenButton.Size = new System.Drawing.Size(388, 23);
            this.btKinectSettingsOpenButton.TabIndex = 15;
            this.btKinectSettingsOpenButton.Text = "Individual Client Settings";
            this.btKinectSettingsOpenButton.UseVisualStyleBackColor = true;
            this.btKinectSettingsOpenButton.Click += new System.EventHandler(this.btKinectSettingsOpenButton_Click);
            // 
            // syncWorker
            // 
            this.syncWorker.WorkerSupportsCancellation = true;
            this.syncWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.syncWorker_DoWork);
            this.syncWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.syncWorker_RunWorkerCompleted);
            // 
            // lClientsHeader
            // 
            this.lClientsHeader.AutoSize = true;
            this.lClientsHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lClientsHeader.Location = new System.Drawing.Point(6, 12);
            this.lClientsHeader.Name = "lClientsHeader";
            this.lClientsHeader.Size = new System.Drawing.Size(51, 15);
            this.lClientsHeader.TabIndex = 17;
            this.lClientsHeader.Text = "Clients";
            // 
            // lCalibrationHeader
            // 
            this.lCalibrationHeader.AutoSize = true;
            this.lCalibrationHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lCalibrationHeader.Location = new System.Drawing.Point(6, 245);
            this.lCalibrationHeader.Name = "lCalibrationHeader";
            this.lCalibrationHeader.Size = new System.Drawing.Size(77, 15);
            this.lCalibrationHeader.TabIndex = 18;
            this.lCalibrationHeader.Text = "Calibration";
            // 
            // lSettingsHeader
            // 
            this.lSettingsHeader.AutoSize = true;
            this.lSettingsHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lSettingsHeader.Location = new System.Drawing.Point(6, 315);
            this.lSettingsHeader.Name = "lSettingsHeader";
            this.lSettingsHeader.Size = new System.Drawing.Size(59, 15);
            this.lSettingsHeader.TabIndex = 19;
            this.lSettingsHeader.Text = "Settings";
            // 
            // lCaptureHeader
            // 
            this.lCaptureHeader.AutoSize = true;
            this.lCaptureHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lCaptureHeader.Location = new System.Drawing.Point(8, 577);
            this.lCaptureHeader.Name = "lCaptureHeader";
            this.lCaptureHeader.Size = new System.Drawing.Size(57, 15);
            this.lCaptureHeader.TabIndex = 20;
            this.lCaptureHeader.Text = "Capture";
            // 
            // rExportRaw
            // 
            this.rExportRaw.AutoSize = true;
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
            this.rExportPointclouds.AutoSize = true;
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
            this.gvClients.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.gvClients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvClients.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.SyncState});
            this.gvClients.Location = new System.Drawing.Point(9, 33);
            this.gvClients.MultiSelect = false;
            this.gvClients.Name = "gvClients";
            this.gvClients.ReadOnly = true;
            this.gvClients.Size = new System.Drawing.Size(385, 164);
            this.gvClients.TabIndex = 25;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Column1.HeaderText = "ID";
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
            this.SyncState.Name = "SyncState";
            this.SyncState.ReadOnly = true;
            this.SyncState.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.SyncState.ToolTipText = "The current temporal synchronisation state";
            // 
            // glLiveView
            // 
            this.glLiveView.BackColor = System.Drawing.Color.Black;
            this.glLiveView.Location = new System.Drawing.Point(406, 12);
            this.glLiveView.Name = "glLiveView";
            this.glLiveView.Size = new System.Drawing.Size(808, 713);
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
            this.lFPS.Location = new System.Drawing.Point(1168, 12);
            this.lFPS.Name = "lFPS";
            this.lFPS.Size = new System.Drawing.Size(46, 23);
            this.lFPS.TabIndex = 27;
            this.lFPS.Text = "30 FPS";
            this.lFPS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chMergeScans
            // 
            this.chMergeScans.AutoSize = true;
            this.chMergeScans.Location = new System.Drawing.Point(216, 24);
            this.chMergeScans.Name = "chMergeScans";
            this.chMergeScans.Size = new System.Drawing.Size(89, 17);
            this.chMergeScans.TabIndex = 30;
            this.chMergeScans.Text = "Merge Scans";
            this.chMergeScans.UseVisualStyleBackColor = true;
            this.chMergeScans.CheckedChanged += new System.EventHandler(this.chMergeScans_CheckedChanged);
            // 
            // pInfoPointclouds
            // 
            this.pInfoPointclouds.Image = global::KinectServer.Properties.Resources.info_box;
            this.pInfoPointclouds.Location = new System.Drawing.Point(277, 3);
            this.pInfoPointclouds.Name = "pInfoPointclouds";
            this.pInfoPointclouds.Size = new System.Drawing.Size(20, 19);
            this.pInfoPointclouds.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoPointclouds.TabIndex = 29;
            this.pInfoPointclouds.TabStop = false;
            this.tooltips.SetToolTip(this.pInfoPointclouds, resources.GetString("pInfoPointclouds.ToolTip"));
            // 
            // pInfoRawFrames
            // 
            this.pInfoRawFrames.Image = global::KinectServer.Properties.Resources.info_box;
            this.pInfoRawFrames.Location = new System.Drawing.Point(91, 3);
            this.pInfoRawFrames.Name = "pInfoRawFrames";
            this.pInfoRawFrames.Size = new System.Drawing.Size(20, 19);
            this.pInfoRawFrames.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoRawFrames.TabIndex = 28;
            this.pInfoRawFrames.TabStop = false;
            this.tooltips.SetToolTip(this.pInfoRawFrames, "Save recording as color (.jpg) and depth (.tiff) frames. Best capture performance" +
        " and maximum quality, but requires postprocessing");
            // 
            // pInfoRefineCalib
            // 
            this.pInfoRefineCalib.Image = global::KinectServer.Properties.Resources.info_box;
            this.pInfoRefineCalib.Location = new System.Drawing.Point(262, 267);
            this.pInfoRefineCalib.Name = "pInfoRefineCalib";
            this.pInfoRefineCalib.Size = new System.Drawing.Size(20, 19);
            this.pInfoRefineCalib.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoRefineCalib.TabIndex = 24;
            this.pInfoRefineCalib.TabStop = false;
            this.tooltips.SetToolTip(this.pInfoRefineCalib, "Refine the calibration with an ICP algorithm. Doesn\'t need a marker, but the came" +
        "ras field of view needs to be overlapping");
            // 
            // pInfoCalibrate
            // 
            this.pInfoCalibrate.Image = global::KinectServer.Properties.Resources.info_box;
            this.pInfoCalibrate.Location = new System.Drawing.Point(109, 268);
            this.pInfoCalibrate.Name = "pInfoCalibrate";
            this.pInfoCalibrate.Size = new System.Drawing.Size(20, 19);
            this.pInfoCalibrate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoCalibrate.TabIndex = 23;
            this.pInfoCalibrate.TabStop = false;
            this.tooltips.SetToolTip(this.pInfoCalibrate, "Calibrate all cameras spatially. A marker nneds to be visible to all cameras");
            // 
            // lTempSyncHeader
            // 
            this.lTempSyncHeader.AutoSize = true;
            this.lTempSyncHeader.Location = new System.Drawing.Point(7, 340);
            this.lTempSyncHeader.Name = "lTempSyncHeader";
            this.lTempSyncHeader.Size = new System.Drawing.Size(81, 13);
            this.lTempSyncHeader.TabIndex = 31;
            this.lTempSyncHeader.Text = "Temporal Sync:";
            // 
            // chHardwareSync
            // 
            this.chHardwareSync.AutoSize = true;
            this.chHardwareSync.Location = new System.Drawing.Point(99, 339);
            this.chHardwareSync.Name = "chHardwareSync";
            this.chHardwareSync.Size = new System.Drawing.Size(99, 17);
            this.chHardwareSync.TabIndex = 32;
            this.chHardwareSync.Text = "Hardware Sync";
            this.chHardwareSync.UseVisualStyleBackColor = true;
            this.chHardwareSync.Click += new System.EventHandler(this.chHardwareSync_Clicked);
            // 
            // chNetworkSync
            // 
            this.chNetworkSync.AutoSize = true;
            this.chNetworkSync.Location = new System.Drawing.Point(239, 339);
            this.chNetworkSync.Name = "chNetworkSync";
            this.chNetworkSync.Size = new System.Drawing.Size(93, 17);
            this.chNetworkSync.TabIndex = 33;
            this.chNetworkSync.Text = "Network Sync";
            this.chNetworkSync.UseVisualStyleBackColor = true;
            this.chNetworkSync.Click += new System.EventHandler(this.chNetworkSync_Clicked);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::KinectServer.Properties.Resources.info_box;
            this.pictureBox1.Location = new System.Drawing.Point(194, 337);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(20, 19);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 34;
            this.pictureBox1.TabStop = false;
            this.tooltips.SetToolTip(this.pictureBox1, "Before activating, make sure that all Kinects are connected to the server and pro" +
        "perly connected via the sync cables.");
            // 
            // pInfoNetworkSync
            // 
            this.pInfoNetworkSync.Image = global::KinectServer.Properties.Resources.info_box;
            this.pInfoNetworkSync.Location = new System.Drawing.Point(329, 336);
            this.pInfoNetworkSync.Name = "pInfoNetworkSync";
            this.pInfoNetworkSync.Size = new System.Drawing.Size(20, 21);
            this.pInfoNetworkSync.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoNetworkSync.TabIndex = 35;
            this.pInfoNetworkSync.TabStop = false;
            this.tooltips.SetToolTip(this.pInfoNetworkSync, "No special hardware configuration needed. Might give more consistent frame timing" +
        "s between the devices, but reduces the framerate a lot.");
            // 
            // lExposure
            // 
            this.lExposure.AutoSize = true;
            this.lExposure.Location = new System.Drawing.Point(7, 378);
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
            this.trManualExposure.Location = new System.Drawing.Point(218, 372);
            this.trManualExposure.Maximum = -5;
            this.trManualExposure.Minimum = -11;
            this.trManualExposure.Name = "trManualExposure";
            this.trManualExposure.Size = new System.Drawing.Size(173, 45);
            this.trManualExposure.TabIndex = 39;
            this.trManualExposure.Value = -8;
            this.trManualExposure.Scroll += new System.EventHandler(this.trManualExposure_Scroll);
            // 
            // PInfoExposure
            // 
            this.PInfoExposure.Image = global::KinectServer.Properties.Resources.info_box;
            this.PInfoExposure.Location = new System.Drawing.Point(60, 374);
            this.PInfoExposure.Name = "PInfoExposure";
            this.PInfoExposure.Size = new System.Drawing.Size(20, 19);
            this.PInfoExposure.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PInfoExposure.TabIndex = 40;
            this.PInfoExposure.TabStop = false;
            this.tooltips.SetToolTip(this.PInfoExposure, "Sets the exposure on all cameras. When Hardware Sync is enabled, manual mode is r" +
        "equired");
            // 
            // lPerformance
            // 
            this.lPerformance.AutoSize = true;
            this.lPerformance.Location = new System.Drawing.Point(7, 415);
            this.lPerformance.Name = "lPerformance";
            this.lPerformance.Size = new System.Drawing.Size(70, 13);
            this.lPerformance.TabIndex = 41;
            this.lPerformance.Text = "Performance:";
            // 
            // cbEnablePreview
            // 
            this.cbEnablePreview.AutoSize = true;
            this.cbEnablePreview.Location = new System.Drawing.Point(82, 414);
            this.cbEnablePreview.Margin = new System.Windows.Forms.Padding(2);
            this.cbEnablePreview.Name = "cbEnablePreview";
            this.cbEnablePreview.Size = new System.Drawing.Size(199, 17);
            this.cbEnablePreview.TabIndex = 42;
            this.cbEnablePreview.Text = "Enable Client preview during capture";
            this.cbEnablePreview.UseVisualStyleBackColor = true;
            this.cbEnablePreview.CheckedChanged += new System.EventHandler(this.cbEnablePreview_Clicked);
            // 
            // pInfoClientPreview
            // 
            this.pInfoClientPreview.Image = global::KinectServer.Properties.Resources.info_box;
            this.pInfoClientPreview.Location = new System.Drawing.Point(279, 412);
            this.pInfoClientPreview.Name = "pInfoClientPreview";
            this.pInfoClientPreview.Size = new System.Drawing.Size(20, 19);
            this.pInfoClientPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoClientPreview.TabIndex = 43;
            this.pInfoClientPreview.TabStop = false;
            this.tooltips.SetToolTip(this.pInfoClientPreview, "When disabled, clients will disable their viewport during capture. Might increase" +
        " capture performance");
            // 
            // gbExposure
            // 
            this.gbExposure.Controls.Add(this.rExposureManual);
            this.gbExposure.Controls.Add(this.rExposureAuto);
            this.gbExposure.Location = new System.Drawing.Point(86, 372);
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
            this.gbFrameExport.Location = new System.Drawing.Point(3, 629);
            this.gbFrameExport.Name = "gbFrameExport";
            this.gbFrameExport.Size = new System.Drawing.Size(302, 45);
            this.gbFrameExport.TabIndex = 45;
            // 
            // MainWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1226, 746);
            this.Controls.Add(this.gbFrameExport);
            this.Controls.Add(this.gbExposure);
            this.Controls.Add(this.pInfoClientPreview);
            this.Controls.Add(this.cbEnablePreview);
            this.Controls.Add(this.lPerformance);
            this.Controls.Add(this.PInfoExposure);
            this.Controls.Add(this.trManualExposure);
            this.Controls.Add(this.lExposure);
            this.Controls.Add(this.pInfoNetworkSync);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.chNetworkSync);
            this.Controls.Add(this.chHardwareSync);
            this.Controls.Add(this.lTempSyncHeader);
            this.Controls.Add(this.lFPS);
            this.Controls.Add(this.glLiveView);
            this.Controls.Add(this.gvClients);
            this.Controls.Add(this.pInfoRefineCalib);
            this.Controls.Add(this.pInfoCalibrate);
            this.Controls.Add(this.lCaptureHeader);
            this.Controls.Add(this.lSettingsHeader);
            this.Controls.Add(this.lCalibrationHeader);
            this.Controls.Add(this.lClientsHeader);
            this.Controls.Add(this.btKinectSettingsOpenButton);
            this.Controls.Add(this.lbSeqName);
            this.Controls.Add(this.btSettings);
            this.Controls.Add(this.btRefineCalib);
            this.Controls.Add(this.txtSeqName);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btRecord);
            this.Controls.Add(this.btCalibrate);
            this.MaximizeBox = false;
            this.Name = "MainWindowForm";
            this.Text = "LiveScanServer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvClients)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoPointclouds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoRawFrames)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoRefineCalib)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoCalibrate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoNetworkSync)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trManualExposure)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PInfoExposure)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoClientPreview)).EndInit();
            this.gbExposure.ResumeLayout(false);
            this.gbExposure.PerformLayout();
            this.gbFrameExport.ResumeLayout(false);
            this.gbFrameExport.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btCalibrate;
        private System.Windows.Forms.Button btRecord;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.ComponentModel.BackgroundWorker recordingWorker;
        private System.Windows.Forms.TextBox txtSeqName;
        private System.Windows.Forms.Button btRefineCalib;
        private System.ComponentModel.BackgroundWorker OpenGLWorker;
        private System.ComponentModel.BackgroundWorker savingWorker;
        private System.ComponentModel.BackgroundWorker updateWorker;
        private System.Windows.Forms.Button btSettings;
        private System.ComponentModel.BackgroundWorker refineWorker;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.Label lbSeqName;
        private System.Windows.Forms.Button btKinectSettingsOpenButton;
        private System.ComponentModel.BackgroundWorker syncWorker;
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
        private System.Windows.Forms.CheckBox cbEnablePreview;
        private System.Windows.Forms.ToolTip tooltips;
        private System.Windows.Forms.PictureBox pInfoClientPreview;
        private System.Windows.Forms.Panel gbExposure;
        private System.Windows.Forms.Panel gbFrameExport;
    }
}

