#include "KinectConfiguration.h"
#include <k4a/k4atypes.h>
#include <utils.h>
#include <Log.h>


	std::string sSerialNumber;
	k4a_device_configuration_t config;
	SYNC_STATE eSoftwareSyncState;
	SYNC_STATE eHardwareSyncState;
	int nSyncOffset;
	int nGlobalDeviceIndex;
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

		message[0] = (char)config.depth_mode;
		message[1] = (char)config.color_format;
		message[2] = (char)config.color_resolution;
		message[3] = (char)(int)eSoftwareSyncState; //Main = 0, Subordinate = 1, Standalone = 2, Unknown = 3
		message[4] = (char)(int)eHardwareSyncState;
		message[5] = (char)(int)nSyncOffset;
		for (int i = 6; i < serialNumberSize+6; i++) {
			message[i] = (int)serialNumber[i - 6];//ascii->char
		}
		message[19] = nGlobalDeviceIndex;
		message[20] = filter_depth_map ? 1 : 0;
		message[21] = filter_depth_map_size;

		return message;
	}

	void KinectConfiguration::SetFromBytes(std::string received)
	{
		//if length is not byteLength, throw error.
		if (received.length() != byteLength) {
			//TODO: Throw error (debug only)
		}

		int i = 0;

		//set depth mode.
		int depthMode = (int)received[i];
		//see: https://microsoft.github.io/Azure-Kinect-Sensor-SDK/master/group___enumerations_ga3507ee60c1ffe1909096e2080dd2a05d.html
		config.depth_mode = static_cast<k4a_depth_mode_t>(depthMode);
		i++;

		//Set color format (BGRA, YUV, MJPEG)
		int colorFormat = int(received[i]);
		config.color_format = static_cast<k4a_image_format_t>(colorFormat);
		i++;

		int colorRes = int(received[i]);
		config.color_resolution = static_cast<k4a_color_resolution_t>(colorRes);
		i++;

		//Certain color/depth resolutions only support 15 FPS
		if (depthMode == 4 || colorRes == 6)
			config.camera_fps = K4A_FRAMES_PER_SECOND_15;
		else
			config.camera_fps = K4A_FRAMES_PER_SECOND_30;

		//set software sync_state
		//Main = 0, Subordinate = 1, Standalone = 2, Unknown = 3
		eSoftwareSyncState = (SYNC_STATE)received[i];
		i++;

		//Hardware sync state is only set by the kinect device itself, so we skip that
		i++;

		//set sync_offset

		nSyncOffset = (int)received[i];
		i++;

		//ignore re-setting the SerialNumber
		i += serialNumberSize;

		//i == 18 at this point
		nGlobalDeviceIndex = (int)received[i];
		i++;
		filter_depth_map = ((int)received[i] == 0) ? false : true;
		i++;
		filter_depth_map_size = int(received[i]);
		//update const byteLength when changing this.

		std::ostringstream stream;
		stream << "Setting configuration: depthMode = " << depthMode << ", Software Sync State = " << eSoftwareSyncState
			   << ", Sync Offset = " << nSyncOffset << ", GlobalDeviceIndex = " << nGlobalDeviceIndex;
		Log::Get().LogDebug(stream.str());

	}

	void KinectConfiguration::InitializeDefaults()
	{
		config = K4A_DEVICE_CONFIG_INIT_DISABLE_ALL;
		config.color_format = K4A_IMAGE_FORMAT_COLOR_MJPG;
		config.camera_fps = K4A_FRAMES_PER_SECOND_30;
		config.color_resolution = K4A_COLOR_RESOLUTION_720P;
		config.depth_mode = K4A_DEPTH_MODE_NFOV_UNBINNED;
		config.synchronized_images_only = true;

		nGlobalDeviceIndex = 0;
		nSyncOffset = 0;
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
			break;

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
