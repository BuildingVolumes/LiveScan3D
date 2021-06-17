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
using System.Diagnostics;

namespace KinectServer
{
    public delegate void SocketListChangedHandler(List<KinectSocket> list);
    public class KinectServer
    {
        Socket oServerSocket;
        bool bServerRunning = false;
        KinectSettings oSettings;
        SettingsForm fSettingsForm;
        Dictionary<int, KinectConfigurationForm> kinectSettingsForms;
        MainWindowForm fMainWindowForm;
        object oClientSocketLock = new object();
        object oFrameRequestLock = new object();
        const float networkTimeout = 5f;

        List<KinectSocket> lClientSockets = new List<KinectSocket>();
        public event SocketListChangedHandler eSocketListChanged;

        public bool bTempSyncEnabled = false;

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
                Thread listeningThread = new Thread(this.ListeningWorker);
                listeningThread.Start();
                Thread receivingThread = new Thread(this.ReceivingWorker);
                receivingThread.Start();
            }
        }

        public void StopServer()
        {
            if (bServerRunning)
            {
                bServerRunning = false;

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

            return false;
        }


        /// <summary>
        /// Restarts all clients in the list and updates their configuration
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

            if (!GetAllConfigurations())
            {
                return false;
            }

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
        public bool RestartTemporalSync()
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
                fMainWindowForm?.SetStatusBarOnTimer("Could not restart main. Please connect and try again:", 5000);
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
                if (!GetAllConfigurations())
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
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    KinectConfiguration newConfig = lClientSockets[i].configuration;
                    newConfig.eSoftwareSyncState = KinectConfiguration.SyncState.Standalone;

                    if (!SetAndConfirmConfig(lClientSockets[i], newConfig))
                        return false;
                }
            }

            return true;

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
        public bool GetAllConfigurations()
        {
            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].RequestConfiguration();
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
                    for (int i = 0; i < lClientSockets.Count; i++)
                    {
                        if (!lClientSockets[i].bConfigurationReceived)
                            recievedData = false;
                    }
                }
            }

            timer.Stop();

            lock (oClientSocketLock)
            {
                for (int i = 0; i < lClientSockets.Count; i++)
                {
                    lClientSockets[i].UpdateSocketState("");

                    if (!lClientSockets[i].bConfigurationReceived)
                    {
                        fMainWindowForm?.SetStatusBarOnTimer("Could not update configuration file, please check your network", 5000);
                        return false;
                    }
                }
            }


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

        private void ListeningWorker()
        {
            while (bServerRunning)
            {
                try
                {
                    Socket newClient = oServerSocket.Accept();

                    //we do not want to add new clients while a frame is being requested
                    lock (oFrameRequestLock)
                    {
                        lock (oClientSocketLock)
                        {
                            lClientSockets.Add(new KinectSocket(newClient));
                            lClientSockets[lClientSockets.Count - 1].SendSettings(oSettings);
                            lClientSockets[lClientSockets.Count - 1].eChanged += new SocketChangedHandler(SocketListChanged);

                            if (eSocketListChanged != null)
                            {
                                eSocketListChanged(lClientSockets);
                            }
                        }

                        GetAllConfigurations();
                    }
                }
                catch (SocketException)
                {
                }
                System.Threading.Thread.Sleep(100);
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
