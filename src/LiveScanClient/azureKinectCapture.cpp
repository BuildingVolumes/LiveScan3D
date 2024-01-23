#include "azureKinectCapture.h"



AzureKinectCapture::AzureKinectCapture()
{
	turboJpeg = tjInitDecompress();
}

AzureKinectCapture::~AzureKinectCapture()
{
	DisposeDevice();

	if (turboJpeg)
		tjDestroy(turboJpeg);

	if (log)
		log->UnRegisterBuffer(&logBuffer);
}

/// <summary>
/// Opens a device and registers it to this client instance, but doesn't start the capture yet
/// </summary>
/// <returns>Returns true on success, false on error</returns>
bool AzureKinectCapture::OpenDevice()
{
	logBuffer.LogDebug("Opening Azure Kinect Device");

	uint32_t count = k4a_device_get_installed_count();
	int deviceIdx = 0;

	//We save the deviceId of this Client.
	//When the cameras are reinitialized during runtime, we can then gurantee
	//that each LiveScan instance uses the same device as before (In case two or more Kinects are connected to the same PC)
	//A device ID of -1 means that no Kinects have been successfully initalized yet (only happens when the Client starts)
	if (localDeviceIndex != -1)
	{
		deviceIdx = localDeviceIndex;
	}

	kinectSensor = NULL;
	while (K4A_FAILED(k4a_device_open(deviceIdx, &kinectSensor)))
	{
		if (localDeviceIndex == -1)
		{
			deviceIdx++;
			if (deviceIdx >= count)
			{
				bOpen = false;
				logBuffer.LogError("Could not open an Azure Kinect device");
				return bOpen;
			}
		}
	}

	localDeviceIndex = deviceIdx;
	bOpen = true;

	AquireSerialFromDevice();

	return bOpen;
}

/// <summary>
/// Starts a device with the given configuration, so that we can capture images
/// </summary>
/// <param name="configuration"></param>
/// <returns>Returns true on success, false on error</returns>
bool AzureKinectCapture::StartCamera(KinectConfiguration& configuration)
{
	logBuffer.LogDebug("Starting Azure Kinect Camera");

	if (configuration.eSoftwareSyncState == Main)
	{
		configuration.config.wired_sync_mode = K4A_WIRED_SYNC_MODE_MASTER;
		logBuffer.LogInfo("Starting Azure Kinect as Main");
	}

	else if (configuration.eSoftwareSyncState == Subordinate)
	{
		logBuffer.LogInfo("Starting Azure Kinect as Subordinate");
		configuration.config.wired_sync_mode = K4A_WIRED_SYNC_MODE_SUBORDINATE;
		//Sets the offset on subordinate devices. Should be a multiple of 160, each subordinate having a different multiplier in ascending order.
		//It avoids firing the Kinects lasers at the same time.		
		configuration.config.subordinate_delay_off_master_usec = 160 * configuration.nSyncOffset;
	}

	else
	{
		logBuffer.LogInfo("Starting Azure Kinect as Standalone");
		configuration.config.wired_sync_mode = K4A_WIRED_SYNC_MODE_STANDALONE;
		configuration.config.subordinate_delay_off_master_usec = 0;
	}

	// Start the camera with the given configuration
	bStarted = K4A_SUCCEEDED(k4a_device_start_cameras(kinectSensor, &configuration.config));

	if (!bStarted)
	{
		logBuffer.LogError("Could not start Azure Kinect Device");
		return bStarted;
	}

	k4a_calibration_t calibration;
	if (K4A_FAILED(k4a_device_get_calibration(kinectSensor, configuration.config.depth_mode, configuration.config.color_resolution, &calibration)))
	{
		logBuffer.LogError("Could not get Azure Kinect Device calibration");
		bStarted = false;
		return bStarted;
	}

	//Workaround for a bug. When the camera starts in manual Exposure mode, the brightness of the RBG image
	//is much lower than in auto exposure mode. To prevent this, we first set the camera to auto exposure mode and 
	//then switch to manual mode again if it has been enabled before

	if (autoExposureEnabled == false)
	{

		logBuffer.LogDebug("Manual exposure enabled. Setting camera to auto exposure and adjust for one second as a workaround for a bug in exposure settings");

		k4a_device_set_color_control(kinectSensor, K4A_COLOR_CONTROL_EXPOSURE_TIME_ABSOLUTE, K4A_COLOR_CONTROL_MODE_AUTO, 0);

		//Give it a second to adjust
		Sleep(1000);

		SetExposureState(false, exposureTimeStep);
	}

	transformation = k4a_transformation_create(&calibration);


	//It's crucial for this program to output accurately mapped Pointclouds. The highest accuracy mapping is achieved
	//by using the k4a_transformation_depth_image_to_color_camera function. However this converts a small depth image 
	//to a larger size, equivalent to the the color image size. This means more points to process and higher processing costs
	//We can however scale the color image to the depth images size beforehand, to reduce proccesing power. 

	//We calculate the minimum size that the color Image can be, while preserving its aspect ration
	float rescaleRatio = (float)calibration.color_camera_calibration.resolution_height / (float)configuration.GetDepthCameraHeight();
	colorImageDownscaledWidth = calibration.color_camera_calibration.resolution_width / rescaleRatio;
	colorImageDownscaledHeight = calibration.color_camera_calibration.resolution_height / rescaleRatio;

	//We don't only need the size in pixels of the downscaled color image, but also a new k4a_calibration_t which fits the new 
	//sizes
	k4a_calibration_t calibrationColorDownscaled;
	memcpy(&calibrationColorDownscaled, &calibration, sizeof(k4a_calibration_t));
	calibrationColorDownscaled.color_camera_calibration.resolution_width /= rescaleRatio;
	calibrationColorDownscaled.color_camera_calibration.resolution_height /= rescaleRatio;
	calibrationColorDownscaled.color_camera_calibration.intrinsics.parameters.param.cx /= rescaleRatio;
	calibrationColorDownscaled.color_camera_calibration.intrinsics.parameters.param.cy /= rescaleRatio;
	calibrationColorDownscaled.color_camera_calibration.intrinsics.parameters.param.fx /= rescaleRatio;
	calibrationColorDownscaled.color_camera_calibration.intrinsics.parameters.param.fy /= rescaleRatio;
	transformationColorDownscaled = k4a_transformation_create(&calibrationColorDownscaled);

	//If this device is a subordinate, it is expected to start capturing at a later time (When the master has started), so we skip this check  
	//Is this really neccessary?
	if (configuration.eSoftwareSyncState != Subordinate)
	{
		std::chrono::time_point<std::chrono::system_clock> start = std::chrono::system_clock::now();
		bool bTemp;
		do
		{
			bTemp = AquireRawFrame();

			std::chrono::duration<double> elapsedSeconds = std::chrono::system_clock::now() - start;
			if (elapsedSeconds.count() > 5.0)
			{
				bStarted = false;
				break;
			}
		} while (!bTemp);
	}

	SetConfiguration(configuration); //We do this at the end, instead of the beginning, so that later we can move config logic (that doesnt require re-init, like exposure) into SetConfiguration.

	GetIntrinsicsJSON(calibrationBuffer, nCalibrationSize);

	logBuffer.LogInfo("Initialization successfull");

	return bStarted;
}

void AzureKinectCapture::StopCamera()
{
	if (!bStarted)
		return;

	logBuffer.LogInfo("Stopping Azure Kinect camera");

	k4a_device_stop_cameras(kinectSensor);

	//We release the resources here, as the might change dimensions on new start
	k4a_image_release(colorImageMJPG);
	k4a_image_release(depthImage16Int);
	k4a_image_release(pointCloudImage);
	k4a_image_release(depthImageInColor);
	k4a_image_release(transformedDepthImage);
	k4a_image_release(colorImageDownscaled);
	k4a_transformation_destroy(transformationColorDownscaled);
	k4a_transformation_destroy(transformation);

	colorImageMJPG = NULL;
	depthImage16Int = NULL;
	pointCloudImage = NULL;
	transformedDepthImage = NULL;
	depthImageInColor = NULL;
	colorImageDownscaled = NULL;
	transformationColorDownscaled = NULL;
	transformation = NULL;

	bStarted = false;
}

void AzureKinectCapture::DisposeDevice()
{
	logBuffer.LogDebug("Disposing Azure Kinect Device");

	if (!bOpen)
		return;

	StopCamera();
	k4a_device_close(kinectSensor);

	bOpen = false;
}

void AzureKinectCapture::SetConfiguration(KinectConfiguration& configuration)
{
	this->configuration = configuration;
}

void AzureKinectCapture::SetLogger(Log* logger)
{
	log = logger;
	log->RegisterBuffer(&logBuffer);
}

bool AzureKinectCapture::AquireRawFrame()
{
	if (!bStarted)
	{
		logBuffer.LogCaptureDebug("Trying to aquire a Frame, but camera has not been started");
		return false;
	}

	k4a_capture_t capture = NULL;

	k4a_wait_result_t captureResult = k4a_device_get_capture(kinectSensor, &capture, captureTimeoutMs);
	if (captureResult != K4A_WAIT_RESULT_SUCCEEDED)
	{
		k4a_capture_release(capture);
		logBuffer.LogCaptureDebug("Could not aquire frame from device");
		return false;
	}

	k4a_image_release(colorImageMJPG);
	k4a_image_release(depthImage16Int);

	colorImageMJPG = k4a_capture_get_color_image(capture);
	depthImage16Int = k4a_capture_get_depth_image(capture);

	currentTimeStamp = k4a_image_get_device_timestamp_usec(colorImageMJPG);

	if (colorImageMJPG == NULL || depthImage16Int == NULL)
	{
		k4a_capture_release(capture);
		return false;
	}

	k4a_capture_release(capture);

	return true;

}

/// <summary>
/// Decompresses the raw MJPEG image from the camera to a BGRA cvMat using TurboJpeg
/// </summary>
void AzureKinectCapture::DecodeRawColor()
{

	nColorFrameHeight = k4a_image_get_height_pixels(colorImageMJPG);
	nColorFrameWidth = k4a_image_get_width_pixels(colorImageMJPG);

	if (colorBGR.cols != nColorFrameWidth || colorBGR.rows != nColorFrameHeight) //If we use downscaling again, we need to seperate the downscaled and non-downscaled images into seperate buffers
		colorBGR = cv::Mat(nColorFrameHeight, nColorFrameWidth, CV_8UC4);

	tjDecompress2(turboJpeg, k4a_image_get_buffer(colorImageMJPG), static_cast<unsigned long>(k4a_image_get_size(colorImageMJPG)), colorBGR.data, nColorFrameWidth, 0, nColorFrameHeight, TJPF_BGRA, TJFLAG_FASTDCT | TJFLAG_FASTUPSAMPLE);

	int colorSize = colorBGR.total() * colorBGR.elemSize();
	int depthSize = k4a_image_get_size(depthImage16Int);

	/*int colorSizeKB = colorSize / 1000;
	int depthSizeKB = depthSize / 1000;
	std::cout << "Decoded color + depth size KB = " << std::to_string((colorSizeKB + depthSizeKB)) << std::endl;*/
}

void AzureKinectCapture::DownscaleColorImgToDepthImgSize()
{

	//Resize the k4a_image to the precalculated size, so that we later save on resources while transforming the image
	//--> Nice idea, however resizing takes so long, that it's not worth it. 
	//Might only be useful for very high res color recordings in Pointcloud Mode, but that doesn't make sense
	cv::resize(colorBGR, colorBGR, cv::Size(colorImageDownscaledWidth, colorImageDownscaledHeight), cv::INTER_LINEAR);

	nColorFrameHeight = colorBGR.rows;
	nColorFrameWidth = colorBGR.cols;
}


void AzureKinectCapture::MapDepthToColor()
{
	//fix depth image here
	if (configuration.filter_depth_map)
	{
		cv::Mat cImgD = cv::Mat(k4a_image_get_height_pixels(depthImage16Int), k4a_image_get_width_pixels(depthImage16Int), CV_16UC1, k4a_image_get_buffer(depthImage16Int));
		cv::Mat cImgD2 = cv::Mat::zeros(cv::Size(k4a_image_get_height_pixels(depthImage16Int), k4a_image_get_width_pixels(depthImage16Int)), CV_16UC1);// k4a_image_get_buffer(depthImage));
		cv::Mat cImgD3 = cv::Mat::zeros(cv::Size(k4a_image_get_height_pixels(depthImage16Int), k4a_image_get_width_pixels(depthImage16Int)), CV_16UC1);// k4a_image_get_buffer(depthImage));

		cv::Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(configuration.filter_depth_map_size, configuration.filter_depth_map_size));

		//cv::medianBlur(cImgD, cImgD2, 5);
		int CLOSING = 3;
		// 4 will do a good edge detection if you threshold after as well
		//cv::morphologyEx(cImgD, cImgD2, CLOSING, kernel);
		cv::erode(cImgD, cImgD, kernel);
		//cv::GaussianBlur(cImgD3, cImgD, cv::Size(configuration.filter_depth_map_size, configuration.filter_depth_map_size), 0);
	}

	if (transformedDepthImage == NULL)
	{
		k4a_image_create(K4A_IMAGE_FORMAT_DEPTH16, nColorFrameWidth, nColorFrameHeight, nColorFrameWidth * (int)sizeof(uint16_t), &transformedDepthImage);
	}

	k4a_result_t res = k4a_transformation_depth_image_to_color_camera(transformation, depthImage16Int, transformedDepthImage);

}

/// <summary>
/// Creates a Pointcloud out of the transformedDepthImage and saves it in PointcloudImage. Make sure to run MapDepthToColor before calling this function
/// </summary>
void AzureKinectCapture::GeneratePointcloud()
{

	if (pointCloudImage == NULL)
	{
		k4a_image_create(K4A_IMAGE_FORMAT_CUSTOM, nColorFrameWidth, nColorFrameHeight, nColorFrameWidth * 3 * (int)sizeof(int16_t), &pointCloudImage);
	}

	k4a_transformation_depth_image_to_point_cloud(transformation, transformedDepthImage, K4A_CALIBRATION_TYPE_COLOR, pointCloudImage);
}


///<summary>
///Translates the k4a_image_t pointcloud into a easier to handle Point3f array. Make sure to run MapDepthToColorFrame & GeneratePointcloud before calling this function
///</summary>
///<param name="pCameraSpacePoints"></param>
void AzureKinectCapture::PointCloudImageToPoint3f(Point3f* pCameraSpacePoints)
{
	int16_t* pointCloudData = (int16_t*)k4a_image_get_buffer(pointCloudImage);

	for (int i = 0; i < nColorFrameHeight; i++)
	{
		for (int j = 0; j < nColorFrameWidth; j++)
		{
			pCameraSpacePoints[j + i * nColorFrameWidth].X = pointCloudData[3 * (j + i * nColorFrameWidth) + 0] / 1000.0f;
			pCameraSpacePoints[j + i * nColorFrameWidth].Y = pointCloudData[3 * (j + i * nColorFrameWidth) + 1] / 1000.0f;
			pCameraSpacePoints[j + i * nColorFrameWidth].Z = pointCloudData[3 * (j + i * nColorFrameWidth) + 2] / 1000.0f;
		}
	}
}

/// <summary>
/// Enables/Disables Auto Exposure and/or sets the exposure to a step value between -11 and -5 
/// The kinect supports exposure values up to 1, but these are only available in lower FPS modes (5 or 15 FPS)
/// For further information on how this value translates to ms-Values, please take a look
/// at this table: https://github.com/microsoft/Azure-Kinect-Sensor-SDK/blob/develop/src/color/color_priv.h
/// </summary>
/// <param name="exposureStep">The Exposure Step between -11 and 1</param>
void AzureKinectCapture::SetExposureState(bool enableAutoExposure, int exposureStep)
{
	std::string info = "Setting Exposure. Auto exposure enabled: " + std::to_string(enableAutoExposure) + " , exposure step: " + std::to_string(exposureStep);
	logBuffer.LogDebug(info);

	if (bStarted)
	{
		if (enableAutoExposure)
		{
			k4a_device_set_color_control(kinectSensor, K4A_COLOR_CONTROL_EXPOSURE_TIME_ABSOLUTE, K4A_COLOR_CONTROL_MODE_AUTO, 0);

			autoExposureEnabled = true;
		}

		else
		{
			//Formula copied from here: https://github.com/microsoft/Azure-Kinect-Sensor-SDK/blob/7cd8683a1a71b8baebef4a3537e6edd8639d1e95/examples/k4arecorder/main.cpp#L333
			float absExposure = (exp2f((float)exposureStep) * 1000000.0f);
			//We add 0.5 because C++ always truncates when converting to an integer. 
			//This ensures that values will always be rounded correctly
			float absExposureRoundingMargin = absExposure + 0.5;
			int32_t absoluteExposureInt = (int32_t)absExposureRoundingMargin;

			k4a_device_set_color_control(kinectSensor, K4A_COLOR_CONTROL_EXPOSURE_TIME_ABSOLUTE, K4A_COLOR_CONTROL_MODE_MANUAL, absoluteExposureInt);

			autoExposureEnabled = false;
			exposureTimeStep = exposureStep;
		}
	}
}

void AzureKinectCapture::SetWhiteBalanceState(bool enableAutoBalance, int kelvinValue)
{
	std::string info = "Setting White Balance. Auto White Balance Enabled: " + std::to_string(enableAutoBalance) + " , Kelvin (if manual): " + std::to_string(kelvin);
	logBuffer.LogDebug(info);

	if (bStarted)
	{
		if (enableAutoBalance)
		{
			k4a_device_set_color_control(kinectSensor, K4A_COLOR_CONTROL_WHITEBALANCE, K4A_COLOR_CONTROL_MODE_AUTO, 0);
			autoWhiteBalanceEnabled = true;
		}

		else
		{
			//Kelvin comes in as a value between 5-18, we map it to a range of 2500-9000
			int kelvin = kelvinValue * 500;
			k4a_device_set_color_control(kinectSensor, K4A_COLOR_CONTROL_WHITEBALANCE, K4A_COLOR_CONTROL_MODE_MANUAL, kelvin);
			autoWhiteBalanceEnabled = false;
			this->kelvin = kelvin;
		}
	}
}

void AzureKinectCapture::SetFilters(bool depthFilterEnabled, int depthFilterSize)
{
	configuration.filter_depth_map = depthFilterEnabled;
	configuration.filter_depth_map_size = depthFilterSize;
}

bool AzureKinectCapture::AquireSerialFromDevice()
{
	logBuffer.LogDebug("Aquiring Serial Nnmber from device");

	if (!bOpen)
		return false;

	size_t serialNoSize;
	k4a_device_get_serialnum(kinectSensor, NULL, &serialNoSize);
	serialNumber = std::string(serialNoSize, '*');
	serialNumber.pop_back(); //Remove the null terminator, as it adds one character too much

	if (k4a_device_get_serialnum(kinectSensor, (char*)serialNumber.c_str(), &serialNoSize) == K4A_BUFFER_RESULT_SUCCEEDED)
	{
		logBuffer.LogInfo("Device serial number = " + serialNumber);
		return true;
	}

	else
		return false;
}

std::string AzureKinectCapture::GetSerial()
{
	return serialNumber;
}


/// <summary>
/// Determines if this camera is configured as a Master, Subordinate or Standalone. 
/// This is achieved by looking at how the sync out and sync in ports of the device are connected
/// </summary>
/// <returns>Returns int -1 for Subordinate, int 0 for Master and int 1 for Standalone</returns>
int AzureKinectCapture::GetSyncJackState()
{
	k4a_result_t syncJackResult = k4a_device_get_sync_jack(kinectSensor, &syncInConnected, &syncOutConnected);

	if (K4A_RESULT_SUCCEEDED == syncJackResult)
	{
		if (syncOutConnected && !syncInConnected)
			return 0; //Device is Master, as it doens't recieve a signal from its "Sync In" Port, but sends one through its "Sync Out" Port}

		else if (syncInConnected)
			return 1; //Device is Subordinate, as it recieves a signal via its "Sync In" Port

		else
			return 2;

	}

	else
		return 2; //Probably failed because there are no cabels connected, this means the device should be set as standalone
}

/// <summary>
/// Gets the intrinsics calibration json blob of the connected Kinect and writes it to the supplied buffer and size references.
/// </summary>
/// <returns> Returns true when the calibration file got successfully retrieved from the sensor, false when an error has occured</returns>
bool AzureKinectCapture::GetIntrinsicsJSON(std::vector<uint8_t>& calibration_buffer, size_t& calibration_size)
{
	logBuffer.LogDebug("Getting intrinsics as JSON file");

	calibration_size = 0;
	k4a_buffer_result_t buffer_result = k4a_device_get_raw_calibration(kinectSensor, NULL, &calibration_size);
	if (buffer_result == K4A_BUFFER_RESULT_TOO_SMALL)
	{
		calibration_buffer = std::vector<uint8_t>(calibration_size);
		buffer_result = k4a_device_get_raw_calibration(kinectSensor, calibration_buffer.data(), &calibration_size);
		if (buffer_result == K4A_BUFFER_RESULT_SUCCEEDED)
		{
			return true;
		}
	}

	return false;
}


/// <summary>
/// Returns the timestamp in microseconds
/// </summary>
/// <returns></returns>
uint64_t AzureKinectCapture::GetTimeStamp()
{
	return currentTimeStamp;
}

//Not used, but must be defined as required in ICapture
void AzureKinectCapture::SetManualDeviceIndex(int index) {};

