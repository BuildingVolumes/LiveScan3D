#pragma once
#include "ICapture.h"
#include <opencv2/opencv.hpp>
#include "turbojpeg/turbojpeg.h"
#include <chrono>

class AzureKinectCapture : public ICapture
{
public:
	AzureKinectCapture();
	~AzureKinectCapture();

	virtual bool OpenDevice();
	virtual bool StartDevice(KinectConfiguration& configuration);
	virtual void SetLogger(Log* logger);
	virtual void SetManualDeviceIndex(int index);
	virtual bool AquireRawFrame();
	void DecodeRawColor();
	void DownscaleColorImgToDepthImgSize();
	void MapDepthToColor();
	void GeneratePointcloud();
	void PointCloudImageToPoint3f(Point3f* pCameraSpacePoints);
	virtual bool Close();	

	virtual int GetSyncJackState();
	virtual uint64_t GetTimeStamp();
	virtual void SetExposureState(bool enableAutoExposure, int exposureStep);
	virtual bool GetIntrinsicsJSON(std::vector<uint8_t>& calibration_buffer, size_t& calibration_size);
	virtual void SetConfiguration(KinectConfiguration& configuration);
	virtual void SetWhiteBalanceState(bool enableAutoBalance, int kelvin);
	std::string GetSerial();

protected:
	k4a_device_t kinectSensor = NULL;
	int32_t captureTimeoutMs = 1000;
	k4a_image_t depthImageInColor = NULL;
	k4a_image_t colorImageDownscaled = NULL;
	k4a_transformation_t transformationColorDownscaled = NULL;
	k4a_transformation_t transformation = NULL;  
	LogBuffer logBuffer;
	Log* log;

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