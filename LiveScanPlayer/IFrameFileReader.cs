using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveScanPlayer
{
    interface IFrameFileReader
    {
        int frameIdx
        {
            get;
            set;
        }

        int totalFrames { get; } 

        void ReadFrame(List<float> vertices, List<byte> colors);

        void JumpToFrame(int frameIdx);

        void Rewind();

        void CloseReader();
    }
}
