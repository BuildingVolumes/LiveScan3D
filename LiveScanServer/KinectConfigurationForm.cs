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
        private KinectConfiguration displayedConfiguration;
        private DepthModeConfiguration SelectedDepthMode => (DepthModeConfiguration)lDepthModeListBox.SelectedItem;

       
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
        private void UpdateFormItemsFromConfiguration(KinectConfiguration kc)
        {
            displayedConfiguration = kc;
            int d = (int)kc.eDepthMode;
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
            //Add the items pre-defined in ServerUtils
            foreach(var dmc in DepthModeConfiguration.DefaultDepthModes)
            {
                lDepthModeListBox.Items.Add(dmc);
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            kinectSocket.SendConfiguration(displayedConfiguration);
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbDepthModeDetaulsLabel.Text = SelectedDepthMode.depthModeDetails;
            displayedConfiguration.eDepthMode = (KinectConfiguration.depthMode)SelectedDepthMode.value;
        }

        private void KinectSettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            oServer.SetKinectSettingsForm(socketID, null);
            kinectSocket.configurationUpdated -= UpdateFormItemsFromConfiguration;
        }
    }
}
