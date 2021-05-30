using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;

namespace AudioRecorderClient
{
    public class AudioRecorder
    {
        private bool recording = false;
        WaveFileWriter writer = null;
        string outputFilePath;
        string outputFolder;
        WaveInEvent waveIn;
        private bool closing = false;
        List<AudioInDevice> audioInDevices;
        public AudioInDevice SelectedDevice;
        private NetworkClient client;

        public AudioRecorder(NetworkClient client)
        {
            this.client = client;
            client.StopRecording += StartRecording;
            client.StopRecording += StopRecording;
        }

        //recording path
        //take number
        //selected device

        public void RefreshDevicesList()
        {
            audioInDevices = new List<AudioInDevice>();
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                AudioInDevice mic = new AudioInDevice(i,WaveIn.GetCapabilities(i));
                audioInDevices.Add(mic);
            }
        }

        public List<AudioInDevice> GetDevices()
        {
            return audioInDevices;
        }

        private void InitializeAudio()
        {
            //Initialize variables
            recording = false;
            outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Blops");
            Directory.CreateDirectory(outputFolder);
            outputFilePath = Path.Combine(outputFolder, "recorded.wav");
            SetupWaveIn();
        }

        private void SetupWaveIn()
        {
            waveIn = new WaveInEvent();
            //Event Handlers.
            //limit wav file from being too large.
            waveIn.DataAvailable += (s, a) =>
            {
                writer.Write(a.Buffer, 0, a.BytesRecorded);
                if (writer.Position > waveIn.WaveFormat.AverageBytesPerSecond * 30)
                {
                    waveIn.StopRecording();
                }
            };

            //Cleanup. 
            waveIn.RecordingStopped += (s, a) =>
            {
                writer?.Dispose();
                writer = null;
                //RecordingStopped is being called when the form is closing (x), and if it is, lets cleanup.
                if (closing)
                {
                    waveIn.Dispose();
                }
            };
        }

        public void Dispose()
        {
            waveIn.Dispose();
            client.StartRecording -= StartRecording;
            client.StopRecording -= StopRecording;
        }
        private void StartRecording()
        {
            waveIn.DeviceNumber = SelectedDevice.id;
            recording = true;
            writer = new WaveFileWriter(outputFilePath, waveIn.WaveFormat);
            waveIn.StartRecording();
        }
        private void StopRecording()
        {
            waveIn.StopRecording();
            recording = false;
        }
    }
}
