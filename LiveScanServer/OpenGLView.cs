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
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using LiveScanServer;

enum ECameraMode
{
    CAMERA_NONE, CAMERA_TRACK, CAMERA_DOLLY, CAMERA_ORBIT
}

public class ViewportSettings
{

    public float pointSize;
    public int brightness;
    public bool markerVisibility;
    public EColorMode colorMode = EColorMode.BGR;


    public ViewportSettings()
    {
        pointSize = 0;
        brightness = 0;
        markerVisibility = true;
        colorMode = EColorMode.RGB;
    }
}

namespace LiveScanServer
{
    public class OpenGLView
    {
        int PointCount;
        int LineCount;

        uint VBOHandle;

        VertexC4ubV3f[] VBO;
        float PointSize = 0.0f;
        ECameraMode CameraMode = ECameraMode.CAMERA_NONE;

        static float MOUSE_ORBIT_SPEED = 0.30f;     // 0 = SLOWEST, 1 = FASTEST
        static float MOUSE_DOLLY_SPEED = 0.005f;     // same as above...but much more sensitive
        static float MOUSE_TRACK_SPEED = 0.003f;    // same as above...but much more sensitive

        float g_heading = 45;
        float g_pitch = 35;
        float dx = 0.0f;
        float dy = 0.0f;

        byte brightnessModifier = 0;

        Vector2 MousePrevious = new Vector2();
        Vector2 MouseCurrent = new Vector2();
        float[] cameraPosition = new float[3];
        float[] targetPosition = new float[3];

        public List<float> vertices = new List<float>();
        public List<byte> colors = new List<byte>();
        public List<Matrix4x4> cameraPoses = new List<Matrix4x4>();
        public List<Matrix4x4> markerPoses = new List<Matrix4x4>();
        public ClientSettings settings = new ClientSettings();

        public ViewportSettings viewportSettings = new ViewportSettings();

        bool bDrawMarkings = true;

        // this struct is used for drawing
        struct VertexC4ubV3f
        {
            public byte R, G, B, A;
            public Vector3 Position;

            public static int SizeInBytes = 16;
        }

        /// <summary>
        /// Loads the OpenGLWindow specific resources
        /// </summary>
        public void Load()
        {
            Version version = new Version(GL.GetString(StringName.Version).Substring(0, 3));
            Version target = new Version(1, 5);
            if (version < target)
            {
                throw new NotSupportedException(String.Format(
                    "OpenGL {0} is required (you only have {1}).", target, version));
            }

            GL.ClearColor(.1f, 0f, .1f, 0f);
            GL.Enable(EnableCap.DepthTest);

            // Setup parameters for Points
            GL.PointSize(PointSize);
            GL.Enable(EnableCap.PointSmooth);
            GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);

            // Setup VBO state
            GL.EnableClientState(EnableCap.ColorArray);
            GL.EnableClientState(EnableCap.VertexArray);

            GL.GenBuffers(1, out VBOHandle);

            // Since there's only 1 VBO in the app, might aswell setup here.
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOHandle);
            GL.ColorPointer(4, ColorPointerType.UnsignedByte, VertexC4ubV3f.SizeInBytes, (IntPtr)0);
            GL.VertexPointer(3, VertexPointerType.Float, VertexC4ubV3f.SizeInBytes, (IntPtr)(4 * sizeof(byte)));

            PointCount = 0;
            LineCount = 12;
            VBO = new VertexC4ubV3f[PointCount + 2 * LineCount];

            cameraPosition[0] = 0;
            cameraPosition[1] = 0;
            cameraPosition[2] = 8.0f;
            targetPosition[0] = 0;
            targetPosition[1] = 0;
            targetPosition[2] = 0;

        }

        public void Unload()
        {
            GL.DeleteBuffers(1, ref VBOHandle);
        }

        /// <summary>
        /// Updates all the elements that exist inside of the 3D Space
        /// </summary>
        public void UpdateFrame()
        {
            GL.PointSize(viewportSettings.pointSize);
            brightnessModifier = (byte)viewportSettings.brightness;
            bDrawMarkings = viewportSettings.markerVisibility;
            //TargetUpdateFrequency = viewportSettings.targetPlaybackFPS;

            lock (vertices)
            {
                PointCount = vertices.Count / 3;
                LineCount = 0;
                if (bDrawMarkings)
                {
                    //bounding box
                    LineCount += 12;
                    //markers
                    LineCount += markerPoses.Count * 3;
                    //cameras
                    LineCount += cameraPoses.Count * 3;
                }

                VBO = new VertexC4ubV3f[PointCount + 2 * LineCount];

                for (int i = 0; i < PointCount; i++)
                {
                    if (viewportSettings.colorMode == EColorMode.RGB)
                    {
                        VBO[i].R = (byte)Math.Max(0, Math.Min(255, (colors[i * 3] + brightnessModifier)));
                        VBO[i].G = (byte)Math.Max(0, Math.Min(255, (colors[i * 3 + 1] + brightnessModifier)));
                        VBO[i].B = (byte)Math.Max(0, Math.Min(255, (colors[i * 3 + 2] + brightnessModifier)));
                        VBO[i].A = 255;
                    }

                    else if (viewportSettings.colorMode == EColorMode.BGR)
                    {
                        VBO[i].B = (byte)Math.Max(0, Math.Min(255, (colors[i * 3] + brightnessModifier)));
                        VBO[i].G = (byte)Math.Max(0, Math.Min(255, (colors[i * 3 + 1] + brightnessModifier)));
                        VBO[i].R = (byte)Math.Max(0, Math.Min(255, (colors[i * 3 + 2] + brightnessModifier)));
                        VBO[i].A = 255;
                    }


                    VBO[i].Position.X = vertices[i * 3];
                    VBO[i].Position.Y = vertices[i * 3 + 1];
                    VBO[i].Position.Z = vertices[i * 3 + 2];
                }
            }

            if (bDrawMarkings)
            {
                int iCurLineCount = 0;
                iCurLineCount += AddBoundingBox(PointCount + 2 * iCurLineCount);

                byte[] red = new byte[4] { 255, 0, 0, 0 };
                byte[] green = new byte[4] { 0, 255, 0, 0 };

                for (int i = 0; i < markerPoses.Count; i++)
                {
                    iCurLineCount += AddGizmo(PointCount + 2 * iCurLineCount, markerPoses[i], red);
                }
                for (int i = 0; i < cameraPoses.Count; i++)
                {
                    iCurLineCount += AddGizmo(PointCount + 2 * iCurLineCount, cameraPoses[i], green);
                }
            }
        }


        public void RenderFrame()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 lookat = Matrix4.LookAt(0, 0, 5, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);

            GL.LoadMatrix(ref lookat);

            GL.Translate(-cameraPosition[0], -cameraPosition[1], -cameraPosition[2]);
            GL.Rotate(g_pitch, 1.0f, 0.0f, 0.0f);
            GL.Rotate(g_heading, 0.0f, 1.0f, 0.0f);

            // Tell OpenGL to discard old VBO when done drawing it and reserve memory _now_ for a new buffer.
            // without this, GL would wait until draw operations on old VBO are complete before writing to it
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(VertexC4ubV3f.SizeInBytes * (PointCount + 2 * LineCount)), IntPtr.Zero, BufferUsageHint.StreamDraw);
            // Fill newly allocated buffer
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(VertexC4ubV3f.SizeInBytes * (PointCount + 2 * LineCount)), VBO, BufferUsageHint.StreamDraw);

            GL.DrawArrays(BeginMode.Points, 0, PointCount);
            GL.DrawArrays(BeginMode.Lines, PointCount, 2 * LineCount);

            GL.End();
        }

        public void Resize(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
            float aspect_ratio = Math.Max(width, 1) / (float)Math.Max(height, 1);
            Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 0.01f, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perpective);
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyCode;

            if (key == Keys.Add)
            {
                PointSize += 0.1f;
                GL.PointSize(PointSize);
            }
            if (key == Keys.Subtract)
            {
                if (PointSize != 0)
                    PointSize -= 0.1f;
                GL.PointSize(PointSize);
            }

            if (key == Keys.M)
                bDrawMarkings = !bDrawMarkings;

            if (key == Keys.O)
                brightnessModifier = (byte)Math.Max(0, brightnessModifier - 10);
            if (key == Keys.P)
                brightnessModifier = (byte)Math.Min(255, brightnessModifier + 10);
        }

        public void OnMouseWheelChanged(object sender, MouseEventArgs e)
        {
            dy = e.Delta * MOUSE_DOLLY_SPEED;

            cameraPosition[2] -= dy;

            //if (cameraPosition[2] < 0)
            //    cameraPosition[2] = 0;

        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            MouseCurrent.X = e.X;
            MouseCurrent.Y = e.Y;

            // Now use mouse_delta to move the camera

            switch (CameraMode)
            {
                case ECameraMode.CAMERA_TRACK:
                    dx = MouseCurrent.X - MousePrevious.X;
                    dx *= MOUSE_TRACK_SPEED;

                    dy = MouseCurrent.Y - MousePrevious.Y;
                    dy *= MOUSE_TRACK_SPEED;

                    cameraPosition[0] -= dx;
                    cameraPosition[1] += dy;

                    //targetPosition[0] -= dx;
                    //targetPosition[1] += dy;

                    break;

                case ECameraMode.CAMERA_DOLLY:
                    dy = MouseCurrent.Y - MousePrevious.Y;
                    dy *= MOUSE_DOLLY_SPEED;

                    cameraPosition[2] -= dy;

                    //    if (cameraPosition[2] < 0)
                    //       cameraPosition[2] = 0;

                    break;

                case ECameraMode.CAMERA_ORBIT:
                    dx = MouseCurrent.X - MousePrevious.X;
                    dx *= MOUSE_ORBIT_SPEED;

                    dy = MouseCurrent.Y - MousePrevious.Y;
                    dy *= MOUSE_ORBIT_SPEED;

                    g_heading += dx;
                    g_pitch += dy;

                    break;
            }

            MousePrevious.X = MouseCurrent.X;
            MousePrevious.Y = MouseCurrent.Y;
        }

        public void OnMouseButtonUp(object sender, MouseEventArgs e)
        {
            CameraMode = ECameraMode.CAMERA_NONE;
        }

        public void OnMouseButtonDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    CameraMode = ECameraMode.CAMERA_ORBIT;
                    break;
                case MouseButtons.Middle:
                    CameraMode = ECameraMode.CAMERA_DOLLY;
                    break;
                case MouseButtons.Right:
                    CameraMode = ECameraMode.CAMERA_TRACK;
                    break;
            }
            MousePrevious.X = e.X;
            MousePrevious.Y = e.Y;
        }



        private int AddBoundingBox(int startIdx)
        {
            int nLinesBeingAdded = 12;
            //2 points per line
            int nPointsToAdd = 2 * nLinesBeingAdded;

            for (int i = startIdx; i < startIdx + nPointsToAdd; i++)
            {
                VBO[i].R = 255;
                VBO[i].G = 255;
                VBO[i].B = 0;
                VBO[i].A = 0;
            }

            int n = 0;

            //bottom vertices
            //first vertex
            AddLine(startIdx + n, settings.aMinBounds[0], settings.aMinBounds[1], settings.aMinBounds[2],
                settings.aMaxBounds[0], settings.aMinBounds[1], settings.aMinBounds[2]);
            n += 2;
            AddLine(startIdx + n, settings.aMinBounds[0], settings.aMinBounds[1], settings.aMinBounds[2],
                settings.aMinBounds[0], settings.aMaxBounds[1], settings.aMinBounds[2]);
            n += 2;
            AddLine(startIdx + n, settings.aMinBounds[0], settings.aMinBounds[1], settings.aMinBounds[2],
                settings.aMinBounds[0], settings.aMinBounds[1], settings.aMaxBounds[2]);
            n += 2;

            //second vertex
            AddLine(startIdx + n, settings.aMaxBounds[0], settings.aMinBounds[1], settings.aMinBounds[2],
                settings.aMaxBounds[0], settings.aMaxBounds[1], settings.aMinBounds[2]);
            n += 2;
            AddLine(startIdx + n, settings.aMaxBounds[0], settings.aMinBounds[1], settings.aMinBounds[2],
                settings.aMaxBounds[0], settings.aMinBounds[1], settings.aMaxBounds[2]);
            n += 2;

            //third vertex
            AddLine(startIdx + n, settings.aMaxBounds[0], settings.aMinBounds[1], settings.aMaxBounds[2],
                settings.aMaxBounds[0], settings.aMaxBounds[1], settings.aMaxBounds[2]);
            n += 2;
            AddLine(startIdx + n, settings.aMaxBounds[0], settings.aMinBounds[1], settings.aMaxBounds[2],
                settings.aMinBounds[0], settings.aMinBounds[1], settings.aMaxBounds[2]);
            n += 2;

            //fourth vertex
            AddLine(startIdx + n, settings.aMinBounds[0], settings.aMinBounds[1], settings.aMaxBounds[2],
                settings.aMinBounds[0], settings.aMaxBounds[1], settings.aMaxBounds[2]);
            n += 2;

            //top vertices
            //fifth vertex 
            AddLine(startIdx + n, settings.aMinBounds[0], settings.aMaxBounds[1], settings.aMinBounds[2],
                settings.aMaxBounds[0], settings.aMaxBounds[1], settings.aMinBounds[2]);
            n += 2;
            AddLine(startIdx + n, settings.aMinBounds[0], settings.aMaxBounds[1], settings.aMinBounds[2],
                settings.aMinBounds[0], settings.aMaxBounds[1], settings.aMaxBounds[2]);
            n += 2;

            //sixth vertex
            AddLine(startIdx + n, settings.aMaxBounds[0], settings.aMaxBounds[1], settings.aMaxBounds[2],
                settings.aMaxBounds[0], settings.aMaxBounds[1], settings.aMinBounds[2]);
            n += 2;
            AddLine(startIdx + n, settings.aMaxBounds[0], settings.aMaxBounds[1], settings.aMaxBounds[2],
                settings.aMinBounds[0], settings.aMaxBounds[1], settings.aMaxBounds[2]);
            n += 2;

            return nLinesBeingAdded;
        }

        private int AddGizmo(int startIdx, Matrix4x4 pose, byte[] color)
        {
            int nLinesBeingAdded = 3;
            //2 points per line
            int nPointsToAdd = 2 * nLinesBeingAdded;

            for (int i = startIdx; i < startIdx + nPointsToAdd; i++)
            {
                VBO[i].R = color[0];
                VBO[i].G = color[1];
                VBO[i].B = color[2];
                VBO[i].A = color[3];
            }

            int n = 0;

            float x0 = pose.mat[0, 3];
            float y0 = pose.mat[1, 3];
            float z0 = pose.mat[2, 3];

            float x1 = 0.1f;
            float y1 = 0.1f;
            float z1 = 0.1f;

            float x2 = pose.mat[0, 0] * x1;
            float y2 = pose.mat[1, 0] * x1;
            float z2 = pose.mat[2, 0] * x1;

            x2 += pose.mat[0, 3];
            y2 += pose.mat[1, 3];
            z2 += pose.mat[2, 3];

            AddLine(startIdx + n, x0, y0, z0, x2, y2, z2);
            n += 2;

            x2 = pose.mat[0, 1] * y1;
            y2 = pose.mat[1, 1] * y1;
            z2 = pose.mat[2, 1] * y1;

            x2 += pose.mat[0, 3];
            y2 += pose.mat[1, 3];
            z2 += pose.mat[2, 3];

            AddLine(startIdx + n, x0, y0, z0, x2, y2, z2);
            n += 2;

            x2 = pose.mat[0, 2] * z1;
            y2 = pose.mat[1, 2] * z1;
            z2 = pose.mat[2, 2] * z1;

            x2 += pose.mat[0, 3];
            y2 += pose.mat[1, 3];
            z2 += pose.mat[2, 3];

            AddLine(startIdx + n, x0, y0, z0, x2, y2, z2);
            n += 2;

            return nLinesBeingAdded;
        }


        private void AddLine(int startIdx, float x0, float y0, float z0,
            float x1, float y1, float z1)
        {
            VBO[startIdx].Position.X = x0;
            VBO[startIdx].Position.Y = y0;
            VBO[startIdx].Position.Z = z0;

            VBO[startIdx + 1].Position.X = x1;
            VBO[startIdx + 1].Position.Y = y1;
            VBO[startIdx + 1].Position.Z = z1;
        }
    }
}

