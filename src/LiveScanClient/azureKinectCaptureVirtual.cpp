#include "azureKinectCaptureVirtual.h"
#include <fstream>
#include <iostream>

// This class simulates a Azure Kinect for Testing purposes. It is not meant to cover all parts of the hardware
// but rather to provide a basic virtual device, so that you don't need to be connected to the hardware at all times
// while developing and testing new features


void AzureKinectCaptureVirtual::SetManualDeviceIndex(int index)
{
	localDeviceIndex = index;
	virtualDeviceIndex = index % 4;
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
		k4a_calibration_get_from_raw((char*)calibrationBuffer.data(), nCalibrationSize, this->configuration.config.depth_mode, this->configuration.config.color_resolution, &calibration);
		transformation = k4a_transformation_create(&calibration);
	}

	else
	{
		log.LogFatal("Could not load calibration from disk for Virtual Azure Kinect Device!");
		return bInitialized;
	}

	//Create the downscaled image
	float rescaleRatio = (float)calibration.color_camera_calibration.resolution_height / (float)configuration.GetCameraHeight();
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
		log.LogFatal("Cannot open virtual device as images can't be loaded from the disk!");
		return bInitialized;
	}

	if (virtualColorImageSequence.size() != virtualDepthImageSequence.size())
	{
		log.LogFatal("The amount of the virtual color and depth images need to be the same!");
		return bInitialized;
	}

	deviceStartTime = std::chrono::system_clock::now();
	lastFrameTimeus = 0;

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
	Sleep(500);

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
		if ((timeStamp - lastFrameTimeus) < 66000)
		{
			return false;
		}
	}

	if (configuration.config.camera_fps == K4A_FRAMES_PER_SECOND_30)
	{
		if ((timeStamp - lastFrameTimeus) < 33000)
		{
			return false;
		}
	}	

	long framesPassed = timeStamp / 33000;

	int imageSequenceIndex = framesPassed % virtualColorImageSequence.size();

	colorImageMJPG = virtualColorImageSequence[imageSequenceIndex];
	depthImage16Int = virtualDepthImageSequence[imageSequenceIndex];

	currentTimeStamp = timeStamp;
	lastFrameTimeus = timeStamp;

	return true;

}

/// <summary>
/// Here we open up all the supplied test images on disk, which we later feed the app to simulate capture.
/// The Color images are opened as jpeg binary blob, as they would arrive the same way from the camera
/// </summary>
/// <returns></returns>
bool AzureKinectCaptureVirtual::LoadColorImagesfromDisk()
{
	for (size_t i = 0; i < virtualColorImageSequence.size(); i++)
	{
		if (virtualColorImageSequence[i] != NULL)
			k4a_image_release(virtualColorImageSequence[i]);
	}

	virtualColorImageSequence.clear();

	if (!virtualColorImagesBuffer.empty())
	{
		for (size_t i = 0; i < virtualColorImagesBuffer.size(); i++)
		{
			delete[] virtualColorImagesBuffer[i];
		}

		virtualColorImagesBuffer.clear();
	}

	bool imagefound = true;
	int imageIndex = 1;

	std::string imageDirPath = "resources/test/virtualdevice";
	imageDirPath += std::to_string(virtualDeviceIndex);
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
	default:
		log.LogFatal("No viable color resolution specified in config");
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
			virtualColorImagesBuffer.push_back((uint8_t*)imageRaw);

			if(virtualDeviceIndex == 0)
				k4a_image_create_from_buffer(K4A_IMAGE_FORMAT_COLOR_MJPG, 1280, 720, 0, virtualColorImagesBuffer[virtualColorImagesBuffer.size() - 1], imageSize, NULL, NULL, &newImage);
			if (virtualDeviceIndex == 1)
				k4a_image_create_from_buffer(K4A_IMAGE_FORMAT_COLOR_MJPG, 1920, 1080, 0, virtualColorImagesBuffer[virtualColorImagesBuffer.size() - 1], imageSize, NULL, NULL, &newImage);
			if (virtualDeviceIndex == 2)
				k4a_image_create_from_buffer(K4A_IMAGE_FORMAT_COLOR_MJPG, 2560, 1440, 0, virtualColorImagesBuffer[virtualColorImagesBuffer.size() - 1], imageSize, NULL, NULL, &newImage);
			if (virtualDeviceIndex == 3)
				k4a_image_create_from_buffer(K4A_IMAGE_FORMAT_COLOR_MJPG, 2048, 1536, 0, virtualColorImagesBuffer[virtualColorImagesBuffer.size() - 1], imageSize, NULL, NULL, &newImage);

			virtualColorImageSequence.push_back(newImage);
			imageIndex++;
		}

		else
		{
			if (imageIndex < 2)
			{
				log.LogFatal("Could not read minimum required color images from disk for virtual Kinect Device!");
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
	for (size_t i = 0; i < virtualDepthImageSequence.size(); i++)
	{
		if (virtualDepthImageSequence[i] != NULL)
			k4a_image_release(virtualDepthImageSequence[i]);
	}

	virtualDepthImageSequence.clear();	

	bool imagefound = true;
	int imageIndex = 1;

	std::string imageDirPath = "resources/test/virtualdevice";
	imageDirPath += std::to_string(virtualDeviceIndex);
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

			virtualDepthImageSequence.push_back(newImage);
			imageIndex++;
		}

		else
		{
			if (imageIndex < 2)
			{
				log.LogFatal("Could not read minimum required depth images from disk for virtual Kinect Device!");
				return false;
			}

			else
				imagefound = false;
		}
	}

	return true;

}

//Setting the exposure is not yet simulated
void AzureKinectCaptureVirtual::SetExposureState(bool enableAutoExposure, int exposureStep)
{
	if (enableAutoExposure)
		autoExposureEnabled = true;
	else
		autoExposureEnabled = false;
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
	std::ifstream calibrationFile(virtualIntrinsicsPath, std::ios::in | std::ios::binary | std::ios::ate);
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
	long timeSinceStartMS = std::chrono::duration_cast<std::chrono::milliseconds>(nowTime - deviceStartTime).count();

	return timeSinceStartMS * 1000; //Converted to picoseconds
}

