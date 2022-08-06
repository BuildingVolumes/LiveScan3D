#pragma once
#include "azureKinectCapture.h"
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
#include <stdlib.h>

class AzureKinectCaptureVirtual : public AzureKinectCapture
{
public:
	//AzureKinectCaptureVirtual();
	//~AzureKinectCaptureVirtual();

	bool Initialize(KinectConfiguration& configuration) override;
	void SetManualDeviceIndex(int index) override;
	bool AquireRawFrame() override;
	
	bool Close() override;

	int GetSyncJackState() override;
	uint64_t GetTimeStamp() override;
	void SetExposureState(bool enableAutoExposure, int exposureStep) override;
	bool GetIntrinsicsJSON(std::vector<uint8_t>& calibration_buffer, size_t& calibration_size) override;
	void SetConfiguration(KinectConfiguration& configuration);

	bool LoadColorImagesfromDisk();
	bool LoadDepthImagesfromDisk();


private:

	//Here the settings for the individual Virtual Devices are stored.
	//The settings are chosen to cover as many settings as possible

	int virtualDeviceIndex;

	k4a_calibration_t virtualCalibration;

	std::string virtualIntrinsicsPath = "resources/test/intrinsics.json";

	std::vector<k4a_image_t> virtualColorImageSequence;
	std::vector<uint8_t*> virtualColorImagesBuffer;

	std::vector<k4a_image_t> virtualDepthImageSequence;

	std::chrono::system_clock::time_point deviceStartTime;
	long lastFrameTimeus;
};


