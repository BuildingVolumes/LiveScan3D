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

    public class KinectConfiguration
    {
        public static int bytelength = 21;
        public enum SyncState { Main = 0, Subordinate = 1, Standalone = 2, Unknown = 3 };
        public enum depthMode { NFOV320Binned = 1, NFOV640Unbinned = 2, WFOV512Binned = 3, WFOV1024Unbinned = 4 };
        public enum colorMode { MJPEG = 0, NV12 = 1, YUV2 = 2, BGRA = 3}
        public bool FilterDepthMap;
        public int FilterDepthMapSize;
        public string SerialNumber;
        public byte globalDeviceIndex; //Each Client recieves a unique index from the server 
        public SyncState eSoftwareSyncState; //The sync state as set by the server
        public SyncState eHardwareSyncState; //The sync state that the sync jacks on the device are set for.
        public byte syncOffset; //Increasing number starting a 1, indicating the offset time from the master. Formula to get the actual offset time is SyncOffset * 160 us
        public depthMode eDepthMode;
        public readonly colorMode eColorMode; //Readonly! This should only be used to figure out in which state the client is in and is only set by it. 

        public KinectConfiguration()
        {
            eDepthMode = depthMode.NFOV640Unbinned;
            eColorMode = colorMode.BGRA;
            eSoftwareSyncState = SyncState.Standalone;
            eHardwareSyncState = SyncState.Unknown;
            syncOffset = 0;
            SerialNumber = "Unknown";
            globalDeviceIndex = 0;
            FilterDepthMap = false;
            FilterDepthMapSize = 0;
        }


        //Matches KinectConfiguration.cpp
        public KinectConfiguration(byte[] bytes)
        { 
            eDepthMode = (depthMode)bytes[0];
            eColorMode = (colorMode)bytes[1];
            eSoftwareSyncState = (SyncState)bytes[2];
            eHardwareSyncState = (SyncState)bytes[3];
            syncOffset = bytes[4];
            SerialNumber = "";
            for (int i = 5; i < 18; i++)
            {
                SerialNumber += (char)((int)bytes[i]);
            }
            globalDeviceIndex = bytes[18];
            FilterDepthMap = bytes[19] == 0 ? false : true;
            FilterDepthMapSize = bytes[20];
        }

        public byte[] ToBytes()
        {

            byte[] data = new byte[bytelength];
            data[0] = (byte)eDepthMode;
            data[1] = (byte)eColorMode;
            data[2] = (byte)eSoftwareSyncState;
            data[3] = (byte)eHardwareSyncState;
            data[4] = syncOffset;

            for(int i = 0;i<13;i++)
            {
                data[i+5] = (byte)SerialNumber[i];
            }
            data[18] = globalDeviceIndex;
            data[19] = (byte)(FilterDepthMap ? 1 : 0);
            data[20] = (byte)FilterDepthMapSize;
            return data;
        }

        internal static bool RequiresRestartAfterChange(KinectConfiguration oldConfig, KinectConfiguration newConfig)
        {
            if(oldConfig.eDepthMode != newConfig.eDepthMode)
            {
                return true;
            }
            //Todo: this might not require a restart
            if (oldConfig.FilterDepthMap != newConfig.FilterDepthMap)
            {
                return true;
            }


            return false;
        }
    }
}
