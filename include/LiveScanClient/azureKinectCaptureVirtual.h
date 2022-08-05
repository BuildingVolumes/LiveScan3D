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
	bool AquireRawFrame() override;
	
	bool Close() override;

	int GetSyncJackState() override;
	uint64_t GetTimeStamp() override;
	void SetExposureState(bool enableAutoExposure, int exposureStep) override;
	bool GetIntrinsicsJSON(std::vector<uint8_t>& calibration_buffer, size_t& calibration_size) override;
	void SetConfiguration(KinectConfiguration& configuration);

	void SetVirtualDeviceIndex(int index);
	bool LoadColorImagesfromDisk();
	bool LoadDepthImagesfromDisk();


private:

	//Here the settings for the individual Virtual Devices are stored.
	//The settings are chosen to cover as many settings as possible

	k4a_device_configuration_t virtualK4AConfigs[4] =
	{
		{ K4A_IMAGE_FORMAT_COLOR_MJPG, K4A_COLOR_RESOLUTION_720P, K4A_DEPTH_MODE_WFOV_2X2BINNED, K4A_FRAMES_PER_SECOND_30, true, 0, K4A_WIRED_SYNC_MODE_MASTER, 0, false },
		{ K4A_IMAGE_FORMAT_COLOR_MJPG, K4A_COLOR_RESOLUTION_1080P, K4A_DEPTH_MODE_NFOV_2X2BINNED, K4A_FRAMES_PER_SECOND_5 , true, 0, K4A_WIRED_SYNC_MODE_MASTER, 0, false },
		{ K4A_IMAGE_FORMAT_COLOR_MJPG, K4A_COLOR_RESOLUTION_1440P, K4A_DEPTH_MODE_WFOV_UNBINNED, K4A_FRAMES_PER_SECOND_15, true, 0, K4A_WIRED_SYNC_MODE_MASTER, 0, false },
		{ K4A_IMAGE_FORMAT_COLOR_MJPG, K4A_COLOR_RESOLUTION_1536P, K4A_DEPTH_MODE_NFOV_UNBINNED, K4A_FRAMES_PER_SECOND_30, true, 0, K4A_WIRED_SYNC_MODE_MASTER, 0, false }
	};


	KinectConfiguration virtualConfigs[4] =
	{
		KinectConfiguration("0000000000001", virtualK4AConfigs[0], Standalone, Main, 0, 0, false, 0),
		KinectConfiguration("0000000000002", virtualK4AConfigs[1], Standalone, Subordinate, 0, 0, false, 0),
		KinectConfiguration("0000000000003", virtualK4AConfigs[2], Standalone, Subordinate, 0, 0, false, 0),
		KinectConfiguration("0000000000004", virtualK4AConfigs[3], Standalone, Subordinate, 0, 0, false, 0),
	};


	k4a_calibration_t virtualCalibration;

	std::string virtualCalibrationPaths[4] =
	{
		"/test/virtualdevice1/calibration1.json",
		"/test/virtualdevice2/calibration2.json",
		"/test/virtualdevice3/calibration3.json",
		"/test/virtualdevice4/calibration4.json"
	};

	std::string virtualIntrinsicsPaths[4] =
	{
		"/test/virtualdevice1/intrinsics1.json",
		"/test/virtualdevice1/intrinsics2.json",
		"/test/virtualdevice1/intrinsics3.json",
		"/test/virtualdevice1/intrinsics4.json"
	};

	std::vector<k4a_image_t> virtualColorImageSequence;
	std::vector<uint8_t*> virtualColorImagesBuffer;

	std::string virtualColorImagePaths[4] =
	{
		"/test/virtualdevice1/colorimagedata/",
		"/test/virtualdevice2/colorimagedata/",
		"/test/virtualdevice3/colorimagedata/",
		"/test/virtualdevice4/colorimagedata/",
	};

	std::vector<k4a_image_t> virtualDepthImageSequence;

	std::string virtualDepthImagePaths[4] =
	{
		"/test/virtualdevice1/depthimagedata/",
		"/test/virtualdevice2/depthimagedata/",
		"/test/virtualdevice3/depthimagedata/",
		"/test/virtualdevice4/depthimagedata/",
	};

	std::chrono::system_clock::time_point deviceStartTime;
};


