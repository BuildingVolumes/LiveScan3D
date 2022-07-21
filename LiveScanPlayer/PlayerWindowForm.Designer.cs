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
            this.updateWorker = new System.ComponentModel.BackgroundWorker();
            this.OpenGLWorker = new System.ComponentModel.BackgroundWorker();
            this.btRewind = new System.Windows.Forms.Button();
            this.btRemove = new System.Windows.Forms.Button();
            this.lFrameFilesListView = new System.Windows.Forms.ListView();
            this.chSaveFrames = new System.Windows.Forms.CheckBox();
            this.btnSelectPly = new System.Windows.Forms.Button();
            this.tbPointsize = new System.Windows.Forms.TrackBar();
            this.nUDFramerate = new System.Windows.Forms.NumericUpDown();
            this.lPlaybackFramerate = new System.Windows.Forms.Label();
            this.chShowGizmos = new System.Windows.Forms.CheckBox();
            this.tbBrightness = new System.Windows.Forms.TrackBar();
            this.lBrightness = new System.Windows.Forms.Label();
            this.lPointSize = new System.Windows.Forms.Label();
            this.glLiveView = new OpenTK.GLControl();
            this.tbVideoScroll = new System.Windows.Forms.TrackBar();
            this.lFrameCounter = new System.Windows.Forms.Label();
            this.tlMain = new System.Windows.Forms.TableLayoutPanel();
            this.tlControlsParent = new System.Windows.Forms.TableLayoutPanel();
            this.tlPlaybackControls = new System.Windows.Forms.TableLayoutPanel();
            this.lFrameCountHeader = new System.Windows.Forms.Label();
            this.pFPSControls = new System.Windows.Forms.Panel();
            this.tbSettingsControl = new System.Windows.Forms.TableLayoutPanel();
            this.lFPSCounter = new System.Windows.Forms.Label();
            this.pFileControl = new System.Windows.Forms.Panel();
            this.gbPlaybackSettings = new System.Windows.Forms.GroupBox();
            this.chLooping = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.tbPointsize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFramerate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbVideoScroll)).BeginInit();
            this.tlMain.SuspendLayout();
            this.tlControlsParent.SuspendLayout();
            this.tlPlaybackControls.SuspendLayout();
            this.pFPSControls.SuspendLayout();
            this.tbSettingsControl.SuspendLayout();
            this.pFileControl.SuspendLayout();
            this.gbPlaybackSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // btSelect
            // 
            this.btSelect.Location = new System.Drawing.Point(3, 3);
            this.btSelect.Name = "btSelect";
            this.btSelect.Size = new System.Drawing.Size(89, 23);
            this.btSelect.TabIndex = 0;
            this.btSelect.Text = "Open bin Files";
            this.btSelect.UseVisualStyleBackColor = true;
            this.btSelect.Click += new System.EventHandler(this.btSelect_Click);
            // 
            // btStart
            // 
            this.btStart.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btStart.Image = global::LiveScanPlayer.Properties.Resources.Play;
            this.btStart.Location = new System.Drawing.Point(44, 4);
            this.btStart.Name = "btStart";
            this.btStart.Size = new System.Drawing.Size(35, 35);
            this.btStart.TabIndex = 1;
            this.btStart.UseVisualStyleBackColor = true;
            this.btStart.Click += new System.EventHandler(this.btStart_Click);
            // 
            // updateWorker
            // 
            this.updateWorker.WorkerSupportsCancellation = true;
            this.updateWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.updateWorker_DoWork);
            // 
            // OpenGLWorker
            // 
            this.OpenGLWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.OpenGLWorker_DoWork);
            // 
            // btRewind
            // 
            this.btRewind.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btRewind.Image = global::LiveScanPlayer.Properties.Resources.reverse;
            this.btRewind.Location = new System.Drawing.Point(3, 4);
            this.btRewind.Name = "btRewind";
            this.btRewind.Size = new System.Drawing.Size(35, 35);
            this.btRewind.TabIndex = 5;
            this.btRewind.UseVisualStyleBackColor = true;
            this.btRewind.Click += new System.EventHandler(this.btRewind_Click);
            // 
            // btRemove
            // 
            this.btRemove.Location = new System.Drawing.Point(3, 62);
            this.btRemove.Name = "btRemove";
            this.btRemove.Size = new System.Drawing.Size(89, 23);
            this.btRemove.TabIndex = 6;
            this.btRemove.Text = "Remove File";
            this.btRemove.UseVisualStyleBackColor = true;
            this.btRemove.Click += new System.EventHandler(this.btRemove_Click);
            // 
            // lFrameFilesListView
            // 
            this.lFrameFilesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lFrameFilesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lFrameFilesListView.HideSelection = false;
            this.lFrameFilesListView.LabelEdit = true;
            this.lFrameFilesListView.Location = new System.Drawing.Point(103, 3);
            this.lFrameFilesListView.MultiSelect = false;
            this.lFrameFilesListView.Name = "lFrameFilesListView";
            this.lFrameFilesListView.Size = new System.Drawing.Size(281, 92);
            this.lFrameFilesListView.TabIndex = 7;
            this.lFrameFilesListView.UseCompatibleStateImageBehavior = false;
            this.lFrameFilesListView.View = System.Windows.Forms.View.Details;
            // 
            // chSaveFrames
            // 
            this.chSaveFrames.AutoSize = true;
            this.chSaveFrames.Location = new System.Drawing.Point(313, 66);
            this.chSaveFrames.Name = "chSaveFrames";
            this.chSaveFrames.Size = new System.Drawing.Size(123, 17);
            this.chSaveFrames.TabIndex = 8;
            this.chSaveFrames.Text = "Export frames as .ply";
            this.chSaveFrames.UseVisualStyleBackColor = true;
            // 
            // btnSelectPly
            // 
            this.btnSelectPly.Location = new System.Drawing.Point(3, 32);
            this.btnSelectPly.Name = "btnSelectPly";
            this.btnSelectPly.Size = new System.Drawing.Size(89, 23);
            this.btnSelectPly.TabIndex = 9;
            this.btnSelectPly.Text = "Open ply Files";
            this.btnSelectPly.UseVisualStyleBackColor = true;
            this.btnSelectPly.Click += new System.EventHandler(this.btnSelectPly_Click);
            // 
            // tbPointsize
            // 
            this.tbPointsize.AutoSize = false;
            this.tbPointsize.Location = new System.Drawing.Point(12, 43);
            this.tbPointsize.Name = "tbPointsize";
            this.tbPointsize.Size = new System.Drawing.Size(142, 36);
            this.tbPointsize.TabIndex = 10;
            this.tbPointsize.Scroll += new System.EventHandler(this.tbPointsize_Scroll);
            // 
            // nUDFramerate
            // 
            this.nUDFramerate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.nUDFramerate.Location = new System.Drawing.Point(76, 5);
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
            this.nUDFramerate.Size = new System.Drawing.Size(35, 20);
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
            this.lPlaybackFramerate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lPlaybackFramerate.AutoSize = true;
            this.lPlaybackFramerate.Location = new System.Drawing.Point(0, 8);
            this.lPlaybackFramerate.Name = "lPlaybackFramerate";
            this.lPlaybackFramerate.Size = new System.Drawing.Size(77, 13);
            this.lPlaybackFramerate.TabIndex = 15;
            this.lPlaybackFramerate.Text = "Playback FPS:";
            // 
            // chShowGizmos
            // 
            this.chShowGizmos.AutoSize = true;
            this.chShowGizmos.Checked = true;
            this.chShowGizmos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chShowGizmos.Location = new System.Drawing.Point(313, 43);
            this.chShowGizmos.Name = "chShowGizmos";
            this.chShowGizmos.Size = new System.Drawing.Size(90, 17);
            this.chShowGizmos.TabIndex = 14;
            this.chShowGizmos.Text = "Show Gizmos";
            this.chShowGizmos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chShowGizmos.UseVisualStyleBackColor = true;
            this.chShowGizmos.CheckedChanged += new System.EventHandler(this.chShowGizmos_CheckedChanged);
            // 
            // tbBrightness
            // 
            this.tbBrightness.AutoSize = false;
            this.tbBrightness.LargeChange = 2;
            this.tbBrightness.Location = new System.Drawing.Point(154, 45);
            this.tbBrightness.Name = "tbBrightness";
            this.tbBrightness.Size = new System.Drawing.Size(142, 34);
            this.tbBrightness.TabIndex = 13;
            this.tbBrightness.Scroll += new System.EventHandler(this.tbBrightness_Scroll);
            // 
            // lBrightness
            // 
            this.lBrightness.AutoSize = true;
            this.lBrightness.Location = new System.Drawing.Point(151, 21);
            this.lBrightness.Name = "lBrightness";
            this.lBrightness.Size = new System.Drawing.Size(56, 13);
            this.lBrightness.TabIndex = 12;
            this.lBrightness.Text = "Brightness";
            // 
            // lPointSize
            // 
            this.lPointSize.AutoSize = true;
            this.lPointSize.Location = new System.Drawing.Point(15, 21);
            this.lPointSize.Name = "lPointSize";
            this.lPointSize.Size = new System.Drawing.Size(54, 13);
            this.lPointSize.TabIndex = 11;
            this.lPointSize.Text = "Point Size";
            // 
            // glLiveView
            // 
            this.glLiveView.BackColor = System.Drawing.Color.Black;
            this.glLiveView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glLiveView.Location = new System.Drawing.Point(3, 3);
            this.glLiveView.Name = "glLiveView";
            this.glLiveView.Size = new System.Drawing.Size(915, 542);
            this.glLiveView.TabIndex = 13;
            this.glLiveView.VSync = false;
            this.glLiveView.Load += new System.EventHandler(this.glLiveView_Load);
            this.glLiveView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glLiveView_MouseDown);
            this.glLiveView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glLiveView_MouseMove);
            this.glLiveView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glLiveView_MouseUp);
            this.glLiveView.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glLiveView_Scroll);
            this.glLiveView.Resize += new System.EventHandler(this.glLiveView_Resize);
            // 
            // tbVideoScroll
            // 
            this.tbVideoScroll.AutoSize = false;
            this.tbVideoScroll.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tbVideoScroll.LargeChange = 0;
            this.tbVideoScroll.Location = new System.Drawing.Point(160, 7);
            this.tbVideoScroll.Maximum = 0;
            this.tbVideoScroll.Name = "tbVideoScroll";
            this.tbVideoScroll.Size = new System.Drawing.Size(626, 34);
            this.tbVideoScroll.TabIndex = 17;
            this.tbVideoScroll.TickFrequency = 0;
            this.tbVideoScroll.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbVideoScroll.ValueChanged += new System.EventHandler(this.tbVideoScroll_ValueChanged);
            this.tbVideoScroll.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbVideoScroll_MouseDown);
            this.tbVideoScroll.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tbVideoScroll_MouseUp);
            // 
            // lFrameCounter
            // 
            this.lFrameCounter.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lFrameCounter.AutoSize = true;
            this.lFrameCounter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lFrameCounter.Location = new System.Drawing.Point(130, 15);
            this.lFrameCounter.Name = "lFrameCounter";
            this.lFrameCounter.Size = new System.Drawing.Size(24, 13);
            this.lFrameCounter.TabIndex = 18;
            this.lFrameCounter.Text = "0/0";
            this.lFrameCounter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tlMain
            // 
            this.tlMain.ColumnCount = 1;
            this.tlMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlMain.Controls.Add(this.glLiveView, 0, 0);
            this.tlMain.Controls.Add(this.tlControlsParent, 0, 1);
            this.tlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlMain.Location = new System.Drawing.Point(0, 0);
            this.tlMain.Name = "tlMain";
            this.tlMain.RowCount = 2;
            this.tlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tlMain.Size = new System.Drawing.Size(921, 708);
            this.tlMain.TabIndex = 19;
            // 
            // tlControlsParent
            // 
            this.tlControlsParent.ColumnCount = 1;
            this.tlControlsParent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlControlsParent.Controls.Add(this.tlPlaybackControls, 0, 0);
            this.tlControlsParent.Controls.Add(this.tbSettingsControl, 0, 1);
            this.tlControlsParent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlControlsParent.Location = new System.Drawing.Point(3, 551);
            this.tlControlsParent.Name = "tlControlsParent";
            this.tlControlsParent.RowCount = 2;
            this.tlControlsParent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tlControlsParent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlControlsParent.Size = new System.Drawing.Size(915, 154);
            this.tlControlsParent.TabIndex = 14;
            // 
            // tlPlaybackControls
            // 
            this.tlPlaybackControls.ColumnCount = 6;
            this.tlPlaybackControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tlPlaybackControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tlPlaybackControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tlPlaybackControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlPlaybackControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlPlaybackControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tlPlaybackControls.Controls.Add(this.lFrameCountHeader, 2, 0);
            this.tlPlaybackControls.Controls.Add(this.lFrameCounter, 3, 0);
            this.tlPlaybackControls.Controls.Add(this.tbVideoScroll, 4, 0);
            this.tlPlaybackControls.Controls.Add(this.pFPSControls, 5, 0);
            this.tlPlaybackControls.Controls.Add(this.btStart, 1, 0);
            this.tlPlaybackControls.Controls.Add(this.btRewind, 0, 0);
            this.tlPlaybackControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlPlaybackControls.Location = new System.Drawing.Point(3, 3);
            this.tlPlaybackControls.Name = "tlPlaybackControls";
            this.tlPlaybackControls.RowCount = 1;
            this.tlPlaybackControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlPlaybackControls.Size = new System.Drawing.Size(909, 44);
            this.tlPlaybackControls.TabIndex = 0;
            // 
            // lFrameCountHeader
            // 
            this.lFrameCountHeader.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lFrameCountHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lFrameCountHeader.Location = new System.Drawing.Point(85, 7);
            this.lFrameCountHeader.Name = "lFrameCountHeader";
            this.lFrameCountHeader.Size = new System.Drawing.Size(39, 30);
            this.lFrameCountHeader.TabIndex = 19;
            this.lFrameCountHeader.Text = "Frame:";
            this.lFrameCountHeader.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pFPSControls
            // 
            this.pFPSControls.Controls.Add(this.nUDFramerate);
            this.pFPSControls.Controls.Add(this.lPlaybackFramerate);
            this.pFPSControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pFPSControls.Location = new System.Drawing.Point(792, 3);
            this.pFPSControls.Name = "pFPSControls";
            this.pFPSControls.Size = new System.Drawing.Size(114, 38);
            this.pFPSControls.TabIndex = 20;
            // 
            // tbSettingsControl
            // 
            this.tbSettingsControl.ColumnCount = 4;
            this.tbSettingsControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tbSettingsControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tbSettingsControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 450F));
            this.tbSettingsControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tbSettingsControl.Controls.Add(this.lFPSCounter, 3, 0);
            this.tbSettingsControl.Controls.Add(this.pFileControl, 0, 0);
            this.tbSettingsControl.Controls.Add(this.gbPlaybackSettings, 2, 0);
            this.tbSettingsControl.Controls.Add(this.lFrameFilesListView, 1, 0);
            this.tbSettingsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbSettingsControl.Location = new System.Drawing.Point(3, 53);
            this.tbSettingsControl.Name = "tbSettingsControl";
            this.tbSettingsControl.RowCount = 1;
            this.tbSettingsControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tbSettingsControl.Size = new System.Drawing.Size(909, 98);
            this.tbSettingsControl.TabIndex = 1;
            // 
            // lFPSCounter
            // 
            this.lFPSCounter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lFPSCounter.Location = new System.Drawing.Point(854, 85);
            this.lFPSCounter.Name = "lFPSCounter";
            this.lFPSCounter.Size = new System.Drawing.Size(52, 13);
            this.lFPSCounter.TabIndex = 21;
            this.lFPSCounter.Text = "FPS: 0";
            this.lFPSCounter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pFileControl
            // 
            this.pFileControl.Controls.Add(this.btSelect);
            this.pFileControl.Controls.Add(this.btRemove);
            this.pFileControl.Controls.Add(this.btnSelectPly);
            this.pFileControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pFileControl.Location = new System.Drawing.Point(3, 3);
            this.pFileControl.Name = "pFileControl";
            this.pFileControl.Size = new System.Drawing.Size(94, 92);
            this.pFileControl.TabIndex = 0;
            // 
            // gbPlaybackSettings
            // 
            this.gbPlaybackSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbPlaybackSettings.Controls.Add(this.chLooping);
            this.gbPlaybackSettings.Controls.Add(this.tbBrightness);
            this.gbPlaybackSettings.Controls.Add(this.lPointSize);
            this.gbPlaybackSettings.Controls.Add(this.lBrightness);
            this.gbPlaybackSettings.Controls.Add(this.tbPointsize);
            this.gbPlaybackSettings.Controls.Add(this.chSaveFrames);
            this.gbPlaybackSettings.Controls.Add(this.chShowGizmos);
            this.gbPlaybackSettings.Location = new System.Drawing.Point(390, 3);
            this.gbPlaybackSettings.Name = "gbPlaybackSettings";
            this.gbPlaybackSettings.Size = new System.Drawing.Size(444, 92);
            this.gbPlaybackSettings.TabIndex = 20;
            this.gbPlaybackSettings.TabStop = false;
            this.gbPlaybackSettings.Text = "Playback Settings";
            // 
            // chLooping
            // 
            this.chLooping.AutoSize = true;
            this.chLooping.Checked = true;
            this.chLooping.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chLooping.Location = new System.Drawing.Point(313, 20);
            this.chLooping.Name = "chLooping";
            this.chLooping.Size = new System.Drawing.Size(50, 17);
            this.chLooping.TabIndex = 15;
            this.chLooping.Text = "Loop";
            this.chLooping.UseVisualStyleBackColor = true;
            this.chLooping.CheckedChanged += new System.EventHandler(this.chLooping_CheckedChanged);
            // 
            // PlayerWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(921, 708);
            this.Controls.Add(this.tlMain);
            this.MinimumSize = new System.Drawing.Size(570, 500);
            this.Name = "PlayerWindowForm";
            this.Text = "LiveScanPlayer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PlayerWindowForm_FormClosing);
            this.Load += new System.EventHandler(this.PlayerWindowForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tbPointsize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDFramerate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbVideoScroll)).EndInit();
            this.tlMain.ResumeLayout(false);
            this.tlControlsParent.ResumeLayout(false);
            this.tlPlaybackControls.ResumeLayout(false);
            this.tlPlaybackControls.PerformLayout();
            this.pFPSControls.ResumeLayout(false);
            this.pFPSControls.PerformLayout();
            this.tbSettingsControl.ResumeLayout(false);
            this.pFileControl.ResumeLayout(false);
            this.gbPlaybackSettings.ResumeLayout(false);
            this.gbPlaybackSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btSelect;
        private System.Windows.Forms.Button btStart;
        private System.ComponentModel.BackgroundWorker updateWorker;
        private System.ComponentModel.BackgroundWorker OpenGLWorker;
        private System.Windows.Forms.Button btRewind;
        private System.Windows.Forms.Button btRemove;
        private System.Windows.Forms.ListView lFrameFilesListView;
        private System.Windows.Forms.CheckBox chSaveFrames;
        private System.Windows.Forms.Button btnSelectPly;
        private System.Windows.Forms.TrackBar tbPointsize;
        private System.Windows.Forms.TrackBar tbBrightness;
        private System.Windows.Forms.Label lBrightness;
        private System.Windows.Forms.Label lPointSize;
        private System.Windows.Forms.CheckBox chShowGizmos;
        private System.Windows.Forms.NumericUpDown nUDFramerate;
        private System.Windows.Forms.Label lPlaybackFramerate;
        private OpenTK.GLControl glLiveView;
        private System.Windows.Forms.TrackBar tbVideoScroll;
        private System.Windows.Forms.Label lFrameCounter;
        private System.Windows.Forms.TableLayoutPanel tlMain;
        private System.Windows.Forms.GroupBox gbPlaybackSettings;
        private System.Windows.Forms.Label lFrameCountHeader;
        private System.Windows.Forms.TableLayoutPanel tlControlsParent;
        private System.Windows.Forms.TableLayoutPanel tlPlaybackControls;
        private System.Windows.Forms.Panel pFPSControls;
        private System.Windows.Forms.Label lFPSCounter;
        private System.Windows.Forms.TableLayoutPanel tbSettingsControl;
        private System.Windows.Forms.Panel pFileControl;
        private System.Windows.Forms.CheckBox chLooping;
    }
}

