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
        //These cam just be bytes.
        public byte SyncState;
        public byte SyncOffset;
        public byte DepthMode;

        //Matches KinectConfiguration.cpp
        public KinectConfiguration(byte[] bytes)
        {
            DepthMode = bytes[0];
            SyncState = bytes[1];
            SyncOffset = bytes[2];
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
            byte[] data = new byte[3];

            data[0] = DepthMode;
            data[1] = SyncState;
            data[2] = SyncOffset;
            return data;
        }
        
    }
}
