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
        MainWindowForm fMainWindowForm;
        KinectServer oServer;
        KinectSettings oSettings;
        int socketID;
        
        KinectSocket kinectSocket;
        KinectConfiguration displayedConfiguration;
       
        public KinectConfigurationForm()
        {
            InitializeComponent();
            CreateDepthResList();
            CreateColorResList();
        }

        public void Initialize(KinectServer kServer, KinectSettings kSettings, int socketID, MainWindowForm winForm)
        {
            fMainWindowForm = winForm;
            oServer = kServer;
            oSettings = kSettings;
            this.socketID = socketID;
            kinectSocket = oServer.GetKinectSocketByIndex(socketID);
            this.Text = "Settings for device: ";
            this.Update();

            kinectSocket.RequestConfiguration();
            kinectSocket.configurationUpdated += UpdateFormItemsFromConfiguration;
        }

        //Will this run or do we need some kind of event listener?
        private void UpdateFormItemsFromConfiguration(KinectConfiguration kc)
        {
            // Invoke UI logic on the same thread.
            this.BeginInvoke(
                new Action(() =>
                {
                    displayedConfiguration = kc;
                    this.Text = "Settings for device: " + kc.SerialNumber;
                    this.Update(); 
                    cbFilterDepthMap.Checked = kc.FilterDepthMap;
                    nDepthFilterSize.Value = kc.FilterDepthMapSize;
                    
                    //Disable changing depth map filtering if we are in raw frames mode.
                    //Depth filtering only works in point cloud mode.
                    SetDepthFilterBoxActive(oSettings.eExportMode == KinectSettings.ExportMode.Pointcloud);

                    lbDepthRes.SelectedIndex = (int)kc.eDepthRes - 1;
                    lbColorRes.SelectedIndex = (int)kc.eColorRes - 1;
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
                "2048 x 1536 (4:3)",
                "3840 x 2160 (16:9)",
                "4096 x 3072 (4:3, 15 FPS)"
            };


            lbColorRes.Items.Clear();
            lbColorRes.Items.AddRange(colorModes);
        }

        private void btApply_Click(object sender, EventArgs e)
        {
            Log.LogDebug("User changed configuration for device: " + kinectSocket.configuration.SerialNumber);

            if(oServer.SetAndConfirmConfig(kinectSocket, displayedConfiguration))
            {
                if (oServer.bTempHwSyncEnabled)
                    oServer.RestartWithTemporalSyncPattern();

                else
                    oServer.RestartClient(kinectSocket);
            }       
        }

        private void btApplyAll_Click(object sender, EventArgs e)
        {
            Log.LogDebug("User changed configuration all devices");

            //Every config might contain individual settings, so we can't just apply one config to all 

            oServer.GetConfigurations(oServer.GetClientSockets());

            List<KinectSocket> sockets = oServer.GetClientSockets();
            bool socketError = false;

            foreach (KinectSocket socket in sockets)
            {
                socket.configuration.eDepthRes = displayedConfiguration.eDepthRes;
                socket.configuration.eColorRes = displayedConfiguration.eColorRes;
                socket.configuration.FilterDepthMap = displayedConfiguration.FilterDepthMap;
                socket.configuration.FilterDepthMapSize = displayedConfiguration.FilterDepthMapSize;
                if (!oServer.SetAndConfirmConfig(socket, socket.configuration))
                    socketError = true;
            }

            if (!socketError)
            {
                if (oServer.bTempHwSyncEnabled)
                    oServer.RestartWithTemporalSyncPattern();

                else
                    oServer.RestartAllClients();
            }
        }

        private void lbDepthRes_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayedConfiguration.eDepthRes = (KinectConfiguration.depthResolution)lbDepthRes.SelectedIndex + 1;
        }

        private void lbColorRes_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayedConfiguration.eColorRes = (KinectConfiguration.colorResolution)lbColorRes.SelectedIndex + 1;
        }

        private void KinectSettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            oServer.SetKinectSettingsForm(socketID, null);
            kinectSocket.configurationUpdated -= UpdateFormItemsFromConfiguration;
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

        /// <summary>
        /// enable/disable the Set depth filter mode checkbox. This should get disabled in raw capture mode.
        /// </summary>
        internal void SetDepthFilterBoxActive(bool enabled)
        {
            cbFilterDepthMap.Enabled = enabled;
            nDepthFilterSize.Enabled = enabled;
        }

    }
}
