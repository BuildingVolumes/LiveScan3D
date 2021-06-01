using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectServer
{
    /// <summary>
    /// Data object for kinect configuration (per sensor settings).
    /// </summary>
    public struct KinectConfiguration
    {
        public static readonly int bytelength = 18;
        //These cam just be bytes.
        public byte SyncState;
        public byte SyncOffset;
        public byte DepthMode;
        public bool FilterDepthMap;
        public int FilterDepthMapSize;
        public string SerialNumber;
        //Matches KinectConfiguration.cpp
        public KinectConfiguration(byte[] bytes)
        {
            DepthMode = bytes[0];
            SyncState = bytes[1];
            SyncOffset = bytes[2];
            SerialNumber = "";
            for(int i = 3; i < 16; i++)
            {
                SerialNumber += (char)((int)bytes[i]);
            }
            FilterDepthMap = bytes[16] == 0 ? false : true;
            FilterDepthMapSize = bytes[17];
        }

        //In need of refactor.
        public bool RequiresRestart(KinectConfiguration previousConfig)
        {
            if (previousConfig.SyncState != SyncState || previousConfig.DepthMode != DepthMode)
            {
                return true;
            }
            //Do we need to restart when we change the offset?
            if (previousConfig.SyncOffset != SyncOffset)
            {
                return true;
            }
            return false;
        }

        public byte[] ToBytes()
        {
            byte[] data = new byte[bytelength];

            data[0] = DepthMode;
            data[1] = SyncState;
            data[2] = SyncOffset;
            for(int i = 0;i<13;i++)
            {
                data[i+3] = (byte)SerialNumber[i];
            }
            data[16] = (byte)(FilterDepthMap ? 1 : 0);
            data[17] = (byte)FilterDepthMapSize;
            return data;
        }
        
    }
}
