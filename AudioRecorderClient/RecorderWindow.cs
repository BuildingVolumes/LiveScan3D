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

        private static StatusStrip statusStatusBar;
        //private delegate void OffThreadDelegate(string s);
        public static void SetStatusBarStatus(string status)
        {
            Console.Write(status);//helpful debug.
            statusStatusBar.Items[0].Text = status;
        }

        public RecorderWindow()
        {
            InitializeComponent();
            client = new NetworkClient();
            audioRecorder = new AudioRecorder(client);
            audioRecorder.RefreshDevicesList();
            statusStatusBar = statusStrip1;
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
