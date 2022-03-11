#include "azureKinectCapture.h"
#include <opencv2/core.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/opencv.hpp>
#include <chrono>



AzureKinectCapture::AzureKinectCapture()
{
	turboJpeg = tjInitDecompress();
}

AzureKinectCapture::~AzureKinectCapture()
{
	k4a_image_release(colorImageMJPG);
	k4a_image_release(depthImage16Int);
	k4a_image_release(pointCloudImage);
	k4a_image_release(depthImageInColor);
	k4a_image_release(colorImageDownscaled);
	k4a_transformation_destroy(transformation);
	k4a_device_close(kinectSensor);
	tjDestroy(turboJpeg);
}

/// <summary>
/// Initialize a device with the given configuration
/// </summary>
/// <param name="configuration"></param>
/// <returns>Returns true on success, false on error</returns>
bool AzureKinectCapture::Initialize(KinectConfiguration& configuration)
{
	std::cout << "Initializing Azure Kinect Device" << std::endl;

	uint32_t count = k4a_device_get_installed_count();
	int deviceIdx = 0;

	//We save the deviceId of this Client.
	//When the cameras are reinitialized during runtime, we can then gurantee
	//that each LiveScan instance uses the same device as before (In case two or more Kinects are connected to the same PC)
	//A device ID of -1 means that no Kinects have been successfully initalized yet (only happens when the Client starts)
	if (localDeviceIndex != -1) {
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
				bInitialized = false;
				return bInitialized;
			}
		}
	}

	if (configuration.eSoftwareSyncState == Main)
	{
		configuration.config.wired_sync_mode = K4A_WIRED_SYNC_MODE_MASTER;
		std::cout << "Initializing Azure Kinect as Main" << std::endl;
	}

	else if (configuration.eSoftwareSyncState == Subordinate)
	{
		std::cout << "Initializing Azure Kinect as Subordinate" << std::endl;
		configuration.config.wired_sync_mode = K4A_WIRED_SYNC_MODE_SUBORDINATE;
		//Sets the offset on subordinate devices. Should be a multiple of 160, each subordinate having a different multiplier in ascending order.
		//It avoids firing the Kinects lasers at the same time.		
		configuration.config.subordinate_delay_off_master_usec = 160 * configuration.nSyncOffset;
	}

	else
	{
		std::cout << "Initializing Azure Kinect as Standalone" << std::endl;
		configuration.config.wired_sync_mode = K4A_WIRED_SYNC_MODE_STANDALONE;
		configuration.config.subordinate_delay_off_master_usec = 0;
	}

	// Start the camera with the given configuration
	bInitialized = K4A_SUCCEEDED(k4a_device_start_cameras(kinectSensor, &configuration.config));

	if (!bInitialized)
		return bInitialized;

	k4a_calibration_t calibration;
	if (K4A_FAILED(k4a_device_get_calibration(kinectSensor, configuration.config.depth_mode, configuration.config.color_resolution, &calibration)))
	{
		bInitialized = false;
		return bInitialized;
	}

	//Workaround for a bug. When the camera starts in manual Exposure mode, the brightness of the RBG image
	//is much lower than in auto exposure mode. To prevent this, we first set the camera to auto exposure mode and 
	//then switch to manual mode again if it has been enabled before

	if (autoExposureEnabled == false) {

		std::cout << "Manual exposure enabled. Setting camera to auto exposure and adjust for one second as a workaround for a bug in exposure settings" << std::endl;

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
	float rescaleRatio = (float)calibration.color_camera_calibration.resolution_height / (float)configuration.GetCameraHeight();
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
				bInitialized = false;
				break;
			}
		} while (!bTemp);
	}

	size_t serialNoSize;
	k4a_device_get_serialnum(kinectSensor, NULL, &serialNoSize);
	serialNumber = std::string(serialNoSize, '\0');
	k4a_device_get_serialnum(kinectSensor, (char*)serialNumber.c_str(), &serialNoSize);
	configuration.SetSerialNumber(serialNumber);//set the serial number in the configuration struct.

	localDeviceIndex = deviceIdx;

	SetConfiguration(configuration); //We do this at the end, instead of the beginning, so that later we can move config logic (that doesnt require re-init, like exposure) into SetConfiguration.

	GetIntrinsicsJSON(calibrationBuffer, nCalibrationSize);

	std::cout << "Initialization successfull: " << bInitialized << std::endl;

	return bInitialized;
}

void AzureKinectCapture::SetConfiguration(KinectConfiguration& configuration)
{
	std::cout << "Setting configuration to device" << std::endl;

	this->configuration = configuration;
}

bool AzureKinectCapture::Close()
{
	std::cout << "Closing Azure Kinect device" << std::endl;

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
	k4a_device_stop_cameras(kinectSensor);
	k4a_device_close(kinectSensor);

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

bool AzureKinectCapture::AquireRawFrame() {

	if (!bInitialized)
	{
		return false;
	}

	k4a_capture_t capture = NULL;

	k4a_wait_result_t captureResult = k4a_device_get_capture(kinectSensor, &capture, captureTimeoutMs);
	if (captureResult != K4A_WAIT_RESULT_SUCCEEDED)
	{
		k4a_capture_release(capture);
		std::cout << "Dropped Raw Frame" << std::endl;
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
void AzureKinectCapture::DecodeRawColor() {

	nColorFrameHeight = k4a_image_get_height_pixels(colorImageMJPG);
	nColorFrameWidth = k4a_image_get_width_pixels(colorImageMJPG);

	colorBGR = cv::Mat(nColorFrameHeight, nColorFrameWidth, CV_8UC4);

	tjDecompress2(turboJpeg, k4a_image_get_buffer(colorImageMJPG), static_cast<unsigned long>(k4a_image_get_size(colorImageMJPG)), colorBGR.data, nColorFrameWidth, 0, nColorFrameHeight, TJPF_BGRA, TJFLAG_FASTDCT | TJFLAG_FASTUPSAMPLE);
}

void AzureKinectCapture::DownscaleColorImgToDepthImgSize() {

	//Resize the k4a_image to the precalculated size. Takes quite along time, maybe there is a faster algorithm?
	cv::resize(colorBGR, colorBGR, cv::Size(colorImageDownscaledWidth, colorImageDownscaledHeight), cv::INTER_LINEAR);

	nColorFrameHeight = colorBGR.rows;
	nColorFrameWidth = colorBGR.cols;
}


void AzureKinectCapture::MapDepthToColor()
{
	//fix depth image here
//	if (configuration.filter_depth_map)
//	{
//		cv::Mat cImgD = cv::Mat(k4a_image_get_height_pixels(depthImage16Int), k4a_image_get_width_pixels(depthImage16Int), CV_16UC1, k4a_image_get_buffer(depthImage16Int));
//		cv::Mat cImgD2 = cv::Mat::zeros(cv::Size(k4a_image_get_height_pixels(depthImage16Int), k4a_image_get_width_pixels(depthImage16Int)), CV_16UC1);// k4a_image_get_buffer(depthImage));
//		cv::Mat cImgD3 = cv::Mat::zeros(cv::Size(k4a_image_get_height_pixels(depthImage16Int), k4a_image_get_width_pixels(depthImage16Int)), CV_16UC1);// k4a_image_get_buffer(depthImage));
//
//		cv::Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(configuration.filter_depth_map_size, configuration.filter_depth_map_size));
//
//		//cv::medianBlur(cImgD, cImgD2, 5);
//		int CLOSING = 3;
//		// 4 will do a good edge detection if you threshold after as well
//		//cv::morphologyEx(cImgD, cImgD2, CLOSING, kernel);
//		cv::erode(cImgD, cImgD, kernel);
//		//cv::GaussianBlur(cImgD3, cImgD, cv::Size(configuration.filter_depth_map_size, configuration.filter_depth_map_size), 0);
//	}

	if (transformedDepthImage == NULL)
	{
		k4a_image_create(K4A_IMAGE_FORMAT_DEPTH16, nColorFrameWidth, nColorFrameHeight, nColorFrameWidth * (int)sizeof(uint16_t), &transformedDepthImage);
	}

	k4a_result_t res = k4a_transformation_depth_image_to_color_camera(transformationColorDownscaled, depthImage16Int, transformedDepthImage);

}

/// <summary>
/// Creates a Pointcloud out of the transformedDepthImage and saves it in PointcloudImage. Make sure to run MapDepthToColor before calling this function
/// </summary>
void AzureKinectCapture::GeneratePointcloud() {
	
	if (pointCloudImage == NULL)
	{
		k4a_image_create(K4A_IMAGE_FORMAT_CUSTOM, nColorFrameWidth, nColorFrameHeight, nColorFrameWidth * 3 * (int)sizeof(int16_t), &pointCloudImage);
	}

	k4a_transformation_depth_image_to_point_cloud(transformationColorDownscaled, transformedDepthImage, K4A_CALIBRATION_TYPE_COLOR, pointCloudImage);
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
	std::cout << "Setting Exposure. Auto exposure enabled: " << enableAutoExposure << " , exposure step: " << exposureStep << std::endl;

	if (bInitialized)
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


/// <summary>
/// Determines if this camera is configured as a Master, Subordinate or Standalone. 
/// This is achieved by looking at how the sync out and sync in ports of the device are connected
/// </summary>
/// <returns>Returns int -1 for Subordinate, int 0 for Master and int 1 for Standalone</returns>
int AzureKinectCapture::GetSyncJackState()
{
	std::cout << "Getting hardware sync jack state" << std::endl;

	k4a_result_t syncJackResult = k4a_device_get_sync_jack(kinectSensor, &syncInConnected, &syncOutConnected);

	if (K4A_RESULT_SUCCEEDED == syncJackResult)
	{
		if (syncOutConnected && !syncInConnected)
		{
			return 0; //Device is Master, as it doens't recieve a signal from its "Sync In" Port, but sends one through its "Sync Out" Port
		}

		else if(syncInConnected)
		{
			return 1; //Device is Subordinate, as it recieves a signal via its "Sync In" Port
		}

		else 
		{
			return 2;
		}

	}

	else
	{
		return 2; //Probably failed because there are no cabels connected, this means the device should be set as standalone
	}
}

/// <summary>
/// Gets the intrinsics calibration json blob of the connected Kinect and writes it to the supplied buffer and size references.
/// </summary>
/// <returns> Returns true when the calibration file got successfully retrieved from the sensor, false when an error has occured</returns>
bool AzureKinectCapture::GetIntrinsicsJSON(std::vector<uint8_t>& calibration_buffer, size_t& calibration_size)
{
	std::cout << "Getting intrinsics as JSON" << std::endl;

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
	//std::cout << "Getting timestamp at: " << currentTimeStamp << std::endl;
	return currentTimeStamp;
}

