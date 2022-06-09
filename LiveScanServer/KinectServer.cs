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
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace KinectServer
{
    public delegate void SocketListChangedHandler(List<KinectSocket> list);
    public class KinectServer
    {
        Socket oServerSocket;
        bool bServerRunning = false;

        ManualResetEvent allDone = new ManualResetEvent(false);

        KinectSettings oSettings;
        public SettingsForm fSettingsForm;
        public Dictionary<int, KinectConfigurationForm> kinectSettingsForms;
        public MainWindowForm fMainWindowForm;
        Thread listeningThread;
        Thread receivingThread;
        List<KinectSocket> lClientSockets = new List<KinectSocket>();
        public event SocketListChangedHandler eSocketListChanged;

        public bool bTempHwSyncEnabled = false;
        public bool bPointCloudMode = true;

        object oClientSocketLock = new object();
        object oFrameRequestLock = new object();
        const float networkTimeout = 5f;

        public int nClientCount
        {
            get
            {
                int nClients;
                lock (oClientSocketLock)
                {
                    nClients = lClientSockets.Count;
                }
                return nClients;
            }
        }

        public List<AffineTransform> lCameraPoses
        {
            get
            {
                List<AffineTransform> cameraPoses = new List<AffineTransform>();
                lock (oClientSocketLock)
                {
                    for (int i = 0; i < lClientSockets.Count; i++)
                    {
                        cameraPoses.Add(lClientSockets[i].oCameraPose);
                    }
                }
                return cameraPoses;
            }
            set
            {
                lock (oClientSocketLock)
                {
                    for (int i = 0; i < lClientSockets.Count; i++)
                    {
                        lClientSockets[i].oCameraPose = value[i];
                    }
                }
            }
        }

        public List<AffineTransform> lWorldTransforms
        {
            get
            {
                List<AffineTransform> worldTransforms = new List<AffineTransform>();
                lock (oClientSocketLock)
                {
                    for (int i = 0; i < lClientSockets.Count; i++)
                    {
                        worldTransforms.Add(lClientSockets[i].oWorldTransform);
                    }
                }
                return worldTransforms;
            }

            set
            {
                lock (oClientSocketLock)
                {
                    for (int i = 0; i < lClientSockets.Count; i++)
                    {
                        lClientSockets[i].oWorldTransform = value[i];
                    }
                }
            }
        }

        public bool bAllCalibrated
        {
            get
            {
                bool allCalibrated = true;
                lock (oClientSocketLock)
                {
                    for (int i = 0; i < lClientSockets.Count; i++)
                    {
                        if (!lClientSockets[i].bCalibrated)
                        {
                            allCalibrated = false;
                            break;
                        }
                    }

                }
                return allCalibrated;
            }
        }

        public KinectServer(KinectSettings settings)
        {
            this.oSettings = settings;
            kinectSettingsForms = new Dictionary<int, KinectConfigurationForm>();
        }

        public void SetSettingsForm(SettingsForm settings)
        {
            fSettingsForm = settings;
        }
        public void SetKinectSettingsForm(int id, KinectConfigurationForm form)
        {
            if (kinectSettingsForms.ContainsKey(id))
            {
                kinectSettingsForms[id] = form;
            }
            else
            {
                kinectSettingsForms.Add(id, form);
            }
        }

        public List<KinectSocket> GetClientSocketsCopy()
        {
            List<KinectSocket> sockets = new List<KinectSocket>(lClientSockets);
            return sockets;
        }

        public KinectSocket GetKinectSocketByIndex(int socketIndex)
        {
            return lClientSockets[socketIndex];
        }

        public List<string> GetSerialNumbers()
        {
            List<string> serialNumbers = new List<string>();

            for (int i = 0; i < lClientSockets.Count; i++)
            {
                serialNumbers.Add(lClientSockets[i].configuration.SerialNumber);
            }

            return serialNumbers;
        }

        public void SetMainWindowForm(MainWindowForm main)
        {
            fMainWindowForm = main;
        }

        public SettingsForm GetSettingsForm()
        {
            return fSettingsForm;
        }

        public KinectConfigurationForm GetKinectSettingsForm(int id)
        {
            if (kinectSettingsForms.TryGetValue(id, out var value))
            {
                return value;
            }
            else
            {
                //Uh Oh. Have we removed or added a camera in an odd way?
                return null;
            }
        }
        private void SocketListChanged()
        {
            if (eSocketListChanged != null)
            {
                eSocketListChanged(lClientSockets);
            }
        }

        public bool StartServer()
        {
            if (!bServerRunning)
            {
                oServerSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                oServerSocket.Blocking = false;

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 48001);

                try
                {
                    oServerSocket.Bind(endPoint);
                    oServerSocket.Listen(10);
                }

                //Probably another server already running
                catch(SocketException se)
                {
                    fMainWindowForm.ShowFatalWindowAndQuit("Another Livescan Server Instance is already running!");
                    return false;
                }

                bServerRunning = true;
                listeningThread = new Thread(this.ListeningWorker);
                listeningThread.Start();
                receivingThread = new Thread(this.ReceivingWorker);
                receivingThread.Start();

                Log.LogDebug("Starting Server");

                return true;
            }

            return true;
        }

        public void StopServer()
        {
            if (bServerRunning)
            {
                Log.LogDebug("Stopping Server");
                bServerRunning = false;
                allDone.Set();
                listeningThread.Join();
                receivingThread.Join();

                oServerSocket.Close();
                lock (oClientSocketLock)
                    lClientSockets.Clear();
            }
        }

        public void CaptureSynchronizedFrame()
        {
            Log.LogDebugCapture("Capturing synchronized frame");
            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                    lClientSockets[i].CaptureFrame();
            }

            //Wait till frames captured
            bool allGathered = false;
            while (!allGathered)
            {
                allGathered = true;

                lock (oClientSocketLock)
                {
                    for (int i = 0; i < lClientSockets.Count; i++)
                    {
                        if (!lClientSockets[i].bFrameCaptured)
                        {
                            allGathered = false;
                            break;
                        }
                    }
                }
            }
        }

        public void Calibrate()
        {
            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].Calibrate();
                }
            }
        }

        public void SendSettings()
        {
            Log.LogDebug("Sending Settings:");

            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].SendSettings(oSettings);
                }
            }
        }

        public void SendCalibrationData()
        {
            Log.LogDebug("Sending Calibration Data");

            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].SendCalibrationData();
                }
            }
        }

        public bool SetAndConfirmConfig(KinectSocket socket, KinectConfiguration newConfig)
        {
            Log.LogDebug("Setting Configuration file for kinect: " + newConfig.SerialNumber);
            Log.LogDebug(newConfig.ToString());

            lock (oClientSocketLock)
            {
                socket.SendConfiguration(newConfig);
                socket.RequestConfiguration();
            }

            bool recievedData = false;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            while (!recievedData && timer.Elapsed.TotalSeconds < networkTimeout)
            {
                recievedData = true;

                lock (oClientSocketLock)
                {
                    if (!socket.bConfigurationReceived)
                        recievedData = false;
                }

            }

            timer.Stop();

            if (!socket.bConfigurationReceived)
            {
                fMainWindowForm?.ShowWarningWindow("Could not confirm configuration file, please check your network");
                Log.LogError("Client did not confirm that it has received configuration file");
                return false;
            }

            return true;
        }


        public bool CloseClient(KinectSocket client)
        {
            return CloseClient(new List<KinectSocket>() { client });
        }

        /// <summary>
        /// Closes all clients in the list 
        /// </summary>
        /// <returns>Returns true on successfull restart, false on restart error</returns>
        public bool CloseClient(List<KinectSocket> clients)
        {
            Log.LogDebug("Closing Client camera");

            bool cameraError = false;

            lock (oClientSocketLock)
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    clients[i].CloseCameraAndConfirm();
                    clients[i].UpdateSocketState(" Reinitializing Camera... ");
                }
            }

            bool camerasClosed = false;
            while (!camerasClosed)
            {
                camerasClosed = true;

                lock (oClientSocketLock)
                {
                    for (int i = 0; i < clients.Count; i++)
                    {
                        if (!clients[i].bCameraClosed)
                            camerasClosed = false;
                    }
                }
            }


            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].bCameraError)
                {
                    Log.LogError("Camera close failed on a least one client");
                    fMainWindowForm?.ShowWarningWindow("Could not close a kinect. Please reconnect the client and try again");
                    clients[i].UpdateSocketState(" Close failed! ");
                    cameraError = true;
                }

                else
                    clients[i].UpdateSocketState("");
            }            

            if (cameraError)
                return false;

            Log.LogDebug("Clients camera closed successfully");

            return true;
        }

        public bool InitializeClient(KinectSocket client)
        {
            return InitializeClient(new List<KinectSocket>() { client });
        }

        public bool InitializeClient(List<KinectSocket> clients)
        {
            Log.LogDebug("Initializing client camera");

            bool cameraError = false;

            lock (oClientSocketLock)
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    clients[i].InitializeCameraAndConfirm();
                    clients[i].UpdateSocketState(" Initializing Camera... ");
                }
            }

            bool camerasInitialized = false;
            while (!camerasInitialized)
            {
                camerasInitialized = true;

                lock (oClientSocketLock)
                {
                    for (int i = 0; i < clients.Count; i++)
                    {
                        if (!clients[i].bCameraInitialized)
                            camerasInitialized = false;
                    }
                }
            }

            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].bCameraError)
                {
                    Log.LogError("Could not initialize at least one camera on client");
                    fMainWindowForm?.ShowWarningWindow("Could not initialize a kinect. Please reconnect the client and try again");
                    clients[i].UpdateSocketState(" Initialization failed! ");
                    cameraError = true;
                }

                else
                    clients[i].UpdateSocketState("");
            }

            if (cameraError)
                return false;

            Log.LogDebug("Cameras initialized successfully");
            return true;
        }

        public bool RestartAllClients()
        {
            Log.LogInfo("Restarting all client cameras");

            if (CloseClient(lClientSockets))
            {
                return InitializeClient(lClientSockets);
            }

            else
                return false;
        }

        /// <summary>
        /// Restarts all clients in a specific order that allows temporal sync to work.
        /// (Restarts all subs first, and then the master)
        /// </summary>
        /// <returns>Returns true on successfull restart, false on restart error</returns>
        public bool RestartWithTemporalSyncPattern()
        {
            Log.LogInfo("Restarting Client cameras with Hardware Sync pattern");

            List<KinectSocket> subordinates = new List<KinectSocket>();
            List<KinectSocket> main = new List<KinectSocket>();

            for (int i = 0; i < lClientSockets.Count; i++)
            {
                if (lClientSockets[i].configuration.eSoftwareSyncState == KinectConfiguration.SyncState.Subordinate)
                    subordinates.Add(lClientSockets[i]);

                if (lClientSockets[i].configuration.eSoftwareSyncState == KinectConfiguration.SyncState.Main)
                    main.Add(lClientSockets[i]);
            }

            //First we need to close all cameras, and ensure no camera is running anymore
            if (!CloseClient(lClientSockets))
            {
                Log.LogError("Could not close one or more client cameras");
                fMainWindowForm?.ShowWarningWindow("Could not close one or more clients. Please try again");
                return false;
            }

            //Then we initialize the subordinates. The subordinates don't start yet, but wait for the master to start
            if (!InitializeClient(subordinates))
            {
                Log.LogError("Could not initialize one or more subordinate cameras");
                fMainWindowForm?.ShowWarningWindow("Could not restart one or more subordinate cameras. Please try again");
                return false;
            }

            //Lastly we initialize the main camera. This will also synchroniously start all other subordinates
            if (!InitializeClient(main))
            {
                Log.LogError("Could not initalize main camera");
                fMainWindowForm?.ShowWarningWindow("Could not restart main camera. Please try again");
                return false;
            }

            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].UpdateSocketState("");
                }
            }

            Log.LogInfo("Hardware Sync restart succesfull");
            return true;
        }

        public bool EnableTemporalSync()
        {
            if (nClientCount > 1)
            {
                Log.LogInfo("Enabling Hardware Sync");

                //Disable the Auto Exposure, as this could interfere with the temporal sync
                oSettings.bAutoExposureEnabled = false;
                oSettings.nExposureStep = -5;

                //Reflect this change in the settings UI
                fMainWindowForm.SetExposureControlsMode(true);

                SendSettings(); //Send settings to update the exposure

                if (SetTempSyncState(true) && RestartWithTemporalSyncPattern())
                {
                    Log.LogInfo("Enabling Hardware Sync succesfull");
                    bTempHwSyncEnabled = true;
                    return true;
                }

                else
                {
                    Log.LogInfo("Enabling Hardware Sync failed");

                    //Enabeling failed, we undo the Exposure Settings
                    oSettings.bAutoExposureEnabled = true;

                    fMainWindowForm.SetExposureControlsMode(false);

                    SendSettings(); //Send settings to update the exposure

                    return false;
                }
            }

            else
            {
                fMainWindowForm?.ShowInfoWindow("At least two client are needed for temporal hardware sync");
                return false;
            }
        }

        public bool DisableTemporalSync()
        {
            Log.LogInfo("Disabling Hardware Sync");

            if (SetTempSyncState(false) && RestartAllClients())
            {
                Log.LogInfo("Disabling Hardware Sync successfull");
                bTempHwSyncEnabled = false;
                return true;
            }

            else
            {
                Log.LogInfo("Disabling Hardware Sync failed");
                return false;
            }

        }

        /// <summary>
        /// Sets the temp sync settings for each device. Evaluates the jack states
        /// of the devices to determine how they should be configured.
        /// </summary>
        /// <returns>Returns true when the configuration was successfull, false on error</returns>
        public bool SetTempSyncState(bool syncEnabled)
        {
            Log.LogDebug("Setting temporal sync state");

            if (syncEnabled)
            {
                //Update the configurations of the kinect, so that we can be sure to get the latest hardware sync state
                if (!GetConfigurations(lClientSockets))
                    return false;

                lock (oClientSocketLock)
                {
                    int mainCount = 0;
                    int subordinateCount = 0;
                    int invalidCount = 0;

                    //First we check if the devices have a valid sync wiring
                    for (int i = 0; i < lClientSockets.Count; i++)
                    {
                        switch (lClientSockets[i].configuration.eHardwareSyncState)
                        {
                            case KinectConfiguration.SyncState.Main:
                                mainCount++;
                                break;
                            case KinectConfiguration.SyncState.Subordinate:
                                subordinateCount++;
                                break;
                            case KinectConfiguration.SyncState.Standalone:
                            case KinectConfiguration.SyncState.Unknown:
                                invalidCount++;
                                break;
                        }
                    }

                    if (mainCount != 1 || subordinateCount < 1 || invalidCount > 0)
                    {
                        //If not, we show a error message
                        Log.LogError("Temporal sync cables are not propery connected");
                        fMainWindowForm?.ShowWarningWindow("Temporal Sync cables not connected properly");
                        return false;
                    }
                }

                byte syncOffSetCounter = 0;

                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    if (lClientSockets[i].configuration.eHardwareSyncState == KinectConfiguration.SyncState.Subordinate)
                    {
                        syncOffSetCounter++;
                    }

                    KinectConfiguration newConfig = lClientSockets[i].configuration;
                    newConfig.eSoftwareSyncState = lClientSockets[i].configuration.eHardwareSyncState;
                    newConfig.syncOffset = syncOffSetCounter;

                    if (!SetAndConfirmConfig(lClientSockets[i], newConfig))
                    {
                        return false;
                    }
                }
            }

            else
            {
                if (!GetConfigurations(lClientSockets))
                    return false;

                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    KinectConfiguration newConfig = lClientSockets[i].configuration;
                    newConfig.eSoftwareSyncState = KinectConfiguration.SyncState.Standalone;
                    newConfig.syncOffset = 0;

                    if (!SetAndConfirmConfig(lClientSockets[i], newConfig))
                        return false;
                }
            }

            Log.LogDebug("Temporal Sync State set successfully");

            return true;
        }

        /// <summary>
        /// This methods checks if all connected clients are in a valid Hardware-Sync configuration
        /// </summary>
        /// <returns></returns>
        public bool CheckTempHwSyncValid()
        {
            int mainCount = 0;
            int subCount = 0;
            int standaloneCount = 0;

            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    switch (lClientSockets[i].configuration.eSoftwareSyncState)
                    {
                        case KinectConfiguration.SyncState.Main:
                            mainCount++;
                            break;
                        case KinectConfiguration.SyncState.Subordinate:
                            subCount++;
                            break;
                        case KinectConfiguration.SyncState.Standalone:
                        case KinectConfiguration.SyncState.Unknown:
                            standaloneCount++;
                            break;

                        default:
                            break;
                    }
                }
            }

            if (mainCount == 1 && standaloneCount == 0 && subCount > 0)
                return true;

            else
                return false;

        }


        /// <summary>
        /// Creates a unique take directory on the client and/or server
        /// </summary>
        /// <param name="takeName"></param>
        /// <returns> Returns the relative directory path for the server when the dir has been successfully created. Returns null when an error has occured on either the client or server </returns>
        public string CreateTakeDirectories(string takeName)
        {
            Log.LogDebug("Creating Take Directories");

            //First we get a unique take index
            int takeIndex = oSettings.GetNewTakeIndex(takeName);

            if (takeIndex == -1)
            {
                Log.LogError("Error reading takes.json");
                fMainWindowForm.ShowWarningWindow("Error reading takes.json. Please check or delete file");
                return null;
            }

            //TODO: Add date to string from Simple_Take_Management Branch
            takeName += "_" + takeIndex;

            string takePathClients = takeName + "\\";

            string takePathServer = "out\\" + takePathClients; //For the server, we also add the general output dir in which all recordings are stored. The client handles that themself


            //If we record pointclouds or export the extrinsics, we create a directory on the Server PC
            if (oSettings.eExportMode == KinectSettings.ExportMode.Pointcloud || oSettings.eExtrinsicsFormat != KinectSettings.ExtrinsicsStyle.None)
            {
                try
                {
                    DirectoryInfo di = Directory.CreateDirectory(takePathServer);
                }

                catch (Exception e)
                {
                    Log.LogError("Could not create take dir on server");
                    return null;
                }
            }

            //We create a directory on the client PC to store the bin file and timestamps
            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].SendCreateTakeDir(takePathClients);
                    //Wait just a tiny amount of time to avoid that the clients race each other on creating the dirs.
                    //Important if two clients are on the same pc
                    Thread.Sleep(10);
                }
            }


            //Wait for confirmation that the dir has been created by the clients
            bool allConfirmedDir = false;
            while (!allConfirmedDir)
            {
                allConfirmedDir = true;

                lock (oClientSocketLock)
                {
                    for (int i = 0; i < lClientSockets.Count; i++)
                    {
                        if (!lClientSockets[i].bDirCreationConfirmed)
                        {
                            allConfirmedDir = false;
                            break;
                        }
                    }
                }
            }

            //Check if there were errors during the dir creation
            bool clientDirError = false;
            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    if (lClientSockets[i].bDirCreationError)
                    {
                        Log.LogError("Client number: " + (i+1) + "could not create take dir" );
                        clientDirError = true;
                        break;
                    }
                }

                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].bDirCreationConfirmed = false;
                    lClientSockets[i].bDirCreationError = false;
                }
            }

            if (clientDirError)
                return null;

            Log.LogDebug("Take dir created successfully");

            return takePathServer;
        }

        public bool GetStoredFrame(List<List<byte>> lFramesRGB, List<List<Single>> lFramesVerts)
        {
            bool bNoMoreStoredFrames;
            lFramesRGB.Clear();
            lFramesVerts.Clear();

            lock (oFrameRequestLock)
            {
                //Request frames
                lock (oClientSocketLock)
                {
                    for (int i = 0; i < lClientSockets.Count; i++)
                        lClientSockets[i].RequestStoredFrame();
                }

                //Wait till frames received
                bool allGathered = false;
                bNoMoreStoredFrames = false;
                while (!allGathered)
                {
                    allGathered = true;
                    lock (oClientSocketLock)
                    {
                        for (int i = 0; i < lClientSockets.Count; i++)
                        {
                            if (!lClientSockets[i].bStoredFrameReceived)
                            {
                                allGathered = false;
                                break;
                            }

                            if (lClientSockets[i].bNoMoreStoredFrames)
                                bNoMoreStoredFrames = true;
                        }
                    }
                }

                //Store received frames
                lock (oClientSocketLock)
                {
                    for (int i = 0; i < lClientSockets.Count; i++)
                    {
                        lFramesRGB.Add(new List<byte>(lClientSockets[i].lFrameRGB));
                        lFramesVerts.Add(new List<Single>(lClientSockets[i].lFrameVerts));
                    }
                }
            }

            if (bNoMoreStoredFrames)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Gets the configurations from the kinects and stores them in their socket
        /// </summary>
        public bool GetConfigurations(List<KinectSocket> kinects)
        {

            lock (oClientSocketLock)
            {
                for (int i = 0; i < kinects.Count; i++)
                {
                    kinects[i].RequestConfiguration();
                    Log.LogDebug("Getting configuration from client number: " + (i+1));
                }
            }

            bool recievedData = false;
            Stopwatch timer = new Stopwatch();
            timer.Start();

            while (!recievedData && timer.Elapsed.TotalSeconds < networkTimeout)
            {
                recievedData = true;

                for (int i = 0; i < kinects.Count; i++)
                {
                    if (!kinects[i].bConfigurationReceived)
                        recievedData = false;
                }
            }

            timer.Stop();

            lock (oClientSocketLock)
            {
                for (int i = 0; i < kinects.Count; i++)
                {
                    kinects[i].UpdateSocketState("");

                    if (!kinects[i].bConfigurationReceived)
                    {
                        Log.LogError("Could not get configuration from client number: " + (i+1));
                        fMainWindowForm?.ShowWarningWindow("Could not update configuration on one client, please check your network");
                        return false;
                    }
                }
            }

            return true;

        }

        public void GetLatestFrame(List<List<byte>> lFramesRGB, List<List<Single>> lFramesVerts)
        {
            lFramesRGB.Clear();
            lFramesVerts.Clear();

            lock (oFrameRequestLock)
            {
                //Request frames
                lock (oClientSocketLock)
                {
                    for (int i = 0; i < lClientSockets.Count; i++)
                        lClientSockets[i].RequestLastFrame();
                }

                //Wait till frames received
                bool allGathered = false;

                while (!allGathered)
                {
                    allGathered = true;

                    lock (oClientSocketLock)
                    {
                        for (int i = 0; i < lClientSockets.Count; i++)
                        {
                            if (!lClientSockets[i].bLatestFrameReceived)
                            {
                                allGathered = false;
                                break;
                            }
                        }
                    }

                }

                //Store received frames
                lock (oClientSocketLock)
                {
                    for (int i = 0; i < lClientSockets.Count; i++)
                    {
                        lFramesRGB.Add(new List<byte>(lClientSockets[i].lFrameRGB));
                        lFramesVerts.Add(new List<Single>(lClientSockets[i].lFrameVerts));
                    }
                }

            }
        }

        public bool GetTimestampLists()
        {
            Log.LogDebug("Getting timestamp lists from clients");

            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].RequestTimestamps();
                }
            }

            bool allGathered = false;

            while (!allGathered)
            {
                allGathered = true;

                lock (oClientSocketLock)
                {
                    for (int i = 0; i < lClientSockets.Count; i++)
                    {
                        if (!lClientSockets[i].bTimeStampsRecieved)
                        {
                            allGathered = false;
                            break;
                        }
                    }
                }
            }

            //TODO: What if the server doesn't receive a timestamp list?
            if (allGathered)
                return true;

            else
            {
                return false;
            }

        }

        public void ClearStoredFrames()
        {
            Log.LogDebug("Clearing stored frame on devices");

            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].ClearStoredFrames();
                }
            }
        }


        public bool CreatePostSyncList()
        {
            Log.LogDebug("Creating Post sync list");

            List<ClientSyncData> allDeviceSyncData = new List<ClientSyncData>();

            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    allDeviceSyncData.Add(new ClientSyncData(lClientSockets[i].lFrameNumbers, lClientSockets[i].lTimeStamps, i));
                }
            }

            List<ClientSyncData> postSyncDeviceData = PostSync.GenerateSyncList(allDeviceSyncData);

            if(postSyncDeviceData == null)
            {
                Log.LogError("Could not generate Post sync List");
                return false;
            }

            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].postSyncedFrames = postSyncDeviceData[i];
                }
            }

            Log.LogDebug("Post sync list generated!");
            return true;
        }

       
        /// <summary>
        /// Sends the postsync-List to the clients and waits until all clients have confirmed that they renumbered their files correctly
        /// </summary>
        /// <returns></returns>
        public bool ReorderSyncFramesOnClient()
        {
            Log.LogDebug("Reordering frames on client");

            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].SendPostSyncList();
                }
            }

            bool postSyncConfirmed = false;

            while (!postSyncConfirmed)
            {
                postSyncConfirmed = true;

                lock (oClientSocketLock)
                {
                    for (int i = 0; i < lClientSockets.Count; i++)
                    {
                        if (!lClientSockets[i].bPostSyncConfirmed)
                            postSyncConfirmed = false;
                    }
                }                
            }
            
            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    if (lClientSockets[i].bPostSyncError)
                    {
                        Log.LogError("Error while reordering frames on client number: " + (i+1));
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Tells the client to prepare themselves for recording. Waits
        /// until all clients are ready
        /// </summary>
        public void SendAndConfirmPreRecordProcess()
        {
            Log.LogDebug("Preparing clients for capture");

            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].SendPreRecordProcessStart();
                }
            }

            bool preProcessConfirmed = false;

            while (!preProcessConfirmed)
            {
                preProcessConfirmed = true;

                lock (oClientSocketLock)
                {
                    for (int i = 0; i < lClientSockets.Count; i++)
                    {
                        if (!lClientSockets[i].bPreRecordProcessConfirmed)
                            preProcessConfirmed = false;
                    }
                }
            }
        }

        /// <summary>
        /// Lets the client process data right after recording.
        /// Waits until all clients are finished
        /// </summary>
        public void SendAndConfirmPostRecordProcess()
        {
            Log.LogDebug("Stopping capture on client and starting post capture process");

            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].SendPostRecordProcessStart();
                }
            }

            bool postProcessConfirmation = false;

            while (!postProcessConfirmation)
            {
                postProcessConfirmation = true;

                lock (oClientSocketLock)
                {
                    for (int i = 0; i < lClientSockets.Count; i++)
                    {
                        if (!lClientSockets[i].bPostRecordProcessConfirmed)
                            postProcessConfirmation = false;
                    }
                }
            }
        }

        public void SendCaptureFramesStart()
        {
            Log.LogDebug("Start capturing frames on clients");

            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].SendCaptureFramesStart();
                }
            }
        }

        public void SendCaptureFramesStop()
        {
            Log.LogDebug("Stop capturing frames on clients");

            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].SendCaptureFramesStop();
                }
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            if (listener == null || !bServerRunning)
            {
                return;
            }
            Socket newSocket = listener.EndAccept(ar);

            // Signal main thread to go ahead
            allDone.Set();

            KinectSocket newClient = new KinectSocket(newSocket);

            //we do not want to add new clients while a frame is being requested
            lock (oFrameRequestLock)
            {
                lock (oClientSocketLock)
                {
                    Log.LogInfo("New client connected!");
                    lClientSockets.Add(newClient);
                    newClient.eChanged += new SocketChangedHandler(SocketListChanged);

                    if (eSocketListChanged != null)
                    {
                        eSocketListChanged(lClientSockets);
                    }
                }

                //Sending the initial configuration and settings to the device.
                newClient.UpdateSocketState("Configuring...");

                if (!GetConfigurations(new List<KinectSocket>() { newClient }))
                {
                    FatalClientError("Client could not be configured, please reconnect", "Could not configure client");
                    return;
                }

                KinectConfiguration newConfig = newClient.configuration;

                //Find a global device index that is unique
                bool found = false;
                int uniqueIndex = 0;
                while (!found)
                {
                    found = true;
                    for (int i = 0; i < lClientSockets.Count - 1; i++)
                    {
                        if (uniqueIndex == lClientSockets[i].configuration.globalDeviceIndex)
                        {
                            found = false;
                            uniqueIndex++;
                        }
                    }
                }

                Log.LogInfo("New client unique device index: " + uniqueIndex);

                newConfig.globalDeviceIndex = (byte)uniqueIndex;

                if (!SetAndConfirmConfig(newClient, newConfig))
                {
                    FatalClientError("Client could not be configured, please reconnect", "Could not configure client");
                    return;
                }

                newClient.SendSettings(oSettings);

                bool restartAsStandalone = false;

                if (!bTempHwSyncEnabled && newConfig.eSoftwareSyncState != KinectConfiguration.SyncState.Standalone)
                    restartAsStandalone = true;

                if (SetTempSyncState(bTempHwSyncEnabled))
                {
                    if (restartAsStandalone)
                    {
                        if(!CloseClient(newClient) || !InitializeClient(newClient))
                        {
                            FatalClientError("Could not restart new client to match temporal sync settings, please reconnect", "Could not close or open new client");
                            return;
                        }
                    }

                    else if (bTempHwSyncEnabled)
                    {
                        Log.LogInfo("New client connected while temporal sync was active. Restarting all clients");

                        if (RestartWithTemporalSyncPattern())
                        {
                            newClient.UpdateSocketState("");
                        }
                    }
                }

                else
                {
                    FatalClientError("Could not send Temp Sync State on client, please reconnect", "Could not set temp sync state on new client");
                    return;
                }
            }
        }

        private void FatalClientError(string userMessage, string logMessage)
        {
            Log.LogError(logMessage);
            DisconnectClient(lClientSockets[lClientSockets.Count - 1]);
            fMainWindowForm.ShowWarningWindow(userMessage);
        }

        private void DisconnectClient(KinectSocket client)
        {
            Log.LogInfo("Client disconnected");

            lock (oClientSocketLock)
            {
                client.DisconnectSocket();
                lClientSockets.Remove(client);
                SocketListChanged();
            }
        }

        private void ListeningWorker()
        {
            while (bServerRunning)
            {
                allDone.Reset();
                try
                {
                    oServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), oServerSocket);
                }
                catch (SocketException e)
                {
                    Log.LogError("Error establishing connection to client socket");
                }
                allDone.WaitOne();
            }

            if (eSocketListChanged != null)
            {
                eSocketListChanged(lClientSockets);
            }
        }

        private void ReceivingWorker()
        {
            System.Timers.Timer checkConnectionTimer = new System.Timers.Timer();
            checkConnectionTimer.Interval = 1000;

            checkConnectionTimer.Elapsed += delegate (object sender, System.Timers.ElapsedEventArgs e)
            {
                lock (oClientSocketLock)
                {
                    for (int i = 0; i < lClientSockets.Count; i++)
                    {
                        if (!lClientSockets[i].SocketConnected())
                        {
                            lClientSockets.RemoveAt(i);
                            if (eSocketListChanged != null)
                            {
                                eSocketListChanged(lClientSockets);
                            }
                            continue;
                        }
                    }
                }
            };

            checkConnectionTimer.Start();

            while (bServerRunning)
            {
                lock (oClientSocketLock)
                {
                    for (int i = 0; i < lClientSockets.Count; i++)
                    {
                        byte[] buffer = lClientSockets[i].Receive(1);

                        while (buffer.Length != 0)
                        {
                            Log.LogTrace("Received message from client: " + buffer[0]);

                            if (buffer[0] == (byte)IncomingMessageType.MSG_CONFIRM_CAPTURED)
                            {
                                lClientSockets[i].bFrameCaptured = true;
                            }
                            else if (buffer[0] == (byte)IncomingMessageType.MSG_CONFIRM_CALIBRATED)
                            {
                                lClientSockets[i].ReceiveCalibrationData();
                            }
                            //stored frame
                            else if (buffer[0] == (byte)IncomingMessageType.MSG_STORED_FRAME)
                            {
                                lClientSockets[i].ReceiveFrame();
                                lClientSockets[i].bStoredFrameReceived = true;
                            }
                            //last frame
                            else if (buffer[0] == (byte)IncomingMessageType.MSG_LAST_FRAME)
                            {
                                lClientSockets[i].ReceiveFrame();
                                lClientSockets[i].bLatestFrameReceived = true;
                            }

                            else if (buffer[0] == (byte)IncomingMessageType.MSG_CONFIGURATION)
                            {
                                lClientSockets[i].RecieveConfiguration();
                            }

                            else if (buffer[0] == (byte)IncomingMessageType.MSG_CONFIRM_CAMERA_CLOSED)
                            {
                                lClientSockets[i].ReceiveCameraClosedConfirmation();
                            }

                            else if (buffer[0] == (byte)IncomingMessageType.MSG_CONFIRM_CAMERA_INIT)
                            {
                                lClientSockets[i].ReceiveCameraInitializedConfirmation();
                            }

                            else if (buffer[0] == (byte)IncomingMessageType.MSG_CONFIRM_DIR_CREATION)
                            {
                                lClientSockets[i].ReceiveDirConfirmation();
                            }

                            else if (buffer[0] == (byte)IncomingMessageType.MSG_SEND_TIMESTAMP_LIST)
                            {
                                lClientSockets[i].ReceiveTimestampList();
                            }

                            else if (buffer[0] == (byte)IncomingMessageType.MSG_CONFIRM_POSTSYNCED)
                            {
                                lClientSockets[i].ReceivePostSyncConfirmation();
                            }

                            else if (buffer[0] == (byte)IncomingMessageType.MSG_CONFIRM_PRE_RECORD_PROCESS)
                            {
                                lClientSockets[i].ReceivePreRecordProcessConfirmation();
                            }

                            else if (buffer[0] == (byte)IncomingMessageType.MSG_CONFIRM_POST_RECORD_PROCESS)
                            {
                                lClientSockets[i].ReceivePostRecordProcessConfirmation();
                            }

                            buffer = lClientSockets[i].Receive(1);
                        }
                    }
                }

                Thread.Sleep(10);
            }

            checkConnectionTimer.Stop();
        }
    }
}
