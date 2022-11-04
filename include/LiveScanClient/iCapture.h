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

#include <k4a/k4atypes.h>
#include <KinectConfiguration.h>
#include "k4a/k4a.h"
#include "opencv2/core.hpp"
#include <stdint.h>

struct Joint
{

};

struct Body
{
	Body()
	{
		bTracked = false;
		vJoints.resize(5);
		vJointsInColorSpace.resize(5);
	}
	bool bTracked;
	std::vector<Joint> vJoints;
	std::vector<Point2f> vJointsInColorSpace;
};

class ICapture
{
public:
	ICapture();
	virtual ~ICapture();

	virtual bool Initialize(KinectConfiguration& configuration) = 0;
	virtual void SetManualDeviceIndex(int index) = 0; //Only used for testing devices for now
	virtual bool AquireRawFrame() = 0;
	//virtual bool AquirePointcloudFrame() = 0;
	virtual void DecodeRawColor() = 0;
	virtual void DownscaleColorImgToDepthImgSize() = 0;
	virtual bool Close() = 0;

	virtual void MapDepthToColor() = 0;
	virtual void GeneratePointcloud() = 0;
	virtual void PointCloudImageToPoint3f(Point3f* pCameraSpacePoints) = 0;

	virtual int GetSyncJackState() = 0;
	virtual uint64_t GetTimeStamp() = 0;
	virtual void SetExposureState(bool enableAutoExposure, int exposureStep) = 0;
	virtual void SetWhiteBalanceState(bool enableAutoWhiteBalance, int kelvin) = 0;
	virtual bool GetIntrinsicsJSON(std::vector<uint8_t>& calibration_buffer, size_t& calibration_size) = 0;
	virtual void SetConfiguration(KinectConfiguration& configuration) = 0;

	bool bInitialized;

	int nColorFrameHeight, nColorFrameWidth;

	k4a_image_t colorImageMJPG;
	k4a_image_t depthImage16Int;
	k4a_image_t transformedDepthImage;
	k4a_image_t pointCloudImage;
	cv::Mat colorBGR;

	std::vector<uint8_t> calibrationBuffer;
	size_t nCalibrationSize;

    uint8_t* pBodyIndex;
	std::vector<Body> vBodies;
	std::string serialNumber;
};