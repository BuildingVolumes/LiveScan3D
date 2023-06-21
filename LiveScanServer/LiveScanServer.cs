using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
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
        TransferServer oTransferServer;

        bool bRecording = false;
        bool bSaving = false;

        KinectSettings oSettings = new KinectSettings();

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
                UI.ShowFatalWindowAndQuit("Could not access logging file, another Livescan Server instance is probably already open!");
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
                UI.ShowFatalWindowAndQuit("Could not create working directories, please restart Application!");
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

                if (settingsStream != null)
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

            if (!oServer.StartServer()) //If another Livescan Instance is already open, we terminate the app here
                return;

            oTransferServer.StartServer();
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

            if (oTransferServer != null)
                oTransferServer.StopServer();

            Log.LogInfo("Programm termined normally, exiting");
            Log.CloseLog();
        }

        //Performs recording which is synchronized or unsychronized frame capture.
        //The frames are downloaded from the clients and saved once recording is finished.


        public void ClientListChanged(List<KinectSocket> socketList)
        {
            UI.upda
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



        void RestartUpdateWorker()
        {
            if (!updateWorker.IsBusy)
                updateWorker.RunWorkerAsync();
        }

        //Performs the ICP based pose refinement.


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






        #region UIInput

        public void Calibrate()
        {
            Log.LogInfo("Starting Calibration");
            oServer.Calibrate();
        }

        public void RefineCalibration()
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

        public void RecordButtonClicked()
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

        public void SetHardwareSync(bool enabled)
        {

        }

        public void SetNetworkSync(bool enabled)
        {

        }

        public void SetAutoExposure(bool enabled)
        {

        }

        public void SetManualExposure(bool enabled)
        {

        }

        public void SetClientPreview(bool enabled)
        {

        }

        public void SetExportMode()
        {

        }

        public void SetMergeScans()
        {

        }

        public void SetAutoExposure()
        {

        }

        public void SetWhiteBalance()
        {

        }

        #endregion

        #region WorkerThreads

        private async Task refineWorker()
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

        private void refineWorker_Completed()
        {
            Log.LogInfo("ICP refinement done");
            //Re-enable all of the buttons after refinement.
            btRefineCalib.Enabled = true;
            btCalibrate.Enabled = true;
            btRecord.Enabled = true;
        }

        private async Task savingWorker()
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
                    Utils.saveToPly(outputFilename, lFrameVerts, lFrameBGR, EColorMode.BGR, oSettings.bSaveAsBinaryPLY);
                }
            }

            oSettings.takePath = null;

            savingWorker_Completed();
        }

        private void savingWorker_Completed()
        {
            Log.LogInfo("Saving complete!");
            oServer.ClearStoredFrames();
            bSaving = false;

            CaptureComplete();
        }

        private async Task recordingWorker()
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
            recordingWorkerCompleted();

        }

        private void recordingWorkerCompleted()
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

        private void syncWorker_DoWork()
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

            syncWorker_Completed(synced);
        }

        private void syncWorker_Completed(bool success)
        {
            if (success)
                StartSaving();
            else
                CaptureComplete();
        }

        //Continually requests frames that will be displayed in the live view window.
        private async Task updateWorker()
        {
            List<List<byte>> lFramesRGB = new List<List<byte>>();
            List<List<Single>> lFramesVerts = new List<List<Single>>();

            BackgroundWorker worker = (BackgroundWorker)sender;
            while (!worker.CancellationPending)
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
                            UpdateFPSCounter((int)(1000 / timer.ElapsedMilliseconds));
                        else
                            UpdateFPSCounter(0);
                    }
                }


            }

            updateWorker_Completed();
        }

        private void updateWorker_Completed()
        {
            lock (lAllVertices)
            {
                lAllVertices.Clear();
                lAllColors.Clear();
            }
        }


        #endregion


        public LiveScanState GetState()
        {
            //Todo: Update settings & Clients here;
            return state;
        }

        public void SettingsChanged(KinectSettings newSettings)
        {
            //Todo: Need to lock settings?
            state.settings = newSettings;
        }
    }


    public class LiveScanState 
    {
        public appState appState;
        public KinectSettings settings;
        public List<KinectSocket> clients;
    }

}
