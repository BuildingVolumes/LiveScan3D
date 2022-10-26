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

#include "calibration.h"
#include "opencv2\opencv.hpp"

#include <fstream>

Calibration::Calibration()
{
	bCalibrated = false;
	nSampleCounter = 0;
	nRequiredSamples = 20;

	sensorToMarkerTransform = Matrix4x4::GetIdentity();
	currentMarkerPose = Matrix4x4::GetIdentity();
	markerOffsetTransform = Matrix4x4::GetIdentity();
	refinementTransform = Matrix4x4::GetIdentity();
	worldTransform = Matrix4x4::GetIdentity();

	pDetector = new MarkerDetector();
}

Calibration::~Calibration()
{
	if (pDetector != NULL)
	{
		delete pDetector;
		pDetector = NULL;
	}
}

bool Calibration::Calibrate(cv::Mat* colorMat, Point3f* pCameraCoordinates, int cColorWidth, int cColorHeight)
{
	MarkerInfo marker;

	refinementTransform = Matrix4x4::GetIdentity(); //Reset the refinement

	bool res = pDetector->GetMarker(colorMat, marker); //Find the biggest marker in the image
	if (!res)
		return false;

	int indexInPoses = -1;

	//Check if the marker we've found is also enabled by the user
	for (unsigned int j = 0; j < markerPoses.size(); j++)
	{
		if (marker.id == markerPoses[j].markerId)
		{
			indexInPoses = j;
			break;
		}
	}
	if (indexInPoses == -1)
		return false;

	MarkerPose markerPose = markerPoses[indexInPoses];
	iUsedMarkerId = markerPose.markerId;

	//Get the Marker Corner coordinates in 3D camera space
	vector<Point3f> marker3D(marker.corners.size());
	bool success = GetMarkerCorners3D(marker3D, marker, pCameraCoordinates, cColorWidth, cColorHeight);

	if (!success)
	{
		return false;
	}

	marker3DSamples.push_back(marker3D);
	nSampleCounter++;

	//Get at least 20 samples (frames) of the marker coordinates, before acutally using them for calibration. 
	//Makes marker detection more robust
	if (nSampleCounter < nRequiredSamples)
		return false;

	//Average out the samples
	for (size_t i = 0; i < marker3D.size(); i++)
	{
		marker3D[i] = Point3f();
		for (int j = 0; j < nRequiredSamples; j++)
		{
			marker3D[i].X += marker3DSamples[j][i].X / (float)nRequiredSamples;
			marker3D[i].Y += marker3DSamples[j][i].Y / (float)nRequiredSamples;
			marker3D[i].Z += marker3DSamples[j][i].Z / (float)nRequiredSamples;
		}
	}

	//Try to find out how the marker is rotated and translated in 3D space
	sensorToMarkerTransform = Procrustes(marker, marker3D);

	//The T from the matrix we got from the procustes doesn't have the roation
	//factor applied yet, so we do this here
	sensorToMarkerTransform = sensorToMarkerTransform.GetR() * sensorToMarkerTransform.GetT();	

	UpdateClientPose();

	bCalibrated = true;

	marker3DSamples.clear();
	nSampleCounter = 0;

	return true;
}

void Calibration::UpdateClientPose()
{
	//Get the currently used marker pose from the list of markers
	//The rotation is inversed so that a positive value in the offset field will give us a clockwise rotation
	currentMarkerPose = markerPoses[iUsedMarkerId].pose;
	currentMarkerPose.SetR(currentMarkerPose.GetR().Inverse());

	//We first apply the Marker Rotation, for user convinience. 
	markerOffsetTransform = currentMarkerPose.GetR() * sensorToMarkerTransform;

	//We then apply the Marker translation. As this should happen in world space, we don't
	//rotate the translation factor
	markerOffsetTransform = markerPoses[iUsedMarkerId].pose.GetT() * markerOffsetTransform;

	//Apply the refinement pose from the ICP algorithm to the global pose
	worldTransform = refinementTransform.GetT() * markerOffsetTransform;
	worldTransform = refinementTransform.GetR().Inverse() * worldTransform;
}

bool Calibration::LoadCalibration(const string& serialNumber)
{
	ifstream file;
	file.open("calibration_" + serialNumber + ".txt");
	if (!file.is_open())
		return false;

	for (int i = 0; i < 4; i++)
	{
		for (int j = 0; j < 4; j++)
		{
			file >> sensorToMarkerTransform.mat[i][j];
		}
	}

	for (int i = 0; i < 4; i++)
	{
		for (int j = 0; j < 4; j++)
		{
			file >> refinementTransform.mat[i][j];
		}
	}

	file >> iUsedMarkerId;
	file >> bCalibrated;

	return true;
}

void Calibration::SaveCalibration(const string& serialNumber)
{
	ofstream file;
	file.open("calibration_" + serialNumber + ".txt");

	for (int i = 0; i < 4; i++)
	{
		for (int j = 0; j < 4; j++)
		{
			file << sensorToMarkerTransform.mat[i][j];
			file << " ";
		}

		file << endl;
	}

	file << endl;

	for (int i = 0; i < 4; i++)
	{
		for (int j = 0; j < 4; j++)
		{
			file << refinementTransform.mat[i][j];
			file << " ";
		}

		file << endl;
	}

	file << iUsedMarkerId << endl;
	file << bCalibrated << endl;

	file.close();
}

Matrix4x4 Calibration::Procrustes(MarkerInfo& marker, vector<Point3f>& markerInWorld)
{
	int nVertices = marker.points.size(); //nVertices = 5
	Matrix4x4 worldToMarker = Matrix4x4::GetIdentity();

	Point3f markerCenterInWorld;
	Point3f markerCenter;
	for (int i = 0; i < nVertices; i++)
	{
		markerCenterInWorld.X += markerInWorld[i].X / nVertices;
		markerCenterInWorld.Y += markerInWorld[i].Y / nVertices;
		markerCenterInWorld.Z += markerInWorld[i].Z / nVertices;

		markerCenter.X += marker.points[i].X / nVertices;
		markerCenter.Y += marker.points[i].Y / nVertices;
		markerCenter.Z += marker.points[i].Z / nVertices;
	}

	worldToMarker.mat[0][3] = -markerCenterInWorld.X;
	worldToMarker.mat[1][3] = -markerCenterInWorld.Y;
	worldToMarker.mat[2][3] = -markerCenterInWorld.Z;

	vector<Point3f> markerInWorldTranslated(nVertices);
	vector<Point3f> markerTranslated(nVertices);
	for (int i = 0; i < nVertices; i++)
	{
		markerInWorldTranslated[i].X = markerInWorld[i].X + worldToMarker.mat[0][3];
		markerInWorldTranslated[i].Y = markerInWorld[i].Y + worldToMarker.mat[1][3];
		markerInWorldTranslated[i].Z = markerInWorld[i].Z + worldToMarker.mat[2][3];

		markerTranslated[i].X = marker.points[i].X - markerCenter.X;
		markerTranslated[i].Y = marker.points[i].Y - markerCenter.Y;
		markerTranslated[i].Z = marker.points[i].Z - markerCenter.Z;
	}

	cv::Mat A(nVertices, 3, CV_64F);
	cv::Mat B(nVertices, 3, CV_64F);

	for (int i = 0; i < nVertices; i++)
	{
		A.at<double>(i, 0) = markerTranslated[i].X;
		A.at<double>(i, 1) = markerTranslated[i].Y;
		A.at<double>(i, 2) = markerTranslated[i].Z;

		B.at<double>(i, 0) = markerInWorldTranslated[i].X;
		B.at<double>(i, 1) = markerInWorldTranslated[i].Y;
		B.at<double>(i, 2) = markerInWorldTranslated[i].Z;
	}

	cv::Mat M = A.t() * B;

	cv::SVD svd;
	svd(M);
	cv::Mat R = svd.u * svd.vt;

	double det = cv::determinant(R);

	if (det < 0)
	{
		cv::Mat temp = cv::Mat::eye(3, 3, CV_64F);
		temp.at<double>(2, 2) = -1;
		R = svd.u * temp * svd.vt;
	}

	for (int i = 0; i < 3; i++)
	{
		for (int j = 0; j < 3; j++)
		{
			worldToMarker.mat[i][j] = static_cast<float>(R.at<double>(i, j));
		}
	}

	return worldToMarker;
}

bool Calibration::GetMarkerCorners3D(vector<Point3f>& marker3D, MarkerInfo& marker, Point3f* pCameraCoordinates, int cColorWidth, int cColorHeight)
{
	for (unsigned int i = 0; i < marker.corners.size(); i++)
	{
		int minX = static_cast<int>(marker.corners[i].X);
		int maxX = minX + 1;
		int minY = static_cast<int>(marker.corners[i].Y);
		int maxY = minY + 1;

		float dx = marker.corners[i].X - minX;
		float dy = marker.corners[i].Y - minY;

		Point3f pointMin = pCameraCoordinates[minX + minY * cColorWidth];
		Point3f pointXMaxYMin = pCameraCoordinates[maxX + minY * cColorWidth];
		Point3f pointXMinYMax = pCameraCoordinates[minX + maxY * cColorWidth];
		Point3f pointMax = pCameraCoordinates[maxX + maxY * cColorWidth];

		if (pointMin.Z <= 0 || pointXMaxYMin.Z <= 0 || pointXMinYMax.Z <= 0 || pointMax.Z <= 0)
			return false;

		marker3D[i].X = (1 - dx) * (1 - dy) * pointMin.X + dx * (1 - dy) * pointXMaxYMin.X + (1 - dx) * dy * pointXMinYMax.X + dx * dy * pointMax.X;
		marker3D[i].Y = (1 - dx) * (1 - dy) * pointMin.Y + dx * (1 - dy) * pointXMaxYMin.Y + (1 - dx) * dy * pointXMinYMax.Y + dx * dy * pointMax.Y;
		marker3D[i].Z = (1 - dx) * (1 - dy) * pointMin.Z + dx * (1 - dy) * pointXMaxYMin.Z + (1 - dx) * dy * pointXMinYMax.Z + dx * dy * pointMax.Z;
	}

	return true;
}
