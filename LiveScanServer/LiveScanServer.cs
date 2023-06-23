using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KinectServer
{
    public class LiveScanServer
    {
        [DllImport("ICP.dll")]
        static extern float ICP(IntPtr verts1, IntPtr verts2, int nVerts1, int nVerts2, float[] R, float[] t, int maxIter = 200);

        MainWindowForm UI;
        LiveScanState state;
        KinectServer oServer;
        //TransferServer oTransferServer;

        Thread previewWorker;
        Thread syncWorker;
        Thread captureWorker;
        Thread saveWorker;
        Thread refineWorker;

        bool cancelPreviewWorker = false;
        bool cancelCaptureWorker = false;
        bool cancelSaveWorker = false;
        bool cancelrefineWorker = false;


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

            Stream settingsStream = null;
            //This tries to read the settings from "settings.bin", if it fails, the settings stay at default values.
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

                if (settingsStream != null)
                    settingsStream.Dispose();
            }

            oSettings.AddDefaultMarkers();
            oServer = new KinectServer(oSettings);
            oServer.eSocketListChanged += new SocketListChangedHandler(ClientListChanged);
            oServer.SetMainWindowForm(this);

            //oTransferServer = new TransferServer();
            //oTransferServer.lVertices = lAllVertices;
            //oTransferServer.lColors = lAllColors;

            StartPreviewWorker();

            if (!oServer.StartServer()) //If another Livescan Instance is already open, we terminate the app here
                return;

            //oTransferServer.StartServer();
            Log.LogInfo("Starting Server");
        }

        public void Terminate()
        {
            //The current settings are saved to a files.
            IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            Stream stream = new FileStream("temp/settings.bin", FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, oSettings);
            stream.Close();

            if (oServer != null)
                oServer.StopServer();

            //if (oTransferServer != null)
            //    oTransferServer.StopServer();



            Log.LogInfo("Programm termined normally, exiting");
            Log.CloseLog();
        }

        public void StateChanged()
        {
            UI.UpdateUI(state);
        }

        public void ClientListChanged(List<KinectSocket> socketList)
        {
            state.clients = socketList;
            StateChanged();
        }

        public LiveScanState GetState()
        {
            //Todo: Update settings & Clients here;
            return state;
        }

        public void SetSettings(KinectSettings newSettings)
        {
            //Todo: Need to lock settings?
            state.settings = newSettings;
            oServer.SendSettings();
            StateChanged();

        }

        public void SetConfiguration(KinectConfiguration newConfig)
        {
            KinectConfiguration currentConfig = GetConfigFromSerial(newConfig.SerialNumber);

            bool restartClient = false;
            bool restartAllClients = false;

            if (currentConfig.eDepthRes != newConfig.eDepthRes || currentConfig.eColorRes != newConfig.eColorRes)
                if (state.settings.eSyncMode == KinectSettings.SyncMode.Hardware)
                    restartAllClients = true;
                else
                    restartClient = true;


            oServer.SetAndConfirmConfig(newConfig);

            Cursor.Current = Cursors.WaitCursor;

            if (restartClient)
                oServer.RestartClient(newConfig.SerialNumber);
            if (restartAllClients)
                oServer.RestartAllClients();

            Cursor.Current = Cursors.Default;

            StateChanged();
        }

        public KinectConfiguration GetConfigFromSerial(string serialnumber)
        {
            KinectConfiguration config = null;

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


        #region UIInput

        public void CalibrateButtonClicked()
        {
            if (state.appState == appState.idle)
            {
                Log.LogInfo("Starting Calibration");
                oServer.Calibrate();
                state.appState = appState.calibrating;
            }

            if (state.appState == appState.calibrating)
            {
                Log.LogInfo("Canceled Calibration");
                oServer.StopCalibration();
                state.appState = appState.idle;
            }

            StateChanged();

        }

        public void RefineCalibrationButtonClicked()
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

                refineWorker = new Thread(new ThreadStart(RefineWorker));
                refineWorker.Start();

                state.appState = appState.refinining;
            }

            if (state.appState == appState.refinining)
            {
                Log.LogInfo("Calibration refinement canceled by user");
                cancelrefineWorker = true;
                refineWorker.Join();
                cancelrefineWorker = false;

                state.appState = appState.idle;
            }

            StateChanged();
        }

        public void RecordButtonClicked(string sequenceName)
        {
            //Start recording
            if (state.appState == appState.idle)
            {
                if (oServer.nClientCount < 1)
                {
                    UI.ShowMessageBox(MessageBoxIcon.Warning, "At least one client needs to be connected for recording.");
                    return;
                }

                //Stop the update worker to reduce the load on the clients
                StopPreviewWorker();

                string takePath = oServer.CreateTakeDirectories(sequenceName);

                if (takePath == null)
                {
                    UI.ShowMessageBox(MessageBoxIcon.Warning, "Error: Couldn't create take directory on either the server or the clients");
                    return;
                }

                //Store the camera extrinsics
                Utils.SaveExtrinsics(state.settings.eExtrinsicsFormat, takePath, oServer.GetClientSockets());

                Log.LogInfo("Starting Recording");
                captureWorker = new Thread(new ThreadStart(CaptureWorker));
                captureWorker.Start();

                state.settings.takePath = takePath;
                state.appState = appState.recording;
            }

            //Stop recording
            else if (state.appState == appState.recording)
            {
                cancelCaptureWorker = true;
                captureWorker.Join();
                cancelCaptureWorker = false;

                //After recording has been terminated it is time to begin sorting the frames.
                if (state.settings.eSyncMode == KinectSettings.SyncMode.Hardware)
                {
                    Log.LogInfo("Starting to synchronize recording");
                    syncWorker = new Thread(new ThreadStart(SyncWorker));
                    syncWorker.Start();
                    state.appState = appState.syncing;
                }

                //If sync wasn't enabled, we go straight to saving the files
                else
                {
                    if (state.settings.eExportMode == KinectSettings.ExportMode.Pointcloud)
                    {
                        Log.LogInfo("Starting to save frames ");
                        saveWorker = new Thread(new ThreadStart(SavingWorker));
                        saveWorker.Start();
                        state.appState = appState.saving;
                    }

                    else
                    {
                        StartPreviewWorker();
                        state.appState = appState.idle;
                    }

                }
            }

            //If we are saving frames right now, this button stops saving.
            else if (state.appState == appState.saving)
            {
                Log.LogInfo("User canceled frame saving");
                cancelSaveWorker = true;
                saveWorker.Join();
                cancelSaveWorker = false;

                StartPreviewWorker();
                state.appState = appState.idle;
            }

            StateChanged();
        }

        public void SetSyncMode(KinectSettings.SyncMode syncMode)
        {
            if (syncMode != state.settings.eSyncMode)
                return;

            bool hwSyncWasEnabled = state.settings.eSyncMode == KinectSettings.SyncMode.Hardware;

            state.settings.eSyncMode = syncMode;
            oServer.SendSettings();

            //Check if we need to restart the cameras
            if (syncMode == KinectSettings.SyncMode.Hardware)
                if (!oServer.EnableTemporalSync())
                    state.settings.eSyncMode = KinectSettings.SyncMode.off;

                else if (syncMode == KinectSettings.SyncMode.off && hwSyncWasEnabled)
                    oServer.DisableTemporalSync();

            Cursor.Current = Cursors.Default;
            StateChanged();
        }

        public void SetExposureMode(bool auto)
        {
            state.settings.bAutoExposureEnabled = auto;
            oServer.SendSettings();
            StateChanged();
        }

        public void SetExposureValue(int step)
        {
            state.settings.nExposureStep = step;
            oServer.SendSettings();
            StateChanged();
        }

        public void SetWhiteBalanceMode(bool auto)
        {
            state.settings.bAutoWhiteBalanceEnabled = auto;
            oServer.SendSettings();
            StateChanged();
        }

        public void SetWhiteBalanceValue(int kelvin)
        {
            state.settings.nKelvin = kelvin;
            oServer.SendSettings();
            StateChanged();
        }

        public void SetClientPreview(bool enabled)
        {
            state.settings.bPreviewEnabled = enabled;
            oServer.SendSettings();
            StateChanged();
        }

        public void SetExportMode(KinectSettings.ExportMode exportMode)
        {
            state.settings.eExportMode = exportMode;
            oServer.SendSettings();
            StateChanged();
        }

        public void SetMergeScans(bool merge)
        {
            state.settings.bMergeScansForSave = merge;
            oServer.SendSettings();
            StateChanged();
        }


        #endregion

        #region WorkerThreads

        private void RefineWorker()
        {
            Log.LogInfo("Start ICP pose refinement");

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

                if (cancelrefineWorker)
                    break;
            }

            if (!cancelrefineWorker)
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
                oServer.UpdateMarkerTransforms();
                oServer.SendRefinementData();
            }
        }

        private void SavingWorker()
        {


            Log.LogInfo("Start saving Pointcloud frames");
            Log.LogInfo("Merging Pointclouds: " + state.settings.bMergeScansForSave);

            //Saving is downloading the frames from clients and saving them locally.
            int nFrames = 0;

            if (state.settings.takePath == null)
                return;

            //This loop is running till it is either cancelled, or till there are no more stored frames.
            while (!cancelSaveWorker)
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

                UI.ShowStatus("Saving frame " + (nFrames).ToString() + ".", 5000);
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

        private void savingWorker_Completed()
        {
            Log.LogInfo("Saving complete!");
            oServer.ClearStoredFrames();
            bSaving = false;

            //TODO: Restart Update worker when saving has terminated

            CaptureComplete();
        }

        //Performs recording which is synchronized or unsychronized frame capture.
        //The frames are downloaded from the clients and saved once recording is finished.

        private void CaptureWorker()
        {
            oServer.ClearStoredFrames();

            oServer.SendAndConfirmPreRecordProcess();

            //If we don't use a server-controlled sync method, we just let the clients capture as fast as possible
            if (state.settings.eSyncMode != KinectSettings.SyncMode.Network)
            {
                oServer.SendCaptureFramesStart();

                Stopwatch counter = new Stopwatch();
                counter.Start();

                Log.LogInfo("Starting Recording");

                while (cancelCaptureWorker)
                {
                    UI.ShowStatus("Recording: " + counter.Elapsed.Minutes.ToString("D2") + ":" + counter.Elapsed.Seconds.ToString("D2"));
                }

                Log.LogInfo("Stopping Recording");

                oServer.SendCaptureFramesStop();
                counter.Stop();

            }

            //A server controlled sync method, the server gives the command to capture a single frame and waits until all devices have captured it
            else
            {
                int nCaptured = 0;

                Log.LogInfo("Starting Network-synced recording");

                while (cancelCaptureWorker)
                {
                    oServer.CaptureSynchronizedFrame();
                    nCaptured++;
                    UI.ShowStatus("Captured frame " + (nCaptured).ToString() + ".");
                }

                Log.LogInfo("Stopping Network-synced recording");
            }

            oServer.SendAndConfirmPostRecordProcess();

        }

        private void SyncWorker()
        {
            if (state.settings.eSyncMode == KinectSettings.SyncMode.Hardware)
            {
                if (!oServer.GetTimestampLists())
                {
                    Log.LogError("Could not get Timestamp List. Saved recording is unsychronized!");
                    UI.ShowMessageBox(MessageBoxIcon.Error, "Could not get Timestamp List. Saved recording is unsychronized!");
                    return;
                }

                if (!oServer.CreatePostSyncList())
                {
                    Log.LogError("Could not match timestamps. Please check your Temporal Sync setup and Kinect firmware!");
                    UI.ShowMessageBox(MessageBoxIcon.Error, "Could not match timestamps. Please check your Temporal Sync setup and Kinect firmware!");
                    return;
                }

                if (!oServer.ReorderSyncFramesOnClient())
                {
                    Log.LogError("Could not reorganize files for sync on at least one device!");
                    UI.ShowMessageBox(MessageBoxIcon.Error, "Could not reorganize files for sync on at least one device!");
                    return;
                }

                Log.LogInfo("Sync successfull!");

                //TODO: Call saving function/thread
            }

        }


        private void syncWorker_Completed(bool success)
        {
            if (success)
                StartSaving();
            else
                CaptureComplete();
        }

        void StartPreviewWorker()
        {
            previewWorker = new Thread(new ThreadStart(PreviewWorker));
            previewWorker.Start();
        }

        void StopPreviewWorker()
        {
            cancelPreviewWorker = true;
            previewWorker.Join();
            cancelPreviewWorker = false;
        }

        //Continually requests frames that will be displayed in the live view window.
        private void PreviewWorker()
        {
            List<List<byte>> lFramesRGB = new List<List<byte>>();
            List<List<Single>> lFramesVerts = new List<List<Single>>();

            while (!cancelPreviewWorker)
            {
                Thread.Sleep(1);

                Stopwatch timer = new Stopwatch();
                timer.Start();

                if (oServer != null)
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

    public class LiveScanState
    {
        public appState appState;
        public KinectSettings settings;
        public List<KinectSocket> clients;

        public float previewWindowFPS = 0;
    }

}
