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

        public bool bTempSyncEnabled = false;
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

            if (settings.eExportMode != KinectSettings.ExportMode.Pointcloud)
                bPointCloudMode = false;

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

        public void StartServer()
        {
            if (!bServerRunning)
            {
                oServerSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                oServerSocket.Blocking = false;

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 48001);
                oServerSocket.Bind(endPoint);
                oServerSocket.Listen(10);

                bServerRunning = true;
                listeningThread = new Thread(this.ListeningWorker);
                listeningThread.Start();
                receivingThread = new Thread(this.ReceivingWorker);
                receivingThread.Start();
            }
        }

        public void StopServer()
        {
            if (bServerRunning)
            {
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
            lock (oClientSocketLock)
            {

                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].SendCalibrationData();
                }
            }
        }

        //TODO: This code isn't needed anymore, as better functions to set the configuration already exist.
        public void SendConfigurationToSocket(KinectSocket socket, KinectConfiguration newConfig)
        {
            var gotConfigurations = GetConfigurations(new List<KinectSocket>() { socket });
            if (!gotConfigurations)
            {
                //error
                return;
            }

            var oldConfig = socket.configuration;
            bool needsRestart = KinectConfiguration.RequiresRestartAfterChange(oldConfig, newConfig);
            var confirmed = SetAndConfirmConfig(socket, newConfig);

            if (confirmed)
            {
                if (needsRestart)
                {
                    if (bTempSyncEnabled)
                    {
                        RestartWithTemporalSyncPattern();
                    }
                    else
                    {
                        RestartClients(new List<KinectSocket>() { socket });//todo: method overload for single socket.
                    }
                }
            }
            else
            {
                //error in getting configuration 
            }
        }

        public bool SetAndConfirmConfig(KinectSocket socket, KinectConfiguration newConfig)
        {
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
                fMainWindowForm?.SetStatusBarOnTimer("Could not confirm configuration file, please check your network", 5000);
                return false;
            }

            return true;
        }


        /// <summary>
        /// Restarts all clients in the list 
        /// </summary>
        /// <returns>Returns true on successfull restart, false on restart error</returns>
        public bool RestartClients(List<KinectSocket> clients)
        {
            lock (oClientSocketLock)
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    clients[i].ReinitializeAndConfirm();
                    clients[i].UpdateSocketState(" Restarting... ");
                }
            }

            bool recievedData = false;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            while (!recievedData && timer.Elapsed.TotalSeconds < networkTimeout)
            {
                recievedData = true;

                lock (oClientSocketLock)
                {
                    for (int i = 0; i < clients.Count; i++)
                    {
                        if (!clients[i].bReinitialized)
                            recievedData = false;
                    }
                }


            }

            timer.Stop();

            bool restartSuccess = true;

            for (int i = 0; i < clients.Count; i++)
            {
                if (!clients[i].bReinitialized || clients[i].bReinizializationError)
                {
                    fMainWindowForm?.SetStatusBarOnTimer("Could not restart a kinect. Please connect and try again:", 5000);
                    restartSuccess = false;
                    clients[i].UpdateSocketState(" Restart failed! ");
                }

                else
                {
                    clients[i].UpdateSocketState("");
                }
            }

            if (!restartSuccess)
                return false;

            return true;
        }

        public bool RestartAllClients()
        {
            return RestartClients(lClientSockets);
        }

        /// <summary>
        /// Restarts all clients in a specific order that allows temporal sync to work.
        /// (Restarts all subs first, and then the master)
        /// </summary>
        /// <returns>Returns true on successfull restart, false on restart error</returns>
        public bool RestartWithTemporalSyncPattern()
        {
            List<KinectSocket> subordinates = new List<KinectSocket>();
            List<KinectSocket> main = new List<KinectSocket>();

            for (int i = 0; i < lClientSockets.Count; i++)
            {
                if (lClientSockets[i].configuration.eSoftwareSyncState == KinectConfiguration.SyncState.Subordinate)
                    subordinates.Add(lClientSockets[i]);

                if (lClientSockets[i].configuration.eSoftwareSyncState == KinectConfiguration.SyncState.Main)
                    main.Add(lClientSockets[i]);
            }

            if (!RestartClients(subordinates))
            {
                fMainWindowForm?.SetStatusBarOnTimer("Could not restart one ore more subordinates. Please try again:", 5000);
                return false;
            }

            if (!RestartClients(main))
            {
                fMainWindowForm?.SetStatusBarOnTimer("Could not restart main. Please try again:", 5000);
                return false;
            }


            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].UpdateSocketState("");
                }
            }

            return true;
        }

        public bool EnableTemporalSync()
        {
            if (nClientCount > 1)
            {
                //Disable the Auto Exposure, as this could interfere with the temporal sync
                oSettings.bAutoExposureEnabled = false;
                oSettings.nExposureStep = -5;

                if (fSettingsForm != null) //Reflect this change in the settings UI
                    fSettingsForm.SetExposureControlsToManual();

                SendSettings(); //Send settings to update the exposure

                if (SetTempSyncState(true) && RestartWithTemporalSyncPattern())
                {
                    bTempSyncEnabled = true;
                    return true;
                }

                else
                    return false;
            }

            else
                return false;
        }

        public bool DisableTemporalSync()
        {
            if (SetTempSyncState(false) && RestartAllClients())
            {
                bTempSyncEnabled = false;
                return true;
            }

            else
                return false;
        }

        /// <summary>
        /// Sets the temp sync settings for each device. Evaluates the jack states
        /// of the devices to determine how they should be configured.
        /// </summary>
        /// <returns>Returns true when the configuration was successfull, false on error</returns>
        public bool SetTempSyncState(bool syncEnabled)
        {
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
                        fMainWindowForm?.SetStatusBarOnTimer("Temporal Sync cables not connected properly", 5000);
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

            return true;

        }


        /// <summary>
        /// Creates a unique take directory on the client and/or server
        /// </summary>
        /// <param name="takeName"></param>
        /// <returns> Returns the relative directory path for the server when the dir has been successfully created. Returns null when an error has occured on either the client or server </returns>
        public string CreateTakeDirectories(string takeName)
        {
            //First we get a unique take index
            int takeIndex = oSettings.GetNewTakeIndex(takeName);

            if (takeIndex == -1)
            {
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
                        fMainWindowForm?.SetStatusBarOnTimer("Could not update configuration file, please check your network", 5000);
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

        public void ClearStoredFrames()
        {
            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].ClearStoredFrames();
                }
            }
        }



        public void SendRecordingStartSignal()
        {
            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].SendRecordingStart();
                }
            }
        }



        public void SendRecordingStopSignal()
        {
            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].SendRecordingStop();
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

            //we do not want to add new clients while a frame is being requested
            lock (oFrameRequestLock)
            {
                lock (oClientSocketLock)
                {
                    lClientSockets.Add(new KinectSocket(newSocket));
                    lClientSockets[lClientSockets.Count - 1].eChanged += new SocketChangedHandler(SocketListChanged);

                    if (eSocketListChanged != null)
                    {
                        eSocketListChanged(lClientSockets);
                    }
                }

                //Sending the initial configuration and settings to the device.
                lClientSockets[lClientSockets.Count - 1].UpdateSocketState("Configuring...");

                if (!GetConfigurations(new List<KinectSocket>() { lClientSockets[lClientSockets.Count - 1] }))
                {
                    DisconnectClient(lClientSockets[lClientSockets.Count - 1]);
                    fMainWindowForm.SetStatusBarOnTimer("Client couldn't be configured, please reconnect", 5000);
                    return;
                }

                KinectConfiguration newConfig = lClientSockets[lClientSockets.Count - 1].configuration;


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

                newConfig.globalDeviceIndex = (byte)uniqueIndex;

                if (!SetAndConfirmConfig(lClientSockets[lClientSockets.Count - 1], newConfig))
                {
                    DisconnectClient(lClientSockets[lClientSockets.Count - 1]);
                    fMainWindowForm.SetStatusBarOnTimer("Client couldn't be configured, please reconnect", 5000);
                    return;
                }

                lClientSockets[lClientSockets.Count - 1].SendSettings(oSettings);


                if (bTempSyncEnabled)
                {
                    if (RestartWithTemporalSyncPattern())
                    {
                        lClientSockets[lClientSockets.Count - 1].UpdateSocketState("");
                    }
                }
            }
        }

        private void DisconnectClient(KinectSocket client)
        {
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
                    Console.WriteLine(e.ToString());
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
                            if (buffer[0] == (byte)IncomingMessageType.MSG_CONFIRM_CAPTURED)
                            {
                                lClientSockets[i].bFrameCaptured = true;
                                Console.WriteLine("Confirmed Frame Client: " + lClientSockets[i].configuration.SerialNumber + "  : " + DateTime.Now.Millisecond);
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

                            else if (buffer[0] == (byte)IncomingMessageType.MSG_CONFIRM_RESTART)
                            {
                                lClientSockets[i].RecieveRestartConfirmation();
                            }



                            else if (buffer[0] == (byte)IncomingMessageType.MSG_CONFIRM_DIR_CREATION)
                            {
                                lClientSockets[i].RecieveDirConfirmation();
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
