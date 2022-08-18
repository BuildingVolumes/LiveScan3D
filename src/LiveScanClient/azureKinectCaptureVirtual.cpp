#include "azureKinectCaptureVirtual.h"
#include <fstream>
#include <iostream>

// This class simulates a Azure Kinect for Testing purposes. It is not meant to cover all parts of the hardware
// but rather to provide a basic virtual device, so that you don't need to be connected to the hardware at all times
// while developing and testing new features


AzureKinectCaptureVirtual::AzureKinectCaptureVirtual()
{
	turboJpeg = tjInitDecompress();
	localDeviceIndex = GetAndLockDeviceIndex();
}

AzureKinectCaptureVirtual::~AzureKinectCaptureVirtual()
{
	DeleteIndexLockFile();
}

bool AzureKinectCaptureVirtual::Initialize(KinectConfiguration& configuration)
{
	log.LogDebug("Starting Virtual Azure Kinect Device initialization");
	bInitialized = false;

	if (localDeviceIndex < 0)
	{
		log.LogFatal("Virtual Azure Kinect Device Index not set, can't initialize!");
		return bInitialized;
	}

	//Load the calibration from disk
	k4a_calibration_t calibration;

	if (GetIntrinsicsJSON(calibrationBuffer, nCalibrationSize))
	{
		k4a_calibration_get_from_raw((char*)calibrationBuffer.data(), nCalibrationSize, configuration.config.depth_mode, configuration.config.color_resolution, &calibration);
		transformation = k4a_transformation_create(&calibration);
	}

	else
	{
		log.LogFatal("Could not load calibration from disk for Virtual Azure Kinect Device!");
		return bInitialized;
	}

	//Create the downscaled image
	float rescaleRatio = (float)calibration.color_camera_calibration.resolution_height / (float)configuration.GetDepthCameraHeight();
	colorImageDownscaledWidth = calibration.color_camera_calibration.resolution_width / rescaleRatio;
	colorImageDownscaledHeight = calibration.color_camera_calibration.resolution_height / rescaleRatio;

	k4a_calibration_t calibrationColorDownscaled;
	memcpy(&calibrationColorDownscaled, &calibration, sizeof(k4a_calibration_t));
	calibrationColorDownscaled.color_camera_calibration.resolution_width /= rescaleRatio;
	calibrationColorDownscaled.color_camera_calibration.resolution_height /= rescaleRatio;
	calibrationColorDownscaled.color_camera_calibration.intrinsics.parameters.param.cx /= rescaleRatio;
	calibrationColorDownscaled.color_camera_calibration.intrinsics.parameters.param.cy /= rescaleRatio;
	calibrationColorDownscaled.color_camera_calibration.intrinsics.parameters.param.fx /= rescaleRatio;
	calibrationColorDownscaled.color_camera_calibration.intrinsics.parameters.param.fy /= rescaleRatio;
	transformationColorDownscaled = k4a_transformation_create(&calibrationColorDownscaled);

	SetConfiguration(configuration);
	serialNumber = this->configuration.serialNumber;

	if (!LoadColorImagesfromDisk() || !LoadDepthImagesfromDisk())
	{
		log.LogFatal("Cannot open virtual device, images can't be loaded from the disk!");
		return bInitialized;
	}

	if (m_vVirtualColorImageSequence.size() != m_vVirtualDepthImageSequence.size())
	{
		log.LogFatal("The amount of the virtual color and depth images need to be the same!");
		return bInitialized;
	}

	m_DeviceStartTime = std::chrono::system_clock::now();
	m_lLastFrameTimeus = 0;

	log.LogInfo("Virtual Device Initialization successful!");
	bInitialized = true;
	return bInitialized;
}

/// <summary>
/// We overwrite the values in the configuration file with values from our list of Virtual Kinect configurations.
///	These configurations aim to cover a broad base of possible settings
///	We only take non-device depedent settings from the server-provided config
/// </summary>
/// <param name="configuration"></param>
void AzureKinectCaptureVirtual::SetConfiguration(KinectConfiguration& configuration)
{
	std::string serialNumber = "000000000000";
	serialNumber += std::to_string(localDeviceIndex + 1);
	configuration.SetSerialNumber(serialNumber);
	this->configuration = configuration;
}

bool AzureKinectCaptureVirtual::Close()
{
	log.LogInfo("Closing Virtual Azure Kinect device");

	if (!bInitialized)
	{
		return false;
	}

	k4a_image_release(colorImageMJPG);
	k4a_image_release(depthImage16Int);
	k4a_image_release(pointCloudImage);
	k4a_image_release(depthImageInColor);
	k4a_image_release(transformedDepthImage);
	k4a_image_release(colorImageDownscaled);
	k4a_transformation_destroy(transformationColorDownscaled);
	k4a_transformation_destroy(transformation);

	//Simulate the hardware closing the device, ususally takes a short time
	Sleep(300);

	colorImageMJPG = NULL;
	depthImage16Int = NULL;
	pointCloudImage = NULL;
	transformedDepthImage = NULL;
	depthImageInColor = NULL;
	colorImageDownscaled = NULL;
	transformationColorDownscaled = NULL;
	transformation = NULL;

	bInitialized = false;

	return true;
}

bool AzureKinectCaptureVirtual::AquireRawFrame()
{
	if (!bInitialized)
	{
		log.LogCaptureDebug("Trying to aquire a Frame, but virtual camera is not initialized");
		return false;
	}

	long timeStamp = GetTimeStamp();

	if (configuration.config.camera_fps == K4A_FRAMES_PER_SECOND_15)
	{
		if ((timeStamp - m_lLastFrameTimeus) < 66000)
		{
			return false;
		}
	}

	if (configuration.config.camera_fps == K4A_FRAMES_PER_SECOND_30)
	{
		if ((timeStamp - m_lLastFrameTimeus) < 33000)
		{
			return false;
		}
	}	

	long framesPassed = timeStamp / 33000;

	int imageSequenceIndex = framesPassed % m_vVirtualColorImageSequence.size();

	//We have to release the depth image, as it get's copied every frame
	k4a_image_release(depthImage16Int);

	colorImageMJPG = m_vVirtualColorImageSequence[imageSequenceIndex];

	//We create a copy of the depth images, so that the buffered sequence won't be affected by possible modifications to the "aquired" image
	//We don't have to do this for the color image, as it will be decoded and duplicated anyways
	int width = k4a_image_get_width_pixels(m_vVirtualDepthImageSequence[imageSequenceIndex]);
	int height = k4a_image_get_height_pixels(m_vVirtualDepthImageSequence[imageSequenceIndex]);
	int step = k4a_image_get_stride_bytes(m_vVirtualDepthImageSequence[imageSequenceIndex]);
	k4a_image_create(K4A_IMAGE_FORMAT_DEPTH16, width, height, step, &depthImage16Int);
	memcpy(k4a_image_get_buffer(depthImage16Int), k4a_image_get_buffer(m_vVirtualDepthImageSequence[imageSequenceIndex]), step * height);

	currentTimeStamp = timeStamp;
	m_lLastFrameTimeus = timeStamp;

	return true;

}

/// <summary>
/// Here we open up all the supplied test images on disk, which we later feed the app to simulate capture.
/// The Color images are opened as jpeg binary blob, as they would arrive the same way from the camera
/// </summary>
/// <returns></returns>
bool AzureKinectCaptureVirtual::LoadColorImagesfromDisk()
{
	for (size_t i = 0; i < m_vVirtualColorImageSequence.size(); i++)
	{
		if (m_vVirtualColorImageSequence[i] != NULL)
			k4a_image_release(m_vVirtualColorImageSequence[i]);
	}

	m_vVirtualColorImageSequence.clear();

	if (!m_vVirtualColorImagesBuffer.empty())
	{
		for (size_t i = 0; i < m_vVirtualColorImagesBuffer.size(); i++)
		{
			delete[] m_vVirtualColorImagesBuffer[i];
		}

		m_vVirtualColorImagesBuffer.clear();
	}

	bool imagefound = true;
	int imageIndex = 1;

	std::string imageDirPath = "resources/testdata/virtualdevice/virtualdevice";
	imageDirPath += std::to_string(localDeviceIndex);
	imageDirPath += "/colorImages/";

	switch (this->configuration.config.color_resolution)
	{
	case K4A_COLOR_RESOLUTION_720P:
		imageDirPath += "720p/";
		break;
	case K4A_COLOR_RESOLUTION_1080P:
		imageDirPath += "1080p/";
		break;
	case K4A_COLOR_RESOLUTION_1440P:
		imageDirPath += "1440p/";
		break;
	case K4A_COLOR_RESOLUTION_2160P:
		imageDirPath += "2160p/";
		break;
	case K4A_COLOR_RESOLUTION_1536P:
		imageDirPath += "1536p/";
		break;
	case K4A_COLOR_RESOLUTION_3072P:
		imageDirPath += "3072p/";
		break;
	default:
		log.LogFatal("No valid color resolution specified in config");
		return false;
	}

	while (imagefound)
	{
		std::string imagePath = imageDirPath;
		imagePath += +"Color_";
		imagePath += std::to_string(imageIndex);
		imagePath += ".jpg";

		int imageSize;
		char* imageRaw;

		//For JPEG images we just want to load the binary data into a buffer, as the device also only returns a JPEG buffer, not structured data
		std::ifstream colorJpeg(imagePath, std::ios::in | std::ios::binary | std::ios::ate);
		if (colorJpeg.is_open())
		{
			imageSize = colorJpeg.tellg();
			imageRaw = new char[imageSize];
			colorJpeg.seekg(0, std::ios::beg);

			colorJpeg.read(imageRaw, imageSize);
			colorJpeg.close();

			k4a_image_t newImage;

			//We can't directly create a k4a_image_t with unstructured JPEG data, so we need to provide it from a buffer that is stored seperatly
			m_vVirtualColorImagesBuffer.push_back((uint8_t*)imageRaw);

			int W = configuration.GetDepthCameraWidth();
			int h = configuration.GetDepthCameraHeight();

			k4a_image_create_from_buffer(K4A_IMAGE_FORMAT_COLOR_MJPG, configuration.GetColorCameraWidth(), configuration.GetColorCameraHeight(), 0, m_vVirtualColorImagesBuffer[m_vVirtualColorImagesBuffer.size() - 1], imageSize, NULL, NULL, &newImage);

			m_vVirtualColorImageSequence.push_back(newImage);
			imageIndex++;
		}

		else
		{
			if (imageIndex < 2)
			{
				log.LogFatal("Could not read minimum required color images for virtual Device! Did you install the test git submodule?");
				log.LogFatal(imagePath);
				return false;
			}

			else
				imagefound = false;
		}
	}

	return true;
}

bool AzureKinectCaptureVirtual::LoadDepthImagesfromDisk()
{
	for (size_t i = 0; i < m_vVirtualDepthImageSequence.size(); i++)
	{
		if (m_vVirtualDepthImageSequence[i] != NULL)
			k4a_image_release(m_vVirtualDepthImageSequence[i]);
	}

	m_vVirtualDepthImageSequence.clear();	

	bool imagefound = true;
	int imageIndex = 1;

	std::string imageDirPath = "resources/testdata/virtualdevice/virtualdevice";
	imageDirPath += std::to_string(localDeviceIndex);
	imageDirPath += "/depthImages/";

	switch (this->configuration.config.depth_mode)
	{
	case K4A_DEPTH_MODE_NFOV_UNBINNED:
		imageDirPath += "NFOV_UNBINNED/";
		break;
	case K4A_DEPTH_MODE_NFOV_2X2BINNED:
		imageDirPath += "NFOV_2X2BINNED/";
		break;
	case K4A_DEPTH_MODE_WFOV_UNBINNED:
		imageDirPath += "WFOV_UNBINNED/";
		break;
	case K4A_DEPTH_MODE_WFOV_2X2BINNED:
		imageDirPath += "WFOV_2X2BINNED/";
		break;
	default:
		log.LogFatal("No viable Depth resolution specified in config");
		return false;
	}

	while (imagefound)
	{
		std::string imagePath = imageDirPath;
		imagePath += "Depth_";
		imagePath += std::to_string(imageIndex);
		imagePath += ".tiff";		

		k4a_image_t newImage;
		cv::Mat cvImgDepth = cv::imread(imagePath, cv::ImreadModes::IMREAD_ANYDEPTH);

		if (!cvImgDepth.empty())
		{
			k4a_image_create(K4A_IMAGE_FORMAT_DEPTH16, cvImgDepth.cols, cvImgDepth.rows, cvImgDepth.step[0], &newImage);
			memcpy(k4a_image_get_buffer(newImage), cvImgDepth.data, cvImgDepth.step[0] * cvImgDepth.rows);

			m_vVirtualDepthImageSequence.push_back(newImage);
			imageIndex++;
		}

		else
		{
			if (imageIndex < 2)
			{
				log.LogFatal("Could not read minimum required depth images for virtual Device! Did you install the test git submodule?");
				log.LogFatal(imagePath);
				return false;
			}

			else
				imagefound = false;
		}
	}

	return true;

}

/// <summary>
/// This function checks if any virtual devices are already in use, so that
/// </summary>
/// <returns></returns>
int AzureKinectCaptureVirtual::GetAndLockDeviceIndex()
{
	int index = 0;
	bool found = false;

	while (!found && index < 4)
	{
		std::string path = "resources/testdata/virtualdevice/virtualdevice";
		path += std::to_string(index);
		path += "/device.locked";

		std::ifstream readLockfile(path, std::ios::in | std::ios::binary);
		if (readLockfile.is_open())
		{
			index++;
			readLockfile.close();
		}

		else
		{
			found = true;
			std::ofstream lockfile(path);
			lockfile.close();
			return index;
		}
	}
	
	return -1;
}

void AzureKinectCaptureVirtual::DeleteIndexLockFile()
{
	std::string path = "resources/testdata/virtualdevice/virtualdevice";
	path += std::to_string(localDeviceIndex);
	path += "/device.locked";
	int sucess = remove(path.c_str());
}

//Setting the exposure is not yet simulated
void AzureKinectCaptureVirtual::SetExposureState(bool enableAutoExposure, int exposureStep)
{
	autoExposureEnabled = enableAutoExposure;
}

void AzureKinectCaptureVirtual::SetWhiteBalanceState(bool enableAutoBalance, int kelvinValue)
{
	autoWhiteBalanceEnabled = enableAutoBalance;
	kelvin = kelvinValue;
}

//Here, the first camera is always the master, the other ones subordinates
int AzureKinectCaptureVirtual::GetSyncJackState() 
{
	if (localDeviceIndex == 0)
		return 0;
	if (localDeviceIndex > 0)
		return 1;
}

bool AzureKinectCaptureVirtual::GetIntrinsicsJSON(std::vector<uint8_t>& calibrationBuffer, size_t& calibrationSize)
{
	std::ifstream calibrationFile(m_sVirtualIntrinsicsPath, std::ios::in | std::ios::binary | std::ios::ate);
	if (calibrationFile.is_open())
	{
		calibrationSize = calibrationFile.tellg();
		char* calibrationRaw = new char[calibrationSize];
		calibrationFile.seekg(0, std::ios::beg);

		calibrationFile.read(calibrationRaw, calibrationSize);
		calibrationFile.close();

		calibrationBuffer.clear();
		calibrationBuffer.insert(calibrationBuffer.end(), calibrationRaw, calibrationRaw + calibrationSize);

		return true;
	}

	else
		return false;
}

uint64_t AzureKinectCaptureVirtual::GetTimeStamp()
{
	//We get the time that has passed since the startup, to simulate timestamp behaviour
	std::chrono::system_clock::time_point nowTime = std::chrono::system_clock::now();
	long timeSinceStartMS = std::chrono::duration_cast<std::chrono::milliseconds>(nowTime - m_DeviceStartTime).count();

	return timeSinceStartMS * 1000; //Converted to picoseconds
}

