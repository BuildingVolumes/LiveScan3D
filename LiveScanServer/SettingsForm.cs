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

namespace KinectServer
{
    public partial class SettingsForm : Form
    {
        public KinectSettings oSettings;
        public KinectServer oServer;
        bool bFormLoaded = false;
        bool bSendSettingsLock = false;

        Image[] markerthumbs = new Image[6]
        {
            Properties.Resources.Marker_0_thumb,
            Properties.Resources.Marker_1_thumb,
            Properties.Resources.Marker_2_thumb,
            Properties.Resources.Marker_3_thumb,
            Properties.Resources.Marker_4_thumb,
            Properties.Resources.Marker_5_thumb,
        };


        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            txtMinX.Text = oSettings.aMinBounds[0].ToString(CultureInfo.InvariantCulture);
            txtMinY.Text = oSettings.aMinBounds[1].ToString(CultureInfo.InvariantCulture);
            txtMinZ.Text = oSettings.aMinBounds[2].ToString(CultureInfo.InvariantCulture);

            txtMaxX.Text = oSettings.aMaxBounds[0].ToString(CultureInfo.InvariantCulture);
            txtMaxY.Text = oSettings.aMaxBounds[1].ToString(CultureInfo.InvariantCulture);
            txtMaxZ.Text = oSettings.aMaxBounds[2].ToString(CultureInfo.InvariantCulture);

            for (int i = 0; i < oSettings.lMarkerPoses.Count; i++)
                lisMarkers.Items.Add("Marker " + oSettings.lMarkerPoses[i].id);

            lisMarkers.SelectedIndex = 0;
            UpdateMarkerFields();

            cbCompressionLevel.SelectedText = oSettings.iCompressionLevel.ToString();

            txtICPIters.Text = oSettings.nNumICPIterations.ToString();
            txtRefinIters.Text = oSettings.nNumRefineIters.ToString();

            cbExtrinsicsFormat.SelectedIndex = (int)oSettings.eExtrinsicsFormat;

            if (oSettings.bSaveAsBinaryPLY)
            {
                rBinaryPly.Checked = true;
                rAsciiPly.Checked = false;
            }
            else
            {
                rBinaryPly.Checked = false;
                rAsciiPly.Checked = true;
            }

            bFormLoaded = true;
        }

        void UpdateClients()
        {
            if (bFormLoaded && !UpdateClientsBackgroundWorker.IsBusy && !bSendSettingsLock)
            {
                Cursor.Current = Cursors.WaitCursor;

                oServer.SendSettings();

                Log.LogDebug("Updating settings on clients");

                Cursor.Current = Cursors.Default;
            }
        }

        void SettingsChanged()
        {
            UpdateClients();
        }

        void UpdateMarkerFields()
        {
            //Otherwise this would trigger the textbox_text_changed event, which would lead to updating the clients
            //multiple times in a row
            bSendSettingsLock = true;

            if (lisMarkers.SelectedIndex >= 0)
            {
                MarkerPose pose = oSettings.lMarkerPoses[lisMarkers.SelectedIndex];

                float X, Y, Z;
                pose.GetOrientation(out X, out Y, out Z);

                txtOrientationX.Text = X.ToString(CultureInfo.InvariantCulture);
                txtOrientationY.Text = Y.ToString(CultureInfo.InvariantCulture);
                txtOrientationZ.Text = Z.ToString(CultureInfo.InvariantCulture);

                txtTranslationX.Text = pose.pose.mat[0,3].ToString(CultureInfo.InvariantCulture);
                txtTranslationY.Text = pose.pose.mat[1,3].ToString(CultureInfo.InvariantCulture);
                txtTranslationZ.Text = pose.pose.mat[2,3].ToString(CultureInfo.InvariantCulture);

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

            bSendSettingsLock = false;
        }

        private void txtMinX_TextChanged(object sender, EventArgs e)
        {
            Single.TryParse(txtMinX.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out oSettings.aMinBounds[0]);
            SettingsChanged();
        }

        private void txtMinY_TextChanged(object sender, EventArgs e)
        {
            Single.TryParse(txtMinY.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out oSettings.aMinBounds[1]);
            SettingsChanged();
        }

        private void txtMinZ_TextChanged(object sender, EventArgs e)
        {
            Single.TryParse(txtMinZ.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out oSettings.aMinBounds[2]);
            SettingsChanged();
        }

        private void txtMaxX_TextChanged(object sender, EventArgs e)
        {
            Single.TryParse(txtMaxX.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out oSettings.aMaxBounds[0]);
            SettingsChanged();
        }

        private void txtMaxY_TextChanged(object sender, EventArgs e)
        {
            Single.TryParse(txtMaxY.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out oSettings.aMaxBounds[1]);
            SettingsChanged();
        }

        private void txtMaxZ_TextChanged(object sender, EventArgs e)
        {
            Single.TryParse(txtMaxZ.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out oSettings.aMaxBounds[2]);
            SettingsChanged();
        }

        private void txtICPIters_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(txtICPIters.Text, out oSettings.nNumICPIterations);
        }

        private void txtRefinIters_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(txtRefinIters.Text, out oSettings.nNumRefineIters);
        }

        private void lisMarkers_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMarkerFields();
        }


        private string OrientationSetValue(string input, bool x, bool y, bool z)
        {

            if (lisMarkers.SelectedIndex >= 0)
            {
                MarkerPose pose = oSettings.lMarkerPoses[lisMarkers.SelectedIndex];
                float X, Y, Z;
                float output = 0;
                pose.GetOrientation(out X, out Y, out Z);

                if (input == "" || input == "-" || input == ".")
                    input = "0";

                if (Single.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out output))
                {
                    if (x)
                        pose.SetOrientation(output, Y, Z);
                    if (y)
                        pose.SetOrientation(X, output, Z);
                    if (z)
                        pose.SetOrientation(X, Y, output);

                    SettingsChanged();
                    return output.ToString();
                }               
            }

            return input;
        }

        private string TranslationSetValue(string input, bool x, bool y, bool z)
        {
            if (lisMarkers.SelectedIndex >= 0)
            {
                if (input == "" || input == "-" || input == ".")
                    input = "0";

                MarkerPose pose = oSettings.lMarkerPoses[lisMarkers.SelectedIndex];
                float output;
                Single.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out output);

                if (x)
                    pose.pose.mat[0,3] = output;
                if (y)
                    pose.pose.mat[1, 3] = output;
                if (z)
                    pose.pose.mat[2, 3] = output;

                SettingsChanged();

                return output.ToString();
            }

            return "0";
        }

        private void PlyFormat_CheckedChanged(object sender, EventArgs e)
        {
            if (rAsciiPly.Checked)
            {
                oSettings.bSaveAsBinaryPLY = false;
            }
            else
            {
                oSettings.bSaveAsBinaryPLY = true;
            }
        }

        private void cbCompressionLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = cbCompressionLevel.SelectedIndex;
            if (index == 0)
                oSettings.iCompressionLevel = 0;
            else if (index == 2)
                oSettings.iCompressionLevel = 2;
            else
            {
                string value = cbCompressionLevel.SelectedItem.ToString();
                bool tryParse = Int32.TryParse(value, out oSettings.iCompressionLevel);
                if (!tryParse)
                    oSettings.iCompressionLevel = 0;
            }

            SettingsChanged();
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            oServer.SetSettingsForm(null);
        }

        private void cbExtrinsicsFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            oSettings.eExtrinsicsFormat = (KinectSettings.ExtrinsicsStyle)cbExtrinsicsFormat.SelectedIndex;
            SettingsChanged();
        }

        private void btSaveMarker_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Marker Pose Text files (*txt) | *.txt";
            saveFileDialog.Title = "Save Marker Pose Text file";

            DialogResult dlgres = saveFileDialog.ShowDialog();

            if (dlgres == DialogResult.OK)
            {
                lock (oSettings.lMarkerPoses)
                {
                    if(!Utils.SaveMarkerPoses(saveFileDialog.FileName, oSettings.lMarkerPoses))
                        oServer.fMainWindowForm.ShowWarningWindow("Could not save marker file, is the file already open in another program?");
                    return;
                }
            }

            else if(dlgres == DialogResult.Cancel)
            {
                return;
            }
        }

        private void btLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Marker Pose Text files (*txt) | *.txt";
            openFileDialog.Title = "Open Marker Pose Text file";

            bool success = false;

            DialogResult dlgres = openFileDialog.ShowDialog();

            List<MarkerPose> poses = new List<MarkerPose>();

            if (dlgres == DialogResult.OK)
            {
                lock (oSettings.lMarkerPoses)
                {
                    poses = Utils.LoadMarkerPoses(openFileDialog.FileName);

                    if (poses != null)
                    {
                        if (poses.Count == 6)
                            success = true;
                    }
                }
            }

            else if (dlgres == DialogResult.Cancel)
                return;

            if (success)
            {
                oSettings.lMarkerPoses = poses;
                UpdateMarkerFields();
                SettingsChanged();

            }

            else
            {
                oServer.fMainWindowForm.ShowWarningWindow("Could not load Marker Poses from file. File could not be openend or corrupted data!");
            }
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
    }
}