using System;
using System.Collections.Generic;

namespace KinectServer
{
    public class PostSync
    {
        public static List<ClientSyncData> GenerateSyncList(List<ClientSyncData> allDeviceSyncData)
        {
            if (allDeviceSyncData.Count < 2) //We need at least two clients for syncing
            {
                Console.WriteLine("Error! At least two clients are needed for syncing!");
                return null;
            }

            if (!CheckForRoughTemporalCoherency(33333, allDeviceSyncData, 10))
            {
                Console.WriteLine("Error! Timestamps don't align! Check your temporal sync setup and if all devices have the same firmware!");
                return null;
            }

            return GenerateGlobalSyncIndex(allDeviceSyncData);
        }

        public static List<ClientSyncData> GetTestData()
        {
            List<ClientSyncData> testdata = new List<ClientSyncData>();

            ClientSyncData client1 = new ClientSyncData();
            ClientSyncData client2 = new ClientSyncData();

            client1.frames.Add(new SyncFrame(0, 33333u, 0));
            client1.frames.Add(new SyncFrame(1, 66666u, 0));
            client1.frames.Add(new SyncFrame(2, 100000u, 0));
            client1.frames.Add(new SyncFrame(3, 133333u, 0));
            //client1.frames.Add(new SyncFrame(4, 166666u, 0));
            //client1.frames.Add(new SyncFrame(5, 200000u, 0));
            //client1.frames.Add(new SyncFrame(6, 233333u, 0));
            //client1.frames.Add(new SyncFrame(7, 300000u, 0));
            //client1.frames.Add(new SyncFrame(8, 333333u, 0));
            client1.frames.Add(new SyncFrame(4, 366666u, 0));
            client1.frames.Add(new SyncFrame(5, 400000u, 0));
            client1.frames.Add(new SyncFrame(6, 433333u, 0));

            //client2.frames.Add(new SyncFrame(0, 33333, 0));
            //client2.frames.Add(new SyncFrame(1, 66666, 0));
            client2.frames.Add(new SyncFrame(0, 100000u, 1));
            client2.frames.Add(new SyncFrame(1, 133333u, 1));
            client2.frames.Add(new SyncFrame(2, 166666u, 1));
            client2.frames.Add(new SyncFrame(3, 200000u, 1));
            client2.frames.Add(new SyncFrame(4, 233333u, 1));
            client2.frames.Add(new SyncFrame(5, 266666u, 1));
            client2.frames.Add(new SyncFrame(6, 300000u, 1));
            client2.frames.Add(new SyncFrame(7, 333333u, 1));
            client2.frames.Add(new SyncFrame(8, 366666u, 1));
            client2.frames.Add(new SyncFrame(9, 400000u, 1));
            //client2.frames.Add(new SyncFrame(10, 433333u, 1));

            testdata.Add(client1);
            testdata.Add(client2);

            return testdata;

        }



        /// <summary>
        /// Check if the timestamp lists are somewhat temporally coherent, meaning if the first frames are within maxToleranceFrames to each other.
        /// This allows us to see if the temporal synchronisation was set up correctly and if the firmwares are matching
        /// </summary>
        /// <param name="frameTiming"></param>
        /// <param name="allSyncData"></param>
        /// <returns></returns>
        static bool CheckForRoughTemporalCoherency(uint frameTiming, List<ClientSyncData> allSyncData, uint maxToleranceFrames)
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
        static List<ClientSyncData> GenerateGlobalSyncIndex(List<ClientSyncData> syncCollections)
        {
            List<GroupedFrame> allGroupedFrames = new List<GroupedFrame>();

            for (int i = 0; i < syncCollections.Count; i++) // Go through all devices
            {
                for (int j = 0; j < syncCollections[i].frames.Count; j++) //Go through all frames of device
                {
                    if (!syncCollections[i].frames[j].grouped) //Skip if frame is already grouped
                    {
                        ulong timestamp = syncCollections[i].frames[j].timestamp;

                        GroupedFrame newGroupedFrame = new GroupedFrame(); //A group of all frames that match this frames timestamp
                        newGroupedFrame.listIndex.Add(new ClientFrame(j, i)); //Add the current frame
                        newGroupedFrame.minTimestamp = timestamp;

                        for (int k = 0; k < syncCollections.Count; k++) //Search through all frames of all devices for fitting timestamp
                        {
                            if (k == i) //Don't search in devices own data
                                continue;

                            //TODO: Optimize search by specifying searchStartFrameID
                            int matchingFrame = GetMatchingSyncedFrameIndex(0, timestamp, syncCollections[k]); //Search if there is a frame in this collection that matches the timestamp

                            if (matchingFrame != -1)
                            {
                                syncCollections[k].frames[matchingFrame].grouped = true;
                                newGroupedFrame.listIndex.Add(new ClientFrame(matchingFrame, k));
                                newGroupedFrame.minTimestamp = timestamp;
                            }
                        }

                        allGroupedFrames.Add(newGroupedFrame);
                    }
                }
            }

            allGroupedFrames.Sort((x, y) => x.minTimestamp.CompareTo(y.minTimestamp));

            foreach (GroupedFrame gf in allGroupedFrames)
            {
                Console.WriteLine("Grouped Frame " + gf.minTimestamp + " Contains: ");
                foreach (ClientFrame cf in gf.listIndex)
                {
                    Console.WriteLine("Client: " + cf.indexClient + " Frame " + cf.indexFrame);
                }

                Console.WriteLine("");
            }

            //Create a new List that contains all the correctly synced frames for each device 
            List<ClientSyncData> postSyncedData = new List<ClientSyncData>();

            for (int i = 0; i < syncCollections.Count; i++)
            {
                postSyncedData.Add(new ClientSyncData());
            }

            for (int i = 0; i < allGroupedFrames.Count; i++) //Go through all grouped/synced frames
            {
                for (int k = 0; k < syncCollections.Count; k++) //Go through each client
                {
                    bool deviceFoundInCurrentFrame = false;

                    for (int l = 0; l < allGroupedFrames[i].listIndex.Count; l++) //See if we can find the client in this grouped frame
                    {
                        ClientFrame currentDeviceFrame = allGroupedFrames[i].listIndex[l];

                        //If we can find the client in this frame, we give it a fitting device index
                        if (k == currentDeviceFrame.indexClient)
                        {
                            deviceFoundInCurrentFrame = true;
                            postSyncedData[currentDeviceFrame.indexClient].frames.Add(new SyncFrame(currentDeviceFrame.indexFrame, currentDeviceFrame.indexClient, i));
                            break;
                        }
                    }

                    //If we cant find the client in this frame, we insert a frame anyways with the data set to -1, which tells the client to insert an empty frame
                    //This prevents dropped frames from messing up the frametiming
                    if (!deviceFoundInCurrentFrame)
                    {
                        postSyncedData[k].frames.Add(new SyncFrame(-1, k, -1));
                    }
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

            return postSyncedData;

        }

        /// <summary>
        /// Gets a matching frame for a given timestamp from the syncCollection
        /// </summary>
        /// <param name="searchStartFrameID"> The frame from which the search should start from. Used for optimization, if unclear set to 0</param>
        /// <param name="timestamp"> The timestamp which is searched for</param>
        /// <param name="syncCollection"> A set of Timestamps</param>
        /// <returns></returns>
        static int GetMatchingSyncedFrameIndex(int searchStartFrameID, ulong timestamp, ClientSyncData syncCollection)
        {
            ulong maxTimestampDifferenceUs = 2000; // Maximum numbers of temporal synced Kinect devices = 9, typical sync timing between devices = 160us, rounded to 200 for some buffer, 9 * 200 + some additional buffer (200) = 2000us
            ulong timeStampMax = timestamp + maxTimestampDifferenceUs;
            ulong timeStampMin = 0;
            if (timestamp > maxTimestampDifferenceUs)
                timeStampMin = timestamp - maxTimestampDifferenceUs;

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
    public class ClientSyncData
    {
        public List<SyncFrame> frames;

        public ClientSyncData()
        {
            this.frames = new List<SyncFrame>();
        }

        public ClientSyncData(List<SyncFrame> frames)
        {
            this.frames = frames;
        }

        public ClientSyncData(List<int> frameNumbers, List<ulong> timestamps, int deviceIndex)
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

        public SyncFrame(int frameID, int deviceIndex, int syncedFrameID)
        {
            this.frameID = frameID;
            this.timestamp = 0;
            this.syncedFrameID = syncedFrameID;
            this.grouped = false;
            this.deviceIndex = deviceIndex;
        }
    }

    /// <summary>
    /// Groups all frames, from all devices, which have a matching timestamp 
    /// </summary>
    public class GroupedFrame
    {
        public List<ClientFrame> listIndex;
        public ulong minTimestamp;

        public GroupedFrame()
        {
            this.listIndex = new List<ClientFrame>();
            this.minTimestamp = 0;
        }

        public GroupedFrame(List<ClientFrame> frames, ulong minTimestamp)
        {
            this.listIndex = frames;
            this.minTimestamp = minTimestamp;
        }
    }

    /// <summary>
    /// Contains a collection of a frame matched to a device
    /// </summary>
    public class ClientFrame
    {
        public int indexFrame;
        public int indexClient;

        public ClientFrame(int indexFrame, int indexClient)
        {
            this.indexFrame = indexFrame;
            this.indexClient = indexClient;
        }

    }
}

