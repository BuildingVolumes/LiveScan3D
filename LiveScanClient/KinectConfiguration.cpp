#include "KinectConfiguration.h"
#include <k4a/k4atypes.h>
#include <utils.h>


	std::string sSerialNumber;
	k4a_device_configuration_t config;
	SYNC_STATE eSoftwareSyncState;
	SYNC_STATE eHardwareSyncState;
	int nSync_offset;
	int m_nDepth_camera_width;
	int m_nDepth_camera_height;
	bool filter_depth_map;
	int filter_depth_map_size = 5;
	const byte serialNumberSize = 13;

	KinectConfiguration::KinectConfiguration()
	{
		serialNumber = "0000000000000";
		InitializeDefaults();
	}

	char* KinectConfiguration::ToBytes()
	{
		//update const byteLength when changing this
		char* message = new char[byteLength];
		//add depth mode.
		char depthMode = (char)config.depth_mode;
		message[0] = depthMode;

		//Main = 0, Subordinate = 1, Standalone = 2, Unknown = 3
		//add software sync_state
		message[1] = (char)(int)eSoftwareSyncState;
		//add hardware sync_state
		message[2] = (char)(int)eHardwareSyncState;
		
		//add sync_offset
		message[3] = (char)(int)nSync_offset;

		for (int i = 4; i < serialNumberSize+4; i++) {
			message[i] = (int)serialNumber[i - 4];//ascii->char
		}
		
		//Filter Depth Map option
		message[17] = filter_depth_map ? 1 : 0;
		message[18] = filter_depth_map_size;
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


		//set software sync_state
		//Main = 0, Subordinate = 1, Standalone = 2, Unknown = 3
		eSoftwareSyncState = (SYNC_STATE)received[i];
		i++;

		//Hardware sync state is only set by the kinect device itself, so we skip that
		i++;

		//set sync_offset

		nSync_offset = (int)received[i];
		i++;

		//ignore re-setting the SerialNumber
		i += serialNumberSize;

		//i == 17 at this point
		filter_depth_map = ((int)received[i] == 0) ? false : true;
		i++;
		filter_depth_map_size = int(received[i]);
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

		nSync_offset = 0;
		eSoftwareSyncState = Standalone;
		eHardwareSyncState = Unknown;
		filter_depth_map = false;
		filter_depth_map_size = 5;
	}

	int KinectConfiguration::GetCameraWidth()
	{
		UpdateWidthAndHeight();

		return m_nDepth_camera_width;
	}

	int KinectConfiguration::GetCameraHeight()
	{
		UpdateWidthAndHeight();

		return m_nDepth_camera_height;
	}

	//No way to get the depth pixel values from the SDK at the moment, so this is hardcoded
	void KinectConfiguration::UpdateWidthAndHeight()
	{
		switch (config.depth_mode)
		{
		case K4A_DEPTH_MODE_NFOV_UNBINNED:

			m_nDepth_camera_width = 640;
			m_nDepth_camera_height = 576;
			break;
		case K4A_DEPTH_MODE_NFOV_2X2BINNED:

			m_nDepth_camera_width = 320;
			m_nDepth_camera_height = 288;
			break;
		case K4A_DEPTH_MODE_WFOV_UNBINNED:

			m_nDepth_camera_width = 1024;
			m_nDepth_camera_height = 1024;
		case K4A_DEPTH_MODE_WFOV_2X2BINNED:

			m_nDepth_camera_width = 512;
			m_nDepth_camera_height = 512;
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
