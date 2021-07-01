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
        public static int bytelength = 19;
        public enum SyncState { Main = 0, Subordinate = 1, Standalone = 2, Unknown = 3 };
        public enum depthMode { NFOV320Binned = 1, NFOV640Unbinned = 2, WFOV512Binned = 3, WFOV1024Unbinned = 4 };
        public bool FilterDepthMap;
        public int FilterDepthMapSize;
        public string SerialNumber;
        public SyncState eSoftwareSyncState; //The sync state as set by the server
        public SyncState eHardwareSyncState; //The sync state that the sync jacks on the device are set for.
        public byte syncOffset; //Increasing number starting a 1, indicating the offset time from the master. Formula to get the actual offset time is SyncOffset * 160 us
        public depthMode eDepthMode;

        public KinectConfiguration()
        {
            eDepthMode = depthMode.NFOV640Unbinned;
            eSoftwareSyncState = SyncState.Standalone;
            eHardwareSyncState = SyncState.Unknown;
            syncOffset = 0;
            SerialNumber = "Unknown";
            FilterDepthMap = false;
            FilterDepthMapSize = 0;
        }


        //Matches KinectConfiguration.cpp
        public KinectConfiguration(byte[] bytes)
        { 
            eDepthMode = (depthMode)bytes[0];
            eSoftwareSyncState = (SyncState)bytes[1];
            eHardwareSyncState = (SyncState)bytes[2];
            syncOffset = bytes[3];
            SerialNumber = "";
            for (int i = 4; i < 17; i++)
            {
                SerialNumber += (char)((int)bytes[i]);
            }
            FilterDepthMap = bytes[17] == 0 ? false : true;
            FilterDepthMapSize = bytes[18];
        }

        public byte[] ToBytes()
        {

            byte[] data = new byte[bytelength];
            data[0] = (byte)eDepthMode;
            data[1] = (byte)eSoftwareSyncState;
            data[2] = (byte)eHardwareSyncState;
            data[3] = syncOffset;

            for(int i = 0;i<13;i++)
            {
                data[i+4] = (byte)SerialNumber[i];
            }

            data[17] = (byte)(FilterDepthMap ? 1 : 0);
            data[18] = (byte)FilterDepthMapSize;
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
