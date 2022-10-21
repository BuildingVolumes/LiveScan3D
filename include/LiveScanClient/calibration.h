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
#pragma once
#include "stdafx.h"
#include "marker.h"
#include "utils.h"

vector<float> RotatePoint(vector<float> &point, std::vector<std::vector<float>> &R);
vector<float> InverseRotatePoint(vector<float> &point, std::vector<std::vector<float>> &R);

struct MarkerPose
{
	int markerId;
	Matrix4x4 pose;
};

class Calibration
{
public:
	Matrix4x4 markerToWorldTransform; //The transform mapping from the marker position to the camera
	vector<MarkerPose> markerPoses; //How the marker iteself is positioned in the world
	Matrix4x4 refinementTransform; //A refinement offset given by the ICP algorithm
	Matrix4x4 clientPose; //All transforms above affecting the sensor pose added together
	int iUsedMarkerId;


	bool bCalibrated;

	Calibration();
	~Calibration();

	bool Calibrate(cv::Mat *colorMat, Point3f *pCameraCoordinates, int cColorWidth, int cColorHeight);
	bool LoadCalibration(const string &serialNumber);
	void SaveCalibration(const string &serialNumber);
	void UpdateClientPose();

private:
	IMarker *pDetector;
	int nSampleCounter;
	int nRequiredSamples;

	vector<vector<Point3f>> marker3DSamples;

	Matrix4x4 Procrustes(MarkerInfo &marker, vector<Point3f> &markerInWorld);
	bool GetMarkerCorners3D(vector<Point3f> &marker3D, MarkerInfo &marker, Point3f *pCameraCoordinates, int cColorWidth, int cColorHeight);
};

