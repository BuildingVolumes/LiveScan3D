#pragma once

#include "stdafx.h"
#include "ICapture.h"
#include <k4a/k4a.h>
#include <k4a/k4atypes.h>
#include <opencv2/opencv.hpp>
#include "utils.h"
#include <opencv2/core.hpp>
#include <opencv2/imgproc.hpp>
#include "turbojpeg/turbojpeg.h"
#include "Log.h"

class AzureKinectCapture : public ICapture
{
public:
	AzureKinectCapture();
	~AzureKinectCapture();

	bool Initialize(KinectConfiguration& configuration);
	bool AquireRawFrame();
	void DecodeRawColor();
	void DownscaleColorImgToDepthImgSize();
	void MapDepthToColor();
	void GeneratePointcloud();
	void PointCloudImageToPoint3f(Point3f* pCameraSpacePoints);
	bool Close();	

	int GetSyncJackState();
	uint64_t GetTimeStamp();
	void SetExposureState(bool enableAutoExposure, int exposureStep);
	void SetWhiteBalanceState(bool enableAutoBalance, int kelvin);
	bool GetIntrinsicsJSON(std::vector<uint8_t>& calibration_buffer, size_t& calibration_size);
	void SetConfiguration(KinectConfiguration& configuration);

private:
	k4a_device_t kinectSensor = NULL;
	int32_t captureTimeoutMs = 1000;
	k4a_image_t depthImageInColor = NULL;
	k4a_image_t colorImageDownscaled = NULL;
	k4a_transformation_t transformationColorDownscaled = NULL;
	k4a_transformation_t transformation = NULL;  
	Log& log = Log::Get();


	int colorImageDownscaledWidth;
	int colorImageDownscaledHeight;

	bool syncInConnected = false;
	bool syncOutConnected = false;
	uint64_t currentTimeStamp = 0;
	int localDeviceIndex = -1;
	int restartAttempts = 0;
	bool autoExposureEnabled = true;
	int exposureTimeStep = 0;
	bool autoWhiteBalanceEnabled = true;
	int kelvin = 5000;

	KinectConfiguration configuration;
	tjhandle turboJpeg;
};