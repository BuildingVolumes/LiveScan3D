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
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;

namespace KinectServer
{
    [Serializable]
    public class KinectSettings
    {
        public float[] aMinBounds = new float[3];
        public float[] aMaxBounds = new float[3];

        public bool bFilter = false;
        public int nFilterNeighbors = 10;
        public float fFilterThreshold = 0.1f;

        public BindingList<MarkerPose> lMarkerPoses = new BindingList<MarkerPose>();

        public bool bStreamOnlyBodies = false;
        public bool bShowSkeletons = true;
        public int iCompressionLevel = 2;       // 0 for no compression, 2 is recommended

        public int nNumICPIterations = 10;
        public int nNumRefineIters = 2;
        public bool bMergeScansForSave = true;
        public bool bSaveAsBinaryPLY = true;

        public bool bSyncEnabled = false;

        public bool bAutoExposureEnabled = true;
        public int nExposureStep = -8;

        public enum ExportMode { Pointcloud = 0, RawFrames = 1}
        public ExportMode eExportMode = ExportMode.Pointcloud;

        public enum ExtrinsicsStyle { None = 0, Open3D = 1, OpenMVS = 2}
        public ExtrinsicsStyle eExtrinsicsFormat = ExtrinsicsStyle.Open3D;

        public string takePath;

        public KinectSettings()
        {
            aMinBounds[0] = -5f;
            aMinBounds[1] = -5f;
            aMinBounds[2] = -5f;

            aMaxBounds[0] = 5f;
            aMaxBounds[1] = 5f;
            aMaxBounds[2] = 5f;
        }

        public List<byte> ToByteList()
        {
            List<byte> lData = new List<byte>();

            byte[] bTemp = new byte[sizeof(float) * 3];

            Buffer.BlockCopy(aMinBounds, 0, bTemp, 0, sizeof(float) * 3);
            lData.AddRange(bTemp);
            Buffer.BlockCopy(aMaxBounds, 0, bTemp, 0, sizeof(float) * 3);
            lData.AddRange(bTemp);

            if (bFilter)
                lData.Add(1);
            else
                lData.Add(0);

            bTemp = BitConverter.GetBytes(nFilterNeighbors);
            lData.AddRange(bTemp);

            bTemp = BitConverter.GetBytes(fFilterThreshold);
            lData.AddRange(bTemp);

            bTemp = BitConverter.GetBytes(lMarkerPoses.Count);
            lData.AddRange(bTemp);

            for (int i = 0; i < lMarkerPoses.Count; i++)
            {
                bTemp = new byte[sizeof(float) * 9];
                Buffer.BlockCopy(lMarkerPoses[i].pose.R, 0, bTemp, 0, sizeof(float) * 9);
                lData.AddRange(bTemp);

                bTemp = new byte[sizeof(float) * 3];
                Buffer.BlockCopy(lMarkerPoses[i].pose.t, 0, bTemp, 0, sizeof(float) * 3);
                lData.AddRange(bTemp);

                bTemp = BitConverter.GetBytes(lMarkerPoses[i].id);
                lData.AddRange(bTemp);
            }

            if (bStreamOnlyBodies)
                lData.Add(1);
            else
                lData.Add(0);

            bTemp = BitConverter.GetBytes(iCompressionLevel);
            lData.AddRange(bTemp);

            if (bAutoExposureEnabled)
                lData.Add(1);
            else
                lData.Add(0);

            bTemp = BitConverter.GetBytes(nExposureStep);
            lData.AddRange(bTemp);

            bTemp = BitConverter.GetBytes((int)eExportMode);
            lData.AddRange(bTemp);

            bTemp = BitConverter.GetBytes((int)eExtrinsicsFormat);
            lData.AddRange(bTemp);


            return lData;
        }


        /// <summary>
        /// Given a take name, it gives back an integer that is unique to this take.
        /// Returns -1 if an error happened during the reading/writing of this file.
        /// </summary>
        /// <param name="takeName"></param>
        /// <returns></returns>
        public int GetNewTakeIndex(string takeName)
        {
            Dictionary<String, int> takeDict = new Dictionary<string, int>();
            string jsonPath = "takes.json";
            string jsonContent = string.Empty;

            if (File.Exists(jsonPath))
            {
                try
                {
                    jsonContent = File.ReadAllText(jsonPath);
                    takeDict = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonContent);
                }

                catch (Exception e)
                {
                    return -1; //Error
                }
            }

            int takeIndex = 1;

            if (takeDict.TryGetValue(takeName, out takeIndex))
            {
                takeIndex++;
                takeDict[takeName] = takeIndex;
            }

            else
            {
                takeDict.Add(takeName, takeIndex);
            }

            try
            {
                jsonContent = JsonConvert.SerializeObject(takeDict);
                File.WriteAllText(jsonPath, jsonContent);
            }

            catch
            {
                return -1;
            }

            return takeIndex;

        }
    }
}
