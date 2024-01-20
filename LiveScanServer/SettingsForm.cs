//   Copyright (C) 2015  Marek Kowalski (M.Kowalski@ire.pw.edu.pl), Jacek Naruniec (J.Naruniec@ire.pw.edu.pl)
//   License: MIT Software License   See LICENSE.txt for the full license.

//   If you use this software in your research, then please use the following citation:

//    Kowalski, M.; Naruniec, J.; Daniluk, M.: "LiveScan3D: A Fast and Inexpensive 3D Data
//    Acquisition System for Multiple Kinect v2 Sensors". in 3D Vision (3DV), 2015 International Conference on, Lyon, France, 2015

//    @INPROCEEDINGS{Kowalski15,
//        author={Kowalski, M. and Naruniec, J. and Daniluk, M.},
//        booktitle={3D Vision (3DV), 2015 International Conference on},
//        title={LiveScan3D: A Fast and Inexpensive 3D Data Acquisition System for Multiple Kinect v2 Sensors},
//        year={2015},
//    }
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Reflection.Emit;

namespace LiveScanServer
{
    public partial class SettingsForm : Form
    {
        ClientSettings settings;
        LiveScanServer liveScanServer;
        bool lockSettingsUpdate = false;

        Image[] markerthumbs = new Image[6]
        {
            Properties.Resources.Marker_0_thumb,
            Properties.Resources.Marker_1_thumb,
            Properties.Resources.Marker_2_thumb,
            Properties.Resources.Marker_3_thumb,
            Properties.Resources.Marker_4_thumb,
            Properties.Resources.Marker_5_thumb,
        };


        public SettingsForm(ClientSettings settings, LiveScanServer liveScanServer)
        {
            this.liveScanServer = liveScanServer;
            InitializeComponent();
            UpdateUI(settings);
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        public void UpdateUI(ClientSettings settings)
        {
            this.settings = settings;

            if(!txtMinX.Focused)
                txtMinX.Text = settings.aMinBounds[0].ToString(CultureInfo.InvariantCulture);
            if (!txtMinY.Focused)
                txtMinY.Text = settings.aMinBounds[1].ToString(CultureInfo.InvariantCulture);
            if (!txtMinZ.Focused)
                txtMinZ.Text = settings.aMinBounds[2].ToString(CultureInfo.InvariantCulture);

            if (!txtMaxX.Focused)
                txtMaxX.Text = settings.aMaxBounds[0].ToString(CultureInfo.InvariantCulture);
            if (!txtMaxY.Focused)
                txtMaxY.Text = settings.aMaxBounds[1].ToString(CultureInfo.InvariantCulture);
            if (!txtMaxZ.Focused)
                txtMaxZ.Text = settings.aMaxBounds[2].ToString(CultureInfo.InvariantCulture);

            if(lisMarkers.Items.Count != settings.lMarkerPoses.Count)
            {
                lisMarkers.Items.Clear();
                for (int i = 0; i < settings.lMarkerPoses.Count; i++)
                    lisMarkers.Items.Add("Marker " + settings.lMarkerPoses[i].id);
            }

            if(lisMarkers.SelectedIndex < 0)
                lisMarkers.SelectedIndex = 0;
            UpdateMarkerFields();

            nudCompressionLvl.Value = settings.iCompressionLevel;

            txtICPIters.Text = settings.nNumICPIterations.ToString();
            txtRefinIters.Text = settings.nNumRefineIters.ToString();

            cbExtrinsicsFormat.SelectedIndex = (int)settings.eExtrinsicsFormat;

            if (settings.bSaveAsBinaryPLY)
            {
                rBinaryPly.Checked = true;
                rAsciiPly.Checked = false;
            }
            else
            {
                rBinaryPly.Checked = false;
                rAsciiPly.Checked = true;
            }

        }

        void UpdateSettings()
        {
            if (lockSettingsUpdate)
                return;

            lockSettingsUpdate = true;
            //Get the latest settings state
            ClientSettings currentSettings = liveScanServer.GetState().settings;
            settings = ApplySettings(currentSettings);
            liveScanServer.SetSettings(settings);
            UpdateUI(settings);
            Log.LogDebug("Updating settings on clients");
            lockSettingsUpdate = false;
        }

        ClientSettings ApplySettings(ClientSettings currentSettings)
        {
            //Only apply settings the we can actually change in this UI
            currentSettings.nExposureStep = settings.nExposureStep;
            currentSettings.bMergeScansForSave = settings.bMergeScansForSave;
            currentSettings.aMaxBounds = settings.aMaxBounds;
            currentSettings.aMinBounds = settings.aMinBounds;
            currentSettings.lMarkerPoses = settings.lMarkerPoses;
            currentSettings.bSaveAsBinaryPLY = settings.bSaveAsBinaryPLY;
            currentSettings.iCompressionLevel = settings.iCompressionLevel;
            currentSettings.nNumICPIterations = settings.nNumICPIterations;
            currentSettings.nNumRefineIters = settings.nNumRefineIters;
            return currentSettings;
        }

        void UpdateMarkerFields()
        {
            if (lisMarkers.SelectedIndex >= 0)
            {
                MarkerPose pose = settings.lMarkerPoses[lisMarkers.SelectedIndex];

                float X, Y, Z;
                pose.GetOrientation(out X, out Y, out Z);

                if(!txtOrientationX.Focused)
                    txtOrientationX.Text = X.ToString(CultureInfo.InvariantCulture);
                if (!txtOrientationY.Focused)
                    txtOrientationY.Text = Y.ToString(CultureInfo.InvariantCulture);
                if (!txtOrientationZ.Focused)
                    txtOrientationZ.Text = Z.ToString(CultureInfo.InvariantCulture);

                if(!txtTranslationX.Focused)
                    txtTranslationX.Text = pose.pose.mat[0, 3].ToString(CultureInfo.InvariantCulture);
                if (!txtTranslationY.Focused)
                    txtTranslationY.Text = pose.pose.mat[1, 3].ToString(CultureInfo.InvariantCulture);
                if (!txtTranslationZ.Focused)
                    txtTranslationZ.Text = pose.pose.mat[2, 3].ToString(CultureInfo.InvariantCulture);

                pMarkerThumb.Image = markerthumbs[lisMarkers.SelectedIndex];
            }
            else
            {
                txtOrientationX.Text = "";
                txtOrientationY.Text = "";
                txtOrientationZ.Text = "";

                txtTranslationX.Text = "";
                txtTranslationY.Text = "";
                txtTranslationZ.Text = "";
            }
        }



        private void txtICPIters_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(txtICPIters.Text, out settings.nNumICPIterations);
            UpdateSettings();
        }

        private void txtRefinIters_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(txtRefinIters.Text, out settings.nNumRefineIters);
            UpdateSettings();
        }

        private void lisMarkers_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSettings();
        }

        private void PlyFormat_CheckedChanged(object sender, EventArgs e)
        {
            settings.bSaveAsBinaryPLY = rBinaryPly.Checked;
            UpdateSettings();
        }

        private void nudCompressionLvl_ValueChanged(object sender, EventArgs e)
        {
            settings.iCompressionLevel = (int)nudCompressionLvl.Value;
            UpdateSettings();
        }

        private void cbExtrinsicsFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            settings.eExtrinsicsFormat = (ClientSettings.ExtrinsicsStyle)cbExtrinsicsFormat.SelectedIndex;
            UpdateSettings();
        }

        private void btSaveMarker_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Marker Pose Text files (*txt) | *.txt";
            saveFileDialog.Title = "Save Marker Pose Text file";

            DialogResult dlgres = saveFileDialog.ShowDialog();

            if (dlgres == DialogResult.OK)
            {
                lock (settings.lMarkerPoses)
                {
                    if (!Utils.SaveMarkerPoses(saveFileDialog.FileName, settings.lMarkerPoses))
                    {
                        MessageBox.Show("Could not save marker file, is the file already open in another program?", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            else if (dlgres == DialogResult.Cancel)
                return;
        }

        private void btLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Marker Pose Text files (*txt) | *.txt";
            openFileDialog.Title = "Open Marker Pose Text file";

            DialogResult dlgres = openFileDialog.ShowDialog();

            List<MarkerPose> poses = new List<MarkerPose>();

            if (dlgres == DialogResult.OK)
            {
                poses = Utils.LoadMarkerPoses(openFileDialog.FileName);

                if (poses == null)
                {
                    MessageBox.Show("Could not load Marker Poses from file. File could not be openend or corrupted data!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            else if (dlgres == DialogResult.Cancel)
                return;

            settings.lMarkerPoses = poses;
            UpdateMarkerFields();
            UpdateSettings();
        }

        private void txtOrientationX_Changed(object sender, EventArgs e)
        {
            OrientationSetValue(txtOrientationX.Text, true, false, false);
        }

        private void txtOrientationY_Changed(object sender, EventArgs e)
        {
            OrientationSetValue(txtOrientationY.Text, false, true, false);
        }

        private void txtOrientationZ_Changed(object sender, EventArgs e)
        {
            OrientationSetValue(txtOrientationZ.Text, false, false, true);
        }

        private void txtTranslationX_Changed(object sender, EventArgs e)
        {
            TranslationSetValue(txtTranslationX.Text, true, false, false);
        }

        private void txtTranslationY_Changed(object sender, EventArgs e)
        {
            TranslationSetValue(txtTranslationY.Text, false, true, false);
        }

        private void txtTranslationZ_Changed(object sender, EventArgs e)
        {
            TranslationSetValue(txtTranslationZ.Text, false, false, true);
        }

        private void txtMinX_TextChanged(object sender, EventArgs e)
        {
            Single.TryParse(txtMinX.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out settings.aMinBounds[0]);
            UpdateSettings();
        }

        private void txtMinY_TextChanged(object sender, EventArgs e)
        {
            Single.TryParse(txtMinY.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out settings.aMinBounds[1]);
            UpdateSettings();
        }

        private void txtMinZ_TextChanged(object sender, EventArgs e)
        {
            Single.TryParse(txtMinZ.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out settings.aMinBounds[2]);
            UpdateSettings();
        }

        private void txtMaxX_TextChanged(object sender, EventArgs e)
        {
            Single.TryParse(txtMaxX.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out settings.aMaxBounds[0]);
            UpdateSettings();
        }

        private void txtMaxY_TextChanged(object sender, EventArgs e)
        {
            Single.TryParse(txtMaxY.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out settings.aMaxBounds[1]);
            UpdateSettings();
        }

        private void txtMaxZ_TextChanged(object sender, EventArgs e)
        {
            Single.TryParse(txtMaxZ.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out settings.aMaxBounds[2]);
            UpdateSettings();
        }

        private void OrientationSetValue(string input, bool x, bool y, bool z)
        {
            if (lisMarkers.SelectedIndex >= 0)
            {
                MarkerPose pose = settings.lMarkerPoses[lisMarkers.SelectedIndex];
                float X, Y, Z;
                float output = 0;
                pose.GetOrientation(out X, out Y, out Z);

                if (Single.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out output))
                {
                    if (x)
                        pose.SetOrientation(output, Y, Z);
                    if (y)
                        pose.SetOrientation(X, output, Z);
                    if (z)
                        pose.SetOrientation(X, Y, output);

                    UpdateSettings();
                }
            }
        }

        private void TranslationSetValue(string input, bool x, bool y, bool z)
        {
            if (lisMarkers.SelectedIndex >= 0)
            {
                MarkerPose pose = settings.lMarkerPoses[lisMarkers.SelectedIndex];
                float output;
                Single.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out output);

                if (x)
                    pose.pose.mat[0, 3] = output;
                if (y)
                    pose.pose.mat[1, 3] = output;
                if (z)
                    pose.pose.mat[2, 3] = output;

                UpdateSettings();
            }
        }
    }
}