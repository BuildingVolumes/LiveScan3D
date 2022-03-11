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
            this.nUDFramerate = new System.Windows.Forms.NumericUpDown();
            this.lPlaybackFramerate = new System.Windows.Forms.Label();
            this.chShowGizmos = new System.Windows.Forms.CheckBox();
            this.tbBrightness = new System.Windows.Forms.TrackBar();
            this.lBrightness = new System.Windows.Forms.Label();
            this.lPointSize = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.tbPointsize)).BeginInit();
            this.gbRecordingWindowControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFramerate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBrightness)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btSelect
            // 
            this.btSelect.Location = new System.Drawing.Point(18, 18);
            this.btSelect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btSelect.Name = "btSelect";
            this.btSelect.Size = new System.Drawing.Size(134, 35);
            this.btSelect.TabIndex = 0;
            this.btSelect.Text = "Select bin files";
            this.btSelect.UseVisualStyleBackColor = true;
            this.btSelect.Click += new System.EventHandler(this.btSelect_Click);
            // 
            // btStart
            // 
            this.btStart.Location = new System.Drawing.Point(18, 152);
            this.btStart.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btStart.Name = "btStart";
            this.btStart.Size = new System.Drawing.Size(134, 35);
            this.btStart.TabIndex = 1;
            this.btStart.Text = "Play";
            this.btStart.UseVisualStyleBackColor = true;
            this.btStart.Click += new System.EventHandler(this.btStart_Click);
            // 
            // btShow
            // 
            this.btShow.Location = new System.Drawing.Point(18, 242);
            this.btShow.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btShow.Name = "btShow";
            this.btShow.Size = new System.Drawing.Size(134, 35);
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
            this.btRewind.Location = new System.Drawing.Point(18, 197);
            this.btRewind.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btRewind.Name = "btRewind";
            this.btRewind.Size = new System.Drawing.Size(134, 35);
            this.btRewind.TabIndex = 5;
            this.btRewind.Text = "Rewind";
            this.btRewind.UseVisualStyleBackColor = true;
            this.btRewind.Click += new System.EventHandler(this.btRewind_Click);
            // 
            // btRemove
            // 
            this.btRemove.Location = new System.Drawing.Point(18, 108);
            this.btRemove.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btRemove.Name = "btRemove";
            this.btRemove.Size = new System.Drawing.Size(134, 35);
            this.btRemove.TabIndex = 6;
            this.btRemove.Text = "Remove file";
            this.btRemove.UseVisualStyleBackColor = true;
            this.btRemove.Click += new System.EventHandler(this.btRemove_Click);
            // 
            // lFrameFilesListView
            // 
            this.lFrameFilesListView.HideSelection = false;
            this.lFrameFilesListView.LabelEdit = true;
            this.lFrameFilesListView.Location = new System.Drawing.Point(160, 18);
            this.lFrameFilesListView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lFrameFilesListView.MultiSelect = false;
            this.lFrameFilesListView.Name = "lFrameFilesListView";
            this.lFrameFilesListView.Size = new System.Drawing.Size(577, 256);
            this.lFrameFilesListView.TabIndex = 7;
            this.lFrameFilesListView.UseCompatibleStateImageBehavior = false;
            this.lFrameFilesListView.View = System.Windows.Forms.View.Details;
            this.lFrameFilesListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.lFrameFilesListView_AfterLabelEdit);
            this.lFrameFilesListView.DoubleClick += new System.EventHandler(this.lFrameFilesListView_DoubleClick);
            // 
            // chSaveFrames
            // 
            this.chSaveFrames.AutoSize = true;
            this.chSaveFrames.Location = new System.Drawing.Point(9, 29);
            this.chSaveFrames.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chSaveFrames.Name = "chSaveFrames";
            this.chSaveFrames.Size = new System.Drawing.Size(183, 24);
            this.chSaveFrames.TabIndex = 8;
            this.chSaveFrames.Text = "Export Frames as ply";
            this.chSaveFrames.UseVisualStyleBackColor = true;
            // 
            // btnSelectPly
            // 
            this.btnSelectPly.Location = new System.Drawing.Point(18, 63);
            this.btnSelectPly.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSelectPly.Name = "btnSelectPly";
            this.btnSelectPly.Size = new System.Drawing.Size(134, 35);
            this.btnSelectPly.TabIndex = 9;
            this.btnSelectPly.Text = "Select ply files";
            this.btnSelectPly.UseVisualStyleBackColor = true;
            this.btnSelectPly.Click += new System.EventHandler(this.btnSelectPly_Click);
            // 
            // tbPointsize
            // 
            this.tbPointsize.Location = new System.Drawing.Point(9, 68);
            this.tbPointsize.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbPointsize.Name = "tbPointsize";
            this.tbPointsize.Size = new System.Drawing.Size(213, 69);
            this.tbPointsize.TabIndex = 10;
            this.tbPointsize.Scroll += new System.EventHandler(this.tbPointsize_Scroll);
            // 
            // gbRecordingWindowControls
            // 
            this.gbRecordingWindowControls.Controls.Add(this.nUDFramerate);
            this.gbRecordingWindowControls.Controls.Add(this.lPlaybackFramerate);
            this.gbRecordingWindowControls.Controls.Add(this.chShowGizmos);
            this.gbRecordingWindowControls.Controls.Add(this.tbBrightness);
            this.gbRecordingWindowControls.Controls.Add(this.lBrightness);
            this.gbRecordingWindowControls.Controls.Add(this.lPointSize);
            this.gbRecordingWindowControls.Controls.Add(this.tbPointsize);
            this.gbRecordingWindowControls.Location = new System.Drawing.Point(18, 286);
            this.gbRecordingWindowControls.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gbRecordingWindowControls.Name = "gbRecordingWindowControls";
            this.gbRecordingWindowControls.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gbRecordingWindowControls.Size = new System.Drawing.Size(507, 175);
            this.gbRecordingWindowControls.TabIndex = 11;
            this.gbRecordingWindowControls.TabStop = false;
            this.gbRecordingWindowControls.Text = "View controls";
            // 
            // nUDFramerate
            // 
            this.nUDFramerate.Location = new System.Drawing.Point(272, 134);
            this.nUDFramerate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.nUDFramerate.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.nUDFramerate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDFramerate.Name = "nUDFramerate";
            this.nUDFramerate.Size = new System.Drawing.Size(52, 26);
            this.nUDFramerate.TabIndex = 16;
            this.nUDFramerate.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nUDFramerate.ValueChanged += new System.EventHandler(this.nUDFramerate_ValueChanged);
            // 
            // lPlaybackFramerate
            // 
            this.lPlaybackFramerate.AutoSize = true;
            this.lPlaybackFramerate.Location = new System.Drawing.Point(333, 140);
            this.lPlaybackFramerate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lPlaybackFramerate.Name = "lPlaybackFramerate";
            this.lPlaybackFramerate.Size = new System.Drawing.Size(150, 20);
            this.lPlaybackFramerate.TabIndex = 15;
            this.lPlaybackFramerate.Text = "Playback Framerate";
            // 
            // chShowGizmos
            // 
            this.chShowGizmos.AutoSize = true;
            this.chShowGizmos.Checked = true;
            this.chShowGizmos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chShowGizmos.Location = new System.Drawing.Point(18, 138);
            this.chShowGizmos.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chShowGizmos.Name = "chShowGizmos";
            this.chShowGizmos.Size = new System.Drawing.Size(129, 24);
            this.chShowGizmos.TabIndex = 14;
            this.chShowGizmos.Text = "Show gizmos";
            this.chShowGizmos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chShowGizmos.UseVisualStyleBackColor = true;
            this.chShowGizmos.CheckedChanged += new System.EventHandler(this.chShowGizmos_CheckedChanged);
            // 
            // tbBrightness
            // 
            this.tbBrightness.Location = new System.Drawing.Point(272, 71);
            this.tbBrightness.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbBrightness.Maximum = 25;
            this.tbBrightness.Name = "tbBrightness";
            this.tbBrightness.Size = new System.Drawing.Size(213, 69);
            this.tbBrightness.SmallChange = 2;
            this.tbBrightness.TabIndex = 13;
            this.tbBrightness.Scroll += new System.EventHandler(this.tbBrightness_Scroll);
            // 
            // lBrightness
            // 
            this.lBrightness.AutoSize = true;
            this.lBrightness.Location = new System.Drawing.Point(267, 35);
            this.lBrightness.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lBrightness.Name = "lBrightness";
            this.lBrightness.Size = new System.Drawing.Size(85, 20);
            this.lBrightness.TabIndex = 12;
            this.lBrightness.Text = "Brightness";
            // 
            // lPointSize
            // 
            this.lPointSize.AutoSize = true;
            this.lPointSize.Location = new System.Drawing.Point(14, 35);
            this.lPointSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lPointSize.Name = "lPointSize";
            this.lPointSize.Size = new System.Drawing.Size(77, 20);
            this.lPointSize.TabIndex = 11;
            this.lPointSize.Text = "Point size";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chSaveFrames);
            this.groupBox1.Location = new System.Drawing.Point(534, 286);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(206, 63);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Export";
            // 
            // PlayerWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(756, 480);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbRecordingWindowControls);
            this.Controls.Add(this.btnSelectPly);
            this.Controls.Add(this.lFrameFilesListView);
            this.Controls.Add(this.btRemove);
            this.Controls.Add(this.btShow);
            this.Controls.Add(this.btRewind);
            this.Controls.Add(this.btStart);
            this.Controls.Add(this.btSelect);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "PlayerWindowForm";
            this.Text = "LiveScanPlayer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PlayerWindowForm_FormClosing);
            this.Load += new System.EventHandler(this.PlayerWindowForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tbPointsize)).EndInit();
            this.gbRecordingWindowControls.ResumeLayout(false);
            this.gbRecordingWindowControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFramerate)).EndInit();
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
        private System.Windows.Forms.NumericUpDown nUDFramerate;
        private System.Windows.Forms.Label lPlaybackFramerate;
    }
}

