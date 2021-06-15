#pragma once

#include "stdafx.h"
#include "ICapture.h"
#include <k4a/k4a.h>
#include <k4a/k4atypes.h>
#include <opencv2/opencv.hpp>
#include "utils.h"
#include <opencv2/core.hpp>
#include <opencv2/imgproc.hpp>

class AzureKinectCapture : public ICapture
{
public:
	AzureKinectCapture();
	~AzureKinectCapture();

	bool Initialize(KinectConfiguration& configuration);
	bool AquireRawFrame();
	bool AquirePointcloudFrame();
	bool Close();
	void MapDepthFrameToCameraSpace(Point3f *pCameraSpacePoints);
	void MapColorFrameToCameraSpace(Point3f *pCameraSpacePoints);
	void MapDepthFrameToColorSpace(UINT16 *pDepthInColorSpace);
	void MapColorFrameToDepthSpace(RGB *pColorInDepthSpace);
	int GetSyncJackState();
	uint64_t GetTimeStamp();
	int GetDeviceIndex();
	void SetExposureState(bool enableAutoExposure, int exposureStep);
	bool GetIntrinsicsJSON(std::vector<uint8_t>& calibration_buffer, size_t& calibration_size);
	void SetConfiguration(KinectConfiguration& configuration);

private:
	k4a_device_t kinectSensor = NULL;
	int32_t captureTimeoutMs = 1000;
	k4a_image_t pointCloudImage = NULL;
	k4a_image_t transformedDepthImage = NULL;
	k4a_image_t colorImageInDepth = NULL;
	k4a_image_t depthImageInColor = NULL;
	k4a_image_t colorImageDownscaled = NULL;
  k4a_transformation_t transformationColorDownscaled = NULL;
  k4a_transformation_t transformation = NULL;

	int colorImageDownscaledWidth;
	int colorImageDownscaledHeight;

	bool syncInConnected = false;
	bool syncOutConnected = false;
	uint64_t currentTimeStamp = 0;
	int deviceIDForRestart = -1;
	int restartAttempts = 0;
	bool autoExposureEnabled = true;
	int exposureTimeStep = 0;
  
	void UpdateDepthPointCloud();
	void UpdateDepthPointCloudForColorFrame();

	KinectConfiguration configuration;
};