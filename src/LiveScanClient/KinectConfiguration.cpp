#include "KinectConfiguration.h"


std::string sSerialNumber;
k4a_device_configuration_t config;
SYNC_STATE eSoftwareSyncState;
SYNC_STATE eHardwareSyncState;
int nSyncOffset;
int nGlobalDeviceIndex;
int m_nDepthCameraWidth;
int m_nDepthCameraHeight;
int m_nColorCameraHeight;
int m_nColorCameraWidth;
bool filter_depth_map;
int filter_depth_map_size = 5;
const int serialNumberSize = 13;
const int nickNameSize = 20;

KinectConfiguration::KinectConfiguration()
{
	serialNumber = std::string(serialNumberSize, '0');
	nickname = std::string(nickNameSize, ' ');
	InitializeDefaults();
}

KinectConfiguration::KinectConfiguration(std::string serial)
{
	serialNumber = serial;
	nickname = std::string(nickNameSize, ' ');
	InitializeDefaults();
}

KinectConfiguration::KinectConfiguration(std::string serialNo, std::string name, k4a_device_configuration_t conf, SYNC_STATE softwareSyncState, SYNC_STATE hardwareSyncState, int syncOffset,
	int globalDeviceIndex, bool filterDepth, int filterDepthSize)
{
	serialNumber = serialNo;
	nickname = name;
	config = conf;
	eSoftwareSyncState = softwareSyncState;
	eHardwareSyncState = hardwareSyncState;
	nSyncOffset = syncOffset;
	nGlobalDeviceIndex = globalDeviceIndex;
	filter_depth_map = filterDepth;
	filter_depth_map_size = filterDepthSize;
}

char* KinectConfiguration::ToBytes()
{
	//update const byteLength when changing this
	char* message = new char[byteLength];
	int i = 0;

	message[i] = (char)config.depth_mode;
	i++;
	message[i] = (char)config.color_format;
	i++;
	message[i] = (char)config.color_resolution;
	i++;
	message[i] = (char)(int)eSoftwareSyncState; //Main = 0, Subordinate = 1, Standalone = 2, Unknown = 3
	i++;
	message[i] = (char)(int)eHardwareSyncState;
	i++;
	message[i] = (char)(int)nSyncOffset;
	i++;

	int c = i;

	for (; i < (serialNumberSize + c); i++)
	{
		message[i] = (int)serialNumber[i - c];//ascii->char
	}

	c = i;
	for (; i < (nickNameSize + c); i++)
	{
		message[i] = nickname[i - c];
	}

	message[i] = nGlobalDeviceIndex;
	i++;
	message[i] = filter_depth_map ? 1 : 0;
	i++;
	message[i] = filter_depth_map_size;

	return message;
}

void KinectConfiguration::SetFromBytes(char* received)
{
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

	//Set Nickname
	for (int j = 0; j < nickNameSize; j++)
	{
		nickname[j] = received[i];
		i++;
	}

	//i == 40 at this point
	nGlobalDeviceIndex = (int)received[i];
	i++;
	filter_depth_map = ((int)received[i] == 0) ? false : true;
	i++;
	filter_depth_map_size = int(received[i]);
	//update const byteLength when changing this.
}

void KinectConfiguration::Save()
{
	std::string path = "temp/configuration_" + serialNumber + ".txt";

	std::string content = nickname + "\n";
	content += std::to_string((int)config.depth_mode) + "\n";
	content += std::to_string((int)config.color_resolution) + "\n";
	content += std::to_string((int)config.camera_fps) + "\n";
	content += std::to_string((bool)filter_depth_map) + "\n";
	content += std::to_string((int)filter_depth_map_size) + "\n";

	std::ofstream configFile;
	configFile.open(path);
	configFile << content;
	configFile.close();
}

void KinectConfiguration::TryLoad()
{
	std::string path = "temp/configuration_" + serialNumber + ".txt";
	
	if (!std::filesystem::exists(path))
		return;

	std::ifstream configFile;
	configFile.open(path);
	std::stringstream stream;
	stream << configFile.rdbuf();
	std::string content(stream.str());
	configFile.close();

	std::string name = "";
	int i = 0;
	for (; i < nickNameSize; i++)
	{
		name += content[i];
	}

	nickname = name;
	i++; // Skip \n

	config.depth_mode = (k4a_depth_mode_t)(content[i] - 48);
	i+=2;
	config.color_resolution = (k4a_color_resolution_t)(content[i] - 48);
	i+=2;
	config.camera_fps = (k4a_fps_t)(content[i] - 48);
	i += 2;
	filter_depth_map = (bool)(content[i] - 48);
	i += 2;
	filter_depth_map_size = (int)(content[i] - 48);

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
	eHardwareSyncState = UnknownState;
	filter_depth_map = false;
	filter_depth_map_size = 5;
}

int KinectConfiguration::GetDepthCameraWidth()
{
	UpdateWidthAndHeight();
	return m_nDepthCameraWidth;
}

int KinectConfiguration::GetDepthCameraHeight()
{
	UpdateWidthAndHeight();
	return m_nDepthCameraHeight;
}

int KinectConfiguration::GetColorCameraWidth()
{
	UpdateWidthAndHeight();
	return m_nColorCameraWidth;
}

int KinectConfiguration::GetColorCameraHeight()
{
	UpdateWidthAndHeight();
	return m_nColorCameraHeight;

}

//No way to get the colr/depth pixel values from the SDK at the moment, so this is hardcoded
void KinectConfiguration::UpdateWidthAndHeight()
{
	switch (config.depth_mode)
	{
	case K4A_DEPTH_MODE_NFOV_UNBINNED:
		m_nDepthCameraWidth = 640;
		m_nDepthCameraHeight = 576;
		break;

	case K4A_DEPTH_MODE_NFOV_2X2BINNED:
		m_nDepthCameraWidth = 320;
		m_nDepthCameraHeight = 288;
		break;

	case K4A_DEPTH_MODE_WFOV_UNBINNED:
		m_nDepthCameraWidth = 1024;
		m_nDepthCameraHeight = 1024;
		break;

	case K4A_DEPTH_MODE_WFOV_2X2BINNED:
		m_nDepthCameraWidth = 512;
		m_nDepthCameraHeight = 512;
		break;
	default:
		break;
	}

	switch (config.color_resolution)
	{
	case K4A_COLOR_RESOLUTION_720P:
		m_nColorCameraWidth = 1280;
		m_nColorCameraHeight = 720;
		break;
	case K4A_COLOR_RESOLUTION_1080P:
		m_nColorCameraWidth = 1920;
		m_nColorCameraHeight = 1080;
		break;
	case K4A_COLOR_RESOLUTION_1440P:
		m_nColorCameraWidth = 2560;
		m_nColorCameraHeight = 1440;
		break;
	case K4A_COLOR_RESOLUTION_2160P:
		m_nColorCameraWidth = 3840;
		m_nColorCameraHeight = 2160;
		break;
	case K4A_COLOR_RESOLUTION_1536P:
		m_nColorCameraWidth = 2048;
		m_nColorCameraHeight = 1536;
		break;
	case K4A_COLOR_RESOLUTION_3072P:
		m_nColorCameraWidth = 4096;
		m_nColorCameraHeight = 3072;
		break;
	}
}

void KinectConfiguration::SetDepthMode(k4a_depth_mode_t depthMode)
{
	config.depth_mode = depthMode;
}
