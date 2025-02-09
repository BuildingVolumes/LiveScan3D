﻿//   Copyright (C) 2015  Marek Kowalski (M.Kowalski@ire.pw.edu.pl), Jacek Naruniec (J.Naruniec@ire.pw.edu.pl)
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
using System.Windows.Forms;
using System.Drawing;
using LiveScanServer.Properties;
using System.Threading;
using System.Reflection.Emit;

namespace LiveScanServer
{
    public partial class MainWindowForm : Form
    {
        //Update Timers
        System.Windows.Forms.Timer UpdateTimer60FPS;
        System.Windows.Forms.Timer UpdateTimer1FPS;

        //Settings
        private System.Windows.Forms.Timer scrollTimerExposure = null;
        private System.Windows.Forms.Timer scrollTimerWhiteBalance = null;

        LiveScanServer liveScanServer = null;
        LiveScanState liveScanState = new LiveScanState();

        //Other windows
        SettingsForm settingsForm;
        public Dictionary<string, ClientConfigurationForm> clientConfigurationsForms;

        //The live preview
        OpenGLView oOpenGLView;

        //Image resources
        Image stopImage;
        Image recordImage;
        Image loadAnimImage;

        bool updateUIRequest = false;
        bool mBoxRequest = false;
        object oUpdateUILock = new object();
        object oMBoxLock = new object();

        MessageBoxIcon mBoxLevel = MessageBoxIcon.None;
        string mBoxMessage = "";
        bool mBoxQuitApp = false;

        public MainWindowForm()
        {
            InitializeComponent();
            liveScanServer = new LiveScanServer(this);
            oOpenGLView = new OpenGLView();
            clientConfigurationsForms = new Dictionary<string, ClientConfigurationForm>();

            stopImage = Resources.stop;
            recordImage = Resources.recording;
            loadAnimImage = Resources.Loading_Animation;

            this.Icon = Resources.Server_Icon;
        }

        private void MainWindowForm_Load(object sender, EventArgs e)
        {
            StartUpdateTimers();
            liveScanServer.QueueUIUpdate();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            liveScanServer.Terminate();
            UpdateTimer60FPS.Stop();
            UpdateTimer1FPS.Stop();
            oOpenGLView.Unload();
        }

        public void RequestUIUpdate()
        {
            lock (oUpdateUILock)
                updateUIRequest = true;
        }
        public void UIUpdateImmidiate()
        {
            SetUIState(liveScanServer.GetState());
        }

        //For UI stuff that needs a regular, but not real-time update at 1 FPS 
        public void SlowUpdate()
        {
            lFPS.Text = liveScanState.previewWindowFPS.ToString() + " FPS";
            SetStateIndicator(liveScanState.appState, liveScanState.stateIndicatorSuffix);
        }

        //For UI Stuff that needs to update as fast as possible, running at 60 FPS
        private void FastUpdate()
        {
            if (updateUIRequest)
            {
                lock (oUpdateUILock)
                    updateUIRequest = false;
                SetUIState(liveScanServer.GetState());
            }

            ShowMessageBox();

            glLiveView.MakeCurrent();
            oOpenGLView.UpdateFrame();
            oOpenGLView.RenderFrame();
            glLiveView.SwapBuffers();
        }

        void SetUIState(LiveScanState newState)
        {
            //Calibration Button
            if (newState.appState == appState.calibrating)
            {
                btCalibrate.Text = "Stop calibration";
                btCalibrate.Enabled = true;
            }

            else if (newState.appState == appState.idle)
            {
                btCalibrate.Text = "Calibrate";
                btCalibrate.Enabled = true;
            }

            else
                btCalibrate.Enabled = false;


            //Refine Calibration Button
            if (newState.appState == appState.refinining)
            {
                btRefineCalib.Text = "Stop Refinement";
                btRefineCalib.Enabled = true;
            }

            else if (newState.appState == appState.idle)
            {
                btRefineCalib.Text = "Refine Calibration";
                btRefineCalib.Enabled = true;
            }

            else
                btRefineCalib.Enabled = false;


            //Capture Button
            if (newState.appState == appState.idle)
            {
                btRecord.Image = recordImage;
                btRecord.Text = " Start Capture";
                btRecord.Enabled = true;
            }
            else if (newState.appState == appState.recording)
            {
                btRecord.Image = stopImage;
                btRecord.Text = " Stop capture";
                btRecord.Enabled = true;
            }

            else if (newState.appState == appState.syncing)
            {
                btRecord.Image = null;
                btRecord.Text = " Syncing, please wait";
                btRecord.Enabled = false;
            }
            else if (newState.appState == appState.downloading)
            {
                btRecord.Image = stopImage;
                btRecord.Text = " Stop Downloading";
                btRecord.Enabled = true;
            }

            else
            {
                btRecord.Enabled = false;
            }

            //Apply buttons in KinectConfiguration
            foreach (KeyValuePair<string, ClientConfigurationForm> ccf in clientConfigurationsForms)
            {
                ccf.Value.SetButtonsInteractive(newState.appState == appState.idle);
            }



            //////////////
            //Set controls
            //////////////


            //Set client list view
            UpdateClientGridView(newState.clients);

            //Check if we need to close configuration forms
            List<ClientConfigurationForm> ccfsToClose = new List<ClientConfigurationForm>();
            foreach (KeyValuePair<string, ClientConfigurationForm> ccf in clientConfigurationsForms)
            {
                bool clientStillConnected = false;

                for (int i = 0; i < newState.clients.Count; i++)
                {
                    if (newState.clients[i].configuration != null)
                    {
                        if (ccf.Key == newState.clients[i].configuration.SerialNumber)
                            clientStillConnected = true;
                    }
                }

                if (!clientStillConnected)
                    ccfsToClose.Add(ccf.Value);
            }

            for (int i = ccfsToClose.Count - 1; i >= 0; i--)
            {
                ccfsToClose[i].CloseConfiguration();
            }

            //Temporal Sync
            if (newState.settings.eSyncMode == ClientSettings.SyncMode.Off)
            {
                chHardwareSync.Enabled = true;
                chHardwareSync.Checked = false;
                chNetworkSync.Enabled = true;
                chNetworkSync.Checked = false;
            }

            if (newState.settings.eSyncMode == ClientSettings.SyncMode.Network)
            {
                chHardwareSync.Enabled = false;
                chHardwareSync.Checked = false;
                chNetworkSync.Enabled = true;
                chNetworkSync.Checked = true;
            }

            if (newState.settings.eSyncMode == ClientSettings.SyncMode.Hardware)
            {
                chHardwareSync.Enabled = true;
                chHardwareSync.Checked = true;
                chNetworkSync.Enabled = false;
                chNetworkSync.Checked = false;
            }

            //Álso lock hardware sync button when the sensors are doing anything
            if (newState.appState == appState.idle)
                chHardwareSync.Enabled = true;

            else
                chHardwareSync.Enabled = false;

            //Exposure
            rExposureAuto.Checked = newState.settings.bAutoExposureEnabled;
            rExposureManual.Checked = !newState.settings.bAutoExposureEnabled;
            trManualExposure.Value = newState.settings.nExposureStep;
            trManualExposure.Enabled = !newState.settings.bAutoExposureEnabled;

            if (newState.settings.eSyncMode == ClientSettings.SyncMode.Hardware)
            {
                rExposureManual.Checked = true;
                rExposureAuto.Checked = false;
                rExposureManual.Enabled = false;
                rExposureAuto.Enabled = false;
                trManualExposure.Enabled = true;
            }

            else
            {
                rExposureManual.Enabled = true;
                rExposureAuto.Enabled = true;
            }

            //White Balance
            rWhiteBalanceAuto.Checked = newState.settings.bAutoWhiteBalanceEnabled;
            rWhiteBalanceManual.Checked = !newState.settings.bAutoWhiteBalanceEnabled;
            trWhiteBalance.Enabled = !newState.settings.bAutoWhiteBalanceEnabled;
            trWhiteBalance.Value = newState.settings.nKelvin;

            //Performance
            chEnablePreview.Checked = newState.settings.bPreviewEnabled;

            //Capture
            rExportRaw.Checked = newState.settings.eExportMode == ClientSettings.ExportMode.RawFrames;
            rExportPointclouds.Checked = newState.settings.eExportMode == ClientSettings.ExportMode.Pointcloud;
            chMergeScans.Checked = newState.settings.bMergeScansForSave;

            //Stats
            SetStateIndicator(newState.appState, newState.stateIndicatorSuffix);
            lFPS.Text = newState.previewWindowFPS.ToString() + " FPS";

            //Live Preview
            oOpenGLView.settings = newState.settings;

            liveScanState = newState;

        }

        public void SetStateIndicator(appState appstate, string suffix)
        {
            switch (appstate)
            {
                case appState.idle:
                    pStatusIndicator.Image = null;
                    lStateIndicator.Text = "";
                    break;
                case appState.recording:
                    pStatusIndicator.Image = recordImage;
                    lStateIndicator.Text = "Capturing: " + suffix;
                    break;
                case appState.syncing:
                    pStatusIndicator.Image = loadAnimImage;
                    lStateIndicator.Text = "Syncing capture, please wait...";
                    break;
                case appState.downloading:
                    if (pStatusIndicator.Image != loadAnimImage) // Otherwise animation would start again when updating the suffix
                        pStatusIndicator.Image = loadAnimImage;
                    lStateIndicator.Text = "Downloading frames from clients: " + suffix;
                    break;
                case appState.calibrating:
                    pStatusIndicator.Image = loadAnimImage;
                    lStateIndicator.Text = "Calibrating, please wait...";
                    break;
                case appState.refinining:
                    pStatusIndicator.Image = loadAnimImage;
                    lStateIndicator.Text = "Refining calibration, please wait...";
                    break;
                case appState.restartingClients:
                    pStatusIndicator.Image = loadAnimImage;
                    lStateIndicator.Text = "Restarting clients, please wait...";
                    break;
                default:
                    lStateIndicator.Text = "";
                    break;
            }
        }

        public void RequestMessagBox(MessageBoxIcon level, string message, bool quitApp = false)
        {
            lock (oMBoxLock)
            {
                mBoxRequest = true;
                mBoxLevel = level;
                mBoxMessage = message;
                mBoxQuitApp = quitApp;
            }
        }

        void ShowMessageBox()
        {
            if (mBoxRequest)
            {
                lock (oMBoxLock)
                {
                    mBoxRequest = false;

                    string title = "";

                    switch (mBoxLevel)
                    {
                        case MessageBoxIcon.Error:
                            title = "Error!";
                            Log.LogError(mBoxMessage);
                            break;
                        case MessageBoxIcon.Warning:
                            title = "Warning";
                            Log.LogWarning(mBoxMessage);
                            break;
                        case MessageBoxIcon.Information:
                            title = "Info";
                            Log.LogInfo(mBoxMessage);
                            break;
                        default:
                            break;
                    }

                    if (mBoxQuitApp)
                        title = "Unrecoverable Error!";

                    MessageBox.Show(mBoxMessage, title, MessageBoxButtons.OK, mBoxLevel);

                    if (mBoxQuitApp)
                        Close();

                }
            }
        }

        public void AddClientConfigurationForm(string serial, ClientConfigurationForm form)
        {
            if (GetClientConfigurationForm(serial) == null)
                clientConfigurationsForms.Add(serial, form);
        }

        public ClientConfigurationForm GetClientConfigurationForm(string serial)
        {
            if (clientConfigurationsForms.TryGetValue(serial, out var value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        public void RemoveClientConfigurationForm(string serial)
        {
            clientConfigurationsForms.Remove(serial);
        }


        //Opens the settings form
        private void btSettings_Click(object sender, EventArgs e)
        {
            if (settingsForm == null)
            {
                settingsForm = new SettingsForm(liveScanServer.GetState().settings, liveScanServer);
                settingsForm.FormClosed += (senderer, ea) => { SettingsFormClosed(senderer, ea); };
                settingsForm.Show();
            }

            else
                settingsForm.Focus();
        }

        private void SettingsFormClosed(object sender, EventArgs e)
        {
            settingsForm.Dispose();
            settingsForm = null;
        }

        //This is used for: starting/stopping the recording worker, stopping the saving worker
        private void btRecord_Click(object sender, EventArgs e)
        {
            liveScanServer.Capture(txtSeqName.Text);
        }

        private void btCalibrate_Click(object sender, EventArgs e)
        {
            liveScanServer.Calibrate();
        }

        private void btRefineCalib_Click(object sender, EventArgs e)
        {
            liveScanServer.RefineCalibration();
        }

        public void UpdateClientGridView(List<ClientSocket> socketList)
        {
            List<string[]> gridViewRows = new List<string[]>();

            for (int i = 0; i < socketList.Count; i++)
            {
                if (socketList[i].configuration != null)
                {
                    gridViewRows.Add(new string[]
                    {
                    socketList[i].configuration.globalDeviceIndex.ToString(),
                    socketList[i].configuration.NickName[0] == ' ' ? socketList[i].configuration.SerialNumber : socketList[i].configuration.NickName,
                    socketList[i].GetIP(),
                    socketList[i].bCalibrated? "Yes" : "No",
                    socketList[i].configuration.eSoftwareSyncState.ToString(),
                    socketList[i].bVisible.ToString(),
                    "⚙️"
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

        private void gvClients_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //Visibility clicked
            if (e.ColumnIndex == 5)
            {
                liveScanServer.SetVisibility(e.RowIndex, !Convert.ToBoolean(gvClients.Rows[e.RowIndex].Cells[5].Value));
            }

            //Open configuration settings
            else if (e.ColumnIndex == 6)
            {
                OpenConfigurationForm(liveScanServer.GetState().clients[e.RowIndex].configuration.SerialNumber);
            }
        }

        private void gvClients_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //Nickname changed
            if (e.ColumnIndex == 1)
            {
                liveScanServer.SetNickname(liveScanServer.GetState().clients[e.RowIndex].configuration.SerialNumber, Convert.ToString(gvClients.Rows[e.RowIndex].Cells[1].Value));
            }
        }

        private void OpenConfigurationForm(string serialNumber)
        {
            ClientConfigurationForm ccf;
            ccf = GetClientConfigurationForm(serialNumber);

            if (ccf == null)
            {
                ccf = new ClientConfigurationForm(liveScanServer, serialNumber);
                AddClientConfigurationForm(serialNumber, ccf);
                ccf.FormClosing += (senderer, ea) => { ConfigurationFormClosed(senderer, ea); };
                ccf.Show();
            }

            else
                ccf.Focus();
        }

        public void UpdateConfigurationsForms()
        {
            foreach (KeyValuePair<string, ClientConfigurationForm> ccf in clientConfigurationsForms)
                ccf.Value.UpdateUI();
        }

        private void ConfigurationFormClosed(object sender, EventArgs e)
        {
            RemoveClientConfigurationForm((sender as ClientConfigurationForm).serialnumber);
        }

        private void chHardwareSync_Clicked(object sender, EventArgs e)
        {
            if (chHardwareSync.Checked && !chNetworkSync.Checked)
                liveScanServer.SetSyncMode(ClientSettings.SyncMode.Hardware);
            else if (!chHardwareSync.Checked && !chNetworkSync.Checked)
                liveScanServer.SetSyncMode(ClientSettings.SyncMode.Off);

        }

        private void chNetworkSync_Clicked(object sender, EventArgs e)
        {
            if (chNetworkSync.Checked && !chHardwareSync.Checked)
                liveScanServer.SetSyncMode(ClientSettings.SyncMode.Network);
            else if (!chNetworkSync.Checked && !chHardwareSync.Checked)
                liveScanServer.SetSyncMode(ClientSettings.SyncMode.Off);
        }

        private void rExposureAuto_Clicked(object sender, EventArgs e)
        {
            if (rExposureAuto.Checked)
                liveScanServer.SetExposureMode(true);
        }

        private void rExposureManual_Clicked(object sender, EventArgs e)
        {
            if (rExposureManual.Checked)
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

        private void rWhiteBalanceAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (rWhiteBalanceAuto.Checked)
                liveScanServer.SetWhiteBalanceMode(true);
        }

        private void rWhiteBalanceManual_CheckedChanged(object sender, EventArgs e)
        {
            if (rWhiteBalanceManual.Checked)
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

        private void cbEnablePreview_Clicked(object sender, EventArgs e)
        {
            liveScanServer.SetClientPreview(chEnablePreview.Checked);
        }

        private void rExportRaw_Clicked(object sender, EventArgs e)
        {
            liveScanServer.SetExportMode(ClientSettings.ExportMode.RawFrames);
        }

        private void rExportPointclouds_Clicked(object sender, EventArgs e)
        {
            liveScanServer.SetExportMode(ClientSettings.ExportMode.Pointcloud);
        }

        private void chMergeScans_CheckedChanged(object sender, EventArgs e)
        {
            liveScanServer.SetMergeScans(chMergeScans.Checked);
        }



        #region LivePreviewRender

        private void StartUpdateTimers()
        {
            UpdateTimer1FPS = new System.Windows.Forms.Timer();
            UpdateTimer1FPS.Tick += (senderer, ea) => { SlowUpdate(); };
            UpdateTimer1FPS.Interval = 1000;   // once per second
            UpdateTimer1FPS.Start();

            UpdateTimer60FPS = new System.Windows.Forms.Timer();
            UpdateTimer60FPS.Tick += (senderer, ea) => { FastUpdate(); };
            UpdateTimer60FPS.Interval = 16;   // 60 fps
            UpdateTimer60FPS.Start();
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
            glLiveView_Resize(glLiveView, EventArgs.Empty);

            oOpenGLView.viewportSettings = liveScanServer.viewportSettings;

            lock (oOpenGLView.vertices)
            {
                oOpenGLView.vertices = liveScanServer.lAllVertices;
                oOpenGLView.colors = liveScanServer.lAllColors;
                oOpenGLView.cameraPoses = liveScanServer.lAllCameraPoses;
                oOpenGLView.markerPoses = liveScanServer.lAllMarkerPoses;
            }

            oOpenGLView.Load();
        }

        private void glLiveView_Resize(object sender, EventArgs e)
        {
            glLiveView.MakeCurrent();

            if (glLiveView.ClientSize.Height == 0)
                glLiveView.ClientSize = new System.Drawing.Size(glLiveView.ClientSize.Width, 1);

            if (oOpenGLView != null)
                oOpenGLView.Resize(glLiveView.ClientSize.Width, glLiveView.ClientSize.Height);
        }

        private void glLiveView_Paint(object sender, PaintEventArgs e)
        {
            FastUpdate();
        }

        private void glLiveView_MouseDown(object sender, MouseEventArgs e)
        {
            oOpenGLView.OnMouseButtonDown(sender, e);
        }

        private void glLiveView_MouseMove(object sender, MouseEventArgs e)
        {
            oOpenGLView.OnMouseMove(sender, e);
        }

        private void glLiveView_MouseUp(object sender, MouseEventArgs e)
        {
            oOpenGLView.OnMouseButtonUp(sender, e);
        }

        private void glLiveView_Scroll(object sender, MouseEventArgs e)
        {
            oOpenGLView.OnMouseWheelChanged(sender, e);
        }

        private void glLiveView_KeyDown(object sender, KeyEventArgs e)
        {
            oOpenGLView.OnKeyDown(sender, e);
        }


        #endregion


    }
}
