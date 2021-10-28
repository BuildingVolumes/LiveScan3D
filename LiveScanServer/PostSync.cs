﻿using System;
using System.Collections.Generic;

namespace KinectServer
{
    public class PostSync
    {
        public static List<DeviceSyncData> GenerateSyncList(List<DeviceSyncData> allDeviceSyncData)
        {

            if (!CheckForRoughTemporalCoherency(33333, allDeviceSyncData, 10))
            {
                //Console.WriteLine("Error! Timestamps don't align! Check your temporal sync setup and if all devices have the same firmware!");
                return null;
            }

            GenerateGlobalSyncIndex(ref allDeviceSyncData);

            return allDeviceSyncData;
        }


        /// <summary>
        /// Check if the timestamp lists are somewhat temporally coherent, meaning if the first frames are within maxToleranceFrames to each other.
        /// This allows us to see if the temporal synchronisation was set up correctly and if the firmwares are mathing
        /// </summary>
        /// <param name="frameTiming"></param>
        /// <param name="allSyncData"></param>
        /// <returns></returns>
        static bool CheckForRoughTemporalCoherency(uint frameTiming, List<DeviceSyncData> allSyncData, uint maxToleranceFrames)
        {
            List<ulong> firstTimeStamps = new List<ulong>();

            for (int i = 0; i < allSyncData.Count; i++)
            {
                firstTimeStamps.Add(allSyncData[i].frames[0].timestamp);
                //Console.WriteLine("First Frame of Device " + i + " : " + firstTimeStamps[i]);
            }

            firstTimeStamps.Sort();

            ulong difference = firstTimeStamps[firstTimeStamps.Count - 1] - firstTimeStamps[0];
            ulong maxToleratedTime = frameTiming * maxToleranceFrames;

            //Console.WriteLine("Difference Between Timestamp starts is: " + difference + ", max tolerated time is: " + maxToleratedTime);

            if (difference < maxToleratedTime)
                return true;

            else
                return false;
        }


        /// <summary>
        /// Generates a global sync Index for each frame, which indicates which frames belong together
        /// </summary>
        /// <param name="syncCollections"></param>
        /// <returns></returns>
        static void GenerateGlobalSyncIndex(ref List<DeviceSyncData> syncCollections)
        {
            List<GroupedFrame> allGroupedFrames = new List<GroupedFrame>();

            for (int i = 0; i < syncCollections.Count; i++)
            {
                for (int j = 0; j < syncCollections[i].frames.Count; j++)
                {
                    if (!syncCollections[i].frames[j].grouped) //Skip already grouped frames
                    {
                        ulong timestamp = syncCollections[i].frames[j].timestamp;

                        GroupedFrame newGroupedFrame = new GroupedFrame();

                        for (int k = 0; k < syncCollections.Count; k++)
                        {

                            //TODO: Optimize search by specifying searchStartFrameID
                            int matchingFrame = GetMatchingSyncedFrameIndex(0, timestamp, syncCollections[k]);

                            if (matchingFrame != -1)
                            {
                                syncCollections[k].frames[matchingFrame].grouped = true;
                                newGroupedFrame.listIndex.Add(new ClientListIndex(matchingFrame, k));
                                newGroupedFrame.minTimestamp = timestamp;
                            }
                        }

                        allGroupedFrames.Add(newGroupedFrame);
                    }
                }
            }

            allGroupedFrames.Sort((x, y) => x.minTimestamp.CompareTo(y.minTimestamp));

            for (int i = 0; i < allGroupedFrames.Count; i++)
            {
                for (int j = 0; j < allGroupedFrames[i].listIndex.Count; j++)
                {
                    int indexClient = allGroupedFrames[i].listIndex[j].indexClient;
                    int indexFrame = allGroupedFrames[i].listIndex[j].indexFrame;

                    syncCollections[indexClient].frames[indexFrame].syncedFrameID = i;
                }
            }

            //for (int i = 0; i < syncCollections.Count; i++)
            //{
            //    for (int j = 0; j < syncCollections[i].frames.Count; j++)
            //    {
            //        Console.WriteLine("SyncID: " + syncCollections[i].frames[j].syncedFrameID + ", Timestamp: " + syncCollections[i].frames[j].timestamp);
            //    }

            //    Console.WriteLine("\r");
            //    Console.WriteLine("\r");
            //}

        }

        /// <summary>
        /// Gets a matching frame for a given timestamp from the syncCollection
        /// </summary>
        /// <param name="searchStartFrameID"> The frame from which the search should start from. Used for optimization, if unclear set to 0</param>
        /// <param name="timestamp"> The timestamp which is searched for</param>
        /// <param name="syncCollection"> A set of Timestamps</param>
        /// <returns></returns>
        static int GetMatchingSyncedFrameIndex(int searchStartFrameID, ulong timestamp, DeviceSyncData syncCollection)
        {
            ulong maxTimestampDifferenceUs = 2000; // Maximum numbers of temporal synced Kinect devices = 9, typical sync timing between devices = 160us, rounded to 200 for some buffer, 9 * 200 + some additional buffer (200) = 2000us
            ulong timeStampMin = timestamp - maxTimestampDifferenceUs;
            ulong timeStampMax = timestamp + maxTimestampDifferenceUs;

            for (int i = searchStartFrameID; i < syncCollection.frames.Count; i++)
            {
                if (syncCollection.frames[i].timestamp < timeStampMax && syncCollection.frames[i].timestamp > timeStampMin)
                {
                    return i;
                }
            }

            return -1;
        }
    }

    /// <summary>
    /// Contains all sync data for one device & recording
    /// </summary>
    public class DeviceSyncData
    {
        public List<SyncFrame> frames;

        public DeviceSyncData(List<SyncFrame> frames)
        {
            this.frames = frames;
        }

        public DeviceSyncData(List<int> frameNumbers, List<ulong> timestamps, int deviceIndex)
        {
            List<SyncFrame> newSyncFrames = new List<SyncFrame>();

            for (int i = 0; i < frameNumbers.Count; i++)
            {
                newSyncFrames.Add(new SyncFrame(frameNumbers[i], timestamps[i], deviceIndex));
            }

            this.frames = newSyncFrames;
        }
    }

    /// <summary>
    /// Class contains all data needed to sync for a single frame
    /// </summary>
    public class SyncFrame
    {
        public int frameID;
        public int syncedFrameID;
        public int deviceIndex;
        public ulong timestamp;
        public bool grouped;

        public SyncFrame(int frameID, ulong timestamp, int deviceIndex)
        {
            this.frameID = frameID;
            this.timestamp = timestamp;
            this.syncedFrameID = 0;
            this.grouped = false;
            this.deviceIndex = deviceIndex;
        }
    }

    /// <summary>
    /// Groups all frames, from all devices, which have a matching timestamp 
    /// </summary>
    public class GroupedFrame
    {
        public List<ClientListIndex> listIndex;
        public ulong minTimestamp;

        public GroupedFrame()
        {
            this.listIndex = new List<ClientListIndex>();
            this.minTimestamp = 0;
        }

        public GroupedFrame(List<ClientListIndex> frames, ulong minTimestamp)
        {
            this.listIndex = frames;
            this.minTimestamp = minTimestamp;
        }
    }

    /// <summary>
    /// Contains a collection of a frame matched to a device
    /// </summary>
    public class ClientListIndex
    {
        public int indexFrame;
        public int indexClient;

        public ClientListIndex(int indexFrame, int indexClient)
        {
            this.indexFrame = indexFrame;
            this.indexClient = indexClient;
        }

    }

}

