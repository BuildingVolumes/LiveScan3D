using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioRecorderClient
{
    public partial class RecorderWindow : Form
    {
        AudioRecorder audioRecorder;
        NetworkClient client;
        public RecorderWindow()
        {
            InitializeComponent();
            client = new NetworkClient();
            audioRecorder = new AudioRecorder(client);
            audioRecorder.RefreshDevicesList();
            UpdateList();
        }

        public void UpdateList()
        {
            deviceSelectionList.Items.Clear();
            foreach(var mic in audioRecorder.GetDevices())
            {
                deviceSelectionList.Items.Add(mic);
            }
        }

        private void deviceSelectionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            audioRecorder.SelectedDevice = (AudioInDevice)deviceSelectionList.SelectedItem;
        }

        private void RecorderWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            audioRecorder.Dispose();//cleanup
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            client.Connect(connectionTextField.Text);
        }
    }
}
