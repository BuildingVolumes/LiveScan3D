#pragma once
#include "azureKinectCapture.h"
//#include <stdlib.h>
#include <fstream>
#include <iostream>
#include <filesystem>


class AzureKinectCaptureVirtual : public AzureKinectCapture
{
public:
	AzureKinectCaptureVirtual();
	~AzureKinectCaptureVirtual();

	int GetAndLockDeviceIndex();
	void ReleaseDeviceIndexLock();

	bool OpenDevice() override;
	bool StartCamera(KinectConfiguration& configuration) override;
	void StopCamera() override;
	void DisposeDevice() override;
	
	bool AquireRawFrame() override;	

	bool AquireSerialFromDevice() override;
	int GetSyncJackState() override;
	uint64_t GetTimeStamp() override;
	void SetExposureState(bool enableAutoExposure, int exposureStep) override;
	void SetWhiteBalanceState(bool enableAutoBalance, int kelvinValue) override;
	bool GetIntrinsicsJSON(std::vector<uint8_t>& calibration_buffer, size_t& calibration_size) override;
	bool LoadColorImagesfromDisk();
	bool LoadDepthImagesfromDisk();


private:

	k4a_calibration_t m_VirtualCalibration;
	std::vector<k4a_image_t> m_vVirtualColorImageSequence;
	std::vector<uint8_t*> m_vVirtualColorImagesBuffer;
	std::vector<k4a_image_t> m_vVirtualDepthImageSequence;

	HANDLE virtualDeviceSystemMutex; //Used to lock a virtual device on the system, so that no other thread/process uses it

	long m_lLastFrameTimeus;
};


