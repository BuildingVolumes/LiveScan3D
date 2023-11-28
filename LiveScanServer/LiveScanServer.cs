using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LiveScanServer
{
    public class LiveScanState
    {
        public appState appState;
        public ClientSettings settings;
        public List<ClientSocket> clients;
        public string stateIndicatorSuffix;
        public float previewWindowFPS = 0;
        public int capturedFramesTotal = 0;

        public LiveScanState()
        {
            appState = appState.idle;
            settings = new ClientSettings();
            clients = new List<ClientSocket>();
        }
    }

    public class LiveScanServer
    {
        [DllImport("ICP.dll")]
        static extern float ICP(IntPtr verts1, IntPtr verts2, int nVerts1, int nVerts2, float[] R, float[] t, int maxIter = 200);

        MainWindowForm UI;
        LiveScanState state;
        ClientCommunication oServer;
        //TransferServer oTransferServer;

        BackgroundWorker previewWorker;
        BackgroundWorker processingWorker;
        bool processingWorkerComplete = false;


        //Those three four are shared with the OpenGL class and are used to exchange data with it.
        //Vertices from all of the sensors
        public List<float> lAllVertices = new List<float>();
        //Color data from all of the sensors
        public List<byte> lAllColors = new List<byte>();
        //Sensor poses from all of the sensors
        public List<Matrix4x4> lAllCameraPoses = new List<Matrix4x4>();
        //Marker poses from all of the sensors
        public List<Matrix4x4> lAllMarkerPoses = new List<Matrix4x4>();
        //Viewport settings
        public ViewportSettings viewportSettings = new ViewportSettings();

        //TODO: Protect anything that the threads are touching with locks, especially the state
        //TODO: While app is not in idle state, other actions that change the state should not be able to run!


        public LiveScanServer(MainWindowForm UI)
        {
            this.UI = UI;

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
                UI.ShowMessageBox(MessageBoxIcon.Error, "Could not access logging file, another Livescan Server instance is probably already open!", true);
                return;
            }

            Setup();

            oServer = new ClientCommunication(this);
            oServer.eSocketListChanged += new SocketListChangedHandler(ClientListChanged);
            oServer.SetMainWindowForm(UI);

            StartPreviewWorker();

            if (!oServer.StartServer()) //If another Livescan Instance is already open, we terminate the app here
                Terminate();

            //oTransferServer.StartServer();
            Log.LogInfo("Starting Server");
        }

        private void Setup()
        {
            //Setup directories
            try
            {
                if (!Directory.Exists("temp"))
                    Directory.CreateDirectory("temp");
            }

            catch (Exception)
            {
                UI.ShowMessageBox(MessageBoxIcon.Error, "Could not create working directories, please restart Application!", true);
            }

            state = new LiveScanState();

            Stream settingsStream = null;
            //This tries to read the settings from "settings.bin", if it fails, the settings stay at default values.
            try
            {
                IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                settingsStream = new FileStream("temp/settings.bin", FileMode.Open, FileAccess.Read);
                state.settings = (ClientSettings)formatter.Deserialize(settingsStream);

                //Set settings that should not be loaded from the save
                state.settings.bAutoExposureEnabled = false;
                state.settings.nExposureStep = -5;

                settingsStream.Dispose();


            }
            catch (Exception)
            {
                Log.LogWarning("Could not read settings. Reverting to default settings");

                if (settingsStream != null)
                    settingsStream.Dispose();
            }

            state.settings.AddDefaultMarkers();

            viewportSettings.colorMode = EColorMode.BGR;
            previewWorker = new BackgroundWorker();
            previewWorker.WorkerSupportsCancellation = true;
            previewWorker.DoWork += new DoWorkEventHandler(PreviewWorker);
        }

        public void Terminate()
        {
            //The current settings are saved to a files.
            IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            Stream stream = new FileStream("temp/settings.bin", FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, state.settings);
            stream.Close();

            if (oServer != null)
                oServer.StopServer();

            PreviewWorker_Cancel();
            ProcessingWorker_Cancel();

            Log.LogInfo("Programm termined normally, exiting");
            Log.CloseLog();

        }

        public void UpdateUI()
        {
            UI.UpdateUI(state);
        }

        public void ClientListChanged(List<ClientSocket> socketList)
        {
            state.clients = socketList;
            UpdateUI();
        }

        public LiveScanState GetState()
        {
            return state;
        }

        public void SetState(LiveScanState newState)
        {
            state = newState;
        }

        public void SetSettings(ClientSettings newSettings)
        {
            //Todo: Need to lock settings?
            state.settings = newSettings;
            oServer.SendCurrentSettings();
            UpdateUI();

        }

        public void SetConfigurations(List<ClientConfiguration> newConfigs, bool restart)
        {
            bool restartAll = false;

            if (state.settings.eSyncMode == ClientSettings.SyncMode.Hardware)
                restartAll = true;

            for (int i = 0; i < newConfigs.Count; i++)
            {
                oServer.SetAndConfirmConfig(newConfigs[i]);
            }

            Cursor.Current = Cursors.WaitCursor;

            if (restart && newConfigs.Count == 1)
                oServer.RestartClient(newConfigs[0].SerialNumber);

            if (restartAll || (restart && newConfigs.Count > 1))
                oServer.RestartAllClients();

            Cursor.Current = Cursors.Default;

            UpdateUI();
        }

        public ClientConfiguration GetConfigFromSerial(string serialnumber)
        {
            ClientConfiguration config = null;

            //Search for our Config in all configs
            for (int i = 0; i < state.clients.Count; i++)
            {
                if (serialnumber == state.clients[i].configuration.SerialNumber)
                    config = state.clients[i].configuration;
            }

            if (config == null)
                UI.ShowMessageBox(MessageBoxIcon.Error, "Configuration could not be updated, the client is probably disconnected");

            return config;
        }


        #region Actions

        public void Calibrate()
        {
            if (state.appState == appState.idle)
            {
                Log.LogInfo("Starting Calibration");

                if (!oServer.Calibrate(true))
                {
                    UI.ShowMessageBox(MessageBoxIcon.Information, "No clients connected!");
                    return;
                }

                ProcessingWorker_Start(Calibration_DoWork, Calibration_Completed);
                state.appState = appState.calibrating;
                UpdateUI();
            }

            else if (state.appState == appState.calibrating)
            {
                Log.LogInfo("Canceling Calibration");
                oServer.Calibrate(false);
                ProcessingWorker_Cancel();
            }

        }

        public void RefineCalibration()
        {
            if (state.appState == appState.idle)
            {
                if (oServer.nClientCount < 2)
                {
                    UI.ShowMessageBox(MessageBoxIcon.Warning, "To refine calibration you need at least 2 connected devices.");
                    return;
                }

                if (oServer.bAllCalibrated == false)
                {
                    UI.ShowMessageBox(MessageBoxIcon.Warning, "Not all of the devices are calibrated");
                    return;
                }

                ProcessingWorker_Start(RefinementWorker, RefinementWorker_Completed);
                state.appState = appState.refinining;
                UpdateUI();
            }

            else if (state.appState == appState.refinining)
            {
                Log.LogInfo("Calibration refinement canceled by user");
                ProcessingWorker_Cancel();
            }

        }

        public void Capture(string sequenceName)
        {
            //Start recording
            if (state.appState == appState.idle)
            {
                if (oServer.nClientCount < 1)
                {
                    UI.ShowMessageBox(MessageBoxIcon.Warning, "At least one client needs to be connected for recording.");
                    return;
                }

                string takePath = oServer.CreateTakeDirectories(sequenceName);

                if (takePath == null)
                {
                    UI.ShowMessageBox(MessageBoxIcon.Warning, "Error: Couldn't create take directory on either the server or the clients");
                    return;
                }

                //Stop the update worker to reduce the load on the clients
                PreviewWorker_Cancel();

                //Store the camera extrinsics
                Utils.SaveExtrinsics(state.settings.eExtrinsicsFormat, takePath, oServer.GetClientSockets());

                ProcessingWorker_Start(Capture_DoWork, Capture_Complete);
                state.settings.takePath = takePath;
                state.appState = appState.recording;
                UpdateUI();
                return;
            }

            //Stop recording
            else if (state.appState == appState.recording)
            {
                ProcessingWorker_Cancel();
                Sync();                 //After recording has been terminated it is time to begin sorting the frames.
                return;
            }

            else if (state.appState == appState.downloading)
            {
                ProcessingWorker_Cancel();
                return;
            }
        }

        public void Sync()
        {
            state.appState = appState.syncing;
            UpdateUI();
            ProcessingWorker_Start(SyncWorker, Sync_Completed);
        }

        public void Save()
        {
            state.appState = appState.downloading;
            UpdateUI();
            ProcessingWorker_Start(SavingWorker, Saving_Completed);
        }

        public void SetSyncMode(ClientSettings.SyncMode syncMode)
        {
            if (syncMode == state.settings.eSyncMode)
                return;

            bool hwSyncWasEnabled = state.settings.eSyncMode == ClientSettings.SyncMode.Hardware;

            state.settings.eSyncMode = syncMode;
            oServer.SendCurrentSettings();

            Cursor.Current = Cursors.Default;
            //Check if we need to restart the cameras
            if (syncMode == ClientSettings.SyncMode.Hardware)
                if (!oServer.EnableTemporalSync())
                    state.settings.eSyncMode = ClientSettings.SyncMode.Off;

                else if (syncMode == ClientSettings.SyncMode.Off && hwSyncWasEnabled)
                    oServer.DisableTemporalSync();

            Cursor.Current = Cursors.Default;
            UpdateUI();
        }

        public void SetExposureMode(bool auto)
        {
            state.settings.bAutoExposureEnabled = auto;
            oServer.SendCurrentSettings();
            UpdateUI();
        }

        public void SetExposureValue(int step)
        {
            state.settings.nExposureStep = step;
            oServer.SendCurrentSettings();
            UpdateUI();
        }

        public void SetWhiteBalanceMode(bool auto)
        {
            state.settings.bAutoWhiteBalanceEnabled = auto;
            oServer.SendCurrentSettings();
            UpdateUI();
        }

        public void SetWhiteBalanceValue(int kelvin)
        {
            state.settings.nKelvin = kelvin;
            oServer.SendCurrentSettings();
            UpdateUI();
        }

        public void SetClientPreview(bool enabled)
        {
            state.settings.bPreviewEnabled = enabled;
            oServer.SendCurrentSettings();
            UpdateUI();
        }

        public void SetExportMode(ClientSettings.ExportMode exportMode)
        {
            state.settings.eExportMode = exportMode;
            oServer.SendCurrentSettings();
            UpdateUI();
        }

        public void SetMergeScans(bool merge)
        {
            state.settings.bMergeScansForSave = merge;
            oServer.SendCurrentSettings();
            UpdateUI();
        }

        public void SetVisibility(int index, bool visible)
        {
            state.clients[index].bVisible = visible;
            UpdateUI();
        }

        public void SetNickname(string serial, string nickname)
        {
            string formattedName = "";

            //Convert to ASCII only
            for (int i = 0; i < nickname.Length; i++)
                formattedName += nickname[i] > 255 ? '?' : nickname[i];

            //Max 20 chars allowed
            if (formattedName.Length > 20)
                formattedName = formattedName.Substring(0, 20);

            //..But we also need exactly 20 chars
            for (int i = 0; i < 20 - formattedName.Length; i++)
                formattedName += ' ';

            ClientConfiguration newConfig = GetConfigFromSerial(serial);
            newConfig.NickName = formattedName;
            oServer.SetAndConfirmConfig(newConfig);

            UpdateUI();
        }


        #endregion

        #region Workers

        private void ProcessingWorker_Start(DoWorkEventHandler worker, RunWorkerCompletedEventHandler workerCompleted)
        {
            ProcessingWorker_Cancel(); //We can only have one processingworker doing work
            processingWorker = new BackgroundWorker();
            processingWorker.WorkerSupportsCancellation = true;
            processingWorker.DoWork += new DoWorkEventHandler(worker);
            processingWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workerCompleted);
            processingWorkerComplete = false;
            processingWorker.RunWorkerAsync();
        }

        private void ProcessingWorker_Cancel()
        {
            if (processingWorker == null)
                return;

            if (!processingWorker.IsBusy)
                return;

            processingWorker.CancelAsync();

            while (!processingWorkerComplete)
            {
                Thread.Sleep(1);
            }

            processingWorker.Dispose();
        }

        private void Calibration_DoWork(object sender, DoWorkEventArgs e)
        {
            Log.LogInfo("Waiting for calibration to finish");

            bool allCalibrated = false;

            while (!processingWorker.CancellationPending && !allCalibrated)
            {
                allCalibrated = true;
                Log.LogInfo("Calibrating...");
                Thread.Sleep(10);

                Log.LogInfo("Client count = " + state.clients.Count);

                for (int i = 0; i < state.clients.Count; i++)
                {
                    Log.LogDebug("Client " + state.clients[i].configuration.SerialNumber + " calibrated = " + state.clients[i].bCalibrated);
                    if (!state.clients[i].bCalibrated)
                        allCalibrated = false;
                }
            }

            if (allCalibrated)
                Log.LogInfo("Calibration complete");
            else
                Log.LogInfo("Calibration canceled");

            processingWorkerComplete = true;
        }

        private void Calibration_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            state.appState = appState.idle;
            UpdateUI();
        }

        private void RefinementWorker(object sender, DoWorkEventArgs e)
        {
            Log.LogInfo("Start ICP pose refinement");

            //Download a frame from each client.
            List<List<float>> lAllFrameVertices = new List<List<float>>();
            List<List<byte>> lAllFrameColors = new List<List<byte>>();
            oServer.GetLatestFrame(lAllFrameColors, lAllFrameVertices, true);

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

            for (int refineIter = 0; refineIter < state.settings.nNumRefineIters; refineIter++)
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

                    ICP(pVerts1, pVerts2, otherFramesVertices.Count / 3, lAllFrameVertices[i].Count / 3, Rs[i], Ts[i], state.settings.nNumICPIterations);

                    Marshal.Copy(pVerts2, verts2, 0, verts2.Length);
                    lAllFrameVertices[i].Clear();
                    lAllFrameVertices[i].AddRange(verts2);
                }

                if (processingWorker.CancellationPending)
                    break;
            }

            if (!processingWorker.CancellationPending)
            {
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
                }

                oServer.lRefinementTransforms = icpTransforms;

            }

            processingWorkerComplete = true;
        }

        private void RefinementWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            oServer.UpdateMarkerTransforms();
            oServer.SendRefinementData();

            state.appState = appState.idle;
            UpdateUI();
        }



        //Performs recording which is synchronized or unsychronized frame capture.
        //The frames are downloaded from the clients and saved once recording is finished.
        private void Capture_DoWork(object sender, DoWorkEventArgs e)
        {
            oServer.ClearStoredFrames();
            oServer.SendAndConfirmPreRecordProcess();

            //If we don't use a server-controlled sync method, we just let the clients capture as fast as possible
            if (state.settings.eSyncMode == ClientSettings.SyncMode.Hardware || state.settings.eSyncMode == ClientSettings.SyncMode.Off)
            {
                oServer.SendCaptureFramesStart();

                Stopwatch counter = new Stopwatch();
                counter.Start();

                Log.LogInfo("Starting Recording");

                while (!processingWorker.CancellationPending)
                {
                    state.stateIndicatorSuffix = "" + counter.Elapsed.Minutes.ToString("D2") + ":" + counter.Elapsed.Seconds.ToString("D2");
                    //UpdateUI();
                }

                state.capturedFramesTotal = (int)counter.Elapsed.TotalSeconds * 30;

                Log.LogInfo("Stopping Recording");

                oServer.SendCaptureFramesStop();
                counter.Stop();

            }

            //A server controlled sync method, the server gives the command to capture a single frame and waits until all devices have captured it
            else if (state.settings.eSyncMode == ClientSettings.SyncMode.Network)
            {
                int nCaptured = 0;

                Log.LogInfo("Starting Network-synced recording");

                while (!processingWorker.CancellationPending)
                {
                    oServer.CaptureSynchronizedFrame();
                    nCaptured++;
                    state.stateIndicatorSuffix = "Frame " + (nCaptured).ToString() + ".";
                    //UpdateUI();
                }

                state.capturedFramesTotal = nCaptured;

                Log.LogInfo("Stopping Network-synced recording");
            }

            oServer.SendAndConfirmPostRecordProcess();

            processingWorkerComplete = true;
        }

        void Capture_Complete(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void SyncWorker(object sender, DoWorkEventArgs e)
        {
            e.Result = true;

            if (state.settings.eSyncMode == ClientSettings.SyncMode.Hardware)
            {
                Log.LogInfo("Starting Sync");

                if (!oServer.GetTimestampLists())
                {
                    Log.LogError("Could not get Timestamp List. Saved recording is unsychronized!");
                    UI.ShowMessageBox(MessageBoxIcon.Error, "Could not get Timestamp List. Saved recording is unsychronized!");
                    e.Result = false;
                    return;
                }

                if (!oServer.CreatePostSyncList())
                {
                    Log.LogError("Could not match timestamps. Please check your Temporal Sync setup and Kinect firmware!");
                    UI.ShowMessageBox(MessageBoxIcon.Error, "Could not match timestamps. Please check your Temporal Sync setup and Kinect firmware!");
                    e.Result = false;
                    return;
                }

                if (!oServer.ReorderSyncFramesOnClient())
                {
                    Log.LogError("Could not reorganize files for sync on at least one device!");
                    UI.ShowMessageBox(MessageBoxIcon.Error, "Could not reorganize files for sync on at least one device!");
                    e.Result = false;
                    return;
                }

                Log.LogInfo("Sync successfull!");
            }

            processingWorkerComplete = true;
        }

        private void Sync_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            state.appState = appState.idle;
            UpdateUI();

            if ((bool)e.Result)
            {
                Save();
            }

            else
            {
                Log.LogError("Sync could not be completed!");
                UI.ShowMessageBox(MessageBoxIcon.Warning, "Sync could not succesfully be completed. Don't worry, your files are still saved, please try syncing with the LiveScan Editor.");
            }
        }

        private void SavingWorker(object sender, DoWorkEventArgs e)
        {
            e.Result = true;

            if (state.settings.eExportMode == ClientSettings.ExportMode.Pointcloud)
            {
                Log.LogInfo("Start saving Pointcloud frames");
                Log.LogInfo("Merging Pointclouds: " + state.settings.bMergeScansForSave);

                //Saving is downloading the frames from clients and saving them locally.
                int nFrames = 0;

                if (state.settings.takePath == null)
                    return;

                e.Result = false;

                //This loop is running till it is either cancelled, or till there are no more stored frames.
                while (!processingWorker.CancellationPending)
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

                    state.stateIndicatorSuffix = " " + (nFrames).ToString() + " / " + state.capturedFramesTotal;
                    for (int i = 0; i < lFrameBGRAllDevices.Count; i++)
                    {
                        lFrameBGR.AddRange(lFrameBGRAllDevices[i]);
                        lFrameVerts.AddRange(lFrameVertsAllDevices[i]);

                        //This is ran if the frames from each client are to be placed in separate files.
                        if (state.settings.bMergeScansForSave)
                        {
                            string outputFilename = state.settings.takePath + "\\" + nFrames.ToString().PadLeft(5, '0') + i.ToString() + ".ply";
                            Utils.saveToPly(outputFilename, lFrameVertsAllDevices[i], lFrameBGRAllDevices[i], EColorMode.BGR, state.settings.bSaveAsBinaryPLY);
                        }
                    }

                    //This is ran if the frames from all clients are to be placed in a single file.
                    if (!state.settings.bMergeScansForSave)
                    {
                        string outputFilename = state.settings.takePath + "\\" + nFrames.ToString().PadLeft(5, '0') + ".ply";
                        Utils.saveToPly(outputFilename, lFrameVerts, lFrameBGR, EColorMode.BGR, state.settings.bSaveAsBinaryPLY);
                    }
                }

            }

            processingWorkerComplete = true;
        }

        private void Saving_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)e.Result)
            {
                Log.LogInfo("Saving complete");
            }

            else
            {
                Log.LogInfo("Saving cancelled");
            }

            oServer.ClearStoredFrames();
            StartPreviewWorker();
            state.appState = appState.idle;
            UpdateUI();
        }

        void StartPreviewWorker()
        {
            previewWorker.RunWorkerAsync();
        }

        void PreviewWorker_Cancel()
        {
            previewWorker.CancelAsync();
        }

        //Continually requests frames that will be displayed in the live view window.
        private void PreviewWorker(object sender, DoWorkEventArgs e)
        {
            List<List<byte>> lFramesRGB = new List<List<byte>>();
            List<List<Single>> lFramesVerts = new List<List<Single>>();

            while (!previewWorker.CancellationPending)
            {
                Thread.Sleep(1);

                Stopwatch timer = new Stopwatch();
                timer.Start();

                if (oServer != null)
                {
                    oServer.GetLatestFrame(lFramesRGB, lFramesVerts, false);
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
                            state.previewWindowFPS = ((int)(1000 / timer.ElapsedMilliseconds));
                        else
                            state.previewWindowFPS = 0;
                    }
                }
            }

            //Thread ended, cleanup
            lock (lAllVertices)
            {
                lAllVertices.Clear();
                lAllColors.Clear();
            }
        }

        #endregion
    }



}
