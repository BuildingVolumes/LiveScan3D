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
	AzureKinectCaptureVirtual();
	~AzureKinectCaptureVirtual();

	int GetAndLockDeviceIndex();
	void DeleteIndexLockFile();
	bool Initialize(KinectConfiguration& configuration) override;
	bool AquireRawFrame() override;	
	bool Close() override;
	int GetSyncJackState() override;
	uint64_t GetTimeStamp() override;
	void SetExposureState(bool enableAutoExposure, int exposureStep) override;
	void SetWhiteBalanceState(bool enableAutoBalance, int kelvinValue) override;
	bool GetIntrinsicsJSON(std::vector<uint8_t>& calibration_buffer, size_t& calibration_size) override;
	void SetConfiguration(KinectConfiguration& configuration);
	bool LoadColorImagesfromDisk();
	bool LoadDepthImagesfromDisk();


private:

	k4a_calibration_t m_VirtualCalibration;
	std::string m_sVirtualIntrinsicsPath = "resources/testdata/virtualdevice/intrinsics.json";

	std::vector<k4a_image_t> m_vVirtualColorImageSequence;
	std::vector<uint8_t*> m_vVirtualColorImagesBuffer;

	std::vector<k4a_image_t> m_vVirtualDepthImageSequence;

	std::chrono::system_clock::time_point m_DeviceStartTime;
	long m_lLastFrameTimeus;
};


