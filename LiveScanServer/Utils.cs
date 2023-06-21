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
using System.IO;
using System.Globalization;

namespace KinectServer
{
    public struct Point2f
    {
        public float X;
        public float Y;
    }

    public struct Point3f
    {
        public float X;
        public float Y;
        public float Z;

        public Point3f(float x, float y, float z)
        {
            X = x; Y = y; Z = z; 
        }

        public static Point3f operator *(Matrix4x4 mat, Point3f v)
        {
            float[] inputV = new float[4];
            float[] resultV = new float[4];

            //We need to add one more component to the Vector, to make it the same size as the Matrix
            inputV[0] = v.X;
            inputV[1] = v.Y;
            inputV[2] = v.Z;
            inputV[3] = 1;

            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < 4; k++)
                {
                    resultV[i] += mat.mat[i, k] * inputV[k];
                }
            }

            Point3f output = new Point3f(resultV[0], resultV[1], resultV[2]);

            return output;
        }

    }

    [Serializable]
    public class Matrix4x4
    {
        public float[,] mat = new float[4, 4];

        public Matrix4x4()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (i == j)
                        mat[i, j] = 1;
                    else
                        mat[i, j] = 0;
                }
            }
        }

        public static Matrix4x4 GetEmptyMatrix()
        {
            Matrix4x4 empty = new Matrix4x4();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    empty.mat[i, j] = 0;
                }
            }

            return empty;
        }

        public static Matrix4x4 operator *(Matrix4x4 lhs, Matrix4x4 rhs)
        {
            Matrix4x4 result = Matrix4x4.GetEmptyMatrix();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                         result.mat[i,j] += lhs.mat[i,k] * rhs.mat[k,j];

                        float resultf = result.mat[i, j];
                        float lhsV = lhs.mat[i, k];
                        float rhsV = rhs.mat[i, k];
                    }
                }
            }

            return result;
        }    
    }

    [Serializable]
    public class MarkerPose
    {
        public Matrix4x4 pose = new Matrix4x4();
        public int id = -1;
        private float[] r = new float[3];

        public MarkerPose()
        {
            UpdateRotationMatrix();
        }

        public void SetOrientation(float X, float Y, float Z)
        {
            r[0] = X;
            r[1] = Y;
            r[2] = Z;

            UpdateRotationMatrix();
        }

        public void GetOrientation(out float X, out float Y, out float Z)
        {
            X = r[0];
            Y = r[1];
            Z = r[2];
        }

        private void UpdateRotationMatrix()
        {
            float radX = r[0] * (float)Math.PI / 180.0f;
            float radY = r[1] * (float)Math.PI / 180.0f;
            float radZ = r[2] * (float)Math.PI / 180.0f;

            float c1 = (float)Math.Cos(radZ);
            float c2 = (float)Math.Cos(radY);
            float c3 = (float)Math.Cos(radX);
            float s1 = (float)Math.Sin(radZ);
            float s2 = (float)Math.Sin(radY);
            float s3 = (float)Math.Sin(radX);

            //Z Y X rotation
            pose.mat[0, 0] = c1 * c2;
            pose.mat[0, 1] = c1 * s2 * s3 - c3 * s1;
            pose.mat[0, 2] = s1 * s3 + c1 * c3 * s2;
            pose.mat[1, 0] = c2 * s1;
            pose.mat[1, 1] = c1 * c3 + s1 * s2 * s3;
            pose.mat[1, 2] = c3 * s1 * s2 - c1 * s3;
            pose.mat[2, 0] = -s2;
            pose.mat[2, 1] = c2 * s3;
            pose.mat[2, 2] = c2 * c3;
        }

    }

    public enum appState 
    { 
        idle = 0,
        recording = 1,
        syncing = 2,
        saving = 3,
        calibrating = 4,
        refinining = 5,
        restartingClients = 6
    }

    public enum captureMode
    {
        raw = 0,
        pointcloud = 1
    }


    public enum TrackingState
    {
        TrackingState_NotTracked = 0,
        TrackingState_Inferred = 1,
        TrackingState_Tracked = 2
    }

    public enum JointType
    {
        JointType_SpineBase = 0,
        JointType_SpineMid = 1,
        JointType_Neck = 2,
        JointType_Head = 3,
        JointType_ShoulderLeft = 4,
        JointType_ElbowLeft = 5,
        JointType_WristLeft = 6,
        JointType_HandLeft = 7,
        JointType_ShoulderRight = 8,
        JointType_ElbowRight = 9,
        JointType_WristRight = 10,
        JointType_HandRight = 11,
        JointType_HipLeft = 12,
        JointType_KneeLeft = 13,
        JointType_AnkleLeft = 14,
        JointType_FootLeft = 15,
        JointType_HipRight = 16,
        JointType_KneeRight = 17,
        JointType_AnkleRight = 18,
        JointType_FootRight = 19,
        JointType_SpineShoulder = 20,
        JointType_HandTipLeft = 21,
        JointType_ThumbLeft = 22,
        JointType_HandTipRight = 23,
        JointType_ThumbRight = 24,
        JointType_Count = (JointType_ThumbRight + 1)
    }

    public struct Joint
    {
        public Point3f position;
        public JointType jointType;
        public TrackingState trackingState;
    }

    public enum EColorMode { RGB, BGR };

    public class Utils
    {
        public static void saveToPly(string filename, List<Single> vertices, List<byte> colors, EColorMode colorMode,bool binary)
        {
            Log.LogDebugCapture("Saving .ply frame: " + filename);

            int nVertices = vertices.Count / 3;

            FileStream fileStream = File.Open(filename, FileMode.Create);

            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(fileStream);
            System.IO.BinaryWriter binaryWriter = new System.IO.BinaryWriter(fileStream);

            //PLY file header is written here.
            if (binary)
                streamWriter.WriteLine("ply\nformat binary_little_endian 1.0");
            else
                streamWriter.WriteLine("ply\nformat ascii 1.0");
            streamWriter.Write("element vertex " + nVertices.ToString() + "\n");
            streamWriter.Write(
                "property float x\n" +
                "property float y\n" +
                "property float z\n" +
                "property uchar red\n" +
                "property uchar green\n" +
                "property uchar blue\n" +
                "property uchar alpha\n" +
                "end_header\n");
            streamWriter.Flush();

            //Vertex and color data are written here.
            if (binary)
            {
                for (int j = 0; j < vertices.Count / 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                        binaryWriter.Write(vertices[j * 3 + k]);

                    if (colorMode == EColorMode.BGR)
                        for (int k = 2; k > -1; k--)
                        {
                            byte temp = colors[j * 3 + k];
                            binaryWriter.Write(temp);
                        }
                            

                    else if (colorMode == EColorMode.RGB)
                        for (int k = 0; k < 3; k++)
                        {
                            byte temp = colors[j * 3 + k];
                            binaryWriter.Write(temp);
                        }

                    binaryWriter.Write((byte)0);
                }
            }
            else
            {
                for (int j = 0; j < vertices.Count / 3; j++)
                {
                    string s = "";
                    for (int k = 0; k < 3; k++)
                        s += vertices[j * 3 + k].ToString(CultureInfo.InvariantCulture) + " ";

                    if(colorMode == EColorMode.BGR)
                        for (int k = 2; k > -1; k--)
                        {
                            s += colors[j * 3 + k].ToString(CultureInfo.InvariantCulture) + " ";
                        }
                            

                    if (colorMode == EColorMode.RGB)
                        for (int k = 0; k < 3; k++)
                        {
                            s += colors[j * 3 + k].ToString(CultureInfo.InvariantCulture) + " ";
                        }
                    
                    s += "0 "; //Adding an alpha color value of 0/opaque

                    streamWriter.WriteLine(s);
                }
            }
            streamWriter.Flush();
            binaryWriter.Flush();
            fileStream.Close();
        }

        public static void SaveExtrinsics(KinectSettings.ExtrinsicsStyle extrinsicsStyle, string filePath, List<KinectSocket> kinectSockets)
        {
            Log.LogInfo("Saving Extrinsics to: " + filePath);

            switch (extrinsicsStyle)
            {
                case KinectSettings.ExtrinsicsStyle.None:
                    break;
                case KinectSettings.ExtrinsicsStyle.Open3D:
                    SaveExtrinsicsOpen3DStyle(filePath, kinectSockets);
                    break;
                case KinectSettings.ExtrinsicsStyle.OpenMVS:
                    break;
                default:
                    break;
            }
        }

        private static void SaveExtrinsicsOpen3DStyle(string filePath, List<KinectSocket> kinectSockets)
        {
            filePath += "Extrinsics_Open3D.log";

            string content = string.Empty;

            for (int i = 0; i < kinectSockets.Count; i++)
            {
                content += "0\t" + kinectSockets[i].configuration.SerialNumber + "\t" + kinectSockets[i].configuration.globalDeviceIndex + Environment.NewLine;

                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        content += kinectSockets[i].oCameraPose.mat[j, k].ToString("G11", CultureInfo.InvariantCulture) + "\t";
                    }
                }

                content += Environment.NewLine;
            }

            File.WriteAllText(filePath, content);
        }

        public static bool SaveMarkerPoses(string path, List<MarkerPose> poses)
        {
            string content = "";

            for (int i = 0; i < poses.Count; i++)
            {
                float rX, rY, rZ;
                poses[i].GetOrientation(out rX, out rY, out rZ);

                content += "Marker ID " + i + Environment.NewLine;
                
                content += "Translation" + 
                    "\tX: " + poses[i].pose.mat[0,3].ToString(CultureInfo.InvariantCulture) + 
                    "\tY: " + poses[i].pose.mat[1, 3].ToString(CultureInfo.InvariantCulture) + 
                    "\tZ: " + poses[i].pose.mat[2, 3].ToString(CultureInfo.InvariantCulture) + 
                    Environment.NewLine;
                
                content += "Rotation" + 
                    "\tX: " + rX.ToString(CultureInfo.InvariantCulture) + 
                    "\tY: " + rY.ToString(CultureInfo.InvariantCulture) + 
                    "\tZ: " + rZ.ToString(CultureInfo.InvariantCulture) + 
                    Environment.NewLine;
                content += Environment.NewLine;
            }

            try
            {
                File.WriteAllText(path, content);
            }

            catch(Exception e)
            {
                return false;
            }

            return true;

        }

        public static List<MarkerPose> LoadMarkerPoses(string path)
        {
            List<MarkerPose> poses = new List<MarkerPose>();

            StreamReader sr = File.OpenText(path);

            while (!sr.EndOfStream)
            {
                MarkerPose pose = new MarkerPose();
                string line = "";

                line = sr.ReadLine();
                string[] id = line.Split(' ');
                line = sr.ReadLine();
                string[] translation = line.Split(new char[] {' ', '\t'});
                line = sr.ReadLine();
                string[] rotation = line.Split(new char[] { ' ', '\t' });
                sr.ReadLine();


                try
                {
                    pose.id = Int32.Parse(id[2]);
                    pose.pose.mat[0, 3] = float.Parse(translation[2], CultureInfo.InvariantCulture);
                    pose.pose.mat[1, 3] = float.Parse(translation[4], CultureInfo.InvariantCulture);
                    pose.pose.mat[2, 3] = float.Parse(translation[6], CultureInfo.InvariantCulture);
                    pose.SetOrientation(
                        float.Parse(rotation[2], CultureInfo.InvariantCulture), 
                        float.Parse(rotation[4], CultureInfo.InvariantCulture), 
                        float.Parse(rotation[6], CultureInfo.InvariantCulture)
                        );
                }

                catch(Exception e)
                {
                    Log.LogError("Could not load Marker Poses from disk! File corrupt");
                    return null;
                }

                poses.Add(pose);
            }

            return poses;
        }
    }
}