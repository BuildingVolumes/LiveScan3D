using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveScanServer
{
    public partial class ClientConfigurationForm : Form
    {
        LiveScanServer liveScanServer;
        ClientSocket kinectSocket;
        ClientConfiguration displayedConfiguration;
        public string serialnumber;

        public ClientConfigurationForm(LiveScanServer liveScanServer, string serialnumber)
        {
            InitializeComponent();

            this.liveScanServer = liveScanServer;
            this.Text = "Loading Configuration...";

            CreateDepthResList();
            CreateColorResList();

            OpenConfig(serialnumber);

        }

        public void OpenConfig(string serialnumber)
        {
            this.serialnumber = serialnumber;
            UpdateUI();
        }

        //TODO: We need to update the configuration if it has changed
        public void UpdateUI()
        {
            ClientConfiguration newConfig = liveScanServer.GetConfigFromSerial(serialnumber);
            displayedConfiguration = newConfig;

            if (newConfig.NickName[0] == ' ')
                this.Text = "Configuration for device: " + newConfig.SerialNumber;
            else
                this.Text = "Configuration for device: " + newConfig.NickName;

            cbFilterDepthMap.Checked = newConfig.FilterDepthMap;
            nDepthFilterSize.Value = newConfig.FilterDepthMapSize;

            //Disable changing depth map filtering if we are in raw frames mode.
            //Depth filtering only works in point cloud mode.

            lbDepthRes.SelectedIndex = (int)newConfig.eDepthRes - 1;

            //Swab some values so that the list looks neater (4:3 and 16:9 split)
            int colorRes = (int)newConfig.eColorRes;
            if (colorRes == 4)
                colorRes = 5;
            else if (colorRes == 5)
                colorRes = 4;

            lbColorRes.SelectedIndex = colorRes - 1;

        }

        public ClientConfiguration GetCurrentlyShownConfig()
        {
            return displayedConfiguration;
        }

        private void CreateDepthResList()
        {
            string[] depthModes = new string[]
            {
                "NFOV 320 x 288",
                "NFOV 640 x 576",
                "WFOV 512 x 512",
                "WFOV 1024 x 1024 (15 FPS)"
            };

            lbDepthRes.Items.Clear();
            lbDepthRes.Items.AddRange(depthModes);
        }

        private void CreateColorResList()
        {
            string[] colorModes = new string[]
            {
                "1280 x 720 (16:9)",
                "1920 x 1080 (16:9)",
                "2560 x 1440 (16:9)",
                "3840 x 2160 (16:9)",
                "2048 x 1536 (4:3)",
                "4096 x 3072 (4:3, 15 FPS)"
            };


            lbColorRes.Items.Clear();
            lbColorRes.Items.AddRange(colorModes);
        }

        private void btApply_Click(object sender, EventArgs e)
        {
            Log.LogDebug("Updating configuration for device: " + displayedConfiguration.SerialNumber);
            ApplyDisplayedConfiguration(false);
        }

        private void btApplyAll_Click(object sender, EventArgs e)
        {
            Log.LogDebug("Updating configuration for all devices");
            ApplyDisplayedConfiguration(true);
            
        }

        //Applies the values set in this form to the Clients
        void ApplyDisplayedConfiguration(bool applyToAll)
        {
            bool restartNeeded = false;
            List<ClientConfiguration> configs = new List<ClientConfiguration>();

            if (applyToAll)
            {
                LiveScanState currentState = liveScanServer.GetState();
                
                for (int i = 0; i < currentState.clients.Count; i++)
                {
                    configs.Add(currentState.clients[i].configuration);
                }
            }

            else
            {
                configs.Add(liveScanServer.GetConfigFromSerial(serialnumber));
            }

            //Check if a restart is needed and apply new settings
            for (int i = 0; i < configs.Count; i++)
            {
                if (configs[i].eColorRes != displayedConfiguration.eColorRes || configs[i].eDepthRes != displayedConfiguration.eDepthRes)
                    restartNeeded = true;

                configs[i].FilterDepthMap = displayedConfiguration.FilterDepthMap;
                configs[i].FilterDepthMapSize = displayedConfiguration.FilterDepthMapSize;
                configs[i].eDepthRes = displayedConfiguration.eDepthRes;
                configs[i].eColorRes = displayedConfiguration.eColorRes;
            }

            liveScanServer.SetConfigurations(configs, restartNeeded);
        }

        private void lbDepthRes_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayedConfiguration.eDepthRes = (ClientConfiguration.depthResolution)lbDepthRes.SelectedIndex + 1;
        }

        private void lbColorRes_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selected = lbColorRes.SelectedIndex + 1;

            //We swap some values so that it looks neater on the displayed list
            if (selected == 4)
                selected = 5;
            else if (selected == 5)
                selected = 4;

            displayedConfiguration.eColorRes = (ClientConfiguration.colorResolution)selected;
        }

        private void cbFilterDepthMap_CheckedChanged(object sender, EventArgs e)
        {
            displayedConfiguration.FilterDepthMap = cbFilterDepthMap.Checked;
        }

        private void nDepthFilterSize_ValueChanged(object sender, EventArgs e)
        {
            int size = (int)nDepthFilterSize.Value;

            if (size % 2 == 0)
            {
                size--;
            }

            nDepthFilterSize.Value = (decimal)size;
            displayedConfiguration.FilterDepthMapSize = size;
        }

        public void CloseConfiguration()
        {
            Invoke(new Action(() => { Close(); })); //So that we can close the form from other threads as well
        }

        private void KinectSettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
        }
    }
}
