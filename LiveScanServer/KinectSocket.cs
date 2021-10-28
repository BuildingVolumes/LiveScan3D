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
using System.Net.Sockets;


namespace KinectServer
{
    public delegate void SocketChangedHandler();

    public class KinectSocket
    {
        Socket oSocket;
        byte[] byteToSend = new byte[1];
        public bool bFrameCaptured = false;
        public bool bLatestFrameReceived = false;
        public bool bStoredFrameReceived = false;
        public bool bNoMoreStoredFrames = true;
        public bool bConfigurationReceived = false;
        public bool bTimeStampsRecieved = false;


        public bool bCalibrated = false;
        public bool bReinitialized = false;
        public bool bReinizializationError = false;

        public bool bDirCreationConfirmed = false;
        public bool bDirCreationError = false;

        public KinectConfiguration configuration;

        //The pose of the sensor in the scene (used by the OpenGLWindow to show the sensor)
        public AffineTransform oCameraPose = new AffineTransform();

        //The transform that maps the vertices in the sensor coordinate system to the world corrdinate system.
        public AffineTransform oWorldTransform = new AffineTransform();

        public string sSocketState;

        public List<byte> lFrameRGB = new List<byte>();
        public List<Single> lFrameVerts = new List<Single>();
        public List<Body> lBodies = new List<Body>();

        public List<ulong> lTimeStamps = new List<ulong>();
        public List<int> lFrameNumbers = new List<int>();
        public DeviceSyncData postSyncedFrames;

        public event SocketChangedHandler eChanged;

        public Action<KinectConfiguration> configurationUpdated;
        public KinectSocket(Socket clientSocket)
        {
            oSocket = clientSocket;
            UpdateSocketState("");
        }

        public void CaptureFrame()
        {
            Console.WriteLine("Capture Frame Client: " + configuration.SerialNumber + "  : " + DateTime.Now.Millisecond);
            bFrameCaptured = false;
            byteToSend[0] = (byte)OutgoingMessageType.MSG_CAPTURE_SINGLE_FRAME;
            SendByte();
        }
        public void Calibrate()
        {            
            bCalibrated = false;

            byteToSend[0] = (byte)OutgoingMessageType.MSG_CALIBRATE;
            SendByte();

            UpdateSocketState("");
        }

        public void RequestStoredFrame()
        {
            byteToSend[0] = (byte)OutgoingMessageType.MSG_REQUEST_STORED_FRAME;
            SendByte();
            bNoMoreStoredFrames = false;
            bStoredFrameReceived = false;
        }

        public void RequestConfiguration()
        {
            byteToSend[0] = (byte)OutgoingMessageType.MSG_REQUEST_CONFIGURATION;
            SendByte();
            bConfigurationReceived = false;
        }

        public void RequestLastFrame()
        {
            byteToSend[0] = (byte)OutgoingMessageType.MSG_REQUEST_LAST_FRAME;
            SendByte();
            bLatestFrameReceived = false;
        }

        public void RequestTimestamps()
        {
            lTimeStamps.Clear();
            lFrameNumbers.Clear();
            postSyncedFrames.frames.Clear();
            bTimeStampsRecieved = false;

            byteToSend[0] = (byte)OutgoingMessageType.MSG_REQUEST_TIMESTAMP_LIST;
            SendByte();

        }

        public void SendSettings(KinectSettings settings)
        {
            List<byte> lData = settings.ToByteList();

            //why is this doing this in a backwards order?
            byte[] bTemp = BitConverter.GetBytes(lData.Count);
            lData.InsertRange(0, bTemp);
            lData.Insert(0, (byte)OutgoingMessageType.MSG_RECEIVE_SETTINGS);

            if (SocketConnected())
                oSocket.Send(lData.ToArray());
        }

        public void SendConfiguration(KinectConfiguration newConfig)
        {
            byte[] data = newConfig.ToBytes();
            List<byte> message = new List<byte>() { (byte)OutgoingMessageType.MSG_SET_CONFIGURATION };
            message.InsertRange(1, data);

            if(SocketConnected())
            {
                oSocket.Send(message.ToArray());
            }

            configuration = newConfig;
        }

        public void SendCalibrationData()
        {
            int size = 1 + (9 + 3) * sizeof(float);
            byte[] data = new byte[size];
            int i = 0;

            data[i] = (byte)OutgoingMessageType.MSG_RECEIVE_CALIBRATION;//the MSG_ enums are copied from the client for ease. "Receive" is correct but only because of this convenience in keeping the utils classes matching. We may wish to rename the enums more appropriately, like "MSG_CALIBRATION".
            i++;

            Buffer.BlockCopy(oWorldTransform.R, 0, data, i, 9 * sizeof(float));
            i += 9 * sizeof(float);
            Buffer.BlockCopy(oWorldTransform.t, 0, data, i, 3 * sizeof(float));
            i += 3 * sizeof(float);

            if (SocketConnected())
                oSocket.Send(data);
        }

        /// <summary>
        /// Commands the client to create a Take directory on his device. The client then stores all of his take exports there
        /// </summary>
        /// <param name="dirName">Take folder name as ASCII String, not longer than 256 characters</param>
        public void SendCreateTakeDir(string dirName)
        {
            //Convert the string to an ASCII-Encoded byte array
            char[] chars = dirName.ToCharArray();
            List<byte> byteList = new List<byte>();

            for (int i = 0; i < chars.Length; i++)
            {
                //Ditch any non ascii-character
                if ((Int32)chars[i] < 128)
                {
                    byteList.Add((byte)chars[i]);
                }
            }

            byte[] listLength = BitConverter.GetBytes(byteList.Count);

            //Add our message as the first byte and add the length of this byte array
            byteList.Insert(0, (byte)OutgoingMessageType.MSG_CREATE_DIR);
            byteList.InsertRange(1, listLength);

            oSocket.Send(byteList.ToArray());
        }

        public void SendPostSyncList()
        {
            List<byte> byteList = new List<byte>();

            byteList.Add((byte)OutgoingMessageType.MSG_SEND_POSTSYNC_LIST);

            byteList.AddRange(BitConverter.GetBytes(postSyncedFrames.frames.Count));
            
            for (int i = 0; i < postSyncedFrames.frames.Count; i++)
            {
                byteList.AddRange(BitConverter.GetBytes(postSyncedFrames.frames[i].frameID));
            }

            for (int i = 0; i < postSyncedFrames.frames.Count; i++)
            {
                byteList.AddRange(BitConverter.GetBytes(postSyncedFrames.frames[i].syncedFrameID));
            }

            oSocket.Send(byteList.ToArray());
        }

        /// <summary>
        /// This doesn't actually start or stop a recording on a client, it just let's it know that we are in record mode
        /// </summary>
        public void SendRecordingStart()
        {
            byteToSend[0] = (byte)OutgoingMessageType.MSG_RECORDING_START;
            SendByte();
        }

        /// <summary>
        /// Let's the client know that record mode is now off
        /// </summary>
        public void SendRecordingStop()
        {
            byteToSend[0] = (byte)OutgoingMessageType.MSG_RECORDING_STOP;
            SendByte();
        }

        /// <summary>
        /// Sends the command to start capturing frames as fast as possible
        /// </summary>
        public void SendCaptureFramesStart()
        {
            byteToSend[0] = (byte)OutgoingMessageType.MSG_START_CAPTURING_FRAMES;
            SendByte();
        }

        /// <summary>
        /// Sends the command to stop capturing frames.
        /// </summary>
        public void SendCaptureFramesStop()
        {
            byteToSend[0] = (byte)OutgoingMessageType.MSG_STOP_CAPTURING_FRAMES;
            SendByte();
        }

        public void ClearStoredFrames()
        {
            byteToSend[0] = (byte)OutgoingMessageType.MSG_CLEAR_STORED_FRAMES;
            SendByte();
        }

        /// <summary>
        /// Send a signal to Reinitialize the device with its current settings and confirm it to the server.
        /// </summary>
        public void ReinitializeAndConfirm()
        {
            bReinitialized = false;
            bReinizializationError = false;
            byteToSend[0] = (byte)OutgoingMessageType.MSG_REINITIALIZE_WITH_CURRENT_SETTINGS;
            SendByte();
        }

        public void ReceiveCalibrationData()
        {
            bCalibrated = true;

            byte[] buffer = Receive(sizeof(int) * 1);
            //currently not used
            int markerId = BitConverter.ToInt32(buffer, 0);

            buffer = Receive(sizeof(float) * 9);
            Buffer.BlockCopy(buffer, 0, oWorldTransform.R, 0, sizeof(float) * 9);

            buffer = Receive(sizeof(float) * 3);
            Buffer.BlockCopy(buffer, 0, oWorldTransform.t, 0, sizeof(float) * 3);

            oCameraPose.R = oWorldTransform.R;
            for (int i = 0; i < 3; i++)
            {
                oCameraPose.t[i] = 0.0f;
                for (int j = 0; j < 3; j++)
                {
                    oCameraPose.t[i] += oWorldTransform.t[j] * oWorldTransform.R[i, j];
                }
            }

            UpdateSocketState("");
        }

        /// <summary>
        /// Gets a confirmation from the client that the restart has either been successfull, or a failure
        /// </summary>
        /// <returns></returns>
        public void RecieveRestartConfirmation()
        {
            bReinitialized = true;

            byte[] success = Receive(1);


            if (success[0] == 0)
                bReinizializationError = false;

            else
                bReinizializationError = true;
        }

        public void RecieveConfiguration()
        {
            byte[] buffer = Receive(KinectConfiguration.bytelength);
            configuration = new KinectConfiguration(buffer);
            bConfigurationReceived = true;            
            configurationUpdated?.Invoke(configuration);
        }

        public void ReceiveFrame()
        {
            lFrameRGB.Clear();
            lFrameVerts.Clear();
            lBodies.Clear();

            int nToRead;
            byte[] buffer = new byte[1024];

            while (oSocket.Available == 0)
            {
                if (!SocketConnected())
                    return;
            }

            oSocket.Receive(buffer, 8, SocketFlags.None);
            nToRead = BitConverter.ToInt32(buffer, 0);
            int iCompressed = BitConverter.ToInt32(buffer, 4);

            if (nToRead == -1)
            {
                bNoMoreStoredFrames = true;
                return;
            }

            //Sometimes we recieve negative values when the cameras are restarting, I don't know why yet.
            //I'm just patching this up for now.

            if (nToRead <= 0)
            {
                return;
            }

            buffer = new byte[nToRead];
            int nAlreadyRead = 0;

            while (nAlreadyRead != nToRead)
            {
                while (oSocket.Available == 0)
                {
                    if (!SocketConnected())
                        return;
                }

                nAlreadyRead += oSocket.Receive(buffer, nAlreadyRead, nToRead - nAlreadyRead, SocketFlags.None);
            }

            


            if (iCompressed == 1)
                buffer = ZSTDDecompressor.Decompress(buffer);

            //Receive depth and color data
            int startIdx = 0;

            int n_vertices = BitConverter.ToInt32(buffer, startIdx);
            startIdx += 4;

            for (int i = 0; i < n_vertices; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    lFrameRGB.Add(buffer[startIdx++]);
                }
                for (int j = 0; j < 3; j++)
                {
                    float val = BitConverter.ToInt16(buffer, startIdx);
                    //converting from milimeters to meters
                    val /= 1000.0f;
                    lFrameVerts.Add(val);
                    startIdx += 2;
                }
            }

            //Receive body data
            int nBodies = BitConverter.ToInt32(buffer, startIdx);
            startIdx += 4;
            for (int i = 0; i < nBodies; i++)
            {
                Body tempBody = new Body();
                tempBody.bTracked = BitConverter.ToBoolean(buffer, startIdx++);
                int nJoints = BitConverter.ToInt32(buffer, startIdx);
                startIdx += 4;

                tempBody.lJoints = new List<Joint>(nJoints);
                tempBody.lJointsInColorSpace = new List<Point2f>(nJoints);

                for (int j = 0; j < nJoints; j++)
                {
                    Joint tempJoint = new Joint();
                    Point2f tempPoint = new Point2f();

                    tempJoint.jointType = (JointType)BitConverter.ToInt32(buffer, startIdx);
                    startIdx += 4;
                    tempJoint.trackingState = (TrackingState)BitConverter.ToInt32(buffer, startIdx);
                    startIdx += 4;
                    tempJoint.position.X = BitConverter.ToSingle(buffer, startIdx);
                    startIdx += 4;
                    tempJoint.position.Y = BitConverter.ToSingle(buffer, startIdx);
                    startIdx += 4;
                    tempJoint.position.Z = BitConverter.ToSingle(buffer, startIdx);
                    startIdx += 4;

                    tempPoint.X = BitConverter.ToSingle(buffer, startIdx);
                    startIdx += 4;
                    tempPoint.Y = BitConverter.ToSingle(buffer, startIdx);
                    startIdx += 4;

                    tempBody.lJoints.Add(tempJoint);
                    tempBody.lJointsInColorSpace.Add(tempPoint);
                }

                lBodies.Add(tempBody);
            }
        }

        public void RecieveDirConfirmation()
        {
            bDirCreationConfirmed = true;

            byte[] buffer = Receive(1);

            if (buffer[0] == 0)
                bDirCreationError = true;
        }

        public void RecieveTimestampList()
        {
            byte[] buffer = Receive(sizeof(int));
            int timestampSize = BitConverter.ToInt32(buffer, 0);
            byte[]timestampByteList = Receive(timestampSize * sizeof(ulong));
            
            buffer = Receive(sizeof(int));
            int frameNumberSize = BitConverter.ToInt32(buffer, 0);
            byte[] frameNumberByteList = Receive(frameNumberSize * sizeof(int));

            lTimeStamps.Clear();
            lFrameNumbers.Clear();

            for (int i = 0; i < timestampSize; i+= sizeof(ulong))
            {
                lTimeStamps.Add(BitConverter.ToUInt64(timestampByteList, i));
            }

            for (int i = 0; i < frameNumberSize; i += sizeof(int))
            {
                lFrameNumbers.Add(BitConverter.ToInt32(frameNumberByteList, i));
            }

            bTimeStampsRecieved = true;
        }


        public byte[] Receive(int nBytes)
        {
            byte[] buffer;
            if (oSocket.Available != 0)
            {
                buffer = new byte[Math.Min(nBytes, oSocket.Available)];
                oSocket.Receive(buffer, nBytes, SocketFlags.None);
            }
            else
                buffer = new byte[0];

            return buffer;
        }

        public bool SocketConnected()
        {
            bool part1 = oSocket.Poll(1000, SelectMode.SelectRead);
            bool part2 = (oSocket.Available == 0);

            if (part1 && part2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void SendByte()
        {
            oSocket.Send(byteToSend);
        }

        public void UpdateSocketState(string message)
        {
            string tempSyncMessage = "";

            if(configuration != null)
            {
                switch (configuration.eSoftwareSyncState)
                {
                    case KinectConfiguration.SyncState.Main:
                        tempSyncMessage = "[MAIN]";
                        break;
                    case KinectConfiguration.SyncState.Subordinate:
                        tempSyncMessage = "[SUBORDINATE]";
                        break;
                    default:
                        break;
                }
            }            

            sSocketState = oSocket.RemoteEndPoint.ToString() + " " + message + " Calibrated = " + bCalibrated + " " + tempSyncMessage;

            eChanged?.Invoke();
        }

        public string GetEndpoint()
        {
            return oSocket.RemoteEndPoint.ToString();
        }

        public void DisconnectSocket()
        {
            oSocket.Shutdown(SocketShutdown.Both);
            oSocket.Disconnect(false);
        }

    }
}
