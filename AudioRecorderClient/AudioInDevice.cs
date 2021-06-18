using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioRecorderClient
{
    public struct AudioInDevice
    {
        private WaveInCapabilities waveInCapabilities;
        public string productName => waveInCapabilities.ProductName;
        public int id;
        public AudioInDevice(int id, WaveInCapabilities waveInCapabilities) : this()
        {
            this.id = id;
            this.waveInCapabilities = waveInCapabilities;
        }

        public override string ToString()
        {
            return productName;
        }
    }
}
