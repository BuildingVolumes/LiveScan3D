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
        LiveScanServer liveScanServer;        
        KinectSocket kinectSocket;
        public KinectConfiguration displayedConfiguration;
       
        public KinectConfigurationForm(LiveScanServer liveScanServer, KinectConfiguration configuration)
        {
            InitializeComponent();

            this.liveScanServer = liveScanServer;
            this.Text = "Loading Configuration...";

            CreateDepthResList();
            CreateColorResList();

            displayedConfiguration = configuration;
            UpdateUI(displayedConfiguration);
        }

        //TODO: We need to update the configuration if it has changed
        private void UpdateUI(KinectConfiguration kc)
        {
            // Invoke UI logic on the same thread.
            this.BeginInvoke(
                new Action(() =>
                {
                    this.Text = "Configuration for device: " + kc.SerialNumber;
                    this.Update(); 
                    cbFilterDepthMap.Checked = kc.FilterDepthMap;
                    nDepthFilterSize.Value = kc.FilterDepthMapSize;
                    
                    //Disable changing depth map filtering if we are in raw frames mode.
                    //Depth filtering only works in point cloud mode.

                    lbDepthRes.SelectedIndex = (int)kc.eDepthRes - 1;

                    //Swab some values so that the list looks neater (4:3 and 16:9 split)
                    int colorRes = (int)kc.eColorRes;
                    if (colorRes == 4)
                        colorRes = 5;
                    else if (colorRes == 5)
                        colorRes = 4;

                    lbColorRes.SelectedIndex = colorRes - 1;

                    this.Update();
                }
        ));
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

            KinectConfiguration currentConfig = null;
            LiveScanState currentState = liveScanServer.GetState();

            //Search for our Config in all configs
            for (int i = 0; i < currentState.clients.Count; i++)
            {
                if (displayedConfiguration.SerialNumber == currentState.clients[i].configuration.SerialNumber)
                    currentConfig = currentState.clients[i].configuration;
            }

            if(currentConfig == null)
            {
                Log.LogError("Configuration could not be updated, the client is probably disconnected, serial: " + displayedConfiguration.SerialNumber);
                MessageBox.Show("Configuration could not be updated, the client is probably disconnected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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
        KinectConfiguration ApplyConfiguration(KinectConfiguration applyTo, KinectConfiguration applyFrom)
        {
            applyTo.FilterDepthMap = applyFrom.FilterDepthMap;
            applyTo.FilterDepthMapSize = applyFrom.FilterDepthMapSize;
            applyTo.eDepthRes = applyFrom.eDepthRes;
            applyTo.eColorRes = applyFrom.eColorRes;

            return applyTo;
        }

        private void lbDepthRes_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayedConfiguration.eDepthRes = (KinectConfiguration.depthResolution)lbDepthRes.SelectedIndex + 1;
        }

        private void lbColorRes_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selected = lbColorRes.SelectedIndex + 1;

            //We swap some values so that it looks neater on the displayed list
            if (selected == 4)
                selected = 5;
            else if (selected == 5)
                selected = 4;

            displayedConfiguration.eColorRes = (KinectConfiguration.colorResolution)selected;
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
