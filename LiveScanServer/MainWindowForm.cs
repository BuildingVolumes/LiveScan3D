//   Copyright (C) 2015  Marek Kowalski (M.Kowalski@ire.pw.edu.pl), Jacek Naruniec (J.Naruniec@ire.pw.edu.pl)
//   License: MIT Software License   See LICENSE.txt for the full license.

//   If you use this software in your research, then please use the following citation:

//    Kowalski, M.; Naruniec, J.; Daniluk, M.: "LiveScan3D: A Fast and Inexpensive 3D Data
//    Acquisition System for Multiple Kinect v2 Sensors". in 3D Vision (3DV), 2015 International Conference on, Lyon, France, 2015

//    @INPROCEEDINGS{Kowalski15,
//        author={Kowalski, M. and Naruniec, J. and Daniluk, M.},
//        booktitle={3D Vision (3DV), 2015 International Conference on},
//        title={LiveScan3D: A Fast and Inexpensive 3D Data Acquisition System for Multiple Kinect v2 Sensors},
//        year={2015},
//    }
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;


namespace KinectServer
{
    public partial class MainWindowForm : Form
    {
        //Those three four are shared with the OpenGL class and are used to exchange data with it.
        //Vertices from all of the sensors
        List<float> lAllVertices = new List<float>();
        //Color data from all of the sensors
        List<byte> lAllColors = new List<byte>();
        //Sensor poses from all of the sensors
        List<Matrix4x4> lAllCameraPoses = new List<Matrix4x4>();
        //Marker poses from all of the sensors
        List<Matrix4x4> lAllMarkerPoses = new List<Matrix4x4>();
        //Viewport settings
        ViewportSettings viewportSettings = new ViewportSettings();

        System.Windows.Forms.Timer tLiveViewTimer;      
        System.Timers.Timer oStatusBarTimer = new System.Timers.Timer();     

        //Settings
        private System.Windows.Forms.Timer scrollTimerExposure = null;
        private System.Windows.Forms.Timer scrollTimerWhiteBalance = null;

        LiveScanServer liveScanServer = null;
        LiveScanState liveScanState = new LiveScanState();

        //Other windows
        SettingsForm settingsForm;
        KinectConfigurationForm configurationForm;

        //The live preview
        OpenGLWindow oOpenGLWindow;

        public MainWindowForm()
        {
            InitializeComponent();
            OpenGLWorker.RunWorkerAsync();
            liveScanServer = new LiveScanServer(this);

            this.Icon = Properties.Resources.Server_Icon;
        }

        private void MainWindowForm_Load(object sender, EventArgs e)
        {
            UpdateUI(liveScanServer.GetState());      
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {         
            tLiveViewTimer.Stop();
            oOpenGLWindow.Unload(); 
        }

        public void UpdateUI(LiveScanState newState)
        {
            ///////////////////////////
            //Set dynamic button states
            ///////////////////////////

            //Client settings
            if (gvClients.Rows.Count < 1)
                btKinectSettingsOpenButton.Enabled = false;
            else
                btKinectSettingsOpenButton.Enabled = true;

            //Calibration Button
            if (newState.appState == appState.calibrating)
            {
                btCalibrate.Text = "Calibrating...";
                btCalibrate.Enabled = false;
            }
            else
            {
                btCalibrate.Text = "Calibrate";
                btCalibrate.Enabled = true;
            }


            //Refine Calibration Button
            if (newState.appState == appState.refinining)
            {
                btRefineCalib.Text = "Refining Calibration...";
                btRefineCalib.Enabled = false;
            }
            else
            {
                btRefineCalib.Text = "Refine Calibration";
                btRefineCalib.Enabled = true;
            }


            //Capture Button
            if (newState.appState == appState.idle)
            {
                btRecord.Image = Properties.Resources.recording;
                btRecord.Text = " Start Capture";
                btRecord.Enabled = true;
            }
            else if(newState.appState == appState.recording)
            {
                btRecord.Image = Properties.Resources.stop;
                btRecord.Text = " Stop capture";
                btRecord.Enabled = true;
            }

            else if (newState.appState == appState.syncing)
            {
                btRecord.Image = null;
                btRecord.Text = " Syncing, please wait";
                btRecord.Enabled = false;
            }
            else if (newState.appState == appState.saving)
            {
                btRecord.Image = Properties.Resources.stop;
                btRecord.Text = " Stop saving";
                btRecord.Enabled = true;
            }


            //////////////
            //Set controls
            //////////////


            //Set client list view
            if (liveScanState.clients != newState.clients)
            {
                ClientConnectionChanged(newState.clients);
            }

            //Temporal Sync
            if (newState.settings.eSyncMode == KinectSettings.SyncMode.off)
            {
                chHardwareSync.Enabled = true;
                chHardwareSync.Checked = false;
                chNetworkSync.Enabled = true;
                chNetworkSync.Enabled = false;
            }

            if (newState.settings.eSyncMode == KinectSettings.SyncMode.Network)
            {
                chHardwareSync.Enabled = false;
                chHardwareSync.Checked = false;
                chNetworkSync.Enabled = true;
                chNetworkSync.Enabled = true;
            }

            if (newState.settings.eSyncMode == KinectSettings.SyncMode.Network)
            {
                chHardwareSync.Enabled = true;
                chHardwareSync.Checked = true;
                chNetworkSync.Enabled = false;
                chNetworkSync.Enabled = false;
            }


            //Exposure
            rExposureAuto.Checked = newState.settings.bAutoExposureEnabled;
            trManualExposure.Value = newState.settings.nExposureStep;
            trManualExposure.Enabled = !newState.settings.bAutoExposureEnabled;

            if(newState.settings.eSyncMode == KinectSettings.SyncMode.Hardware)
            {
                rExposureManual.Checked = true;
                trManualExposure.Enabled = true;
            }

            //White Balance
            rWhiteBalanceAuto.Checked = newState.settings.bAutoWhiteBalanceEnabled;
            trWhiteBalance.Value = newState.settings.nKelvin;
            trWhiteBalance.Enabled = !newState.settings.bAutoWhiteBalanceEnabled;

            //Performance
            chEnablePreview.Checked = newState.settings.bPreviewEnabled;

            //Capture
            rExportRaw.Checked = newState.settings.eExportMode == KinectSettings.ExportMode.RawFrames;
            chMergeScans.Checked = newState.settings.bMergeScansForSave;

            liveScanState = newState;

        }

        public void ShowMessageBox(MessageBoxIcon level, string message, bool quitApp = false)
        {
            string title = "";

            switch (level)
            {
                case MessageBoxIcon.Error:
                    title = "Error!";
                    break;
                case MessageBoxIcon.Warning:
                    title = "Warning";
                    break;
                case MessageBoxIcon.Information:
                    title = "Info";
                    break;
                default:
                    break;
            }

            if (quitApp)
                title = "Unrecoverable Error!";

            Invoke(new Action(() =>
            {
                MessageBox.Show(message, title, MessageBoxButtons.OK, level);

                if (quitApp)
                    Close();
            }));
        }
             
        //Opens the settings form
        private void btSettings_Click(object sender, EventArgs e)
        {
            //Get the latest data
            UpdateUI(liveScanServer.GetState());

            if (settingsForm == null)
            {
                settingsForm = new SettingsForm(liveScanState.settings, liveScanServer);
                settingsForm.Show();
            }

            else
                settingsForm.Focus();
        }

        //This is used for: starting/stopping the recording worker, stopping the saving worker
        private void btRecord_Click(object sender, EventArgs e)
        {
            liveScanServer.RecordButtonClicked();
        }

        private void btCalibrate_Click(object sender, EventArgs e)
        {
            liveScanServer.Calibrate();
        }

        private void btRefineCalib_Click(object sender, EventArgs e)
        {
            liveScanServer.RefineCalibration();
        }

        public void SetStatusBarOnTimer(string message, int milliseconds)
        {
            //Thread save change of UI Element
            Invoke(new Action(() => { statusLabel.Text = message; }));            

            oStatusBarTimer.Stop();
            oStatusBarTimer = new System.Timers.Timer();

            oStatusBarTimer.Interval = milliseconds;
            oStatusBarTimer.Elapsed += delegate (object sender, System.Timers.ElapsedEventArgs e)
            {
                oStatusBarTimer.Stop();
                Invoke(new Action(() => { statusLabel.Text = ""; }));
            };
            oStatusBarTimer.Start();
        }

        private void ClientConnectionChanged(List<KinectSocket> socketList)
        {
            //Disable the temporal hardware sync if all clients disconnected
            if (socketList.Count == 0)
            {
                Invoke(new Action(() => { chHardwareSync.Checked = false; }));
            }

            UpdateClientGridView(socketList);
        }

        public void UpdateClientGridView(List<KinectSocket> socketList)
        {
            List<string[]> gridViewRows = new List<string[]>();

            for (int i = 0; i < socketList.Count; i++)
            {
                if (socketList[i].configuration != null)
                {
                  gridViewRows.Add(new string[]
                  { 
                    socketList[i].configuration.globalDeviceIndex.ToString(),
                    socketList[i].configuration.SerialNumber,
                    socketList[i].GetIP(),
                    socketList[i].bCalibrated.ToString(),
                    socketList[i].configuration.eSoftwareSyncState.ToString()
                  });
                }

            }

            // Invoke UI logic on the same thread.
            gvClients.BeginInvoke(new Action(() =>
            {
                gvClients.Rows.Clear();

                foreach (string[] row in gridViewRows)
                {
                    gvClients.Rows.Add(row);
                }
            }));
        }

        private void btKinectSettingsOpenButton_Click(object sender, EventArgs e)
        {
            if (gvClients.SelectedRows.Count < 1)
            {
                return;
            }

            int index = gvClients.SelectedRows[0].Index;
            
            if (configurationForm == null)
            {
                configurationForm = new KinectConfigurationForm();
            }

            configurationForm.Initialize(oServer, gvClients.SelectedRows[0].Index);
            configurationForm.Show();
            configurationForm.Focus();
            oServer.SetKinectSettingsForm(gvClients.SelectedRows[0].Index, form);
        }

        private void chHardwareSync_Clicked(object sender, EventArgs e)
        {
            if (chHardwareSync.Checked && !chNetworkSync.Checked)
                liveScanServer.SetSyncMode(KinectSettings.SyncMode.Hardware);
            if (!chHardwareSync.Checked && !chNetworkSync.Checked)
                liveScanServer.SetSyncMode(KinectSettings.SyncMode.off);

        }

        private void chNetworkSync_Clicked(object sender, EventArgs e)
        {
            if (chNetworkSync.Checked && !chHardwareSync.Checked)
                liveScanServer.SetSyncMode(KinectSettings.SyncMode.Network);
            if (!chNetworkSync.Checked && !chHardwareSync.Checked)
                liveScanServer.SetSyncMode(KinectSettings.SyncMode.off);
        }

        private void rExposureAuto_Clicked(object sender, EventArgs e)
        {
            if (rExposureAuto.Checked)
                liveScanServer.SetExposureMode(true);
        }

        private void rExposureManual_Clicked(object sender, EventArgs e)
        {
            if (rExposureAuto.Checked)
                liveScanServer.SetExposureMode(false);
        }

        /// <summary>
        /// When the user scrolls on the trackbar, we wait a short amount of time to check if the user has scrolled again.
        /// This prevents the Manual Exposure to be set too often, and only sets it when the user has stopped scrolling.
        /// Code taken from: https://stackoverflow.com/a/15687418
        /// </summary>
        private void trManualExposure_Scroll(object sender, EventArgs e)
        {
            if (scrollTimerExposure == null)
            {
                // Will tick every 500ms
                scrollTimerExposure = new System.Windows.Forms.Timer()
                {
                    Enabled = false,
                    Interval = 300,
                    Tag = (sender as TrackBar).Value
                };

                scrollTimerExposure.Tick += (s, ea) =>
                {
                    // check to see if the value has changed since we last ticked
                    if (trManualExposure.Value == (int)scrollTimerExposure.Tag)
                    {
                        // scrolling has stopped so we are good to go ahead and do stuff
                        scrollTimerExposure.Stop();

                        // Send the changed exposure to the devices

                        //Clamp Exposure Step between -11 and 1
                        int exposureStep = trManualExposure.Value;
                        int exposureStepClamped = exposureStep < -11 ? -11 : exposureStep > 1 ? 1 : exposureStep;
                        liveScanServer.SetExposureValue(exposureStepClamped);

                        scrollTimerExposure.Dispose();
                        scrollTimerExposure = null;
                    }
                    else
                    {
                        // record the last value seen
                        scrollTimerExposure.Tag = trManualExposure.Value;
                    }
                };
                scrollTimerExposure.Start();
            }
        }

        private void cbEnablePreview_Clicked(object sender, EventArgs e)
        {
            liveScanServer.SetClientPreview(chEnablePreview.Checked);
        }

        private void rExportRaw_Clicked(object sender, EventArgs e)
        {
            liveScanServer.SetExportMode(KinectSettings.ExportMode.RawFrames);
        }

        private void rExportPointclouds_Clicked(object sender, EventArgs e)
        {
            liveScanServer.SetExportMode(KinectSettings.ExportMode.Pointcloud);
        }

        private void chMergeScans_CheckedChanged(object sender, EventArgs e)
        {
           liveScanServer.SetMergeScans(chMergeScans.Checked);
        }

        private void rWhiteBalanceAuto_CheckedChanged(object sender, EventArgs e)
        {
            liveScanServer.SetWhiteBalanceMode(true);
        }

        private void rWhiteBalanceManual_CheckedChanged(object sender, EventArgs e)
        {
            liveScanServer.SetWhiteBalanceMode(false);
        }

        private void tbWhiteBalance_Scroll(object sender, EventArgs e)
        {
            if (scrollTimerWhiteBalance == null)
            {
                // Will tick every 500ms
                scrollTimerWhiteBalance = new System.Windows.Forms.Timer()
                {
                    Enabled = false,
                    Interval = 300,
                    Tag = (sender as TrackBar).Value
                };

                scrollTimerWhiteBalance.Tick += (s, ea) =>
                {
                    // check to see if the value has changed since we last ticked
                    if (trWhiteBalance.Value == (int)scrollTimerWhiteBalance.Tag)
                    {
                        // scrolling has stopped so we are good to go ahead and do stuff
                        scrollTimerWhiteBalance.Stop();

                        liveScanServer.SetWhiteBalanceValue(trWhiteBalance.Value);

                        scrollTimerWhiteBalance.Dispose();
                        scrollTimerWhiteBalance = null;
                    }
                    else
                    {
                        // record the last value seen
                        scrollTimerWhiteBalance.Tag = trWhiteBalance.Value;
                    }
                };
                scrollTimerWhiteBalance.Start();
            }
        }

        #region LivePreviewRender

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
            tLiveViewTimer.Tick += (senderer, ea) => {Render();};
            tLiveViewTimer.Interval = 16;   // 60 fps
            tLiveViewTimer.Start();

            glLiveView_Resize(glLiveView, EventArgs.Empty);

            oOpenGLWindow.Load();

            RestartUpdateWorker();
        }

        private void glLiveView_Resize(object sender, EventArgs e)
        {
            glLiveView.MakeCurrent();

            if (glLiveView.ClientSize.Height == 0)
                glLiveView.ClientSize = new System.Drawing.Size(glLiveView.ClientSize.Width, 1);

            if(oOpenGLWindow != null)
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

        private void UpdateFPSCounter(int fps)
        {
            // Invoke UI logic on the same thread.
            lFPS.BeginInvoke(new Action(() =>
            {
                lFPS.Text = fps.ToString() + " FPS";
            }));
        }

        #endregion

    }
}
