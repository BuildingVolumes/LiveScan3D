#include "KinectConfiguration.h"
#include <k4a/k4atypes.h>
#include <utils.h>

	std::string serialNumber;
	k4a_device_configuration_t config;
	SYNC_STATE state;
	int sync_offset;
	//No way to get the depth pixel values from the SDK at the moment, so this is hardcoded
	int depth_camera_width;
	int depth_camera_height;


	KinectConfiguration::KinectConfiguration()
	{
		InitializeDefaults();
	}



	char* KinectConfiguration::ToBytes()
	{
		//update const byteLength when changing this
		char* message = new char[byteLength];
		//add depth mode.
		char depthMode = (char)config.depth_mode;
		message[0] = depthMode;
		//add sync_state
		message[1] = (char)(int)state;
		//add sync_offset
		message[2] = (char)(int)sync_offset;
		//add color/resolution
		return message;
	}

	void KinectConfiguration::SetFromBytes(std::string received)
	{
		//if length is not byteLength, throw error.
		if (received.length() != byteLength) {
			//error
		}

		int i = 0;

		//set depth mode.
		int depthMode = (int)received[i];
		//see: https://microsoft.github.io/Azure-Kinect-Sensor-SDK/master/group___enumerations_ga3507ee60c1ffe1909096e2080dd2a05d.html
		config.depth_mode = static_cast<k4a_depth_mode_t>(depthMode);
		i++;

		//set sync_state
		//sub-mas-standalone -> 0,1,2
		state = (SYNC_STATE)received[i];
		i++;

		//set sync_offset
		sync_offset = (int)received[i];
		i++;

		//set color/resolution.


		//update const byteLength when changing this.
	}

	void KinectConfiguration::InitializeDefaults()
	{
		config = K4A_DEVICE_CONFIG_INIT_DISABLE_ALL;
		config.camera_fps = K4A_FRAMES_PER_SECOND_30;
		config.color_format = K4A_IMAGE_FORMAT_COLOR_BGRA32;
		config.color_resolution = K4A_COLOR_RESOLUTION_720P;
		config.depth_mode = K4A_DEPTH_MODE_NFOV_UNBINNED;
		config.synchronized_images_only = true;
		sync_offset = 0;
		state = Standalone;
	}

	int KinectConfiguration::GetCameraWidth()
	{
		UpdateWidthAndHeight();
		return depth_camera_width;
	}

	int KinectConfiguration::GetCameraHeight()
	{
		UpdateWidthAndHeight();
		return depth_camera_height;
	}

	void KinectConfiguration::UpdateWidthAndHeight()
	{
		switch (config.depth_mode)
		{
		case K4A_DEPTH_MODE_NFOV_UNBINNED:
			depth_camera_width = 640;
			depth_camera_height = 576;
			break;
		case K4A_DEPTH_MODE_NFOV_2X2BINNED:
			depth_camera_width = 320;
			depth_camera_height = 288;
			break;
		case K4A_DEPTH_MODE_WFOV_UNBINNED:
			depth_camera_width = 1024;
			depth_camera_height = 1024;
		case K4A_DEPTH_MODE_WFOV_2X2BINNED:
			depth_camera_width = 512;
			depth_camera_height = 512;
			break;
		default:
			break;
		}
	}

	void KinectConfiguration::SetDepthMode(k4a_depth_mode_t depthMode)
	{
		config.depth_mode = depthMode;
	}

	void KinectConfiguration::SetSerialNumber(std::string serialNumber)
	{
		this->serialNumber = serialNumber;
	}
