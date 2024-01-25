//   Copyright (C) 2015  Marek Kowalski (M.Kowalski@ire.pw.edu.pl), Jacek Naruniec (J.Naruniec@ire.pw.edu.pl)
//   License: MIT Software License   See LICENSE.txt for the full license.

//   If you use this software in your research, then please use the following citation:

//    Kowalski, M.; Naruniec, J.; Daniluk, M.: "LiveScan3D: A Fast and Inexpensive 3D Data
//    Acquisition System for Multiple Kinect v2 Sensors". in 3D Vision (3DV), 2015 International Conference on, Lyon, France, 2015

//    @INPROCEEDINGS{Kowalski15,
//        author={Kowalski, M. and Naruniec, J. and Daniluk, M.},
//        booktitle={3D Vision (3DV), 2015 International Conference on},
//        title={LiveScan3D: A Fast and Inexpensive 3D Data Acquisition System for Multiple Kinect v2 Sensors},
//        year={2015},
//    }

#include "LiveScanClient.h"


LiveScanClient::LiveScanClient() :

	m_pCameraSpaceCoordinates(NULL),
	m_eClientStatus(STATUS_STARTING),
	m_bVirtualDevice(false),
	m_bCalibrate(false),
	m_bCaptureFrames(false),
	m_bCaptureSingleFrame(false),
	m_bCapturing(false),
	m_bStartPreRecordingProcess(false),
	m_bStartPostRecordingProcess(false),
	m_bSaveCalibration(false),
	m_bConnected(false),
	m_bConfirmCaptured(false),
	m_bSendCalibration(false),
	m_bShowDepth(false),
	m_bShowPreviewDuringRecording(false),
	m_bSocketThread(true),
	m_bFrameCompression(true),
	m_iCompressionLevel(2),
	m_pClientSocket(NULL),
	m_bRequestConfiguration(false),
	m_bSendConfiguration(false),
	m_bSendTimeStampList(false),
	m_bPostSyncedListReceived(false),
	m_bAutoExposureEnabled(true),
	m_nExposureStep(-5),
	m_nExtrinsicsStyle(0), // 0 = no export of extrinsics
	m_eCaptureMode(CM_POINTCLOUD),
	m_nFrameIndex(0),
	m_tFrameTime(0),
	m_tOldFrameTime(0),
	m_fAverageFPS(0),
	m_nFPSFrameCounter(0),
	m_nFPSUpdateCounter(0),
	m_bActiveClient(true),
	m_nAllVerticesSize(0)

{
	m_vBounds.push_back(-0.5);
	m_vBounds.push_back(-0.5);
	m_vBounds.push_back(-0.5);
	m_vBounds.push_back(0.5);
	m_vBounds.push_back(0.5);
	m_vBounds.push_back(0.5);
}

LiveScanClient::~LiveScanClient()
{
	configuration.Save();

	if (pCapture)
	{
		delete pCapture;
		pCapture = NULL;
	}

	if (m_pCameraSpaceCoordinates)
	{
		delete[] m_pCameraSpaceCoordinates;
		m_pCameraSpaceCoordinates = NULL;
	}

	if (m_pClientSocket)
	{
		delete m_pClientSocket;
		m_pClientSocket = NULL;
	}


	delete m_framesFileWriterReader;
	m_framesFileWriterReader = NULL;
	if (log)
		log->UnRegisterBuffer(&logBuffer);
}

void LiveScanClient::RunClient(Log* logger, bool virtualDevice)
{
	std::thread socketThread(&LiveScanClient::SocketThreadFunction, this);

	log = logger;
	log->RegisterBuffer(&logBuffer);

	m_eClientStatus = STATUS_STARTING;

	m_bVirtualDevice = virtualDevice;

	if (m_bVirtualDevice)
	{
		pCapture = new AzureKinectCaptureVirtual();
	}

	else
		pCapture = new AzureKinectCapture();

	pCapture->SetLogger(log);

	m_mRunning.lock();
	m_bRunning = true;
	m_mRunning.unlock();

	m_framesFileWriterReader = new FrameFileWriterReader(log);
	cv::imencode(".jpg", cv::Mat(1, 1, CV_8UC3), emptyJPEGBuffer);

	bool res = false;

	res = pCapture->OpenDevice();

	if (res)
	{
		std::string serial = pCapture->GetSerial();
		configuration = *new KinectConfiguration(serial);
		configuration.TryLoad();

		res = pCapture->StartCamera(configuration);

		if (res)
		{
			logBuffer.ChangeSerial("Device: " + serial);
			if (configuration.nickname[0] != ' ')
				logBuffer.ChangeName(configuration.nickname);
			logBuffer.LogInfo("Device could be opened successfully");

			m_sLastUsedIP = m_framesFileWriterReader->ReadIPFromFile();
			configuration.eHardwareSyncState = static_cast<SYNC_STATE>(pCapture->GetSyncJackState());
			calibration.LoadCalibration(serial);
			m_pCameraSpaceCoordinates = new Point3f[pCapture->nColorFrameWidth * pCapture->nColorFrameHeight];
			pCapture->SetExposureState(true, 0);
			m_eClientStatus = STATUS_RUNNING;
		}
	}

	if (!res)
	{
		logBuffer.LogFatal("Device could not be initialized successfully");
		SetStatusMessage(L"Error: Device could not be initialized!", 10000, true);
		m_mRunning.lock();
		m_bRunning = false;
		m_mRunning.unlock();
		m_eClientStatus = STATUS_CAMERA_ERROR;
	}


	while (m_bRunning)
	{
		UpdateFrame();
	}

	m_framesFileWriterReader->WriteIPToFile(m_sLastUsedIP);

	m_bSocketThread = false;
	socketThread.join();

	if (m_eClientStatus != STATUS_CAMERA_ERROR && m_eClientStatus != STATUS_CRASH)
		m_eClientStatus = STATUS_TERMINATED_NORMALLY;
}

void LiveScanClient::CloseClient()
{
	m_mRunning.lock();
	m_bRunning = false;
	m_mRunning.unlock();
}

void LiveScanClient::SetClientActive(bool active)
{
	m_bActiveClient = active;
}

void LiveScanClient::SetPreviewMode(bool depth)
{
	m_bShowDepth = depth;
}

void LiveScanClient::UpdateFrame()
{
	//Writes the hardware sync status to the configuration file
	if (m_bRequestConfiguration)
	{
		configuration.eHardwareSyncState = static_cast<SYNC_STATE>(pCapture->GetSyncJackState());
		m_bRequestConfiguration = false;
		m_bSendConfiguration = true;
	}

	if (m_bSaveCalibration)
	{
		calibration.SaveCalibration(configuration.serialNumber);
		m_bSaveCalibration = false;
	}

	//Updates global settings on the device
	if (m_bUpdateSettings)
	{
		pCapture->SetExposureState(m_bAutoExposureEnabled, m_nExposureStep);
		pCapture->SetWhiteBalanceState(m_bAutoWhiteBalanceEnabled, m_nKelvin);
		calibration.UpdateClientPose();
		m_bSendCalibration = true;
		m_bUpdateSettings = false;
	}

	if (m_bUpdateFilters)
	{
		pCapture->SetFilters(configuration.filter_depth_map, configuration.filter_depth_map_size);
		m_bUpdateFilters = false;
	}

	if (m_bCloseCamera)
	{
		StopCamera();
		m_bCloseCamera = false;
		m_bConfirmCameraClosed = true;
	}

	if (m_bStartCamera)
	{
		bool init = StartCamera();
		m_bCameraError = !init;
		m_bStartCamera = false;
		m_bConfirmCameraInitialized = true;
	}

	// +++ Below are functions that need the camera to be initialized +++	

	if (m_bStartPreRecordingProcess)
	{
		m_nFrameIndex = 0;
		m_vFrameTimestamps.clear();
		m_vFrameCount.clear();

		if (!m_bShowPreviewDuringRecording)
		{
			m_bPreviewDisabled = true;
		}

		m_bStartPreRecordingProcess = false;
		m_bConfirmPreRecordingProcess = true;
		m_bCapturing = true;
	}

	if (m_bStartPostRecordingProcess)
	{
		m_framesFileWriterReader->WriteTimestampLog(m_vFrameCount, m_vFrameTimestamps, configuration.nGlobalDeviceIndex);

		if (m_bPreviewDisabled)
			m_bPreviewDisabled = false;

		m_bStartPostRecordingProcess = false;
		m_bConfirmPostRecordingProcess = true;
		m_bCapturing = false;
	}

	if (m_bPostSyncedListReceived)
	{
		m_bPostSyncedListReceived = false;
		bool success = true;

		if (m_eCaptureMode == CAPTURE_MODE::CM_RAW)
			success = PostSyncRawFrames();


		if (m_eCaptureMode == CAPTURE_MODE::CM_POINTCLOUD)
			success = PostSyncPointclouds();

		SendPostSyncConfirmation(success);
	}

	//We always need to capture the raw frame data
	if (pCapture->AquireRawFrame())
	{
		//We lock the network thread so it that the requirement variables don't change while we process the frame 
		std::lock_guard<std::mutex> lock(m_mSocketThread);

		//To optimize our use of system resources, we only process what is needed
		bool generateRBGData = false;
		bool generateDepthToColorData = false;
		bool generatePointcloud = false;

		if (m_eCaptureMode == CM_POINTCLOUD || m_bCalibrate || m_bRequestLiveFrame)
		{
			generateRBGData = true;
			generateDepthToColorData = true;
			generatePointcloud = true;
		}

		if (!m_bPreviewDisabled && !m_bShowDepth && m_bActiveClient)
			generateRBGData = true;

		if (!m_bPreviewDisabled && m_bShowDepth)
			generateDepthToColorData = true;

		if (generateRBGData)
		{
			pCapture->DecodeRawColor();
			//pCapture->DownscaleColorImgToDepthImgSize();
		}

		if (generateDepthToColorData)
			pCapture->MapDepthToColor();

		if (generatePointcloud)
		{
			pCapture->GeneratePointcloud();
			StoreFrame(pCapture->pointCloudImage, &pCapture->colorBGR);
		}


		if (m_bCaptureFrames || m_bCaptureSingleFrame)
		{
			uint64_t timeStamp = pCapture->GetTimeStamp();
			m_vFrameCount.push_back(m_nFrameIndex);
			m_vFrameTimestamps.push_back(timeStamp);

			if (m_eCaptureMode == CM_RAW)
			{
				SaveRawFrame();
			}

			else if (m_eCaptureMode == CM_POINTCLOUD)
			{
				SavePointcloudFrame(timeStamp);
			}

			m_bCaptureSingleFrame = false;
			m_bConfirmCaptured = true;
			m_nFrameIndex++;

			//Save the time since the last capture to estimate FPS. While recording, we only save the time after having stored a frame, so that the user gets a grasp of how fast the recording is taking place
			m_tOldFrameTime = m_tFrameTime;
			m_tFrameTime = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::steady_clock::now().time_since_epoch());
		}

		if (!m_bCapturing)
		{
			m_tOldFrameTime = m_tFrameTime;
			m_tFrameTime = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::steady_clock::now().time_since_epoch());
		}

		if (m_bCalibrate)
		{
			Calibrate();
		}

		if (m_bRequestLiveFrame)
		{
			SendFrame(m_vLastFrameVertices, m_vLastFrameVerticesSize, m_vLastFrameRGB, true);
			m_bRequestLiveFrame = false;
		}

		if (m_bActiveClient)
			UpdatePreview();

		UpdateFPS();

	}

}

void LiveScanClient::UpdatePreview()
{
	if (!m_bPreviewDisabled) //TODO: Only update preview when we need it (When the client tab is active)
	{
		std::lock_guard<std::mutex> lock(m_mPreviewResources);

		//Check if we need to reinitialize the memory
		if (m_nPreviewWidth * m_nPreviewHeight != pCapture->nColorFrameHeight * pCapture->nColorFrameWidth || !m_pColorPreview || !m_pDepthPreview)
		{
			if (m_pColorPreview != NULL)
				delete[] m_pColorPreview;

			if (m_pDepthPreview != NULL)
				delete[] m_pDepthPreview;

			m_nPreviewWidth = pCapture->nColorFrameWidth;
			m_nPreviewHeight = pCapture->nColorFrameHeight;

			m_pColorPreview = new RGBA[m_nPreviewWidth * m_nPreviewHeight];
			m_pDepthPreview = new RGBA[m_nPreviewWidth * m_nPreviewHeight];
		}

		if (m_bShowDepth)
		{
			// Make sure we've received valid data
			if (pCapture->transformedDepthImage != NULL)
			{
				uint16_t* pointCloudImageData = (uint16_t*)(void*)k4a_image_get_buffer(pCapture->transformedDepthImage);

				for (int i = 0; i < m_nPreviewWidth * m_nPreviewHeight; i++)
				{
					uint8_t intensity = pointCloudImageData[i] / 40;
					m_pDepthPreview[i].red = rainbowLookup[intensity][0];
					m_pDepthPreview[i].green = rainbowLookup[intensity][1];
					m_pDepthPreview[i].blue = rainbowLookup[intensity][2];
				}
			}
		}

		else
		{
			//We copy the data into a secondary buffer so that we can use it thread safely, without blocking the pCapture resource too long
			if (!pCapture->colorBGR.empty())
			{

				int size = pCapture->colorBGR.step * pCapture->colorBGR.size().height;
				//Just copying, this makes the data actually BGR, not RGB as the type might indicates
				std::memcpy(m_pColorPreview, pCapture->colorBGR.data, m_nPreviewWidth * m_nPreviewHeight * sizeof(RGBA));
			}
		}
	}
}


PreviewFrame LiveScanClient::GetDepthTS()
{
	std::lock_guard<std::mutex> lock(m_mPreviewResources);

	PreviewFrame frame;
	frame.width = m_nPreviewWidth;
	frame.height = m_nPreviewHeight;
	frame.picture = NULL;
	frame.previewDisabled = m_bPreviewDisabled;

	if (m_nPreviewHeight * m_nPreviewWidth > 0 && !m_bPreviewDisabled)
	{
		frame.picture = new RGBA[frame.height * frame.width];
		std::memcpy(frame.picture, m_pDepthPreview, m_nPreviewWidth * m_nPreviewHeight * sizeof(RGBA));
	}

	return frame;
}


PreviewFrame LiveScanClient::GetColorTS()
{
	std::lock_guard<std::mutex> lock(m_mPreviewResources);

	PreviewFrame frame;
	frame.width = m_nPreviewWidth;
	frame.height = m_nPreviewHeight;
	frame.picture = NULL;
	frame.previewDisabled = m_bPreviewDisabled;

	if (m_nPreviewHeight * m_nPreviewWidth > 0 && !m_bPreviewDisabled)
	{
		frame.picture = new RGBA[frame.height * frame.width];
		std::memcpy(frame.picture, m_pColorPreview, m_nPreviewWidth * m_nPreviewHeight * sizeof(RGBA));
	}

	return frame;
}


void LiveScanClient::SaveRawFrame()
{
	m_framesFileWriterReader->WriteColorJPGFile(k4a_image_get_buffer(pCapture->colorImageMJPG), k4a_image_get_size(pCapture->colorImageMJPG), m_nFrameIndex, "");
	m_framesFileWriterReader->WriteDepthTiffFile(pCapture->depthImage16Int, m_nFrameIndex, "");
}

void LiveScanClient::SavePointcloudFrame(uint64_t timeStamp)
{
	m_framesFileWriterReader->writeNextBinaryFrame(m_vLastFrameVertices, m_vLastFrameVerticesSize, m_vLastFrameRGB, timeStamp, configuration.nGlobalDeviceIndex);
}

void LiveScanClient::Calibrate()
{

	logBuffer.LogCaptureDebug("Start Calibration process");

	Point3f* pCameraCoordinates = new Point3f[pCapture->nColorFrameWidth * pCapture->nColorFrameHeight];
	pCapture->PointCloudImageToPoint3f(pCameraCoordinates);

	bool res = calibration.Calibrate(&pCapture->colorBGR, pCameraCoordinates, pCapture->nColorFrameWidth, pCapture->nColorFrameHeight);

	delete[] pCameraCoordinates;

	if (res)
	{
		logBuffer.LogInfo("Calibration Successfull");
		calibration.SaveCalibration(configuration.serialNumber);
		m_bSendCalibration = true;
		m_bCalibrate = false;
	}

	else
		logBuffer.LogCaptureDebug("Calibration unsuccessfull");

}



void LiveScanClient::SocketThreadFunction()
{
	while (m_bSocketThread)
	{
		std::this_thread::sleep_for(std::chrono::milliseconds(1));
		HandleSocket();
	}
}

bool LiveScanClient::Connect(std::string ip)
{
	std::lock_guard<std::mutex> lockSocket(m_mSocketThread);

	m_sLastUsedIP = ip;

	if (!m_bConnected)
	{
		try
		{
			logBuffer.LogInfo("Trying to connect to server");
			m_pClientSocket = new SocketClient(ip, 48001); //This can potentially take some time, depending on the timeout settings

			m_bConnected = true;
			if (calibration.bCalibrated)
				m_bSendCalibration = true;

			//Clear the status bar so that the "Failed to connect..." disappears.
			SetStatusMessage(L"", 1, true);
			return true;
		}

		catch (...)
		{
			logBuffer.LogInfo("Couldn't connect to server");
			SetStatusMessage(L"Failed to connect. Did you start the server?", 10000, true);
			m_bConnected = false;
		}

		return false;
	}
}

bool LiveScanClient::Disconnect()
{
	std::lock_guard<std::mutex> lockSocket(m_mSocketThread);

	if (m_bConnected)
	{
		logBuffer.LogInfo("Disconnecting from server");
		delete m_pClientSocket;
		m_pClientSocket = NULL;
		m_bConnected = false;
		return true;
	}
}

bool LiveScanClient::GetConnectedTS()
{
	return m_bConnected;
}

//This is running on a seperate thread!
//TODO: Put the whole sending/receiving in a seperate file/class, it's taking up a lot of space!

void LiveScanClient::HandleSocket()
{
	char byteToSend;
	std::lock_guard<std::mutex> lock(m_mSocketThread);

	if (!m_bConnected)
	{
		return;
	}

	string received = m_pClientSocket->ReceiveBytes();

	if (!received.empty())
	{

		std::ostringstream message;
		message << "Message Raw data: ";

		for (unsigned int i = 0; i < received.length(); i++)
			message << (int)received[i] << " ";

		logBuffer.LogTrace(message.str());
	}

	for (unsigned int i = 0; i < received.length(); i++)
	{
		logBuffer.LogTrace("Received Server message");

		//Capture a single frame. Used for network-synced recording
		if (received[i] == MSG_CAPTURE_SINGLE_FRAME)
		{
			logBuffer.LogTrace("Capture single frame Received");
			m_bCaptureSingleFrame = true;
		}

		//Capture frames as fast as possible. Used for hardware-synced, or not-synced recording
		if (received[i] == MSG_START_CAPTURING_FRAMES)
		{
			logBuffer.LogTrace("Capture frames start received");
			m_bCaptureFrames = true;
		}

		if (received[i] == MSG_STOP_CAPTURING_FRAMES)
		{
			logBuffer.LogTrace("Capture frames stop received");
			m_bCaptureFrames = false;
		}

		if (received[i] == MSG_PRE_RECORD_PROCESS_START)
		{
			logBuffer.LogTrace("Received pre recording process start");
			m_bStartPreRecordingProcess = true;
		}

		if (received[i] == MSG_POST_RECORD_PROCESS_START)
		{
			logBuffer.LogTrace("Received post recording process start");
			m_bStartPostRecordingProcess = true;
		}

		//calibrate
		else if (received[i] == MSG_CALIBRATE)
		{
			logBuffer.LogTrace("Calibrate command recieved");
			m_bCalibrate = true;
		}

		else if (received[i] == MSG_CANCEL_CALIBRATION)
		{
			logBuffer.LogTrace("Calibration cancel command received");
			m_bCalibrate = false;
		}

		else if (received[i] == MSG_CLOSE_CAMERA)
		{
			logBuffer.LogTrace("Closing camera command received");
			m_bCloseCamera = true;
		}

		else if (received[i] == MSG_START_CAMERA)
		{
			logBuffer.LogTrace("Initialize camera command received");
			m_bStartCamera = true;
		}

		else if (received[i] == MSG_SET_CONFIGURATION)
		{
			i++;

			logBuffer.LogInfo("Recieved new configuration");

			char* message = new char[KinectConfiguration::byteLength];
			for (int x = 0; x < KinectConfiguration::byteLength; x++)
			{
				message[x] = received[i + x];
			}

			i += KinectConfiguration::byteLength;
			configuration.SetFromBytes(message);
			m_bUpdateFilters = true;
			delete[] message;

			i--;
		}

		//receive settings
		//TODO: what if packet is split?
		else if (received[i] == MSG_RECEIVE_SETTINGS)
		{
			logBuffer.LogInfo("Recieved new settings");

			vector<float> bounds(6);
			i++;
			int nBytes = *(int*)(received.c_str() + i);
			i += sizeof(int);

			for (int j = 0; j < 6; j++)
			{
				bounds[j] = *(float*)(received.c_str() + i);
				i += sizeof(float);
			}

			m_vBounds = bounds;

			int nMarkers = *(int*)(received.c_str() + i);
			i += sizeof(int);

			calibration.markerPoses.resize(nMarkers);

			for (int j = 0; j < nMarkers; j++)
			{
				for (int k = 0; k < 4; k++)
				{
					for (int l = 0; l < 4; l++)
					{
						calibration.markerPoses[j].pose.mat[k][l] = *(float*)(received.c_str() + i);
						i += sizeof(float);
					}
				}

				calibration.markerPoses[j].markerId = *(int*)(received.c_str() + i);
				i += sizeof(int);
			}

			m_iCompressionLevel = *(int*)(received.c_str() + i);
			i += sizeof(int);
			if (m_iCompressionLevel > 0)
				m_bFrameCompression = true;
			else
				m_bFrameCompression = false;

			m_bAutoExposureEnabled = (received[i] != 0);
			i++;

			m_nExposureStep = *(int*)(received.c_str() + i);
			i += sizeof(int);

			m_bAutoWhiteBalanceEnabled = (received[i] != 0);
			i++;

			m_nKelvin = *(int*)(received.c_str() + i);
			i += sizeof(int);

			int exportFormat = *(int*)(received.c_str() + i);
			i += sizeof(int);

			if (exportFormat == 0)
			{
				logBuffer.LogInfo("Export format set to Pointcloud");
				m_eCaptureMode = CM_POINTCLOUD;
			}

			if (exportFormat == 1)
			{
				logBuffer.LogInfo("Export format set to Raw Data");
				m_eCaptureMode = CM_RAW;

			}

			m_nExtrinsicsStyle = *(int*)(received.c_str() + i);
			i += sizeof(int);

			m_bShowPreviewDuringRecording = (received[i] != 0);
			i++;

			m_bUpdateSettings = true;

			std::string settingsInfo = "Received Settings: Auto Exposure enabled= " + to_string(m_bAutoExposureEnabled) + ", Exposure Step = " +
				to_string(m_nExposureStep) + ", Extrinsics Stlye = " + to_string(m_nExtrinsicsStyle) + ", Show preview during capture = " + to_string(m_bShowPreviewDuringRecording);
			logBuffer.LogDebug(settingsInfo);

			//so that we do not lose the next character in the stream
			i--;
		}

		//send configuration
		else if (received[i] == MSG_REQUEST_CONFIGURATION)
		{
			logBuffer.LogTrace("Server requests configuration");
			m_bRequestConfiguration = true;
		}

		//send stored frame
		else if (received[i] == MSG_REQUEST_STORED_FRAME)
		{
			logBuffer.LogCaptureDebug("Server requests stored frame");

			Point3s* points = NULL;
			RGBA* colors = NULL;
			int pointsSize;
			int timeStamp;

			bool res = m_framesFileWriterReader->readNextBinaryFrame(points, colors, pointsSize, timeStamp);
			if (res == false)
			{
				int size = -1;
				char message = MSG_STORED_FRAME;
				m_pClientSocket->SendBytes(&message, 1);
				m_pClientSocket->SendBytes((char*)&size, 4);
			}
			else
				SendFrame(points, pointsSize, colors, false);

			delete[] points;
			delete[] colors;
		}
		//send last frame
		else if (received[i] == MSG_REQUEST_LAST_FRAME)
		{
			logBuffer.LogCaptureDebug("Server requests lastest frame");
			m_bRequestLiveFrame = true;
		}

		//receive calibration data
		else if (received[i] == MSG_RECEIVE_CALIBRATION)
		{
			logBuffer.LogInfo("Recieving calibration data");

			Matrix4x4 newRefinement = Matrix4x4();

			i++;
			for (int j = 0; j < 4; j++)
			{
				for (int k = 0; k < 4; k++)
				{
					newRefinement.mat[j][k] = *(float*)(received.c_str() + i);
					i += sizeof(float);
				}
			}

			//We combine the refinement pose with the already existing one
			//As the ICP offset is always based on the last ICP transformation
			calibration.refinementTransform = newRefinement * calibration.refinementTransform;
			calibration.UpdateClientPose();

			//so that we do not lose the next character in the stream
			i--;

			//We save the refined calibration data into a file
			m_bSaveCalibration = true;

		}
		else if (received[i] == MSG_CLEAR_STORED_FRAMES)
		{
			logBuffer.LogTrace("Recieving command to clear stored frames");
			m_framesFileWriterReader->closeFileIfOpened();
		}

		else if (received[i] == MSG_CREATE_DIR) //Creates a dir on the client. Message also marks the start of the recording
		{
			i++;
			int stringLength = *(int*)(received.c_str() + i); //Get the length of the following string
			i += sizeof(int);

			std::string dirPath;

			dirPath.assign(received, i, stringLength); //Recieved is already a string, so we just copy the characters out of it

			i += stringLength;

			//Confirmation message that we have created a valid new directory on this system
			int size = 2;
			char* buffer = new char[size];
			buffer[0] = MSG_CONFIRM_DIR_CREATION;

			if (m_framesFileWriterReader->CreateRecordDirectory(dirPath, configuration.nGlobalDeviceIndex))
			{
				buffer[1] = 1;
				m_pClientSocket->SendBytes(buffer, size);
			}

			else
			{
				//Tell the server that the directory creation has failed, server will abort the recording
				buffer[1] = 0;
				m_pClientSocket->SendBytes(buffer, size);
				logBuffer.LogWarning("Recording directory creation has failed");
			}

			//Write the calibration intrinsics into the newly created dir if we record raw frames
			if (configuration.config.color_format != K4A_IMAGE_FORMAT_COLOR_BGRA32)
				m_framesFileWriterReader->WriteCalibrationJSON(configuration.nGlobalDeviceIndex, pCapture->calibrationBuffer, pCapture->nCalibrationSize);

		}

		else if (received[i] == MSG_REQUEST_TIMESTAMP_LIST)
		{
			logBuffer.LogTrace("Server requests timestamp list");
			m_bSendTimeStampList = true;
		}

		else if (received[i] == MSG_RECEIVE_POSTSYNC_LIST)
		{
			logBuffer.LogInfo("Received Postsync List");

			i++;
			int size = *(int*)(received.c_str() + i);
			i += sizeof(int);

			m_vFrameID.clear();
			m_vPostSyncedFrameID.clear();
			m_vFrameID.resize(size);
			m_vPostSyncedFrameID.resize(size);

			memcpy(m_vFrameID.data(), &received[i], size * sizeof(int));
			i += size * sizeof(int);


			memcpy(m_vPostSyncedFrameID.data(), &received[i], size * sizeof(int));
			i += size * sizeof(int);

			m_bPostSyncedListReceived = true;
		}

	}

	if (m_bConfirmCaptured)
	{
		logBuffer.LogCaptureDebug("Confirming capture to server");

		byteToSend = MSG_CONFIRM_CAPTURED;
		m_pClientSocket->SendBytes(&byteToSend, 1);
		m_bConfirmCaptured = false;
	}

	if (m_bSendCalibration)
	{
		logBuffer.LogDebug("Sending calibration");

		int size = 2 * 16 * sizeof(float) + sizeof(int) + 1;
		char* buffer = new char[size];
		buffer[0] = MSG_CONFIRM_CALIBRATED;
		int i = 1;

		memcpy(buffer + i, &calibration.iUsedMarkerId, 1 * sizeof(int));
		i += 1 * sizeof(int);
		memcpy(buffer + i, calibration.worldTransform.mat[0], 16 * sizeof(float));
		i += 16 * sizeof(float);
		memcpy(buffer + i, calibration.currentMarkerPose.mat, 16 * sizeof(float));
		i += 16 * sizeof(float);

		m_pClientSocket->SendBytes(buffer, size);
		m_bSendCalibration = false;
	}

	if (m_bConfirmPreRecordingProcess)
	{
		logBuffer.LogDebug("Sending Pre Record Process confirmation");

		byteToSend = MSG_CONFIRM_PRE_RECORD_PROCESS;
		m_pClientSocket->SendBytes(&byteToSend, 1);
		m_bConfirmPreRecordingProcess = false;
	}

	if (m_bConfirmPostRecordingProcess)
	{
		logBuffer.LogDebug("Sending Post Record Process confirmation");

		byteToSend = MSG_CONFIRM_POST_RECORD_PROCESS;
		m_pClientSocket->SendBytes(&byteToSend, 1);
		m_bConfirmPostRecordingProcess = false;
	}

	if (m_bSendConfiguration)
	{
		logBuffer.LogDebug("Sending configuration");

		int size = configuration.byteLength + 1;
		char* buffer = new char[size];
		buffer[0] = MSG_CONFIGURATION;
		memcpy(buffer + 1, configuration.ToBytes(), KinectConfiguration::byteLength);
		m_pClientSocket->SendBytes(buffer, size);
		m_bSendConfiguration = false;
	}

	if (m_bSendTimeStampList)
	{
		logBuffer.LogInfo("Sending Timestamp list");

		//Structure of timestamp byte list:
		// Message Char + Timestamps Array Size + Timestamps Array as uint64 + FrameNumbers Array Size + FrameNumbers as Int32

		int byteSizeTimestamps = m_vFrameTimestamps.size() * sizeof(uint64);
		int byteSizeFrameNumbers = m_vFrameCount.size() * sizeof(int);
		int size = (1 + sizeof(int) + byteSizeTimestamps + sizeof(int) + byteSizeFrameNumbers);
		char* buffer = new char[size];
		buffer[0] = MSG_SEND_TIMESTAMP_LIST;
		int i = 1;

		int timestampSize = m_vFrameTimestamps.size();
		memcpy(buffer + i, &timestampSize, sizeof(int));
		i += sizeof(int);

		char* timestampsPtr = (char*)m_vFrameTimestamps.data();
		memcpy(buffer + i, timestampsPtr, byteSizeTimestamps);
		i += byteSizeTimestamps;

		int frameNumberSize = m_vFrameCount.size();
		memcpy(buffer + i, &frameNumberSize, sizeof(int));
		i += sizeof(int);

		char* frameNumberPointer = (char*)m_vFrameCount.data();
		memcpy(buffer + i, frameNumberPointer, byteSizeFrameNumbers);
		i += byteSizeFrameNumbers;

		m_pClientSocket->SendBytes(buffer, size);
		m_bSendTimeStampList = false;

		delete[] buffer;
	}

	if (m_bConfirmCameraClosed)
	{
		int size = 2;
		char* buffer = new char[size];
		buffer[0] = MSG_CONFIRM_CAMERA_CLOSED;

		if (m_bCameraError)
		{
			buffer[1] = 1; // = Error
			logBuffer.LogWarning("Camera could not be closed, reporting to server");
		}
		else
		{
			buffer[1] = 0; // = Success
			logBuffer.LogDebug("Camera could be closed successfully, reporting to server");
		}

		m_pClientSocket->SendBytes(buffer, size);
		m_bConfirmCameraClosed = false;
	}

	if (m_bConfirmCameraInitialized)
	{
		int size = 2;
		char* buffer = new char[size];
		buffer[0] = MSG_CONFIRM_CAMERA_INIT;

		if (m_bCameraError)
		{
			buffer[1] = 1; // = Error
			logBuffer.LogWarning("Camera could not be initialized, reporting to server");
		}
		else
		{
			buffer[1] = 0; // = Success
			logBuffer.LogDebug("Camera could be initialized successfully, reporting to server");
		};

		m_pClientSocket->SendBytes(buffer, size);
		m_bConfirmCameraInitialized = false;

	}
}

bool LiveScanClient::StartCamera()
{
	logBuffer.LogDebug("Starting Camera");

	bool res = false;

	res = pCapture->StartCamera(configuration);
	if (!res)
	{
		logBuffer.LogDebug("Could not start camera");
		SetStatusMessage(L"Camera failed to start! Please restart Application!", 10000, true);
		m_eClientStatus = STATUS_CAMERA_ERROR;
		return false;
	}

	else
	{
		configuration.eHardwareSyncState = static_cast<SYNC_STATE>(pCapture->GetSyncJackState());
		m_pCameraSpaceCoordinates = new Point3f[pCapture->nColorFrameWidth * pCapture->nColorFrameHeight];
	}

	return true;
}

void LiveScanClient::StopCamera()
{
	logBuffer.LogDebug("Stopping Camera");
	pCapture->StopCamera();
}

void LiveScanClient::DisposeDevice()
{
	logBuffer.LogDebug("Disposing Camera");
	pCapture->DisposeDevice();
}

void LiveScanClient::SendPostSyncConfirmation(bool success)
{
	int size = 2;
	char* buffer = new char[size];
	buffer[0] = MSG_CONFIRM_POSTSYNCED;

	if (success)
	{
		buffer[1] = 1;
		logBuffer.LogTrace("Post Sync Confirmation: Successfull");
	}

	else
	{
		logBuffer.LogTrace("Post Sync Confirmation: Failed");
		buffer[1] = 0;
	}

	m_pClientSocket->SendBytes(buffer, size);
}

void LiveScanClient::SendFrame(Point3s* vertices, int verticesSize, RGBA* RGB, bool live)
{
	logBuffer.LogCaptureDebug("Sending Frame to server");

	int size = verticesSize * (3 + 3 * sizeof(short)) + sizeof(int);


	vector<char> buffer(size);
	int pos = 0;

	std::memcpy(buffer.data() + pos, &verticesSize, sizeof(verticesSize));
	pos += sizeof(verticesSize);

	for (unsigned int i = 0; i < verticesSize; i++)
	{
		buffer[pos++] = RGB[i].red;
		buffer[pos++] = RGB[i].green;
		buffer[pos++] = RGB[i].blue;

		std::memcpy(buffer.data() + pos, vertices, sizeof(short) * 3);
		vertices++;
		pos += sizeof(short) * 3;
	}

	int iCompression = static_cast<int>(m_bFrameCompression);

	if (m_bFrameCompression)
	{
		// *2, because according to zstd documentation, increasing the size of the output buffer above a
		// bound should speed up the compression.
		int cBuffSize = ZSTD_compressBound(size) * 2;
		vector<char> compressedBuffer(cBuffSize);
		int cSize = ZSTD_compress(compressedBuffer.data(), cBuffSize, buffer.data(), size, m_iCompressionLevel);
		size = cSize;
		buffer = compressedBuffer;
	}
	char header[8];
	std::memcpy(header, (char*)&size, sizeof(size));
	std::memcpy(header + 4, (char*)&iCompression, sizeof(iCompression));

	char message;

	if (live)
		message = MSG_LAST_FRAME;

	else
		message = MSG_STORED_FRAME;

	if (m_pClientSocket != NULL)
	{
		m_pClientSocket->SendBytes(&message, 1);
		m_pClientSocket->SendBytes((char*)&header, sizeof(int) * 2);
		m_pClientSocket->SendBytes(buffer.data(), size);
	}
}

/// <summary>
/// The goal in this function is to eliminate all vertices from the pointcloud
/// that are either invalid (depth of 0), or out of bounds, so that we don't
/// write them to disk, and uneccessarily bloat the file size
/// </summary>
void LiveScanClient::StoreFrame(k4a_image_t pointcloudImage, cv::Mat* colorImage)
{
	logBuffer.LogTrace("Storing Frame");

	int allVerticesNewSize = pCapture->nColorFrameHeight * pCapture->nColorFrameWidth;

	if (m_nAllVerticesSize != allVerticesNewSize)
	{
		m_nAllVerticesSize = allVerticesNewSize;
		delete[] m_pAllVertices;
		m_pAllVertices = new Point3f[m_nAllVerticesSize];
	}

	int16_t* pointCloudImageData = (int16_t*)(void*)k4a_image_get_buffer(pointcloudImage);
	Point3f invalidPoint = Point3f(0, 0, 0, true);
	Point3f temp = Point3f(0, 0, 0);

	Matrix4x4 scale = Matrix4x4(
		0.001f, 0.0f, 0.0f, 0.0f,
		0.0f, 0.001f, 0.0f, 0.0f,
		0.0f, 0.0f, 0.001f, 0.0f,
		0.0f, 0.0f, 0.0f, 1.0f);

	Matrix4x4 toWorld = calibration.worldTransform * scale;

	int goodVerticesCount = 0;

	for (unsigned int vertexIndex = 0; vertexIndex < m_nAllVerticesSize; vertexIndex++)
	{
		//As the resizing function doesn't return a valid RGB-Reserved value which indicates that this pixel is invalid,
		//we cut all vertices under a distance of 0.0001mm, as the invalid vertices always have a Z-Value of 0
		if (pointCloudImageData[3 * vertexIndex + 2] >= 0.0001) // TODO: Needed? && colorInDepth->data[vertexIndex] == 255)
		{
			temp.X = pointCloudImageData[3 * vertexIndex + 0];
			temp.Y = pointCloudImageData[3 * vertexIndex + 1];
			temp.Z = pointCloudImageData[3 * vertexIndex + 2];

			temp = toWorld * temp;

			if (temp.X < m_vBounds[0] || temp.X > m_vBounds[3]
				|| temp.Y < m_vBounds[1] || temp.Y > m_vBounds[4]
				|| temp.Z < m_vBounds[2] || temp.Z > m_vBounds[5])
			{
				m_pAllVertices[vertexIndex] = invalidPoint;
				continue;
			}

			m_pAllVertices[vertexIndex] = temp;
			goodVerticesCount++;
		}

		else
		{
			m_pAllVertices[vertexIndex] = invalidPoint;
		}
	}


	delete[] m_vLastFrameVertices;
	delete[] m_vLastFrameRGB;

	if (goodVerticesCount > 0)
	{
		m_vLastFrameVertices = new Point3s[goodVerticesCount];
		m_vLastFrameRGB = new RGBA[goodVerticesCount];
		m_vLastFrameVerticesSize = goodVerticesCount;

		uchar* colorValues = colorImage->data;

		//Copy all valid vertices into a clean vector
		int j = 0;
		for (unsigned int i = 0; i < m_nAllVerticesSize; i++)
		{
			if (!m_pAllVertices[i].Invalid)
			{
				RGBA color;
				color.red = colorValues[i * 4];
				color.green = colorValues[(i * 4) + 1];
				color.blue = colorValues[(i * 4) + 2];

				m_vLastFrameVertices[j] = m_pAllVertices[i];
				m_vLastFrameRGB[j] = color;
				j++;
			}
		}
	}

	//If the pointcloud is empty, we can't have an array with zero elements
	else
	{
		m_vLastFrameVertices = new Point3s[1];
		Point3s point(0, 0, 0);
		m_vLastFrameVertices[0] = point;
		
		m_vLastFrameRGB = new RGBA[1];
		RGBA color;
		m_vLastFrameRGB[0] = color;

		m_vLastFrameVerticesSize = 1;
	}	
}

void LiveScanClient::UpdateFPS()
{
	long difference = std::chrono::duration_cast<std::chrono::milliseconds>(m_tFrameTime - m_tOldFrameTime).count();

	if (difference > 0)
		m_nFPSUpdateCounter += difference;

	m_nFPSFrameCounter++;

	//We show the FPS every second
	if (m_nFPSUpdateCounter > 1000)
	{
		//Calculate a moving average of the FPS, so it isn't all over the place
		float alpha = 0.2f;

		std::lock_guard<std::mutex> lock(m_mFPS);
		m_fAverageFPS = alpha * m_fAverageFPS + (1.0f - alpha) * m_nFPSFrameCounter;

		//The UI accesses m_fAverageFPS to display it

		m_nFPSFrameCounter = 0;
		m_nFPSUpdateCounter = 0;
	}

}

float LiveScanClient::GetFPSTS()
{
	std::lock_guard<std::mutex> lock(m_mFPS);
	return m_fAverageFPS;
}

void LiveScanClient::SetStatusMessage(std::wstring message, int time, bool priority)
{
	std::lock_guard<std::mutex> lock(m_mStatus);

	m_bNewMessage = true;
	statusMessage.message = message;
	statusMessage.time = time;
	statusMessage.priority = priority;
}

StatusMessage LiveScanClient::GetStatusMessageTS()
{
	std::lock_guard<std::mutex> lock(m_mStatus);

	if (m_bNewMessage)
	{
		m_bNewMessage = false;
		return statusMessage;
	}
	else
	{
		StatusMessage emtpyMessage = StatusMessage();
		return emtpyMessage;
	}
}

DeviceStatus LiveScanClient::GetDeviceStatusTS()
{
	DeviceStatus deviceStatus;
	deviceStatus.status = m_eClientStatus;

	if (configuration.nickname[0] == ' ')
		deviceStatus.name = configuration.serialNumber;
	else
		deviceStatus.name = configuration.nickname;

	return deviceStatus;
}

bool LiveScanClient::PostSyncPointclouds()
{
	logBuffer.LogDebug("Starting Post Sync for Pointclouds");

	bool success = true;

	Point3s* emptyPoints = new Point3s[0];
	RGBA* emptyColors = new RGBA[0];
	int timestamp = 0;

	//We open a new .bin file in which we copy and paste all the frames from the recorded .bin file,
	//but in the right order
	FrameFileWriterReader* syncedFileWriter = new FrameFileWriterReader(log);
	syncedFileWriter->SetRecordingDirPath(m_framesFileWriterReader->GetRecordingDirPath());
	syncedFileWriter->openNewBinFileForWriting(configuration.nGlobalDeviceIndex, "synced");
	m_framesFileWriterReader->openCurrentBinFileForReading();

	for (size_t i = 0; i < m_vFrameID.size(); i++)
	{
		//-1 indicates that this device doesn't have a valid frame for this capture. To keep a good frame timing, we fill in an empty frame
		if (m_vFrameID[i] == -1)
		{

			if (!syncedFileWriter->writeNextBinaryFrame(emptyPoints, 0, emptyColors, 0, configuration.nGlobalDeviceIndex))
			{
				success = false;
			}
		}

		else
		{

			Point3s* points;
			RGBA* colors;
			int pointsSize;

			m_framesFileWriterReader->seekBinaryReaderToFrame(m_vFrameID[i]);
			m_framesFileWriterReader->readNextBinaryFrame(points, colors, pointsSize, timestamp);
			if (!syncedFileWriter->writeNextBinaryFrame(points, pointsSize, colors, timestamp, configuration.nGlobalDeviceIndex))
			{
				logBuffer.LogWarning("Could not write Pointcloud Frame during post sync. Frame ID: " + to_string(m_vFrameID[i]));
				success = false;
			}

			delete[] points;
			delete[] colors;
		}
	}

	//Close our synced .bin file...
	std::string syncedFilePath = syncedFileWriter->GetBinFilePath();
	syncedFileWriter->closeFileIfOpened();
	m_framesFileWriterReader->closeAndDeleteFile(); //...delete the old .bin file...
	m_framesFileWriterReader->openNewBinFileForReading(syncedFilePath); //... and open it into the default framefilewriter, so that when the server requests stored frames, the client knows where to look

	delete syncedFileWriter;

	return success;
}

bool LiveScanClient::PostSyncRawFrames()
{
	logBuffer.LogDebug("Starting Post Sync for Raw frames");

	bool success = true;
	k4a_image_t emptyDepthFrame;
	k4a_image_create(K4A_IMAGE_FORMAT_DEPTH16, 1, 1, 0, &emptyDepthFrame);

	for (size_t i = 0; i < m_vFrameID.size(); i++)
	{
		//-1 indicates that this device doesn't have a valid frame for this capture. To keep a good frame timing, we fill in an empty frame
		if (m_vFrameID[i] == -1)
		{
			m_framesFileWriterReader->WriteColorJPGFile(emptyJPEGBuffer.data(), emptyJPEGBuffer.size(), m_vPostSyncedFrameID[i], "synced");
			m_framesFileWriterReader->WriteDepthTiffFile(emptyDepthFrame, m_vPostSyncedFrameID[i], "synced");
		}

		else
		{
			if (!m_framesFileWriterReader->RenameRawFramePair(m_vFrameID[i], m_vPostSyncedFrameID[i], std::string("synced_")))
			{
				logBuffer.LogWarning("Could not rename Frame during post sync. Frame ID: " + to_string(m_vFrameID[i]));
				success = false;
			}
		}
	}

	return success;
}


