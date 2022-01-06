namespace LiveScanPlayer
{
    partial class PlayerWindowForm
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
            this.btSelect = new System.Windows.Forms.Button();
            this.btStart = new System.Windows.Forms.Button();
            this.btShow = new System.Windows.Forms.Button();
            this.updateWorker = new System.ComponentModel.BackgroundWorker();
            this.OpenGLWorker = new System.ComponentModel.BackgroundWorker();
            this.btRewind = new System.Windows.Forms.Button();
            this.btRemove = new System.Windows.Forms.Button();
            this.lFrameFilesListView = new System.Windows.Forms.ListView();
            this.chSaveFrames = new System.Windows.Forms.CheckBox();
            this.btnSelectPly = new System.Windows.Forms.Button();
            this.tbPointsize = new System.Windows.Forms.TrackBar();
            this.gbRecordingWindowControls = new System.Windows.Forms.GroupBox();
            this.chShowGizmos = new System.Windows.Forms.CheckBox();
            this.tbBrightness = new System.Windows.Forms.TrackBar();
            this.lBrightness = new System.Windows.Forms.Label();
            this.lPointSize = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.tbPointsize)).BeginInit();
            this.gbRecordingWindowControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbBrightness)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btSelect
            // 
            this.btSelect.Location = new System.Drawing.Point(12, 12);
            this.btSelect.Name = "btSelect";
            this.btSelect.Size = new System.Drawing.Size(89, 23);
            this.btSelect.TabIndex = 0;
            this.btSelect.Text = "Select bin files";
            this.btSelect.UseVisualStyleBackColor = true;
            this.btSelect.Click += new System.EventHandler(this.btSelect_Click);
            // 
            // btStart
            // 
            this.btStart.Location = new System.Drawing.Point(12, 99);
            this.btStart.Name = "btStart";
            this.btStart.Size = new System.Drawing.Size(89, 23);
            this.btStart.TabIndex = 1;
            this.btStart.Text = "Play";
            this.btStart.UseVisualStyleBackColor = true;
            this.btStart.Click += new System.EventHandler(this.btStart_Click);
            // 
            // btShow
            // 
            this.btShow.Location = new System.Drawing.Point(12, 157);
            this.btShow.Name = "btShow";
            this.btShow.Size = new System.Drawing.Size(89, 23);
            this.btShow.TabIndex = 2;
            this.btShow.Text = "Open 4D-View";
            this.btShow.UseVisualStyleBackColor = true;
            this.btShow.Click += new System.EventHandler(this.btShow_Click);
            // 
            // updateWorker
            // 
            this.updateWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.updateWorker_DoWork);
            // 
            // OpenGLWorker
            // 
            this.OpenGLWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.OpenGLWorker_DoWork);
            // 
            // btRewind
            // 
            this.btRewind.Location = new System.Drawing.Point(12, 128);
            this.btRewind.Name = "btRewind";
            this.btRewind.Size = new System.Drawing.Size(89, 23);
            this.btRewind.TabIndex = 5;
            this.btRewind.Text = "Rewind";
            this.btRewind.UseVisualStyleBackColor = true;
            this.btRewind.Click += new System.EventHandler(this.btRewind_Click);
            // 
            // btRemove
            // 
            this.btRemove.Location = new System.Drawing.Point(12, 70);
            this.btRemove.Name = "btRemove";
            this.btRemove.Size = new System.Drawing.Size(89, 23);
            this.btRemove.TabIndex = 6;
            this.btRemove.Text = "Remove file";
            this.btRemove.UseVisualStyleBackColor = true;
            this.btRemove.Click += new System.EventHandler(this.btRemove_Click);
            // 
            // lFrameFilesListView
            // 
            this.lFrameFilesListView.HideSelection = false;
            this.lFrameFilesListView.LabelEdit = true;
            this.lFrameFilesListView.Location = new System.Drawing.Point(107, 12);
            this.lFrameFilesListView.MultiSelect = false;
            this.lFrameFilesListView.Name = "lFrameFilesListView";
            this.lFrameFilesListView.Size = new System.Drawing.Size(386, 168);
            this.lFrameFilesListView.TabIndex = 7;
            this.lFrameFilesListView.UseCompatibleStateImageBehavior = false;
            this.lFrameFilesListView.View = System.Windows.Forms.View.Details;
            this.lFrameFilesListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.lFrameFilesListView_AfterLabelEdit);
            this.lFrameFilesListView.DoubleClick += new System.EventHandler(this.lFrameFilesListView_DoubleClick);
            // 
            // chSaveFrames
            // 
            this.chSaveFrames.AutoSize = true;
            this.chSaveFrames.Location = new System.Drawing.Point(6, 19);
            this.chSaveFrames.Name = "chSaveFrames";
            this.chSaveFrames.Size = new System.Drawing.Size(123, 17);
            this.chSaveFrames.TabIndex = 8;
            this.chSaveFrames.Text = "Export Frames as ply";
            this.chSaveFrames.UseVisualStyleBackColor = true;
            // 
            // btnSelectPly
            // 
            this.btnSelectPly.Location = new System.Drawing.Point(12, 41);
            this.btnSelectPly.Name = "btnSelectPly";
            this.btnSelectPly.Size = new System.Drawing.Size(89, 23);
            this.btnSelectPly.TabIndex = 9;
            this.btnSelectPly.Text = "Select ply files";
            this.btnSelectPly.UseVisualStyleBackColor = true;
            this.btnSelectPly.Click += new System.EventHandler(this.btnSelectPly_Click);
            // 
            // tbPointsize
            // 
            this.tbPointsize.Location = new System.Drawing.Point(6, 44);
            this.tbPointsize.Name = "tbPointsize";
            this.tbPointsize.Size = new System.Drawing.Size(142, 45);
            this.tbPointsize.TabIndex = 10;
            this.tbPointsize.Scroll += new System.EventHandler(this.tbPointsize_Scroll);
            // 
            // gbRecordingWindowControls
            // 
            this.gbRecordingWindowControls.Controls.Add(this.chShowGizmos);
            this.gbRecordingWindowControls.Controls.Add(this.tbBrightness);
            this.gbRecordingWindowControls.Controls.Add(this.lBrightness);
            this.gbRecordingWindowControls.Controls.Add(this.lPointSize);
            this.gbRecordingWindowControls.Controls.Add(this.tbPointsize);
            this.gbRecordingWindowControls.Location = new System.Drawing.Point(12, 186);
            this.gbRecordingWindowControls.Name = "gbRecordingWindowControls";
            this.gbRecordingWindowControls.Size = new System.Drawing.Size(338, 114);
            this.gbRecordingWindowControls.TabIndex = 11;
            this.gbRecordingWindowControls.TabStop = false;
            this.gbRecordingWindowControls.Text = "View controls";
            // 
            // chShowGizmos
            // 
            this.chShowGizmos.AutoSize = true;
            this.chShowGizmos.Checked = true;
            this.chShowGizmos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chShowGizmos.Location = new System.Drawing.Point(12, 91);
            this.chShowGizmos.Name = "chShowGizmos";
            this.chShowGizmos.Size = new System.Drawing.Size(88, 17);
            this.chShowGizmos.TabIndex = 14;
            this.chShowGizmos.Text = "Show gizmos";
            this.chShowGizmos.UseVisualStyleBackColor = true;
            this.chShowGizmos.CheckedChanged += new System.EventHandler(this.chShowGizmos_CheckedChanged);
            // 
            // tbBrightness
            // 
            this.tbBrightness.Location = new System.Drawing.Point(181, 46);
            this.tbBrightness.Maximum = 25;
            this.tbBrightness.Name = "tbBrightness";
            this.tbBrightness.Size = new System.Drawing.Size(142, 45);
            this.tbBrightness.SmallChange = 2;
            this.tbBrightness.TabIndex = 13;
            this.tbBrightness.Scroll += new System.EventHandler(this.tbBrightness_Scroll);
            // 
            // lBrightness
            // 
            this.lBrightness.AutoSize = true;
            this.lBrightness.Location = new System.Drawing.Point(178, 23);
            this.lBrightness.Name = "lBrightness";
            this.lBrightness.Size = new System.Drawing.Size(56, 13);
            this.lBrightness.TabIndex = 12;
            this.lBrightness.Text = "Brightness";
            // 
            // lPointSize
            // 
            this.lPointSize.AutoSize = true;
            this.lPointSize.Location = new System.Drawing.Point(9, 23);
            this.lPointSize.Name = "lPointSize";
            this.lPointSize.Size = new System.Drawing.Size(52, 13);
            this.lPointSize.TabIndex = 11;
            this.lPointSize.Text = "Point size";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chSaveFrames);
            this.groupBox1.Location = new System.Drawing.Point(356, 186);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(137, 41);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Export";
            // 
            // PlayerWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 312);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbRecordingWindowControls);
            this.Controls.Add(this.btnSelectPly);
            this.Controls.Add(this.lFrameFilesListView);
            this.Controls.Add(this.btRemove);
            this.Controls.Add(this.btShow);
            this.Controls.Add(this.btRewind);
            this.Controls.Add(this.btStart);
            this.Controls.Add(this.btSelect);
            this.Name = "PlayerWindowForm";
            this.Text = "LiveScanPlayer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PlayerWindowForm_FormClosing);
            this.Load += new System.EventHandler(this.PlayerWindowForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tbPointsize)).EndInit();
            this.gbRecordingWindowControls.ResumeLayout(false);
            this.gbRecordingWindowControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbBrightness)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btSelect;
        private System.Windows.Forms.Button btStart;
        private System.Windows.Forms.Button btShow;
        private System.ComponentModel.BackgroundWorker updateWorker;
        private System.ComponentModel.BackgroundWorker OpenGLWorker;
        private System.Windows.Forms.Button btRewind;
        private System.Windows.Forms.Button btRemove;
        private System.Windows.Forms.ListView lFrameFilesListView;
        private System.Windows.Forms.CheckBox chSaveFrames;
        private System.Windows.Forms.Button btnSelectPly;
        private System.Windows.Forms.TrackBar tbPointsize;
        private System.Windows.Forms.GroupBox gbRecordingWindowControls;
        private System.Windows.Forms.TrackBar tbBrightness;
        private System.Windows.Forms.Label lBrightness;
        private System.Windows.Forms.Label lPointSize;
        private System.Windows.Forms.CheckBox chShowGizmos;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

