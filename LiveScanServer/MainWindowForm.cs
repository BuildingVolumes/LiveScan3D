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
        [DllImport("ICP.dll")]
        static extern float ICP(IntPtr verts1, IntPtr verts2, int nVerts1, int nVerts2, float[] R, float[] t, int maxIter = 200);

        KinectServer oServer;
        TransferServer oTransferServer;

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

        bool bRecording = false;
        bool bSaving = false;

        System.Timers.Timer oStatusBarTimer = new System.Timers.Timer();

        KinectSettings oSettings = new KinectSettings();


        //Settings
        private System.Windows.Forms.Timer scrollTimerExposure = null;
        private System.Windows.Forms.Timer scrollTimerWhiteBalance = null;

        //The live preview
        OpenGLWindow oOpenGLWindow;

        public MainWindowForm()
        {
            InitializeComponent();
            OpenGLWorker.RunWorkerAsync();

            this.Icon = Properties.Resources.Server_Icon;
        }

        private void MainWindowForm_Load(object sender, EventArgs e)
        {

            //Set the logging level
            string[] cmdLineArgs = Environment.GetCommandLineArgs();
            Log.LogLevel loglevel = Log.LogLevel.Normal;

            if (cmdLineArgs.Length > 1)
            {
                if (cmdLineArgs[1] == "-none" || cmdLineArgs[0] == "-None")
                    loglevel = Log.LogLevel.None;

                if (cmdLineArgs[1] == "-debug" || cmdLineArgs[0] == "-Debug")
                    loglevel = Log.LogLevel.Debug;

                if (cmdLineArgs[1] == "-debugcapture" || cmdLineArgs[1] == "-debugCapture" || cmdLineArgs[0] == "-DebugCapture")
                    loglevel = Log.LogLevel.DebugCapture;

                if (cmdLineArgs[1] == "-debugall" || cmdLineArgs[1] == "-debugAll" || cmdLineArgs[0] == "-DebugAll")
                    loglevel = Log.LogLevel.All;

            }

            if (!Log.StartLog(loglevel))
            {
                ShowFatalWindowAndQuit("Could not access logging file, another Livescan Server instance is probably already open!");
                return;
            }

            //Setup directories
            try
            {
                if (!Directory.Exists("temp"))
                    Directory.CreateDirectory("temp");
            }

            catch(Exception)
            {
                ShowFatalWindowAndQuit("Could not create working directories, please restart Application!");
            }




            Stream settingsStream = null;
            //This tries to read the settings from "settings.bin", if it failes the settings stay at default values.
            try
            {
                IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                settingsStream = new FileStream("temp/settings.bin", FileMode.Open, FileAccess.Read);
                oSettings = (KinectSettings)formatter.Deserialize(settingsStream);

                //Set settings that should not be loaded from the save
                oSettings.bAutoExposureEnabled = false;
                oSettings.nExposureStep = -5;

                settingsStream.Dispose();


            }
            catch (Exception)
            {
                Log.LogWarning("Could not read settings. Reverting to default settings");

                if(settingsStream != null)
                    settingsStream.Dispose();
            }

            oSettings.AddDefaultMarkers();
            oServer = new KinectServer(oSettings);
            oServer.eSocketListChanged += new SocketListChangedHandler(UpdateClientGridView);
            oServer.eSocketListChanged += new SocketListChangedHandler(ClientConnectionChanged);
            oServer.SetMainWindowForm(this);
            oTransferServer = new TransferServer();
            oTransferServer.lVertices = lAllVertices;
            oTransferServer.lColors = lAllColors;
            lock (oOpenGLWindow.settings)
                oOpenGLWindow.settings = oSettings;
            UpdateSettingsButtonEnabled();//will disable settings button with no devices connected.
            SetupButtons();

            if (!oServer.StartServer()) //If another Livescan Instance is already open, we terminate the app here
                return;

            oTransferServer.StartServer();
            Log.LogInfo("Starting Server");
        }

        private void SetupButtons()
        {
            if (oServer != null)
                chHardwareSync.Checked = oServer.bTempHwSyncEnabled;
            else
                chHardwareSync.Checked = false;

            chMergeScans.Checked = oSettings.bMergeScansForSave;
            chNetworkSync.Checked = oSettings.bNetworkSync;
            rExposureAuto.Checked = true;
            rExposureManual.Checked = false;
            rWhiteBalanceAuto.Checked = oSettings.bAutoWhiteBalanceEnabled;
            rWhiteBalanceManual.Checked = !oSettings.bAutoWhiteBalanceEnabled;
            rExportPointclouds.Checked = oSettings.eExportMode == KinectSettings.ExportMode.Pointcloud ? true : false;
            rExportRaw.Checked = !rExportPointclouds.Checked;
            trManualExposure.Value = oSettings.nExposureStep;
            trWhiteBalance.Value = oSettings.nKelvin;
            cbEnablePreview.Checked = oSettings.bPreviewEnabled;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //The current settings are saved to a files.
            IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            Stream stream = new FileStream("temp/settings.bin", FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, oSettings);
            stream.Close();

            tLiveViewTimer.Stop();

            updateWorker.CancelAsync();

            if(oServer != null)
                oServer.StopServer();
            
            if(oTransferServer != null)
                oTransferServer.StopServer();
            
            oOpenGLWindow.Unload();

            Log.LogInfo("Programm termined normally, exiting");
            Log.CloseLog();
        }

        private void ClientConnectionChanged(List<KinectSocket> socketList)
        {
            //Disable the temporal hardware sync if all clients disconnected
            if (socketList.Count == 0)
            {
                Invoke(new Action(() => { chHardwareSync.Checked = false; }));
            }
        }

        //Opens the settings form
        private void btSettings_Click(object sender, EventArgs e)
        {
            if (oServer.GetSettingsForm() == null)
            {
                SettingsForm form = new SettingsForm();
                form.oSettings = oSettings;
                form.oServer = oServer;
                form.Show();
                oServer.SetSettingsForm(form);
            }

            else
                oServer.GetSettingsForm().Focus();
        }

        //Performs recording which is synchronized or unsychronized frame capture.
        //The frames are downloaded from the clients and saved once recording is finished.
        private void recordingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            oServer.ClearStoredFrames();

            bool networkSyncEnabled = oSettings.bNetworkSync;
            BackgroundWorker worker = (BackgroundWorker)sender;

            oServer.SendAndConfirmPreRecordProcess();

            //If we don't use a server-controlled sync method, we just let the clients capture as fast as possible
            if (!networkSyncEnabled || oServer.bTempHwSyncEnabled)
            {
                oServer.SendCaptureFramesStart();

                Stopwatch counter = new Stopwatch();
                counter.Start();

                Log.LogInfo("Starting Recording");

                while (!worker.CancellationPending)
                {
                    SetStatusBarOnTimer("Recording: " + counter.Elapsed.Minutes.ToString("D2") + ":" + counter.Elapsed.Seconds.ToString("D2"), 5000);
                }

                Log.LogInfo("Stopping Recording");

                oServer.SendCaptureFramesStop();
                counter.Stop();

            }

            //A server controlled sync method, the server gives the command to capture a single frame and waits until all devices have captured it
            else if (networkSyncEnabled)
            {
                int nCaptured = 0;

                Log.LogInfo("Starting Network-synced recording");

                while (!worker.CancellationPending)
                {
                    oServer.CaptureSynchronizedFrame();
                    nCaptured++;
                    SetStatusBarOnTimer("Captured frame " + (nCaptured).ToString() + ".", 5000);
                    Log.LogDebugCapture("Captured frame " + (nCaptured).ToString());
                }

                Log.LogInfo("Stopping Network-synced recording");
            }

            oServer.SendAndConfirmPostRecordProcess();

        }

        private void recordingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //After recording has been terminated it is time to begin sorting the frames.
            if (oServer.bTempHwSyncEnabled)
            {
                Log.LogInfo("Starting to synchronize recording");
                SetStatusBarOnTimer("Synchronizing recording. This could take some time", 5000);
                syncWorker.RunWorkerAsync();
            }

            //If sync wasn't enabled, we go straight to saving the files
            else
            {
                Log.LogInfo("Synchronisation not needed, skipping");
                SetStatusBarOnTimer("Recording completed!", 2000);
                StartSaving();
            }

        }

        private void syncWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool synced = false;

            if (oServer.bTempHwSyncEnabled)
            {
                if (!oServer.GetTimestampLists())
                {
                    Log.LogError("Could not get Timestamp List. Saved recording is unsychronized!");
                    ShowWarningWindow("Could not get Timestamp List. Saved recording is unsychronized!");
                }

                else
                {
                    if (!oServer.CreatePostSyncList())
                    {
                        Log.LogError("Could not match timestamps. Please check your Temporal Sync setup and Kinect firmware!");
                        ShowWarningWindow("Could not match timestamps. Please check your Temporal Sync setup and Kinect firmware!");
                    }

                    else
                    {
                        if (!oServer.ReorderSyncFramesOnClient())
                        {
                            Log.LogError("Could not reorganize files for sync on at least one device!");
                            ShowWarningWindow("Could not reorganize files for sync on at least one device!");
                        }

                        else
                        {
                            Log.LogInfo("Sync successfull!");
                            SetStatusBarOnTimer("Sync sucessfull", 5000);
                            synced = true;
                        }
                    }

                }
            }

            e.Result = synced;
        }

        private void syncWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool syncSuccess = (bool)e.Result;

            if (syncSuccess)
                StartSaving();
            else
                CaptureComplete();
        }

        //Opens the live view window
        private void OpenGLWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Log.LogInfo("Live View started");

            oOpenGLWindow = new OpenGLWindow();

            //The variables below are shared between this class and the OpenGLWindow.
            lock (lAllVertices)
            {
                oOpenGLWindow.vertices = lAllVertices;
                oOpenGLWindow.colors = lAllColors;
                oOpenGLWindow.cameraPoses = lAllCameraPoses;
                oOpenGLWindow.markerPoses = lAllMarkerPoses;
                oOpenGLWindow.settings = oSettings;
                oOpenGLWindow.viewportSettings = viewportSettings;
                oOpenGLWindow.viewportSettings.colorMode = EColorMode.BGR;
            }
        }

        private void OpenGLWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        private void StartSaving()
        {
            //Only store frames when capturing pointclouds
            if (oSettings.eExportMode == KinectSettings.ExportMode.Pointcloud)
            {
                bSaving = true;
                btRecord.Text = " Stop Saving";
                btRecord.Image = Properties.Resources.stop;
                btRecord.Enabled = true;
                savingWorker.RunWorkerAsync();
            }

            else
            {
                CaptureComplete();
            }
        }

        private void savingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Log.LogInfo("Start saving Pointcloud frames");
            Log.LogInfo("Merging Pointclouds: " + oSettings.bMergeScansForSave);

            //Saving is downloading the frames from clients and saving them locally.
            int nFrames = 0;

            string outDir;

            if (oSettings.takePath != null)
            {
                outDir = oSettings.takePath;
            }

            else
            {
                return;
            }

            BackgroundWorker worker = (BackgroundWorker)sender;
            //This loop is running till it is either cancelled (using the btRecord button), or till there are no more stored frames.
            while (!worker.CancellationPending)
            {
                List<List<byte>> lFrameBGRAllDevices = new List<List<byte>>();
                List<List<float>> lFrameVertsAllDevices = new List<List<float>>();

                bool success = oServer.GetStoredFrame(lFrameBGRAllDevices, lFrameVertsAllDevices);

                //This indicates that there are no more stored frames.
                if (!success)
                    break;

                nFrames++;
                int nVerticesTotal = 0;
                for (int i = 0; i < lFrameBGRAllDevices.Count; i++)
                {
                    nVerticesTotal += lFrameVertsAllDevices[i].Count;
                }

                List<byte> lFrameBGR = new List<byte>();
                List<Single> lFrameVerts = new List<Single>();

                Log.LogDebugCapture("Saving frame " + (nFrames).ToString());
                SetStatusBarOnTimer("Saving frame " + (nFrames).ToString() + ".", 5000);
                for (int i = 0; i < lFrameBGRAllDevices.Count; i++)
                {
                    lFrameBGR.AddRange(lFrameBGRAllDevices[i]);
                    lFrameVerts.AddRange(lFrameVertsAllDevices[i]);

                    //This is ran if the frames from each client are to be placed in separate files.
                    if (!oSettings.bMergeScansForSave)
                    {
                        string outputFilename = outDir + "\\" + nFrames.ToString().PadLeft(5, '0') + i.ToString() + ".ply";
                        Utils.saveToPly(outputFilename, lFrameVertsAllDevices[i], lFrameBGRAllDevices[i], EColorMode.BGR, oSettings.bSaveAsBinaryPLY);
                    }
                }

                //This is ran if the frames from all clients are to be placed in a single file.
                if (oSettings.bMergeScansForSave)
                {
                    string outputFilename = outDir + "\\" + nFrames.ToString().PadLeft(5, '0') + ".ply";
                    Utils.saveToPly(outputFilename, lFrameVerts, lFrameBGR,EColorMode.BGR, oSettings.bSaveAsBinaryPLY);
                }
            }

            oSettings.takePath = null;
        }

        private void savingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Log.LogInfo("Saving complete!");
            oServer.ClearStoredFrames();
            bSaving = false;

            CaptureComplete();
        }

        private void CaptureComplete()
        {
            Log.LogInfo("Capture has been completed!");

            RestartUpdateWorker();

            btRecord.Enabled = true;
            btRecord.Image = Properties.Resources.recording;
            btRecord.Text = " Start Capture";
            btRefineCalib.Enabled = true;
            btCalibrate.Enabled = true;

        }

        //Continually requests frames that will be displayed in the live view window.
        private void updateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<List<byte>> lFramesRGB = new List<List<byte>>();
            List<List<Single>> lFramesVerts = new List<List<Single>>();

            BackgroundWorker worker = (BackgroundWorker)sender;
            while (!worker.CancellationPending)
            {
                Thread.Sleep(1);

                Stopwatch timer = new Stopwatch();
                timer.Start();

                if(oServer != null)
                {
                    oServer.GetLatestFrame(lFramesRGB, lFramesVerts);
                    Log.LogDebugCapture("Getting latest frame for live view");

                    //Update the vertex and color lists that are common between this class and the OpenGLWindow.
                    lock (lAllVertices)
                    {
                        lAllVertices.Clear();
                        lAllColors.Clear();
                        lAllCameraPoses.Clear();
                        lAllMarkerPoses.Clear();

                        for (int i = 0; i < lFramesRGB.Count; i++)
                        {
                            lAllVertices.AddRange(lFramesVerts[i]);
                            lAllColors.AddRange(lFramesRGB[i]);
                        }

                        lAllCameraPoses.AddRange(oServer.lCameraPoses);
                        lAllMarkerPoses.AddRange(oServer.lMarkerPoses);

                        timer.Stop();

                        if (timer.ElapsedMilliseconds > 0)
                            UpdateFPSCounter((int)(1000 / timer.ElapsedMilliseconds));
                        else
                            UpdateFPSCounter(0);
                    }
                }

                
            }
        }

        private void updateWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            lock (lAllVertices)
            {
                lAllVertices.Clear();
                lAllColors.Clear();
            }            
        }

        //Performs the ICP based pose refinement.
        private void refineWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (oServer.bAllCalibrated == false)
            {
                ShowWarningWindow("Not all of the devices are calibrated");
                return;
            }

            Log.LogInfo("Start ICP refinement");

            //Download a frame from each client.
            List<List<float>> lAllFrameVertices = new List<List<float>>();
            List<List<byte>> lAllFrameColors = new List<List<byte>>();
            oServer.GetLatestFrame(lAllFrameColors, lAllFrameVertices);

            //Initialize containers for the poses.
            List<float[]> Rs = new List<float[]>();
            List<float[]> Ts = new List<float[]>();
            for (int i = 0; i < lAllFrameVertices.Count; i++)
            {
                float[] tempR = new float[9];
                float[] tempT = new float[3];
                for (int j = 0; j < 3; j++)
                {
                    tempT[j] = 0;
                    tempR[j + j * 3] = 1;
                }

                Rs.Add(tempR);
                Ts.Add(tempT);
            }

            //Use ICP to refine the sensor poses.
            //This part is explained in more detail in our article (name on top of this file).

            for (int refineIter = 0; refineIter < oSettings.nNumRefineIters; refineIter++)
            {
                for (int i = 0; i < lAllFrameVertices.Count; i++)
                {
                    List<float> otherFramesVertices = new List<float>();
                    for (int j = 0; j < lAllFrameVertices.Count; j++)
                    {
                        if (j == i)
                            continue;
                        otherFramesVertices.AddRange(lAllFrameVertices[j]);
                    }

                    float[] verts1 = otherFramesVertices.ToArray();
                    float[] verts2 = lAllFrameVertices[i].ToArray();

                    IntPtr pVerts1 = Marshal.AllocHGlobal(otherFramesVertices.Count * sizeof(float));
                    IntPtr pVerts2 = Marshal.AllocHGlobal(lAllFrameVertices[i].Count * sizeof(float));

                    Marshal.Copy(verts1, 0, pVerts1, verts1.Length);
                    Marshal.Copy(verts2, 0, pVerts2, verts2.Length);

                    ICP(pVerts1, pVerts2, otherFramesVertices.Count / 3, lAllFrameVertices[i].Count / 3, Rs[i], Ts[i], oSettings.nNumICPIterations);

                    Marshal.Copy(pVerts2, verts2, 0, verts2.Length);
                    lAllFrameVertices[i].Clear();
                    lAllFrameVertices[i].AddRange(verts2);
                }
            }

            List<Matrix4x4> icpTransforms = oServer.lRefinementTransforms;

            for (int i = 0; i < icpTransforms.Count; i++)
            {
                Matrix4x4 icpOffset = new Matrix4x4();

                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        icpOffset.mat[j, k] = Rs[i][j * 3 + k];
                    }
                }

                for (int j = 0; j < 3; j++)
                {
                    icpOffset.mat[j, 3] = Ts[i][j];
                }

                string s = "";

                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        s += icpOffset.mat[j, k] + ", ";
                    }
                }

                icpTransforms[i] = icpOffset;




               // icpTransforms[i] = icpOffset * icpTransforms[i];

            }

            oServer.lRefinementTransforms = icpTransforms;

            //oServer.lRefinementTransforms = icpTransforms;
            oServer.UpdateMarkerTransforms();
            oServer.SendRefinementData();
        }

        private void refineWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Log.LogInfo("ICP refinement done");
            //Re-enable all of the buttons after refinement.
            btRefineCalib.Enabled = true;
            btCalibrate.Enabled = true;
            btRecord.Enabled = true;
        }

        //This is used for: starting/stopping the recording worker, stopping the saving worker
        private void btRecord_Click(object sender, EventArgs e)
        {
            //If we are saving frames right now, this button stops saving.
            if (bSaving)
            {
                Log.LogInfo("User terminated frame saving");

                btRecord.Enabled = false;
                savingWorker.CancelAsync();
                return;
            }

            if (!bRecording)
            {
                if (oServer.nClientCount < 1)
                {
                    ShowInfoWindow("At least one client needs to be connected for recording.");
                    return;
                }

                if (oServer.bTempHwSyncEnabled)
                {

                    if (!oServer.CheckTempHwSyncValid())
                    {
                        Log.LogWarning("User started recording, but temp sync hardware setup is not valid");
                        ShowWarningWindow("Temporal Hardware Sync Setup not valid! Please check cable arrangement");
                        return;
                    }
                }

                //Stop the update worker to reduce the network usage (provides better synchronization).
                updateWorker.CancelAsync();

                string takePath = oServer.CreateTakeDirectories(txtSeqName.Text);

                //Case: Server needs a path for storing values
                if (takePath != null && (oSettings.eExportMode == KinectSettings.ExportMode.Pointcloud || oSettings.eExtrinsicsFormat != KinectSettings.ExtrinsicsStyle.None))
                {
                    //Store path for the saving worker later
                    //TODO: How do to it without a global variable?
                    oSettings.takePath = takePath;
                }

                else if (takePath == null)
                {
                    Log.LogError("Could not create take directory on either the server or the client, aborting recording");
                    ShowWarningWindow("Error: Couldn't create take directory on either the server or the clients");
                    bRecording = false;
                    return;
                }

                //Store the camera extrinsics
                Utils.SaveExtrinsics(oSettings.eExtrinsicsFormat, takePath, oServer.GetClientSockets());

                bRecording = true;
                recordingWorker.RunWorkerAsync();
                btRecord.Text = " Stop Capture";
                btRecord.Image = Properties.Resources.stop;
                btRefineCalib.Enabled = false;
                btCalibrate.Enabled = false;
                Log.LogInfo("Starting Recording");

            }
            else
            {
                bRecording = false;
                Log.LogInfo("Stopping recording");
                btRecord.Enabled = false;
                recordingWorker.CancelAsync();
            }

        }

        private void btCalibrate_Click(object sender, EventArgs e)
        {
            Log.LogInfo("Starting Calibration");
            oServer.Calibrate();
        }

        public void SetCalibrateButtonActive(bool active)
        {
            btCalibrate.Enabled = active;
        }

        private void btRefineCalib_Click(object sender, EventArgs e)
        {
            if (oServer.nClientCount < 2)
            {
                ShowInfoWindow("To refine calibration you need at least 2 connected devices.");
                return;
            }

            btRefineCalib.Enabled = false;
            btCalibrate.Enabled = false;
            btRecord.Enabled = false;

            refineWorker.RunWorkerAsync();
        }

        public void SetRefineButtonActive(bool active)
        {
            btRefineCalib.Enabled = active;
        }

        void RestartUpdateWorker()
        {
            if (!updateWorker.IsBusy)
                updateWorker.RunWorkerAsync();
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

        public void ShowInfoWindow(string message)
        {
            Invoke(new Action(() =>
            {
                MessageBox.Show(message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        public void ShowWarningWindow(string message)
        {
            Invoke(new Action(() => 
            {
                MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }));
        }

        public void ShowErrorWindow(string message)
        {
            Invoke(new Action(() =>
            {
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }));
        }

        public void ShowFatalWindowAndQuit(string message)
        {
            Invoke(new Action(() =>
            {
                MessageBox.Show(message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }));

            Close();
        }

        private void UpdateClientGridView(List<KinectSocket> socketList)
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

                UpdateSettingsButtonEnabled();
            }));
        }

        private void btKinectSettingsOpenButton_Click(object sender, EventArgs e)
        {
            if (gvClients.SelectedRows.Count < 1)
            {
                return;
            }
            //

            KinectConfigurationForm form = oServer.GetKinectSettingsForm(gvClients.SelectedRows[0].Index);
            if (form == null)
            {
                form = new KinectConfigurationForm();
            }
            //
            form.Initialize(oServer, gvClients.SelectedRows[0].Index);
            form.Show();
            form.Focus();
            oServer.SetKinectSettingsForm(gvClients.SelectedRows[0].Index, form);
        }

        private void UpdateSettingsButtonEnabled()
        {
            //Disable the device Settings button when no items are selected or no items could be selected.
            if (gvClients.Rows.Count < 1)
            {
                btKinectSettingsOpenButton.Enabled = false;
            }
            else
            {
                btKinectSettingsOpenButton.Enabled = true;
            }
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

        // Settings

        private void SettingsChanged()
        {
            Cursor.Current = Cursors.WaitCursor;

            oServer.SendSettings();

            //Check if we need to restart the cameras

            //TODO: Currently, the UI doesn't update as it stalls the thread. How can I get this to work without stalling it?

            Log.LogDebug("Updating settings on clients");

            if (chHardwareSync.Checked != oServer.bTempHwSyncEnabled)
            {
                if (chHardwareSync.Checked)
                {
                    chHardwareSync.Checked = oServer.EnableTemporalSync();
                    Log.LogDebug("Hardware sync is set on by the user");
                }

                else
                {
                    chHardwareSync.Checked = !oServer.DisableTemporalSync();
                    Log.LogDebug("Hardware sync is set off by the user");

                }
            }

            Cursor.Current = Cursors.Default;
        }

        private void chHardwareSync_Clicked(object sender, EventArgs e)
        {
            if (chNetworkSync.Checked)
            {
                chNetworkSync.Checked = false;
                oSettings.bNetworkSync = false;
            }

            SettingsChanged();
        }        

        private void chNetworkSync_Clicked(object sender, EventArgs e)
        {
            if (chHardwareSync.Checked)
            {
                chHardwareSync.Checked = false;
            }

            oSettings.bNetworkSync = chNetworkSync.Checked;
            SettingsChanged();
        }

        private void rExposureAuto_Clicked(object sender, EventArgs e)
        {
            if (rExposureAuto.Checked)
            {
                oSettings.bAutoExposureEnabled = true;
                trManualExposure.Enabled = false;
            }

            SettingsChanged();
        }
        
        private void rExposureManual_Clicked(object sender, EventArgs e)
        {
            if (rExposureManual.Checked)
            {
                oSettings.bAutoExposureEnabled = false;
                trManualExposure.Enabled = true;
            }

            SettingsChanged();
        }

        public void LockExposureForSync(bool locked)
        {
            if (locked)
            {
                rExposureAuto.Enabled = false;
                rExposureManual.Enabled = false;
                rExposureManual.Checked = true;
                rExposureAuto.Checked = false;
                trManualExposure.Enabled = true;
                trManualExposure.Value = -5;
            }

            else
            {
                rExposureAuto.Enabled = true;
                rExposureManual.Enabled = true;
                rExposureManual.Checked = true;
                rExposureAuto.Checked = false;
                trManualExposure.Enabled = true;
            }

            
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
                        oSettings.nExposureStep = exposureStepClamped;

                        SettingsChanged();

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
            oSettings.bPreviewEnabled = cbEnablePreview.Checked;
            SettingsChanged();
        }

        private void rExportRaw_Clicked(object sender, EventArgs e)
        {
            if (rExportRaw.Checked)
            {
                chMergeScans.Enabled = false;
                oSettings.eExportMode = KinectSettings.ExportMode.RawFrames;
            }

            SettingsChanged();
        }

        private void rExportPointclouds_Clicked(object sender, EventArgs e)
        {
            if (rExportPointclouds.Checked)
            {
                chMergeScans.Enabled = true;
                oSettings.eExportMode = KinectSettings.ExportMode.Pointcloud;
            }

            SettingsChanged();

        }

        private void chMergeScans_CheckedChanged(object sender, EventArgs e)
        {
            oSettings.bMergeScansForSave = chMergeScans.Checked;
            SettingsChanged();
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

                        oSettings.nKelvin = trWhiteBalance.Value;

                        SettingsChanged();

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

        private void rWhiteBalanceAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (rWhiteBalanceAuto.Checked)
            {
                oSettings.bAutoWhiteBalanceEnabled = true;
                trWhiteBalance.Enabled = false;
            }

            SettingsChanged();
        }

        private void rWhiteBalanceManual_CheckedChanged(object sender, EventArgs e)
        {
            if (rWhiteBalanceManual.Checked)
            {
                oSettings.bAutoWhiteBalanceEnabled = false;
                trWhiteBalance.Enabled = true;
            }

            SettingsChanged();
        }
    }
}
