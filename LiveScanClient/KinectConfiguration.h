#pragma once
#include <k4a/k4atypes.h>
#include <string>
#include "utils.h"
#include <cereal/archives/json.hpp>
#include <fstream>
#include <iostream>

//Kinect Configuration holds information that may be specific to kinects.
// It does not completely describe a configuration. See azureKinectCapture for that (which contains one of these)
// This is a subset of the kinect information that may need to set over the network.
// Particularly, the settings that may be unique from camera to camera.
//The struct handles serialization to and from bytes.
struct KinectConfiguration {
public:
	KinectConfiguration();
	std::string serialNumber;
	k4a_device_configuration_t config;
	SYNC_STATE eSoftwareSyncState;
	SYNC_STATE eHardwareSyncState;
	int nSyncOffset;
	int nGlobalDeviceIndex;
	static const int byteLength = 21;//Expected length of the serialized form sent over the network. 
	bool filter_depth_map;
	int filter_depth_map_size = 5;
	char* ToBytes();
	void SetFromBytes(std::string bytes);

	void InitializeDefaults();
	int GetCameraWidth();
	int GetCameraHeight();
	void UpdateWidthAndHeight();
	void SetDepthMode(k4a_depth_mode_t depthMode);
	void SetSerialNumber(std::string serialNumber);
	//todo: exposure.


	//This allows us to use cereal to serialize our data into a json. If more data needs to be serialized, it needs to be added to this list!
	template<class Archive>
	void serialize(Archive& archive)
	{
		archive(CEREAL_NVP(serialNumber), CEREAL_NVP(config.camera_fps), CEREAL_NVP(config.color_format), CEREAL_NVP(config.color_resolution),
			CEREAL_NVP(config.depth_delay_off_color_usec), CEREAL_NVP(config.depth_mode), CEREAL_NVP(config.disable_streaming_indicator),
			CEREAL_NVP(config.subordinate_delay_off_master_usec), CEREAL_NVP(config.synchronized_images_only),
			CEREAL_NVP(config.wired_sync_mode), CEREAL_NVP(eSoftwareSyncState), CEREAL_NVP(eHardwareSyncState), CEREAL_NVP(nSyncOffset),
			CEREAL_NVP(nGlobalDeviceIndex), CEREAL_NVP(filter_depth_map), CEREAL_NVP(filter_depth_map_size));
	}

};

class KinectConfigSerializer {
public:
	KinectConfigSerializer();
	~KinectConfigSerializer();
	static std::string SerializeKinectConfig(KinectConfiguration config);
	static KinectConfiguration DeserializeKinectConfig(std::string serializedConfig);
};