#pragma once
#include "azureKinectCapture.h"
#include <stdlib.h>

class AzureKinectCaptureVirtual : public AzureKinectCapture
{
public:
	AzureKinectCaptureVirtual();
	~AzureKinectCaptureVirtual();

	int GetAndLockDeviceIndex();
	void ReleaseDeviceIndexLock();
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

	std::vector<k4a_image_t> m_vVirtualColorImageSequence;
	std::vector<uint8_t*> m_vVirtualColorImagesBuffer;

	std::vector<k4a_image_t> m_vVirtualDepthImageSequence;

	long m_lLastFrameTimeus;
};


