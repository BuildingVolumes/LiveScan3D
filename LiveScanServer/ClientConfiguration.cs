using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveScanServer
{
    /// <summary>
    /// Data object for kinect configuration (per sensor settings).
    /// </summary>

    public class ClientConfiguration
    {
        public static int bytelength = 42;
        static int serialnumberSize = 13;
        static int nicknameSize = 20;
        public enum SyncState { Main = 0, Subordinate = 1, Standalone = 2, Unknown = 3 };
        public enum depthResolution { NFOV320Binned = 1, NFOV640Unbinned = 2, WFOV512Binned = 3, WFOV1024Unbinned = 4 };
        public enum colorMode { MJPEG = 0, NV12 = 1, YUV2 = 2, BGRA = 3};
        public enum colorResolution { R720 = 1, R1080 = 2, R1440 = 3, R1536 = 4, R2160 = 5, R3072 = 6 };

        public bool FilterDepthMap;
        public int FilterDepthMapSize;
        public string SerialNumber;
        public string NickName;
        public byte globalDeviceIndex; //Each Client recieves a unique index from the server 
        public SyncState eSoftwareSyncState; //The sync state as set by the server
        public SyncState eHardwareSyncState; //The sync state that the sync jacks on the device are set for.
        public byte syncOffset; //Increasing number starting a 1, indicating the offset time from the master. Formula to get the actual offset time is SyncOffset * 160 us
        public depthResolution eDepthRes;
        public readonly colorMode eColorMode; //This should only be used to figure out in which state the client is in and is only set by it. 
        public colorResolution eColorRes;

        public ClientConfiguration()
        {
            eDepthRes = depthResolution.NFOV640Unbinned;
            eColorMode = colorMode.BGRA;
            eColorRes = colorResolution.R720;
            eSoftwareSyncState = SyncState.Standalone;
            eHardwareSyncState = SyncState.Unknown;
            syncOffset = 0;
            SerialNumber = "Unknown";
            NickName = new string(' ', 20); //Exactly 20 ASCII chars
            globalDeviceIndex = 0; // 255 = invalid index
            FilterDepthMap = false;
            FilterDepthMapSize = 0;
        }


        //Matches KinectConfiguration.cpp
        public ClientConfiguration(byte[] bytes)
        {
            int i = 0;
            eDepthRes = (depthResolution)bytes[i];
            i++;
            eColorMode = (colorMode)bytes[i];
            i++;
            eColorRes = (colorResolution)bytes[i];
            i++;
            eSoftwareSyncState = (SyncState)bytes[i];
            i++;
            eHardwareSyncState = (SyncState)bytes[i];
            i++;
            syncOffset = bytes[i];
            i++;
            
            SerialNumber = string.Empty;
            int c = i;
            for (; i < c + serialnumberSize; i++)
                SerialNumber += (char)((int)bytes[i]);

            c = i;
            for (; i < c + nicknameSize; i++)
                NickName += (char)((int)bytes[i]);
            
            globalDeviceIndex = bytes[i];
            i++;
            FilterDepthMap = bytes[i] == 0 ? false : true;
            i++;
            FilterDepthMapSize = bytes[i];
        }

        public byte[] ToBytes()
        {
            int i = 0;

            byte[] data = new byte[bytelength];
            data[i] = (byte)eDepthRes;
            i++;
            data[i] = (byte)eColorMode;
            i++;
            data[i] = (byte)eColorRes;
            i++;
            data[i] = (byte)eSoftwareSyncState;
            i++;
            data[i] = (byte)eHardwareSyncState;
            i++;
            data[i] = syncOffset;
            i++;

            int c = i;
            for (; i < c + serialnumberSize; i++)
                data[i] = (byte)SerialNumber[i - c];

            c = i;
            for (; i < c + nicknameSize; i++)
                data[i] = (byte)NickName[i - c];

            data[i] = globalDeviceIndex;
            i++;
            data[i] = (byte)(FilterDepthMap ? 1 : 0);
            i++;
            data[i] = (byte)FilterDepthMapSize;
            return data;
        }
    }
}
