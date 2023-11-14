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

            this.serialnumber = serialnumber;
            UpdateUI();
        }

        //TODO: We need to update the configuration if it has changed
        public void UpdateUI()
        {
            // Invoke UI logic on the same thread.
            this.BeginInvoke(
                new Action(() =>
                {
                    ClientConfiguration newConfig = liveScanServer.GetConfigFromSerial(serialnumber);

                    this.Text = "Configuration for device: " + newConfig.SerialNumber;
                    this.Update(); 
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

                    displayedConfiguration = newConfig;


                    this.Update();
                }
        ));
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
            Log.LogDebug("User changed configuration for device: " + kinectSocket.configuration.SerialNumber);

            Cursor.Current = Cursors.WaitCursor;

            ClientConfiguration currentConfig = liveScanServer.GetConfigFromSerial(serialnumber);
            displayedConfiguration = ApplyConfiguration(currentConfig, displayedConfiguration);
            liveScanServer.SetConfiguration(displayedConfiguration);

            Cursor.Current = Cursors.Default;
        }

        private void btApplyAll_Click(object sender, EventArgs e)
        {
            Log.LogDebug("User changed configuration for all devices");

            LiveScanState currentState = liveScanServer.GetState();
            for (int i = 0; i < currentState.clients.Count; i++)
            {
                displayedConfiguration = ApplyConfiguration(currentState.clients[i].configuration, displayedConfiguration);
                liveScanServer.SetConfiguration(displayedConfiguration);
            }
        }

        

        //Only applies values that can be changed in this UI
        ClientConfiguration ApplyConfiguration(ClientConfiguration applyTo, ClientConfiguration applyFrom)
        {
            applyTo.FilterDepthMap = applyFrom.FilterDepthMap;
            applyTo.FilterDepthMapSize = applyFrom.FilterDepthMapSize;
            applyTo.eDepthRes = applyFrom.eDepthRes;
            applyTo.eColorRes = applyFrom.eColorRes;

            return applyTo;
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
