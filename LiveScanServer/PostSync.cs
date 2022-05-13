using System;
using System.IO;
using System.Collections.Generic;

namespace KinectServer
{
    /// <summary>
    /// Contains all data needed to sync for a single frame
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

        public SyncFrame(int frameID, int deviceIndex, int syncedFrameID, ulong timestamp)
        {
            this.frameID = frameID;
            this.timestamp = timestamp;
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
    /// Contains a frame matched to a device
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

    public class PostSync
    {
        public static List<ClientSyncData> GenerateSyncList(List<ClientSyncData> allDeviceSyncData)
        {
            if (allDeviceSyncData.Count < 2) //We need at least two clients for syncing
            {
                Log.LogWarning("At least two clients are needed for post syncing!");
                return null;
            }

            if (!CheckForRoughTemporalCoherency(33333, allDeviceSyncData, 10))
            {
                Log.LogError("Postsync: Timestamps don't align! Check your temporal sync setup and if all devices have the same firmware!");
                return null;
            }

            return GenerateGlobalSyncIndex(allDeviceSyncData);
        }


        /// <summary>
        /// Check if the timestamp lists are somewhat temporally coherent, meaning if the first frames are within maxToleranceFrames to each other.
        /// This allows us to see if the temporal synchronisation was set up correctly and if the firmwares are matching
        /// </summary>
        /// <param name="frameTiming">The timing between the frames in us. E.g. 30 FPS = 33333us </param>
        /// <param name="allSyncData"></param>
        /// <param name="maxToleranceFrames">The maximum amount of frames the clients can be apart</param>
        /// <returns></returns>
        static bool CheckForRoughTemporalCoherency(uint frameTiming, List<ClientSyncData> allSyncData, uint maxToleranceFrames)
        {
            List<ulong> firstTimeStamps = new List<ulong>();

            for (int i = 0; i < allSyncData.Count; i++)
            {
                firstTimeStamps.Add(allSyncData[i].frames[0].timestamp);
            }

            firstTimeStamps.Sort();

            ulong difference = firstTimeStamps[firstTimeStamps.Count - 1] - firstTimeStamps[0];
            ulong maxToleratedTime = frameTiming * maxToleranceFrames;

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

            if(Log.GetLogLevel() == Log.LogLevel.All)
            {
                Log.LogTrace("Saving Postsync log of all grouped frames before sorting");
                LogGroupedFrameData(allGroupedFrames);
            }

            //Sort all grouped frames by their timestamp
            allGroupedFrames.Sort((x, y) => x.minTimestamp.CompareTo(y.minTimestamp));

            if (Log.GetLogLevel() == Log.LogLevel.All)
            {
                Log.LogTrace("Saving Postsync log of all grouped frames after sorting");
                LogGroupedFrameData(allGroupedFrames);
            }

            //Create a new List for each device, which tells it how to sync it's own frames 
            List<ClientSyncData> postSyncedData = new List<ClientSyncData>();

            for (int i = 0; i < syncCollections.Count; i++)
            {
                postSyncedData.Add(new ClientSyncData());
            }

            for (int i = 0; i < allGroupedFrames.Count; i++) //Go through all grouped/synced frames
            {
                for (int k = 0; k < syncCollections.Count; k++) //Go through each client/device
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

                    //If we cant find the client in this frame, we insert a frame with the data set to -1, which tells the client to insert an empty frame
                    //This prevents dropped frames from messing up the frametiming
                    if (!deviceFoundInCurrentFrame)
                    {
                        postSyncedData[k].frames.Add(new SyncFrame(-1, k, i));
                    }
                }
            }

            if (Log.GetLogLevel() == Log.LogLevel.All)
            {
                Log.LogTrace("Saving detailed Postsync log:");
                LogSyncData(postSyncedData);
            }

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

        /// <summary>
        /// Just for debugging, makes the result of the sorting algorithm above easier to understand
        /// Writes a file, which explains for each frame of a client how the frames before and after sync are numbered
        /// </summary>
        /// <param name="syncData"></param>
        static void LogSyncData(List<ClientSyncData> syncData)
        {
            string text = "";

            for (int i = 0; i < syncData.Count; i++)
            {
                text += "Sync Data for Client: " + i + "\n";

                for (int j = 0; j < syncData[i].frames.Count; j++)
                {
                    text += "Frame original number: " + syncData[i].frames[j].frameID + ", new Sync number: " + syncData[i].frames[j].syncedFrameID + "\n";
                }

                text += "\n\n";
            }

            File.WriteAllText("PostSyncDataForClientsLog.txt", text);
            Log.LogTrace("Saving Post Sync Log Data to: PostSyncDataForClientsLog.txt");
        }

        /// <summary>
        /// Shows the result of the grouping algorithm. This shows which frames belong together
        /// </summary>
        /// <param name="groupedFrames"></param>
        static void LogGroupedFrameData(List<GroupedFrame> groupedFrames)
        {
            string text = "";

            foreach (GroupedFrame gf in groupedFrames)
            {
                text += "Grouped Frame " + gf.minTimestamp + " Contains: " + "\n";

                foreach (ClientFrame cf in gf.listIndex)
                {
                    text += "Client: " + cf.indexClient + " Frame " + cf.indexFrame + "\n";
                }

                text += "\n";
            }

            File.WriteAllText("PostSyncGroupedFramesLog.txt", text);
            Log.LogTrace("Saving Post Sync Grouped Log Data to: PostSyncDataForClientsLog.txt");
        }
    }     
}

