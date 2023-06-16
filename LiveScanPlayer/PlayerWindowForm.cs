using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using KinectServer;
using System.Diagnostics;

namespace LiveScanPlayer
{
    public partial class PlayerWindowForm : Form
    {
        OpenGLWindow oOpenGLWindow = null;
        System.Windows.Forms.Timer tLiveViewTimer;
        float fTargetFPS = 30f;
        long lActualFPS = 30;
        float fFpsUpdateTimer;

        int nMaxFrames;
        int nCurrentFrame;

        BindingList<IFrameFileReader> lFrameFiles = new BindingList<IFrameFileReader>();

        bool bAppOpen = true;

        bool bPaused = true;
        bool bLoop = true;
        bool bScrollbarInUse = false;

        List<float> lAllVertices = new List<float>();
        List<byte> lAllColors = new List<byte>();

        ViewportSettings viewportSettings = new ViewportSettings();

        string lastPLYDir = String.Empty;
        string lastBINDir = String.Empty;

        public PlayerWindowForm()
        {
            InitializeComponent();
            OpenGLWorker.RunWorkerAsync();
            updateWorker.RunWorkerAsync();
            lFrameFilesListView.Columns.Add("Files:", 300);
            this.Icon = Properties.Resources.Player_Icon;
        }

        private void PlayerWindowForm_Load(object sender, EventArgs e)
        {

        }

        private void PlayerWindowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            bAppOpen = false;
            StopUpdateWorker();
        }

        private void btSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Livescan binary files (*.bin) |*.bin";
            dialog.Multiselect = true;

            if (lastBINDir != String.Empty)
            {
                dialog.InitialDirectory = lastBINDir;
                dialog.RestoreDirectory = false;
            }

            dialog.ShowDialog();

            if (dialog.FileNames.Length == 0)
                return;

            lastBINDir = Path.GetDirectoryName(dialog.FileNames[0]);

            lock (lFrameFiles)
            {
                for (int i = 0; i < dialog.FileNames.Length; i++)
                {
                    lFrameFiles.Add(new FrameFileReaderBin(dialog.FileNames[i]));

                    var item = new ListViewItem(new[] { dialog.FileNames[i] });
                    lFrameFilesListView.Items.Add(item);
                }

                lFrameFilesListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }

            nMaxFrames = GetMaxFrameCount();
            UpdateScrollBar();
        }

        private void btnSelectPly_Click(object sender, EventArgs e)
        {

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = "ply files (*.ply) |*.ply";

            if (lastPLYDir != String.Empty)
            {
                dialog.InitialDirectory = lastPLYDir;
                dialog.RestoreDirectory = false;
            }

            dialog.ShowDialog();

            if (dialog.FileNames.Length == 0)
                return;

            lastPLYDir = Path.GetDirectoryName(dialog.FileNames[0]);

            lock (lFrameFiles)
            {
                lFrameFiles.Add(new FrameFileReaderPly(dialog.FileNames));

                var item = new ListViewItem(new[] { Path.GetDirectoryName(dialog.FileNames[0]) });
                lFrameFilesListView.Items.Add(item);

                lFrameFilesListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            }

            nMaxFrames = GetMaxFrameCount();
            UpdateScrollBar();
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            if (bPaused)
                Play();

            else
                Pause();
        }

        private void Play()
        {
            lock (lFrameFiles)
            {
                if (lFrameFiles.Count == 0)
                    return;
            }

            btStart.Image = Properties.Resources.pause;
            bPaused = false;
            return;
        }

        private void Pause()
        {
            lock (lFrameFiles)
            {
                if (lFrameFiles.Count == 0)
                    return;
            }

            bPaused = true;
            btStart.Image = Properties.Resources.Play;
            return;
        }

        private void btRemove_Click(object sender, EventArgs e)
        {
            if (lFrameFilesListView.SelectedIndices.Count == 0)
                return;

            lock (lFrameFiles)
            {
                if (lFrameFiles.Count == 1)
                {
                    RewindPlayer();
                    ClearAllFrames();
                    Pause();
                }

                int idx = lFrameFilesListView.SelectedIndices[0];
                lFrameFilesListView.Items.RemoveAt(idx);
                lFrameFiles[idx].CloseReader();
                lFrameFiles.RemoveAt(idx);
            }

            nMaxFrames = GetMaxFrameCount();
            UpdateScrollBar();

            return;
        }

        private void btRewind_Click(object sender, EventArgs e)
        {
            RewindPlayer();
        }

        private void updateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string outDir = "outPlayer\\";
            DirectoryInfo di = Directory.CreateDirectory(outDir);

            Stopwatch stopwatch = new Stopwatch();

            BackgroundWorker worker = (BackgroundWorker)sender;

            Stopwatch updateTimer = new Stopwatch();
            updateTimer.Start();

            Stopwatch fpsCountTimer = new Stopwatch();
            fpsCountTimer.Restart();

            while (!worker.CancellationPending)
            {
                if (oOpenGLWindow != null)
                {
                    if (!bScrollbarInUse)
                    {
                        if (updateTimer.ElapsedMilliseconds > 1000f / fTargetFPS)
                        {
                            if (nCurrentFrame < nMaxFrames)
                            {
                                updateTimer.Restart();

                                if (!bPaused)
                                {
                                    lock (lAllVertices)
                                    {
                                        lAllVertices.Clear();
                                        lAllColors.Clear();

                                        lock (lFrameFiles)
                                        {
                                            for (int i = 0; i < lFrameFiles.Count; i++)
                                            {
                                                List<float> vertices = new List<float>();
                                                List<byte> colors = new List<byte>();
                                                lFrameFiles[i].ReadFrame(lAllVertices, lAllColors);
                                            }
                                        }

                                        if (chSaveFrames.Checked)
                                            SaveCurrentFrameToFile(outDir, nCurrentFrame);

                                        nCurrentFrame++;
                                        Console.WriteLine(nCurrentFrame);
                                    }                                    
                                }                                

                                try
                                {
                                    this.Invoke((MethodInvoker)delegate { this.UpdateScrollBar(); });
                                }

                                catch (Exception ex)
                                {
                                    //Can happen when the windows form is closing
                                }                             
                            }

                            else
                            {
                                if (bLoop)
                                    RewindPlayer();
                                else
                                {
                                    Pause();
                                    RewindPlayer();
                                }
                            }

                            if (fpsCountTimer.ElapsedMilliseconds > 0)
                            {
                                lActualFPS = 1000 / fpsCountTimer.ElapsedMilliseconds;
                                fFpsUpdateTimer += fpsCountTimer.ElapsedMilliseconds;
                                fpsCountTimer.Restart();

                                //Otherwise backgroundworker might call this method, while the form is already closing
                                if (bAppOpen)
                                {
                                    try
                                    {
                                        this.Invoke((MethodInvoker)delegate { this.ShowFPS(); });
                                    }

                                    catch (Exception ex) { }//Can happen when form is closing
                                }
                            }
                        }
                    }
                }
            }
        }

        private void StopUpdateWorker()
        {
            bPaused = false;
            btStart.Image = Properties.Resources.Play;
            updateWorker.CancelAsync();
        }

        private void OpenGLWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (oOpenGLWindow == null)
            {
                oOpenGLWindow = new OpenGLWindow();

                oOpenGLWindow.vertices = lAllVertices;
                oOpenGLWindow.colors = lAllColors;
                oOpenGLWindow.viewportSettings = this.viewportSettings;
            }
        }

        private void OpenGLWindowClosed(object sender, EventArgs EventArgs)
        {
            oOpenGLWindow = null;
        }

        private void RewindPlayer()
        {
            lock (lFrameFiles)
            {
                if(lFrameFiles.Count == 0)
                {
                    return;
                }

                for (int i = 0; i < lFrameFiles.Count; i++)
                {
                    lFrameFiles[i].Rewind();
                }
            }

            nCurrentFrame = 0;

            ClearAllFrames();
        }

        private void ClearAllFrames()
        {
            lock (lAllVertices)
            {
                lAllVertices.Clear();
                lAllColors.Clear();
            }
        }

        private int GetMaxFrameCount()
        {
            int count = 0;

            lock (lFrameFiles)
            {
                for (int i = 0; i < lFrameFiles.Count; i++)
                {
                    if (lFrameFiles[i].totalFrames > count)
                        count = lFrameFiles[i].totalFrames;
                }
            }

            return count;
        }

        private void SaveCurrentFrameToFile(string outDir, int frameIdx)
        {
            List<float> lVertices = new List<float>();
            List<byte> lColors = new List<byte>();

            lock (lAllVertices)
            {
                lVertices.AddRange(lAllVertices);
                lColors.AddRange(lAllColors);

            }

            string outputFilename = outDir + frameIdx.ToString().PadLeft(5, '0') + ".ply";
            Utils.saveToPly(outputFilename, lVertices, lColors, viewportSettings.colorMode, true);
        }



        private void tbPointsize_Scroll(object sender, EventArgs e)
        {
            viewportSettings.pointSize = tbPointsize.Value;
        }

        private void tbBrightness_Scroll(object sender, EventArgs e)
        {
            int brightness = tbBrightness.Value * 7;
            brightness = Math.Min(255, brightness);
            brightness = Math.Max(0, brightness);

            viewportSettings.brightness = brightness;
        }

        private void chShowGizmos_CheckedChanged(object sender, EventArgs e)
        {
            viewportSettings.markerVisibility = chShowGizmos.Checked;
        }

        private void nUDFramerate_ValueChanged(object sender, EventArgs e)
        {
            fTargetFPS = (int)nUDFramerate.Value;
        }

        private void UpdateScrollBar()
        {
            string paddedCurrentFrame = nCurrentFrame.ToString().PadLeft(nMaxFrames.ToString().Length, '0');
            lFrameCounter.Text = paddedCurrentFrame + "/" + nMaxFrames.ToString();


            if (tbVideoScroll.Maximum != nMaxFrames)
                tbVideoScroll.Maximum = nMaxFrames;

            tbVideoScroll.Value = nCurrentFrame;
        }

        private void ShowFPS()
        {
            //Only update the FPS counter every second or so
            if (fFpsUpdateTimer > 500)
            {
                lFPSCounter.Text = "FPS: " + lActualFPS.ToString();
                fFpsUpdateTimer = 0;
            }
        }

        private void tbVideoScroll_ValueChanged(object sender, EventArgs e)
        {
            lock (lFrameFiles)
            {
                for (int i = 0; i < lFrameFiles.Count; i++)
                {
                    lFrameFiles[i].JumpToFrame(tbVideoScroll.Value);
                }

                nCurrentFrame = tbVideoScroll.Value;
            }

        }

        private void tbVideoScroll_MouseDown(object sender, MouseEventArgs e)
        {
            bScrollbarInUse = true;

            float scrollElementStartPos = 8;
            float scrollElementEndPos = tbVideoScroll.Bounds.Width - 12;
            //As the trackbar is actually a bit smaller than the trackbar elements reported width, we need to map the mouse position to the smaller range to get an accurate position on it
            float mousePosMappedToWidth = (float)tbVideoScroll.Bounds.Width * ((float)e.X - scrollElementStartPos) / (scrollElementEndPos - scrollElementStartPos);

            float percentageClicked = ((1f / tbVideoScroll.Bounds.Width) * mousePosMappedToWidth);
            int nTargetFrame = (int)(nMaxFrames * percentageClicked);

            if (nTargetFrame < 0)
                nTargetFrame = 0;

            if (nTargetFrame > nMaxFrames)
                nTargetFrame = nMaxFrames;


            lock (lFrameFiles)
            {
                for (int i = 0; i < lFrameFiles.Count; i++)
                {
                    lFrameFiles[i].JumpToFrame(nTargetFrame);
                }

                nCurrentFrame = nTargetFrame;
            }

        }
        private void tbVideoScroll_MouseUp(object sender, MouseEventArgs e)
        {
            bScrollbarInUse = false;
            UpdateScrollBar();
        }

        /// <summary>
        /// Gets called when the Live View Window is being loaded. Sets up the render intervall and events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glLiveView_Load(object sender, EventArgs e)
        {
            // Make sure that when the GLControl is resized or needs to be painted,
            // we update our projection matrix or re-render its contents, respectively.
            glLiveView.Resize += glLiveView_Resize;
            glLiveView.Paint += glLiveView_Paint;

            // We have to update the embedded GL window ourselves
            tLiveViewTimer = new System.Windows.Forms.Timer();
            tLiveViewTimer.Tick += (senderer, ea) =>
            {
                Render();
            };
            tLiveViewTimer.Interval = 8;   // 120 fps
            tLiveViewTimer.Start();

            glLiveView_Resize(glLiveView, EventArgs.Empty);

            oOpenGLWindow.Load();
        }

        private void glLiveView_Resize(object sender, EventArgs e)
        {
            glLiveView.MakeCurrent();

            if (glLiveView.ClientSize.Height == 0)
                glLiveView.ClientSize = new System.Drawing.Size(glLiveView.ClientSize.Width, 1);

            oOpenGLWindow.Resize(glLiveView.ClientSize.Width, glLiveView.ClientSize.Height);
        }

        private void glLiveView_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }

        private void Render()
        {
            glLiveView.MakeCurrent();
            oOpenGLWindow.UpdateFrame();
            oOpenGLWindow.RenderFrame();
            glLiveView.SwapBuffers();
        }

        private void glLiveView_MouseDown(object sender, MouseEventArgs e)
        {
            oOpenGLWindow.OnMouseButtonDown(sender, e);
        }

        private void glLiveView_MouseMove(object sender, MouseEventArgs e)
        {
            oOpenGLWindow.OnMouseMove(sender, e);
        }

        private void glLiveView_MouseUp(object sender, MouseEventArgs e)
        {
            oOpenGLWindow.OnMouseButtonUp(sender, e);
        }

        private void glLiveView_Scroll(object sender, MouseEventArgs e)
        {
            oOpenGLWindow.OnMouseWheelChanged(sender, e);
        }

        private void glLiveView_KeyDown(object sender, KeyEventArgs e)
        {
            oOpenGLWindow.OnKeyDown(sender, e);
        }

        private void chLooping_CheckedChanged(object sender, EventArgs e)
        {
            bLoop = chLooping.Checked;
        }
    }
}
