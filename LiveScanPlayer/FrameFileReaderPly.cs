using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LiveScanPlayer
{
    enum VertexType { vFloat, vDouble };

    class FrameFileReaderPly : IFrameFileReader
    {
        string[] filenames;
        int currentFrameIdx = 0;
        int totalFrameCount = 0;

        public FrameFileReaderPly(string[] filenames)
        {
            this.filenames = filenames;
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
                return filenames.Length;
            }
        }

        public void ReadFrame(List<float> vertices, List<byte> colors)
        {
            if (currentFrameIdx >= filenames.Length)
                return;

            BinaryReader reader = new BinaryReader(new FileStream(filenames[currentFrameIdx], FileMode.Open));

            bool alpha = false;
            VertexType vertexType = VertexType.vFloat;
            string line = ReadLine(reader);
            while (!line.Contains("element vertex"))
                line = ReadLine(reader);
            string[] lineElems = line.Split(' ');
            int nPoints = Int32.Parse(lineElems[2]);
            while (!line.Contains("end_header"))
            {
                if (line.Contains("alpha"))
                    alpha = true;
                if(line.Contains("double"))
                    vertexType = VertexType.vDouble;
                if(line.Contains("float"))
                    vertexType = VertexType.vFloat;
                line = ReadLine(reader);
            }

            for (int i = 0; i < nPoints; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if(vertexType == VertexType.vFloat)
                        vertices.Add(reader.ReadSingle());
                    if (vertexType == VertexType.vDouble)
                        vertices.Add((float)reader.ReadDouble());
                }
                for (int j = 0; j < 3; j++)
                {
                    colors.Add(reader.ReadByte());
                }
                //read additional alpha byte
                if (alpha)
                    reader.ReadByte();
            }

            reader.Dispose();

            currentFrameIdx++;
            if (currentFrameIdx >= filenames.Length)
                currentFrameIdx = 0;
        }

        public void JumpToFrame(int frameIdx)
        {
            currentFrameIdx = frameIdx;

        }

        public void Rewind()
        {
            currentFrameIdx = 0;
        }

        public void CloseReader()
        {
            //Not needed here
        }

        public string ReadLine(BinaryReader binaryReader)
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
