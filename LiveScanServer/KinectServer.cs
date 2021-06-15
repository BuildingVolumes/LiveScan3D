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

namespace KinectServer
{
    public delegate void SocketListChangedHandler(List<KinectSocket> list);
    public class KinectServer
    {
        public event SocketListChangedHandler eSocketListChanged;

        bool bServerRunning = false;
        bool bWaitForSubToStart = false;
        ManualResetEvent allDone = new ManualResetEvent(false);

        //This lock prevents the user from enabeling/disabling the Temp Sync State while the cameras are in transition to another state.
        //When starting the server, all devices are already initialized, as the LiveScanClient can only connect with an initialized device
        bool allDevicesInitialized = true; 

        KinectSettings oSettings;
        public SettingsForm fSettingsForm;
        Dictionary<int, KinectConfigurationForm> kinectSettingsForms;
        public MainWindowForm fMainWindowForm;

        Socket oServerSocket;
        Thread listeningThread;
        Thread receivingThread;
        List<KinectSocket> lClientSockets = new List<KinectSocket>();
        object oClientSocketLock = new object();
        object oFrameRequestLock = new object();

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
            if(kinectSettingsForms.ContainsKey(id))
            {
                kinectSettingsForms[id] = form;
            }
            else
            {
                kinectSettingsForms.Add(id, form);
            }
        }
        public KinectSocket GetKinectSocketByIndex(int socketIndex)
        {
            return lClientSockets[socketIndex];
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
            if(kinectSettingsForms.TryGetValue(id, out var value))
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

        /// <summary>
        /// Request the device Sync state, so that we know which Device is Master and which are Subordinates before starting them
        /// </summary>
        public void RequestDeviceSyncState()
        {
            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].RequestTempSyncState();
                }
            }
        }

        /// <summary>
        /// When a client has send its Device Sync State, we try to Set the Client Sync State
        /// </summary>
        public void SendTemporalSyncData()
        {
            lock (oClientSocketLock)
            {
                int masterCount = 0;
                int subordinateCount = 0;

                //First we check if we have recieved the Device Sync State of all Devices
                //If not, we return and the next client who confirms their state starts this function again
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    if(lClientSockets[i].currentDeviceTempSyncState == SyncState.Master)
                    {
                        masterCount++;
                    }

                    if (lClientSockets[i].currentDeviceTempSyncState == SyncState.Standalone)
                    {
                        subordinateCount++;
                    }

                    if (lClientSockets[i].currentDeviceTempSyncState == SyncState.Unknown)
                    {
                        return;
                    }
                }

                //On a second step we check if we have exactly one master and at least one subordinate

                if(masterCount != 1 || subordinateCount < 1)
                {
                    //If not, we show a error message and disable the temporal sync

                    fMainWindowForm?.SetStatusBarOnTimer("Temporal Sync cables not connected properly", 5000);
                    fSettingsForm?.ActivateTempSyncEnableButton();
                    return;
                }
                

                allDevicesInitialized = false;

                byte syncOffSetCounter = 0;

                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    if(lClientSockets[i].currentDeviceTempSyncState == SyncState.Subordinate)
                    {
                        syncOffSetCounter++;
                    }

                    lClientSockets[i].SendTemporalSyncStatus(true, syncOffSetCounter);

                }

                bWaitForSubToStart = true;
            }
        }


        /// <summary>
        /// Sets all clients as standalone
        /// </summary>
        public void DisableTemporalSync()
        {
            allDevicesInitialized = false;

            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].SendTemporalSyncStatus(false, 0);
                }
            }
        }


        public void ConfirmTemporalSyncDisabled()
        {
            if (bWaitForSubToStart)
                return;

            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    if (lClientSockets[i].currentClientTempSyncState != SyncState.Standalone)
                    {
                        return;
                    }
                }
            }

            allDevicesInitialized = true;
        }

        /// <summary>
        /// Called when a sub client has started. This checks if all sub clients have already started. If yes, we initialize the master
        /// </summary>
        public void CheckForMasterStart()
        {
            if (!bWaitForSubToStart)
            {
                return;
            }

            bool allSubsStarted = true;

            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    if (!lClientSockets[i].bSubStarted && lClientSockets[i].currentClientTempSyncState == SyncState.Subordinate)
                    {
                        allSubsStarted = false;
                        break;
                    }
                }

                bWaitForSubToStart = false;

                if (allSubsStarted)
                {
                    for (int i = 0; i < lClientSockets.Count; i++)
                    {
                        if(lClientSockets[i].currentDeviceTempSyncState == SyncState.Master)
                        {
                            lClientSockets[i].SendMasterInitialize();
                            return;
                        }
                    }
                }
            }           
        }

        /// <summary>
        /// Tells the server that it is now ok to start recieving user changes again
        /// </summary>
        public void MasterSuccessfullyRestarted()
        {
            allDevicesInitialized = true; 
        }

        public bool GetAllDevicesInitialized()
        {
            return allDevicesInitialized;
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

            string takePathServer = "out\\" + takePathClients; //For the server, we also add the general output dir in which all recordings are stored. The client handles that himself


            //If we record pointclouds or export the extrinsics, we create a directory on the Server PC
            if (oSettings.eExportMode == KinectSettings.ExportMode.Pointcloud || oSettings.eExtrinsicsFormat != KinectSettings.ExtrinsicsStyle.None)
            {             
                try
                {
                    DirectoryInfo di = Directory.CreateDirectory(takePathServer);
                }

                catch(Exception e)
                {
                    return null;
                }
            }

            //TODO: When exporting intrinsics, also create Dir
            //When storing raw frames or intrinsics on the client, we create a directory on the client PC
            if(oSettings.eExportMode == KinectSettings.ExportMode.RawFrames)
            {
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
            }

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

        public void GetLatestFrame(List<List<byte>> lFramesRGB, List<List<Single>> lFramesVerts, List<List<Body>> lFramesBody)
        {
            lFramesRGB.Clear();
            lFramesVerts.Clear();
            lFramesBody.Clear();

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
                        lFramesBody.Add(new List<Body>(lClientSockets[i].lBodies));
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

        private void AcceptCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            if(listener == null || !bServerRunning)
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
                    lClientSockets[lClientSockets.Count - 1].SendSettings(oSettings);
                    lClientSockets[lClientSockets.Count - 1].eChanged += new SocketChangedHandler(SocketListChanged);
                    lClientSockets[lClientSockets.Count - 1].eSubInitialized += new SubOrdinateInitialized(CheckForMasterStart);
                    lClientSockets[lClientSockets.Count - 1].eMasterRestart += new MasterRestarted(MasterSuccessfullyRestarted);
                    lClientSockets[lClientSockets.Count - 1].eSyncJackstate += new RecievedSyncJackState(SendTemporalSyncData);
                    lClientSockets[lClientSockets.Count - 1].eStandAloneInitialized += new StandAloneInitialized(ConfirmTemporalSyncDisabled);

                    if (eSocketListChanged != null)
                    {
                        eSocketListChanged(lClientSockets);
                    }
                }
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

            checkConnectionTimer.Elapsed += delegate(object sender, System.Timers.ElapsedEventArgs e)
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

                            else if (buffer[0] == (byte)IncomingMessageType.MSG_CONFIRM_TEMP_SYNC_STATUS)
                            {
                                lClientSockets[i].ReceiveTemporalSyncStatus();
                            }

                            else if(buffer[0] == (byte)IncomingMessageType.MSG_CONFIRM_MASTER_RESTART)
                            {
                                lClientSockets[i].RecieveMasterHasRestarted();
                            }

                            else if(buffer[0] == (byte)IncomingMessageType.MSG_SYNC_JACK_STATE)
                            {
                                lClientSockets[i].RecieveDeviceSyncState();
                            }
                            else if(buffer[0] == (byte)IncomingMessageType.MSG_CONFIGURATION)
                            {
                                lClientSockets[i].RecieveConfiguration();
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
