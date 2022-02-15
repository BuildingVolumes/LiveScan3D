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
        OpenGLWindow openGLWindow = null;

        BindingList<IFrameFileReader> lFrameFiles = new BindingList<IFrameFileReader>();
        bool bPlayerRunning = false;

        List<float> lAllVertices = new List<float>();
        List<byte> lAllColors = new List<byte>();

        ViewportSettings viewportSettings = new ViewportSettings();

        TransferServer oTransferServer = new TransferServer();

        AutoResetEvent eUpdateWorkerFinished = new AutoResetEvent(false);

        public PlayerWindowForm()
        {
            InitializeComponent();

            //oTransferServer.lVertices = lAllVertices;
            //oTransferServer.lColors = lAllColors;
            viewportSettings.targetFPS = (int)nUDFramerate.Value;
            lFrameFilesListView.Columns.Add("Current frame", 75);
            lFrameFilesListView.Columns.Add("Filename", 300);
        }

        private void PlayerWindowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            bPlayerRunning = false;
            //oTransferServer.StopServer();
        }

        private void btSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.ShowDialog();

            lock (lFrameFiles)
            {
                for (int i = 0; i < dialog.FileNames.Length; i++)
                {
                    lFrameFiles.Add(new FrameFileReaderBin(dialog.FileNames[i]));

                    var item = new ListViewItem(new[] { "0", dialog.FileNames[i] });
                    lFrameFilesListView.Items.Add(item);

                    //.bin files use BGR color mode
                    viewportSettings.colorMode = ViewportSettings.EColorMode.BGR;
                }
            }
        }

        private void btnSelectPly_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.ShowDialog();

            if (dialog.FileNames.Length == 0)
                return;

            lock (lFrameFiles)
            {
                lFrameFiles.Add(new FrameFileReaderPly(dialog.FileNames));

                var item = new ListViewItem(new[] { "0", Path.GetDirectoryName(dialog.FileNames[0]) });
                lFrameFilesListView.Items.Add(item);

                //Pointclouds use RGB color mode
                viewportSettings.colorMode = ViewportSettings.EColorMode.RGB;
            }
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            bPlayerRunning = !bPlayerRunning;

            if (bPlayerRunning)
            {
                //oTransferServer.StartServer();
                updateWorker.RunWorkerAsync();
                btStart.Text = "Pause";
            }
            else
            {
                //oTransferServer.StopServer();
                btStart.Text = "Play";
                eUpdateWorkerFinished.WaitOne();
            }
        }

        private void btRemove_Click(object sender, EventArgs e)
        {
            if (lFrameFilesListView.SelectedIndices.Count == 0)
                return;

            lock (lFrameFiles)
            {
                int idx = lFrameFilesListView.SelectedIndices[0];
                lFrameFilesListView.Items.RemoveAt(idx);
                lFrameFiles.RemoveAt(idx);
            }
        }

        private void btRewind_Click(object sender, EventArgs e)
        {
            lock (lFrameFiles)
            {
                for (int i = 0; i < lFrameFiles.Count; i++)
                {
                    lFrameFiles[i].Rewind();
                    lFrameFilesListView.Items[i].Text = "0";
                }
            }
        }

        private void btShow_Click(object sender, EventArgs e)
        {
            if (!OpenGLWorker.IsBusy)
            {
                OpenGLWorker.RunWorkerAsync();
            }
        }

        private void updateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int curFrameIdx = 0;
            string outDir = "outPlayer\\";
            DirectoryInfo di = Directory.CreateDirectory(outDir);

            Stopwatch stopwatch = new Stopwatch();

            while (bPlayerRunning)
            {
                //We measure how long the main thread and the OpenGL Thread need to render the frame and
                //if they are faster than the target FPS, let the thread sleep for the remaining time,
                //so that we don't play too fast
                stopwatch.Stop();

                int openGLMS = 0;
                if (openGLWindow != null)
                    openGLMS = openGLWindow.GetLastFrameTimeMS();

                int totalMS = (int)stopwatch.ElapsedMilliseconds + openGLMS;
                int targetMS = 1000 / viewportSettings.targetFPS;
                if(totalMS < targetMS)
                {
                    Thread.Sleep(targetMS - totalMS);
                }

                stopwatch.Restart();

                List<float> tempAllVertices = new List<float>();
                List<byte> tempAllColors = new List<byte>();

                lock (lFrameFiles)
                {
                    for (int i = 0; i < lFrameFiles.Count; i++)
                    {
                        List<float> vertices = new List<float>();
                        List<byte> colors = new List<byte>();
                        lFrameFiles[i].ReadFrame(vertices, colors);

                        tempAllVertices.AddRange(vertices);
                        tempAllColors.AddRange(colors);

                        Console.WriteLine("Frame: " + curFrameIdx + " FileID: " + i);
                    }
                }

                Thread frameIdxUpdate = new Thread(() => this.Invoke((MethodInvoker)delegate { this.UpdateDisplayedFrameIndices(); }));
                frameIdxUpdate.Start();

                lock (lAllVertices)
                {
                    lAllVertices.Clear();
                    lAllColors.Clear();
                    lAllVertices.AddRange(tempAllVertices);
                    lAllColors.AddRange(tempAllColors);
                }

                if (chSaveFrames.Checked)
                    SaveCurrentFrameToFile(outDir, curFrameIdx);


                curFrameIdx++;

                if(openGLWindow != null)
                    openGLWindow.CloudUpdateTick();
            }

            eUpdateWorkerFinished.Set();
        }

        private void OpenGLWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if(openGLWindow == null)
            {
                openGLWindow = new OpenGLWindow();

                openGLWindow.vertices = lAllVertices;
                openGLWindow.colors = lAllColors;
                openGLWindow.viewportSettings = this.viewportSettings;

                openGLWindow.Run();
            }

        }

        private void lFrameFilesListView_DoubleClick(object sender, EventArgs e)
        {
            lFrameFilesListView.SelectedItems[0].BeginEdit();
        }

        private void lFrameFilesListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            int fileIdx = lFrameFilesListView.SelectedIndices[0];
            int frameIdx;
            bool res = Int32.TryParse(e.Label, out frameIdx);

            if (!res)
            {
                e.CancelEdit = true;
                return;
            }

            lock (lFrameFiles)
            {
                lFrameFiles[fileIdx].JumpToFrame(frameIdx);
            }

        }

        private void UpdateDisplayedFrameIndices()
        {
            lock (lFrameFiles)
            {
                for (int i = 0; i < lFrameFiles.Count; i++)
                {
                    lFrameFilesListView.Items[i].SubItems[0].Text = lFrameFiles[i].frameIdx.ToString();
                }
            }
        }

        private void SaveCurrentFrameToFile(string outDir, int frameIdx)
        {
            List<float> lVertices = new List<float>();
            List<byte> lColors = new List<byte>();

            lock (lAllVertices)
            {
                lVertices.AddRange(lAllVertices);
            }

            if (viewportSettings.colorMode == ViewportSettings.EColorMode.RGB)
            {
                lColors.AddRange(lAllColors);
            }

            //Convert BGR to RGB
            if (viewportSettings.colorMode == ViewportSettings.EColorMode.BGR)
            {
                for (int i = 0; i < lAllColors.Count; i += 3)
                {
                    byte[] tempCol = new byte[3];
                    tempCol[0] = lAllColors[i + 2];
                    tempCol[1] = lAllColors[i + 1];
                    tempCol[2] = lAllColors[i + 0];

                    lColors.AddRange(tempCol);
                }
            }

            string outputFilename = outDir + frameIdx.ToString().PadLeft(5, '0') + ".ply";
            Utils.saveToPly(outputFilename, lVertices, lColors, true);
        }

        private void PlayerWindowForm_Load(object sender, EventArgs e)
        {

        }

        private void tbPointsize_Scroll(object sender, EventArgs e)
        {
            viewportSettings.pointSize = tbPointsize.Value;
        }

        private void tbBrightness_Scroll(object sender, EventArgs e)
        {
            int brightness = tbBrightness.Value * 10;
            brightness = Math.Min(255, brightness);
            brightness = Math.Max(0, brightness);

            viewportSettings.brightness = tbBrightness.Value;
        }

        private void chShowGizmos_CheckedChanged(object sender, EventArgs e)
        {
            viewportSettings.markerVisibility = chShowGizmos.Checked;
        }

        private void nUDFramerate_ValueChanged(object sender, EventArgs e)
        {
            viewportSettings.targetFPS = (int)nUDFramerate.Value;
        }
    }
}
