#include "azureKinectCapture.h"



AzureKinectCapture::AzureKinectCapture()
{
	ImageProcessing::InitImageProcessing();
}

AzureKinectCapture::~AzureKinectCapture()
{
	DisposeDevice();

	ImageProcessing::CloseImageProcessing();

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

	imageset = ImageProcessing::CreateImageSet();

	size_t calibrationSize = 0;
	k4a_device_get_raw_calibration(kinectSensor, NULL, &calibrationSize);
	uint8_t* calibrationBuffer = new uint8_t[calibrationSize];
	k4a_device_get_raw_calibration(kinectSensor, calibrationBuffer, &calibrationSize);
	ImageProcessing::ChangeCalibrationFromBuffer(imageset, reinterpret_cast<char*>(calibrationBuffer), calibrationSize, calibration.color_resolution, calibration.depth_mode);
	delete[] calibrationBuffer;

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
	k4a_transformation_destroy(transformation);
	ImageProcessing::DisposeImageSet(imageset);

	colorImageMJPG = NULL;
	depthImage16Int = NULL;
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

	ImageProcessing::ChangeJPEGFromBuffer(imageset, k4a_image_get_width_pixels(colorImageMJPG), k4a_image_get_height_pixels(colorImageMJPG), reinterpret_cast<char*>(k4a_image_get_buffer(colorImageMJPG)), k4a_image_get_size(colorImageMJPG));
	ImageProcessing::ChangeDepthFromBuffer(imageset, k4a_image_get_width_pixels(depthImage16Int), k4a_image_get_height_pixels(depthImage16Int), reinterpret_cast<char*>(k4a_image_get_buffer(depthImage16Int)));

	return true;

}

/// <summary>
/// Decompresses the raw MJPEG image from the camera to a BGRA cvMat using TurboJpeg
/// </summary>
void AzureKinectCapture::DecodeRawColor()
{
	ImageProcessing::JPEG2BGRA32(imageset);

	if(colorBGR.total() * colorBGR.elemSize() != ImageProcessing::GetColorImageSize(imageset))
		colorBGR = cv::Mat(ImageProcessing::GetColorImageHeight(imageset), ImageProcessing::GetColorImageWidth(imageset), CV_8UC4, ImageProcessing::GetColorImageBuffer(imageset));
}


void AzureKinectCapture::MapDepthToColor()
{
	if (configuration.filter_depth_map)
		ImageProcessing::ErodeDepthImageFilter(imageset, configuration.filter_depth_map_size);

	ImageProcessing::MapDepthToColor(imageset);
}

/// <summary>
/// Creates a Pointcloud out of the transformedDepthImage and saves it in PointcloudImage. Make sure to run MapDepthToColor before calling this function
/// </summary>
void AzureKinectCapture::GeneratePointcloud()
{
	ImageProcessing::GeneratePointcloud(imageset);
}

///<summary>
///Translates the k4a_image_t pointcloud into a easier to handle Point3f array. Make sure to run MapDepthToColorFrame & GeneratePointcloud before calling this function
///</summary>
///<param name="pCameraSpacePoints"></param>
void AzureKinectCapture::ProcessPointcloud()
{
	ImageProcessing::PointCloudImageToPoint3f(imageset);
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

