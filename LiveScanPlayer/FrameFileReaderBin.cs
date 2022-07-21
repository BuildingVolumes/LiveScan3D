using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LiveScanPlayer
{
    class FrameFileReaderBin : IFrameFileReader
    {
        BinaryReader binaryReader;
        int currentFrameIdx = 0;
        int totalFrameCount = 0;
        string filename;


        public FrameFileReaderBin(string filename)
        {
            this.filename = filename;
            binaryReader = new BinaryReader(File.Open(this.filename, FileMode.Open));
            GetTotalFrameCount();
        }

        public int frameIdx
        {
            get
            {
                return currentFrameIdx;
            }
            set
            {
                JumpToFrame(value);
            }
        }

        public int totalFrames
        {
            get
            {
                return totalFrameCount;
            }
        }

        public void ReadFrame(List<float> vertices, List<byte> colors)
        {
            if (binaryReader.BaseStream.Position >= binaryReader.BaseStream.Length)
                return;

            string[] lineParts = ReadLine().Split(' ');
            int nPoints = Int32.Parse(lineParts[1]);
            lineParts = ReadLine().Split(' ');
            int frameTimestamp = Int32.Parse(lineParts[1]);

            short[] tempVertices = new short[3 * nPoints];
            byte[] tempColors = new byte[4 * nPoints];

            int bytesPerVertexPoint = 3 * sizeof(short);
            int bytesPerColorPoint = 4 * sizeof(byte);
            int bytesPerPoint = bytesPerVertexPoint + bytesPerColorPoint;
            
            byte[] frameData = binaryReader.ReadBytes(bytesPerPoint * nPoints);

            if (frameData.Length < bytesPerPoint * nPoints)
            {
                return;
            }

            int vertexDataSize = nPoints * bytesPerVertexPoint;
            int colorDataSize = nPoints * bytesPerColorPoint;
            Buffer.BlockCopy(frameData, 0, tempVertices, 0, vertexDataSize);
            Buffer.BlockCopy(frameData, vertexDataSize, tempColors, 0, colorDataSize);

            for (int i = 0; i < nPoints; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    vertices.Add(tempVertices[3 * i + j] / 1000.0f);
                }

                //We convert from BGR colorspace to RGB colorspace
                colors.Add(tempColors[4 * i + 2]);
                colors.Add(tempColors[4 * i + 1]);
                colors.Add(tempColors[4 * i + 0]);
            }

            binaryReader.ReadByte();

            currentFrameIdx++;
        }

        /// <summary>
        /// Skips the current frame in the binaryReader. Returns false when end of file has been reached
        /// </summary>
        /// <returns></returns>
        private bool SkipFrame()
        {
            if (binaryReader.BaseStream.Position >= binaryReader.BaseStream.Length)
                return false;

            string[] lineParts = ReadLine().Split(' ');
            int nPoints = Int32.Parse(lineParts[1]);
            lineParts = ReadLine().Split(' ');

            int bytesPerVertexPoint = 3 * sizeof(short);
            int bytesPerColorPoint = 4 * sizeof(byte);
            int bytesPerPoint = bytesPerVertexPoint + bytesPerColorPoint;

            if (binaryReader.BaseStream.Position + (bytesPerPoint * nPoints) > binaryReader.BaseStream.Length)
                return false;

            binaryReader.BaseStream.Seek(bytesPerPoint * nPoints + 1, SeekOrigin.Current);

            currentFrameIdx++;

            return true;
        }

        public void JumpToFrame(int targetFrame)
        {
            int skipFrameCount = targetFrame;

            if (targetFrame == currentFrameIdx)
                return;

            //Atm we can only jump forward in time, so if the target frame is behind our current frame we need rewind and start from the beginning
            if (targetFrame < currentFrameIdx)
                Rewind();

            else
                skipFrameCount = targetFrame - currentFrameIdx;

            for (int i = 0; i < skipFrameCount; i++)
            {
                SkipFrame();
            }

        }

        public void Rewind()
        {
            currentFrameIdx = 0;
            binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);
        }

        private void GetTotalFrameCount()
        {
            long StreamPosition = binaryReader.BaseStream.Position;

            Rewind();

            int count = 0;

            while (SkipFrame())
            {
                count++;
            }

            totalFrameCount = count;

            binaryReader.BaseStream.Seek(StreamPosition, SeekOrigin.Begin);
        }

        public void CloseReader()
        {
            binaryReader.Close();
            binaryReader.Dispose();
        }

        public string ReadLine()
        {
            StringBuilder builder = new StringBuilder();
            byte buffer = binaryReader.ReadByte();

            while (buffer != '\n')
            {
                builder.Append((char)buffer);
                buffer = binaryReader.ReadByte();
            }

            return builder.ToString();
        }
    }
}
