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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lClientsHeader = new System.Windows.Forms.Label();
            this.lCalibrationHeader = new System.Windows.Forms.Label();
            this.lSettingsHeader = new System.Windows.Forms.Label();
            this.lCaptureHeader = new System.Windows.Forms.Label();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.btShowLive = new System.Windows.Forms.Button();
            this.pInfoCalibrate = new System.Windows.Forms.PictureBox();
            this.pInfoRefineCalib = new System.Windows.Forms.PictureBox();
            this.gvClients = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SyncState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoCalibrate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoRefineCalib)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvClients)).BeginInit();
            this.SuspendLayout();
            // 
            // btCalibrate
            // 
            this.btCalibrate.Location = new System.Drawing.Point(12, 282);
            this.btCalibrate.Name = "btCalibrate";
            this.btCalibrate.Size = new System.Drawing.Size(95, 23);
            this.btCalibrate.TabIndex = 2;
            this.btCalibrate.Text = "Calibrate";
            this.btCalibrate.UseVisualStyleBackColor = true;
            this.btCalibrate.Click += new System.EventHandler(this.btCalibrate_Click);
            // 
            // btRecord
            // 
            this.btRecord.Location = new System.Drawing.Point(106, 693);
            this.btRecord.Name = "btRecord";
            this.btRecord.Size = new System.Drawing.Size(140, 23);
            this.btRecord.TabIndex = 4;
            this.btRecord.Text = "Start recording";
            this.btRecord.UseVisualStyleBackColor = true;
            this.btRecord.Click += new System.EventHandler(this.btRecord_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 728);
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
            this.txtSeqName.Location = new System.Drawing.Point(106, 639);
            this.txtSeqName.MaxLength = 40;
            this.txtSeqName.Name = "txtSeqName";
            this.txtSeqName.Size = new System.Drawing.Size(203, 20);
            this.txtSeqName.TabIndex = 7;
            this.txtSeqName.Text = "Capture";
            // 
            // btRefineCalib
            // 
            this.btRefineCalib.Location = new System.Drawing.Point(170, 282);
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
            this.btSettings.Location = new System.Drawing.Point(305, 510);
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
            this.lbSeqName.Location = new System.Drawing.Point(12, 642);
            this.lbSeqName.Name = "lbSeqName";
            this.lbSeqName.Size = new System.Drawing.Size(88, 13);
            this.lbSeqName.TabIndex = 14;
            this.lbSeqName.Text = "Sequence name:";
            // 
            // btKinectSettingsOpenButton
            // 
            this.btKinectSettingsOpenButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btKinectSettingsOpenButton.Location = new System.Drawing.Point(12, 200);
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
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(406, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(820, 724);
            this.pictureBox1.TabIndex = 16;
            this.pictureBox1.TabStop = false;
            // 
            // lClientsHeader
            // 
            this.lClientsHeader.AutoSize = true;
            this.lClientsHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lClientsHeader.Location = new System.Drawing.Point(9, 9);
            this.lClientsHeader.Name = "lClientsHeader";
            this.lClientsHeader.Size = new System.Drawing.Size(51, 15);
            this.lClientsHeader.TabIndex = 17;
            this.lClientsHeader.Text = "Clients";
            // 
            // lCalibrationHeader
            // 
            this.lCalibrationHeader.AutoSize = true;
            this.lCalibrationHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lCalibrationHeader.Location = new System.Drawing.Point(9, 260);
            this.lCalibrationHeader.Name = "lCalibrationHeader";
            this.lCalibrationHeader.Size = new System.Drawing.Size(77, 15);
            this.lCalibrationHeader.TabIndex = 18;
            this.lCalibrationHeader.Text = "Calibration";
            // 
            // lSettingsHeader
            // 
            this.lSettingsHeader.AutoSize = true;
            this.lSettingsHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lSettingsHeader.Location = new System.Drawing.Point(12, 347);
            this.lSettingsHeader.Name = "lSettingsHeader";
            this.lSettingsHeader.Size = new System.Drawing.Size(59, 15);
            this.lSettingsHeader.TabIndex = 19;
            this.lSettingsHeader.Text = "Settings";
            // 
            // lCaptureHeader
            // 
            this.lCaptureHeader.AutoSize = true;
            this.lCaptureHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lCaptureHeader.Location = new System.Drawing.Point(12, 604);
            this.lCaptureHeader.Name = "lCaptureHeader";
            this.lCaptureHeader.Size = new System.Drawing.Size(57, 15);
            this.lCaptureHeader.TabIndex = 20;
            this.lCaptureHeader.Text = "Capture";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(15, 670);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(84, 17);
            this.radioButton1.TabIndex = 21;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Raw Frames";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(202, 670);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(80, 17);
            this.radioButton2.TabIndex = 22;
            this.radioButton2.Text = "Pointclouds";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // btShowLive
            // 
            this.btShowLive.Location = new System.Drawing.Point(288, 564);
            this.btShowLive.Name = "btShowLive";
            this.btShowLive.Size = new System.Drawing.Size(95, 23);
            this.btShowLive.TabIndex = 12;
            this.btShowLive.Text = "Show live";
            this.btShowLive.UseVisualStyleBackColor = true;
            this.btShowLive.Click += new System.EventHandler(this.btShowLive_Click);
            // 
            // pInfoCalibrate
            // 
            this.pInfoCalibrate.Image = global::KinectServer.Properties.Resources.info_box;
            this.pInfoCalibrate.Location = new System.Drawing.Point(112, 284);
            this.pInfoCalibrate.Name = "pInfoCalibrate";
            this.pInfoCalibrate.Size = new System.Drawing.Size(20, 19);
            this.pInfoCalibrate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoCalibrate.TabIndex = 23;
            this.pInfoCalibrate.TabStop = false;
            // 
            // pInfoRefineCalib
            // 
            this.pInfoRefineCalib.Image = global::KinectServer.Properties.Resources.info_box;
            this.pInfoRefineCalib.Location = new System.Drawing.Point(288, 283);
            this.pInfoRefineCalib.Name = "pInfoRefineCalib";
            this.pInfoRefineCalib.Size = new System.Drawing.Size(20, 19);
            this.pInfoRefineCalib.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pInfoRefineCalib.TabIndex = 24;
            this.pInfoRefineCalib.TabStop = false;
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
            this.gvClients.Location = new System.Drawing.Point(12, 30);
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
            this.Column1.Width = 24;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Column2.HeaderText = "Serial Number";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column2.Width = 90;
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Column3.HeaderText = "IP";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column3.Width = 90;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Column4.HeaderText = "Calibrated";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column4.Width = 60;
            // 
            // SyncState
            // 
            this.SyncState.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.SyncState.HeaderText = "Sync State";
            this.SyncState.Name = "SyncState";
            this.SyncState.ReadOnly = true;
            this.SyncState.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // MainWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1226, 750);
            this.Controls.Add(this.gvClients);
            this.Controls.Add(this.pInfoRefineCalib);
            this.Controls.Add(this.pInfoCalibrate);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.lCaptureHeader);
            this.Controls.Add(this.lSettingsHeader);
            this.Controls.Add(this.lCalibrationHeader);
            this.Controls.Add(this.lClientsHeader);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btKinectSettingsOpenButton);
            this.Controls.Add(this.lbSeqName);
            this.Controls.Add(this.btSettings);
            this.Controls.Add(this.btShowLive);
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoCalibrate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pInfoRefineCalib)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvClients)).EndInit();
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
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lClientsHeader;
        private System.Windows.Forms.Label lCalibrationHeader;
        private System.Windows.Forms.Label lSettingsHeader;
        private System.Windows.Forms.Label lCaptureHeader;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.Button btShowLive;
        private System.Windows.Forms.PictureBox pInfoCalibrate;
        private System.Windows.Forms.PictureBox pInfoRefineCalib;
        private System.Windows.Forms.DataGridView gvClients;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn SyncState;
    }
}

