using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KinectServer
{
    public partial class KinectConfigurationForm : Form
    {
        public KinectServer oServer;
        public KinectSettings oSettings;
        public int socketID;
        
        public KinectSocket kinectSocket;
        private KinectSocket.KinectConfiguration displayedConfiguration;
        private DepthModeConfiguration SelectedDepthMode => (DepthModeConfiguration)lDepthModeListBox.SelectedItem;

        private struct DepthModeConfiguration
        {
            public string depthModeName;
            public string depthModeDetails;
            public int value;//The value that gets sent across the network. It must align with the index of the enum documented in the kinect SDK here: https://microsoft.github.io/Azure-Kinect-Sensor-SDK/master/group___enumerations_ga3507ee60c1ffe1909096e2080dd2a05d.html

            public override string ToString()
            {
                return depthModeName;
            }
        }
        public KinectConfigurationForm()
        {
            InitializeComponent();
            CreateDepthModesList();
        }

        public void Configure(KinectServer kServer, KinectSettings kSettings, int socketID)
        {
            oServer = kServer;
            oSettings = kSettings;
            this.socketID = socketID;
            kinectSocket = oServer.GetKinectSocketByIndex(socketID);
            socketStateLabel.Text = kinectSocket.sSocketState;
            Text = "Settings For " + kinectSocket.GetEndpoint();

            kinectSocket.RequestConfiguration();
            kinectSocket.configurationUpdated += UpdateFormItemsFromConfiguration;
        }

        //Will this run or do we need some kind of event listener?
        private void UpdateFormItemsFromConfiguration(KinectSocket.KinectConfiguration kc)
        {
            displayedConfiguration = kc;
            int d = kc.DepthMode;
            foreach(DepthModeConfiguration item in lDepthModeListBox.Items)
            {
                if(item.value == d)
                {
                    lDepthModeListBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void CreateDepthModesList()
        {
            lDepthModeListBox.Items.Clear();
            lDepthModeListBox.Items.Add(new DepthModeConfiguration()
            {
                depthModeName = "1024 WFOV Unbinned",
                depthModeDetails = "Depth captured at 1024x1024. Passive IR is also captured at 1024x1024.",
                value = 4
            });
            lDepthModeListBox.Items.Add(new DepthModeConfiguration()
            {
                depthModeName = "512 WFOV 2x2 Binned",
                depthModeDetails = "Depth captured at 512x512. Passive IR is also captured at 512x512.",
                value = 3
            });
            lDepthModeListBox.Items.Add(new DepthModeConfiguration()
            {
                depthModeName = "640 NFOV Unbinned",
                depthModeDetails = "Depth captured at 640x576. Passive IR is also captured at 640x576.",
                value = 2
            });
            lDepthModeListBox.Items.Add(new DepthModeConfiguration()
            {
                depthModeName = "320 NFOV 2x2 Binned",
                depthModeDetails = "Depth captured at 320x288. Passive IR is also captured at 320x288.",
                value = 1
            });
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            kinectSocket.SendConfiguration(displayedConfiguration);
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbDepthModeDetaulsLabel.Text = SelectedDepthMode.depthModeDetails;
            displayedConfiguration.DepthMode = SelectedDepthMode.value;
        }

        private void KinectSettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            oServer.SetKinectSettingsForm(socketID, null);
            kinectSocket.configurationUpdated -= UpdateFormItemsFromConfiguration;

        }
    }
}
