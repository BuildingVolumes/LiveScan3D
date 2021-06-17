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
        public enum SyncState { Main = 0, Subordinate = 1, Standalone = 2, Unknown = 3 };
        public enum depthMode { NFOV320Binned = 1, NFOV640Unbinned = 2, WFOV512Binned = 3, WFOV1024Unbinned = 4 };

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
        }

        //Matches KinectConfiguration.cpp
        public KinectConfiguration(byte[] bytes)
        {
            eDepthMode = (depthMode)bytes[0];
            eSoftwareSyncState = (SyncState)bytes[1];
            eHardwareSyncState = (SyncState)bytes[2];
            syncOffset = bytes[3];
        }

        public byte[] ToBytes()
        {
            byte[] data = new byte[4];

            data[0] = (byte)eDepthMode;
            data[1] = (byte)eSoftwareSyncState;
            data[2] = (byte)eHardwareSyncState;
            data[3] = syncOffset;
            return data;
        }
        
    }
}
