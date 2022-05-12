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
#include "stdafx.h"
#include "resource.h"
#include "LiveScanClient.h"
#include "filter.h"
#include <chrono>
#include <strsafe.h>
#include <fstream>
#include "zstd.h"
#include <KinectConfiguration.h>
#include <shellapi.h> // HOGUE
std::mutex m_mSocketThreadMutex;

// HOGUE
int g_winWidth = 800;
int g_winHeight = 540;
int g_winX = 0;
int g_winY = 0;
int g_connectToServerImmediately = 0;


int APIENTRY wWinMain(
	_In_ HINSTANCE hInstance,
	_In_opt_ HINSTANCE hPrevInstance,
	_In_ LPWSTR lpCmdLine,
	_In_ int nShowCmd
)
{
	UNREFERENCED_PARAMETER(hPrevInstance);
	//UNREFERENCED_PARAMETER(lpCmdLine);
	//std::cout << lpCmdLine<<std::endl;
	// HOGUE: THIS SHOULD BE DONE IN A MUCH BETTER WAY
	LPWSTR* szArgList;
	int argCount;
	szArgList = CommandLineToArgvW(GetCommandLine(), &argCount);

	if (argCount >= 5) {
		// assume window width, height, x, y
		g_winWidth = _wtoi(szArgList[1]);
		g_winHeight = _wtoi(szArgList[2]);
		g_winX = _wtoi(szArgList[3]);
		g_winY = _wtoi(szArgList[4]);
		if (argCount >= 6) g_connectToServerImmediately = _wtoi(szArgList[5]);
	}

	LiveScanClient application;
	application.Run(hInstance, nShowCmd);
}

LiveScanClient::LiveScanClient() :
	m_hWnd(NULL),
	m_nLastCounter(0),
	m_nFramesSinceUpdate(0),
	m_fFreq(0),
	m_nNextStatusTime(0LL),
	m_pD2DFactory(NULL),
	m_pD2DImageRenderer(NULL),
	m_pRainbowColorDepth(NULL),
	m_pCameraSpaceCoordinates(NULL),
	m_pColorInColorSpace(NULL),
	m_pDepthInColorSpace(NULL),
	log(log.Get()),
	m_loglevel(Log::LOGLEVEL_INFO),
	m_bCalibrate(false),
	m_bCaptureFrames(false),
	m_bCaptureSingleFrame(false),
	m_bCapturing(false),
	m_bStartPreRecordingProcess(false),
	m_bStartPostRecordingProcess(false),
	m_bConnected(false),
	m_bConfirmCaptured(false),
	m_bConfirmCalibrated(false),
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
	m_tOldFrameTime(0)

{
	pCapture = new AzureKinectCapture();

	LARGE_INTEGER qpf = { 0 };
	if (QueryPerformanceFrequency(&qpf))
	{
		m_fFreq = double(qpf.QuadPart);
	}

	m_vBounds.push_back(-0.5);
	m_vBounds.push_back(-0.5);
	m_vBounds.push_back(-0.5);
	m_vBounds.push_back(0.5);
	m_vBounds.push_back(0.5);
	m_vBounds.push_back(0.5);
}

LiveScanClient::~LiveScanClient()
{
	// clean up Direct2D renderer
	if (m_pD2DImageRenderer)
	{
		delete m_pD2DImageRenderer;
		m_pD2DImageRenderer = NULL;
	}

	if (pCapture)
	{
		delete pCapture;
		pCapture = NULL;
	}

	if (m_pRainbowColorDepth)
	{
		delete[] m_pRainbowColorDepth;
		m_pRainbowColorDepth = NULL;
	}

	if (m_pCameraSpaceCoordinates)
	{
		delete[] m_pCameraSpaceCoordinates;
		m_pCameraSpaceCoordinates = NULL;
	}

	if (m_pColorInColorSpace)
	{
		delete[] m_pColorInColorSpace;
		m_pColorInColorSpace = NULL;
	}

	if (m_pDepthInColorSpace)
	{
		delete[] m_pDepthInColorSpace;
		m_pDepthInColorSpace = NULL;
	}

	if (m_pClientSocket)
	{
		delete m_pClientSocket;
		m_pClientSocket = NULL;
	}

	// clean up Direct2D
	SafeRelease(m_pD2DFactory);

	delete emptyJPEGBuffer;
	emptyJPEGBuffer = NULL;

	if (emptyDepthMat) {
		emptyDepthMat->release();
		emptyDepthMat = NULL;
	}
}

int LiveScanClient::Run(HINSTANCE hInstance, int nCmdShow)
{
	MSG       msg = { 0 };
	WNDCLASS  wc;

	// Dialog custom window class
	ZeroMemory(&wc, sizeof(wc));
	wc.style = CS_HREDRAW | CS_VREDRAW;
	wc.cbWndExtra = DLGWINDOWEXTRA;
	wc.hCursor = LoadCursorW(NULL, IDC_ARROW);
	wc.hIcon = LoadIconW(hInstance, MAKEINTRESOURCE(IDI_APP));
	wc.lpfnWndProc = DefDlgProcW;
	wc.lpszClassName = L"LiveScanClientAppDlgWndClass";

	if (!RegisterClassW(&wc))
	{
		return 0;
	}

	// Create main application window
	HWND hWndApp = CreateDialogParamW(
		NULL,
		MAKEINTRESOURCE(IDD_APP),
		NULL,
		(DLGPROC)LiveScanClient::MessageRouter,
		reinterpret_cast<LPARAM>(this));

	// Show window
	ShowWindow(hWndApp, nCmdShow);

	// HOGUE
	::SetWindowPos(m_hWnd, HWND_TOP, g_winX, g_winY, g_winWidth, g_winHeight, NULL);
	std::thread t1(&LiveScanClient::SocketThreadFunction, this);
	// HOGUE
	if (g_connectToServerImmediately) Connect();

	// Main message loop
	while (WM_QUIT != msg.message)
	{
		UpdateFrame();

		while (PeekMessageW(&msg, NULL, 0, 0, PM_REMOVE))
		{
			if (WM_QUIT == msg.message)
			{
				break;
			}
			// If a dialog message will be taken care of by the dialog proc
			if (hWndApp && IsDialogMessageW(hWndApp, &msg))
			{
				continue;
			}

			TranslateMessage(&msg);
			DispatchMessageW(&msg);
		}
	}

	m_bSocketThread = false;
	t1.join();
	return static_cast<int>(msg.wParam);
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

	//Updates global settings on the device
	if (m_bUpdateSettings)
	{
		pCapture->SetExposureState(m_bAutoExposureEnabled, m_nExposureStep);
		m_bUpdateSettings = false;
	}

	if (m_bCloseCamera) 
	{
		m_bCameraError != CloseCamera();
		m_bCloseCamera = false;
		m_bConfirmCameraClosed = true;
	}

	if (m_bInitializeCamera)
	{
		m_bCameraError != InitializeCamera();
		m_bInitializeCamera = false;
		m_bConfirmCameraInitialized = true;
	}

	// +++ Below are functions that need the camera to be initialized +++	

	if (m_bStartPreRecordingProcess)
	{
		m_nFrameIndex = 0;
		m_vFrameTimestamps.clear();
		m_vFrameCount.clear();

		if (!m_bShowPreviewDuringRecording) {
			m_bPreviewDisabled = true;
		}

		m_bStartPreRecordingProcess = false;
		m_bConfirmPreRecordingProcess = true;
		m_bCapturing = true;
	}

	if (m_bStartPostRecordingProcess)
	{
		m_framesFileWriterReader.WriteTimestampLog(m_vFrameCount, m_vFrameTimestamps, configuration.nGlobalDeviceIndex);

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

		if (configuration.config.color_format == K4A_IMAGE_FORMAT_COLOR_MJPG)
			success = PostSyncRawFrames();

		
		if (configuration.config.color_format == K4A_IMAGE_FORMAT_COLOR_BGRA32)
			success = PostSyncPointclouds();

		
		SendPostSyncConfirmation(success);		
	}

	//We always need to capture the raw frame data
	if (pCapture->AquireRawFrame())
	{
		//We lock the network thread so it that the requirement variables don't change while we process the frame 
		std::lock_guard<std::mutex> lock(m_mSocketThreadMutex);

		//To optimize our use of system resources, we only process what is needed
		bool generateRBGData = false;
		bool generateDepthToColorData = false;
		bool generatePointcloud = false;

		if (m_eCaptureMode == CM_POINTCLOUD || m_bCalibrate || m_bRequestLiveFrame) {
			generateRBGData = true;
			generateDepthToColorData = true;
			generatePointcloud = true;
		}

		if (!m_bPreviewDisabled && !m_bShowDepth)
			generateRBGData = true;

		if (!m_bPreviewDisabled && m_bShowDepth)
			generateDepthToColorData = true;

		if (generateRBGData) {
			pCapture->DecodeRawColor();
			pCapture->DownscaleColorImgToDepthImgSize();
		}

		if (generateDepthToColorData)
			pCapture->MapDepthToColor();

		if (generatePointcloud)
			pCapture->GeneratePointcloud();


		if (m_bCaptureFrames || m_bCaptureSingleFrame)
		{
			uint64_t timeStamp = pCapture->GetTimeStamp();
			m_vFrameCount.push_back(m_nFrameIndex);
			m_vFrameTimestamps.push_back(timeStamp);
			m_nFrameIndex++;

			if (m_eCaptureMode == CM_RAW)
			{
				SaveRawFrame();
			}

			else if (m_eCaptureMode == CM_POINTCLOUD)
			{
				StoreFrame(pCapture->pointCloudImage, &pCapture->colorBGR);
				SavePointcloudFrame(timeStamp);
			}

			m_bCaptureSingleFrame = false;
			m_bConfirmCaptured = true;

			//Save the time since the last capture to estimate FPS. While recording, we only save the time after having stored a frame, so that the user gets a grasp of how fast the recording is taking place
			m_tOldFrameTime = m_tFrameTime;
			m_tFrameTime = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch());
		}

		if(!m_bCapturing)
		{
			m_tOldFrameTime = m_tFrameTime;
			m_tFrameTime = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch());
		}

		if (m_bCalibrate) {
			Calibrate();
		}

		if (m_bRequestLiveFrame) {
			StoreFrame(pCapture->pointCloudImage, &pCapture->colorBGR);
			SendFrame(m_vLastFrameVertices, m_vLastFrameVerticesSize, m_vLastFrameRGB, true);
			m_bRequestLiveFrame = false;
		}

		if (!m_bPreviewDisabled)
		{
			if (!m_bShowDepth)
				ShowColor();
			else
				ShowDepth();
		}

		else {
			ShowPreviewDisabled();
		}
	}

	ShowFPS();
}

LRESULT CALLBACK LiveScanClient::MessageRouter(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	LiveScanClient* pThis = NULL;

	if (WM_INITDIALOG == uMsg)
	{
		pThis = reinterpret_cast<LiveScanClient*>(lParam);
		SetWindowLongPtr(hWnd, GWLP_USERDATA, reinterpret_cast<LONG_PTR>(pThis));
	}
	else
	{
		pThis = reinterpret_cast<LiveScanClient*>(::GetWindowLongPtr(hWnd, GWLP_USERDATA));
	}

	if (pThis)
	{
		return pThis->DlgProc(hWnd, uMsg, wParam, lParam);
	}

	return 0;
}
// HOGUE
void LiveScanClient::Connect() {
	std::lock_guard<std::mutex> lock(m_mSocketThreadMutex);

	if (m_bConnected)
	{
		log.LogInfo("Disconnecting from server");
		delete m_pClientSocket;
		m_pClientSocket = NULL;

		m_bConnected = false;
		SetDlgItemTextA(m_hWnd, IDC_BUTTON_CONNECT, "Connect");
	}
	else
	{
		try
		{
			log.LogInfo("Trying to connect to server");
			char address[20];
			GetDlgItemTextA(m_hWnd, IDC_IP, address, 20);
			m_pClientSocket = new SocketClient(address, 48001);

			m_bConnected = true;
			if (calibration.bCalibrated)
				m_bConfirmCalibrated = true;

			SetDlgItemTextA(m_hWnd, IDC_BUTTON_CONNECT, "Disconnect");
			//Clear the status bar so that the "Failed to connect..." disappears.
			SetStatusMessage(L"", 1, true);
		}
		catch (...)
		{
			log.LogInfo("Couldn't connect to server");
			SetStatusMessage(L"Failed to connect. Did you start the server?", 10000, true);
		}
	}
}
LRESULT CALLBACK LiveScanClient::DlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	UNREFERENCED_PARAMETER(wParam);
	UNREFERENCED_PARAMETER(lParam);

	switch (message)
	{
	case WM_INITDIALOG:
	{
		// Bind application window handle
		m_hWnd = hWnd;

#ifdef _DEBUG
		if (m_loglevel <= Log::LOGLEVEL_DEBUG)
			m_loglevel = Log::LOGLEVEL_DEBUG;
#endif

		// Init Direct2D
		D2D1CreateFactory(D2D1_FACTORY_TYPE_SINGLE_THREADED, &m_pD2DFactory);

		// Get and initialize the default Kinect sensor as standalone
		configuration = *new KinectConfiguration();
		configuration.eSoftwareSyncState = Standalone;
		bool res = pCapture->Initialize(configuration);
		if (res)
		{
			bool openConsole = false;
			if (m_loglevel >= Log::LOGLEVEL_DEBUG)
			{
				openConsole = true;
			}

			bool logging = log.StartLog(pCapture->serialNumber, m_loglevel, openConsole);
			if(!logging)
				SetStatusMessage(L"Log file could not be created!", 10000, true);


			log.LogInfo("Device could be opened successfully");

			configuration.eHardwareSyncState = static_cast<SYNC_STATE>(pCapture->GetSyncJackState());
			calibration.LoadCalibration(pCapture->serialNumber);
			//m_pDepthRGBX = new RGB[pCapture->nColorFrameWidth * pCapture->nColorFrameHeight];
			m_pDepthInColorSpace = new UINT16[pCapture->nColorFrameWidth * pCapture->nColorFrameHeight];
			m_pCameraSpaceCoordinates = new Point3f[pCapture->nColorFrameWidth * pCapture->nColorFrameHeight];
			m_pColorInColorSpace = new RGB[pCapture->nColorFrameWidth * pCapture->nColorFrameHeight];
			pCapture->SetExposureState(true, 0);
		}
		else
		{
			SetStatusMessage(L"Capture device failed to initialize!", 10000, true);
		}

		ReadIPFromFile();
	}
	break;
	case WM_SIZING: {// HOGUE
		/*	RECT r;
			::GetWindowRect(m_hWnd, &r);
			int w = abs(r.right - r.left);
			int h = abs(r.bottom - r.top);
			::SetWindowPos(m_hWnd, HWND_TOP, 0, 0, w, (w / 1.55) + 200, NULL);*/
	}
	case WM_SIZE: {
		// HOGUE: this "works" but is pretty dumb logic, needs fewer hardcoded things but it serves its purpose
		RECT r;
		::GetWindowRect(m_hWnd, &r);
		int w = abs(r.right - r.left);
		int h = abs(r.bottom - r.top);
		int cw = 90;
		int ch = 12;
		float asp = 1920 / 1080;// pCapture->nColorFrameWidth / pCapture->nColorFrameHeight;
		int h2 = w / asp;
		int startB = 80;
		int fixedHeight = 3 * ch + startB;

		::SetWindowPos(GetDlgItem(m_hWnd, IDC_BUTTON_CONNECT), HWND_TOP, 0, h - (ch / 2) - startB, cw, ch, SWP_NOSIZE);
		::SetWindowPos(GetDlgItem(m_hWnd, IDC_BUTTON_SWITCH), HWND_TOP, 0, h - 2 * ch - (ch / 2) - startB, cw, ch, SWP_NOSIZE);

		::SetWindowPos(GetDlgItem(m_hWnd, IDC_IP), HWND_TOP, cw + cw / 2, h - (ch / 2) - startB, cw, ch, SWP_NOSIZE);
		::SetWindowPos(GetDlgItem(m_hWnd, IDC_STATIC), HWND_TOP, cw + cw / 2, h - 2 * ch - (ch / 2) - startB, cw, ch, SWP_NOSIZE);
		::SetWindowPos(GetDlgItem(m_hWnd, IDC_STATUS), HWND_TOP, 0, h - 60, w, ch * 2, NULL);

		::SetWindowPos(GetDlgItem(m_hWnd, IDC_VIDEOVIEW), HWND_TOP, 0, 0, w, h - fixedHeight, NULL);

		break;
	}
				// If the titlebar X is clicked, destroy app
	case WM_CLOSE:
		pCapture->Close();
		WriteIPToFile();
		DestroyWindow(hWnd);
		break;
	case WM_DESTROY:
		// Quit the main message pump
		PostQuitMessage(0);
		break;

		// Handle button press
	case WM_COMMAND:
		if (IDC_BUTTON_CONNECT == LOWORD(wParam) && BN_CLICKED == HIWORD(wParam))
		{
			Connect();
		}
		if (IDC_BUTTON_SWITCH == LOWORD(wParam) && BN_CLICKED == HIWORD(wParam))
		{
			m_bShowDepth = !m_bShowDepth;

			if (m_bShowDepth)
			{
				SetDlgItemTextA(m_hWnd, IDC_BUTTON_SWITCH, "Show color");
			}
			else
			{
				SetDlgItemTextA(m_hWnd, IDC_BUTTON_SWITCH, "Show depth");
			}
		}

		break;
	}

	return FALSE;
}

void LiveScanClient::SaveRawFrame()
{
	m_framesFileWriterReader.WriteColorJPGFile(k4a_image_get_buffer(pCapture->colorImageMJPG), k4a_image_get_size(pCapture->colorImageMJPG), m_nFrameIndex, "");
	m_framesFileWriterReader.WriteDepthTiffFile(pCapture->depthImage16Int, m_nFrameIndex, "");
}

void LiveScanClient::SavePointcloudFrame(uint64_t timeStamp)
{
	m_framesFileWriterReader.writeNextBinaryFrame(m_vLastFrameVertices, m_vLastFrameVerticesSize, m_vLastFrameRGB, timeStamp, configuration.nGlobalDeviceIndex);
}

void LiveScanClient::Calibrate() {

	log.LogInfo("Start Calibration process");

	Point3f* pCameraCoordinates = new Point3f[pCapture->nColorFrameWidth * pCapture->nColorFrameHeight];
	pCapture->PointCloudImageToPoint3f(pCameraCoordinates);

	bool res = calibration.Calibrate(&pCapture->colorBGR, pCameraCoordinates, pCapture->nColorFrameWidth, pCapture->nColorFrameHeight);

	delete[] pCameraCoordinates;

	if (res)
	{
		log.LogInfo("Calibration Successfull");
		calibration.SaveCalibration(pCapture->serialNumber);
		m_bConfirmCalibrated = true;
		m_bCalibrate = false;
	}

	else
		log.LogInfo("Calibration unsuccessfull");

}

/// <summary>
/// Create and initialize a new Direct2D image renderer (take a look at ImageRenderer.h).
/// We'll use this to draw the data we receive from the Kinect to the screen
/// </summary>
/// <returns></returns>
void LiveScanClient::ManagePreviewWindowInitialization() {


	bool initializationNeeded = false;

	if (m_pD2DImageRenderer != NULL)
	{
		//If our image height/width has changed, we need to reinitialize our D2D Renderer
		if (pCapture->nColorFrameWidth * pCapture->nColorFrameHeight != m_pD2DImageRenderer->GetRenderHeight() * m_pD2DImageRenderer->GetRenderWidth())
			initializationNeeded = true;
	}

	else
		initializationNeeded = true;


	if (initializationNeeded)
	{
		log.LogInfo("Initializing Preview Window");

		//if there already is a preview D2D Renderer, delete it
		if (m_pD2DImageRenderer)
		{
			delete m_pD2DImageRenderer;
			m_pD2DImageRenderer = NULL;
		}

		if (m_pRainbowColorDepth) {
			delete[] m_pRainbowColorDepth;
			m_pRainbowColorDepth = NULL;
		}

		HRESULT hr;
		m_pD2DImageRenderer = new ImageRenderer();
		hr = m_pD2DImageRenderer->Initialize(GetDlgItem(m_hWnd, IDC_VIDEOVIEW), m_pD2DFactory, pCapture->nColorFrameWidth, pCapture->nColorFrameHeight, pCapture->colorBGR.step);
		if (FAILED(hr))
		{
			log.LogFatal("Failed to initialize the Direct2D Draw device");
			SetStatusMessage(L"Failed to initialize the Direct2D draw device.", 10000, true);
		}

		//Initialize Preview Resources
		try {
			m_cvPreviewDisabled = cv::imread("resources/preview_disabled.png");
		}

		catch (cv::Exception e) {
			log.LogFatal(e.what());
			SetStatusMessage(L"Failed to load resources", 10000, true);
		}


		m_pRainbowColorDepth = new RGB[pCapture->nColorFrameHeight * pCapture->nColorFrameWidth];
		cv::resize(m_cvPreviewDisabled, m_cvPreviewDisabled, cv::Size(pCapture->nColorFrameWidth, pCapture->nColorFrameHeight), cv::INTER_AREA);
		cv::cvtColor(m_cvPreviewDisabled, m_cvPreviewDisabled, cv::COLOR_BGR2BGRA);
	}

}

void LiveScanClient::ShowDepth()
{
	// Make sure we've received valid data
	if (pCapture->transformedDepthImage != NULL)
	{
		ManagePreviewWindowInitialization();

		uint16_t* pointCloudImageData = (uint16_t*)(void*)k4a_image_get_buffer(pCapture->transformedDepthImage);

		for (int i = 0; i < pCapture->nColorFrameWidth * pCapture->nColorFrameHeight; i++)
		{
			BYTE intensity = pointCloudImageData[i] / 40;
			m_pRainbowColorDepth[i].rgbRed = rainbowLookup[intensity][0];
			m_pRainbowColorDepth[i].rgbGreen = rainbowLookup[intensity][1];
			m_pRainbowColorDepth[i].rgbBlue = rainbowLookup[intensity][2];
		}

		// Draw the data with Direct2D
		m_pD2DImageRenderer->Draw(reinterpret_cast<BYTE*>(m_pRainbowColorDepth), pCapture->nColorFrameWidth * pCapture->nColorFrameHeight * sizeof(RGB), pCapture->vBodies);
	}
}

void LiveScanClient::ShowColor()
{
	// Make sure we've received valid data
	if (!pCapture->colorBGR.empty())
	{
		ManagePreviewWindowInitialization();

		// Draw the data with Direct2D
		m_pD2DImageRenderer->Draw(reinterpret_cast<BYTE*>(pCapture->colorBGR.data), long(pCapture->colorBGR.total() * pCapture->colorBGR.elemSize()), pCapture->vBodies);
	}
}

void LiveScanClient::ShowPreviewDisabled()
{
	ManagePreviewWindowInitialization();

	m_pD2DImageRenderer->Draw(reinterpret_cast<BYTE*>(m_cvPreviewDisabled.data), long(m_cvPreviewDisabled.total() * m_cvPreviewDisabled.elemSize()), pCapture->vBodies);
	m_bPreviewDisabled = true;
}



bool LiveScanClient::SetStatusMessage(_In_z_ WCHAR* szMessage, DWORD nShowTimeMsec, bool bForce)
{
	INT64 now = GetTickCount64();

	if (m_hWnd && (bForce || (m_nNextStatusTime <= now)))
	{
		SetDlgItemText(m_hWnd, IDC_STATUS, szMessage);
		m_nNextStatusTime = now + nShowTimeMsec;

		return true;
	}

	return false;
}

void LiveScanClient::SocketThreadFunction()
{
	while (m_bSocketThread)
	{
		std::this_thread::sleep_for(std::chrono::milliseconds(1));
		HandleSocket();
	}
}

//This is running on a seperate thread!
//TODO: Put the whole sending/receiving in a seperate file/class, it's taking up a lot of space!

void LiveScanClient::HandleSocket()
{
	char byteToSend;
	std::lock_guard<std::mutex> lock(m_mSocketThreadMutex);

	if (!m_bConnected)
	{
		return;
	}

	string received = m_pClientSocket->ReceiveBytes();

	if (!received.empty()) {

		std::ostringstream message;
		message << "Message Raw data: ";

		for (unsigned int i = 0; i < received.length(); i++)
			message << (int)received[i] << " ";

		log.LogTrace(message.str());
	}

	for (unsigned int i = 0; i < received.length(); i++)
	{
		log.LogTrace("Received Server message");

		//Capture a single frame. Used for network-synced recording
		if (received[i] == MSG_CAPTURE_SINGLE_FRAME)
		{
			log.LogTrace("Capture single frame Received");
			m_bCaptureSingleFrame = true;
		}

		//Capture frames as fast as possible. Used for hardware-synced, or not-synced recording
		if (received[i] == MSG_START_CAPTURING_FRAMES)
		{
			log.LogTrace("Capture frames start received");
			m_bCaptureFrames = true;
		}

		if (received[i] == MSG_STOP_CAPTURING_FRAMES)
		{
			log.LogTrace("Capture frames stop received");
			m_bCaptureFrames = false;
		}

		if (received[i] == MSG_PRE_RECORD_PROCESS_START)
		{
			log.LogTrace("Received pre recording process start");
			m_bStartPreRecordingProcess = true;
		}

		if (received[i] == MSG_POST_RECORD_PROCESS_START)
		{
			log.LogTrace("Received post recording process start");
			m_bStartPostRecordingProcess = true;
		}

		//calibrate
		else if (received[i] == MSG_CALIBRATE)
		{
			log.LogTrace("Calibrate command recieved");
			m_bCalibrate = true;
		}

		else if (received[i] == MSG_CLOSE_CAMERA)
		{
			log.LogTrace("Closing camera command received");
			m_bCloseCamera = true;
		}

		else if (received[i] == MSG_INIT_CAMERA)
		{
			log.LogTrace("Initialize camera command received");
			m_bInitializeCamera = true;
		}

		else if (received[i] == MSG_SET_CONFIGURATION)
		{
			log.LogInfo("Recieved new configuration");

			i++;
		std:string message;
			//TODO: this can be done with substrings, im sure.
			for (int x = 0; x < KinectConfiguration::byteLength; x++)
			{
				message.push_back(received[i + x]);
			}

			i += KinectConfiguration::byteLength;
			configuration.SetFromBytes(message);

			i--;
		}

		//receive settings
		//TODO: what if packet is split?
		else if (received[i] == MSG_RECEIVE_SETTINGS)
		{
			log.LogInfo("Recieved new settings");

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
				for (int k = 0; k < 3; k++)
				{
					for (int l = 0; l < 3; l++)
					{
						calibration.markerPoses[j].R[k][l] = *(float*)(received.c_str() + i);
						i += sizeof(float);
					}
				}

				for (int k = 0; k < 3; k++)
				{
					calibration.markerPoses[j].t[k] = *(float*)(received.c_str() + i);
					i += sizeof(float);
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

			int exportFormat = *(int*)(received.c_str() + i);
			i += sizeof(int);

			if (exportFormat == 0)
			{
				log.LogInfo("Export format set to Pointcloud");
				m_eCaptureMode = CM_POINTCLOUD;
			}

			if (exportFormat == 1)
			{
				log.LogInfo("Export format set to Raw Data");
				m_eCaptureMode = CM_RAW;

			}

			m_nExtrinsicsStyle = *(int*)(received.c_str() + i);
			i += sizeof(int);

			m_bShowPreviewDuringRecording = (received[i] != 0);
			i++;

			m_bUpdateSettings = true;

			std::ostringstream stream;
			stream << "Received Settings: Auto Exposure enabled= " << m_bAutoExposureEnabled << ", Exposure Step = " << m_nExposureStep
				<< ", Extrinsics Stlye = " << m_nExtrinsicsStyle << ", Show preview during capture = " << m_bShowPreviewDuringRecording;
			Log::Get().LogDebug(stream.str());

			//so that we do not lose the next character in the stream
			i--;
		}

		//send configuration
		else if (received[i] == MSG_REQUEST_CONFIGURATION)
		{
			log.LogTrace("Server requests configuration");
			m_bRequestConfiguration = true;
		}

		//send stored frame
		else if (received[i] == MSG_REQUEST_STORED_FRAME)
		{
			log.LogCaptureDebug("Server requests stored frame");

			Point3s* points = NULL;
			RGB* colors = NULL;
			int pointsSize;
			int timeStamp;

			bool res = m_framesFileWriterReader.readNextBinaryFrame(points, colors, pointsSize, timeStamp);
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
			log.LogCaptureDebug("Server requests lastest frame");
			m_bRequestLiveFrame = true;
		}

		//receive calibration data
		else if (received[i] == MSG_RECEIVE_CALIBRATION)
		{
			log.LogInfo("Recieving calibration data");

			i++;
			for (int j = 0; j < 3; j++)
			{
				for (int k = 0; k < 3; k++)
				{
					calibration.worldR[j][k] = *(float*)(received.c_str() + i);
					i += sizeof(float);
				}
			}
			for (int j = 0; j < 3; j++)
			{
				calibration.worldT[j] = *(float*)(received.c_str() + i);
				i += sizeof(float);
			}

			//so that we do not lose the next character in the stream
			i--;
		}
		else if (received[i] == MSG_CLEAR_STORED_FRAMES)
		{
			log.LogTrace("Recieving command to clear stored frames");
			m_framesFileWriterReader.closeFileIfOpened();
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

			if (m_framesFileWriterReader.CreateRecordDirectory(dirPath, configuration.nGlobalDeviceIndex))
			{
				buffer[1] = 1;
				m_pClientSocket->SendBytes(buffer, size);
			}

			else
			{
				//Tell the server that the directory creation has failed, server will abort the recording
				buffer[1] = 0;
				m_pClientSocket->SendBytes(buffer, size);
				log.LogWarning("Recording directory creation has failed");
			}

			//Write the calibration intrinsics into the newly created dir if we record raw frames
			if (configuration.config.color_format != K4A_IMAGE_FORMAT_COLOR_BGRA32)
				m_framesFileWriterReader.WriteCalibrationJSON(configuration.nGlobalDeviceIndex, pCapture->calibrationBuffer, pCapture->nCalibrationSize);

		}

		else if (received[i] == MSG_REQUEST_TIMESTAMP_LIST)
		{
			log.LogTrace("Server requests timestamp list");
			m_bSendTimeStampList = true;
		}

		else if (received[i] == MSG_RECEIVE_POSTSYNC_LIST)
		{
			log.LogInfo("Received Postsync List");

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
		log.LogTrace("Confirming capture to server");

		byteToSend = MSG_CONFIRM_CAPTURED;
		m_pClientSocket->SendBytes(&byteToSend, 1);
		m_bConfirmCaptured = false;
	}

	if (m_bConfirmCalibrated)
	{
		log.LogTrace("Sending calibration confirmed");

		int size = (9 + 3) * sizeof(float) + sizeof(int) + 1;
		char* buffer = new char[size];
		buffer[0] = MSG_CONFIRM_CALIBRATED;
		int i = 1;

		memcpy(buffer + i, &calibration.iUsedMarkerId, 1 * sizeof(int));
		i += 1 * sizeof(int);
		memcpy(buffer + i, calibration.worldR[0].data(), 3 * sizeof(float));
		i += 3 * sizeof(float);
		memcpy(buffer + i, calibration.worldR[1].data(), 3 * sizeof(float));
		i += 3 * sizeof(float);
		memcpy(buffer + i, calibration.worldR[2].data(), 3 * sizeof(float));
		i += 3 * sizeof(float);
		memcpy(buffer + i, calibration.worldT.data(), 3 * sizeof(float));
		i += 3 * sizeof(float);

		m_pClientSocket->SendBytes(buffer, size);
		m_bConfirmCalibrated = false;
	}

	if (m_bConfirmPreRecordingProcess) {

		log.LogTrace("Sending Pre Record Process confirmation");

		byteToSend = MSG_CONFIRM_PRE_RECORD_PROCESS;
		m_pClientSocket->SendBytes(&byteToSend, 1);
		m_bConfirmPreRecordingProcess = false;
	}

	if (m_bConfirmPostRecordingProcess) {

		log.LogTrace("Sending Post Record Process confirmation");

		byteToSend = MSG_CONFIRM_POST_RECORD_PROCESS;
		m_pClientSocket->SendBytes(&byteToSend, 1);
		m_bConfirmPostRecordingProcess = false;
	}

	if (m_bSendConfiguration)
	{
		log.LogInfo("Sending configuration");

		int size = configuration.byteLength + 1;
		char* buffer = new char[size];
		buffer[0] = MSG_CONFIGURATION;
		memcpy(buffer + 1, configuration.ToBytes(), KinectConfiguration::byteLength);
		m_pClientSocket->SendBytes(buffer, size);
		m_bSendConfiguration = false;
	}

	if (m_bSendTimeStampList)
	{
		log.LogInfo("Sending Timestamp list");

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

		if (m_bCameraError) {
			buffer[1] = 1; // = Error
			log.LogWarning("Camera could not be closed, reporting to server");
		}
		else {
			buffer[1] = 0; // = Success
			log.LogDebug("Camera could be closed successfully, reporting to server");
		}

		m_pClientSocket->SendBytes(buffer, size);
		m_bConfirmCameraClosed = false;
	}

	if (m_bConfirmCameraInitialized) 
	{
		int size = 2;
		char* buffer = new char[size];
		buffer[0] = MSG_CONFIRM_CAMERA_INIT;

		if (m_bCameraError) {
			buffer[1] = 1; // = Error
			log.LogWarning("Camera could not be initialized, reporting to server");
		}
		else {
			buffer[1] = 0; // = Success
			log.LogDebug("Camera could be initialized successfully, reporting to server");
		};

		m_pClientSocket->SendBytes(buffer, size);
		m_bConfirmCameraInitialized = false;
	}
}

bool LiveScanClient::CloseCamera()
{
	log.LogDebug("Closing Camera");

	bool res = false;
	res = pCapture->Close();
	if (!res) {
		log.LogFatal("Could not close camera");
		SetStatusMessage(L"Device failed to close! Please restart Application!", 10000, true);
		return false;
	}

	return true;
}

bool LiveScanClient::InitializeCamera()
{
	log.LogDebug("Initializing Camera");

	bool res = false;

	res = pCapture->Initialize(configuration);
	if (!res) {
		log.LogDebug("Could not initialize camera");
		SetStatusMessage(L"Device failed to initialize! Please restart Application!", 10000, true);
		return false;
	}

	else
	{
		configuration.eHardwareSyncState = static_cast<SYNC_STATE>(pCapture->GetSyncJackState());
		m_pDepthInColorSpace = new UINT16[pCapture->nColorFrameWidth * pCapture->nColorFrameHeight];
		m_pCameraSpaceCoordinates = new Point3f[pCapture->nColorFrameWidth * pCapture->nColorFrameHeight];
		m_pColorInColorSpace = new RGB[pCapture->nColorFrameWidth * pCapture->nColorFrameHeight];
	}

	return true;
}

void LiveScanClient::SendPostSyncConfirmation(bool success)
{
	int size = 2;
	char* buffer = new char[size];
	buffer[0] = MSG_CONFIRM_POSTSYNCED;

	if (success) {
		buffer[1] = 1;
		log.LogTrace("Post Sync Confirmation: Successfull");
	}
		
	else {
		log.LogTrace("Post Sync Confirmation: Failed");
		buffer[1] = 0;
	}

	m_pClientSocket->SendBytes(buffer, size);
}

void LiveScanClient::SendFrame(Point3s* vertices, int verticesSize, RGB* RGB, bool live)
{

	log.LogCaptureDebug("Sending Frame to server");

	int size = verticesSize * (3 + 3 * sizeof(short)) + sizeof(int);


	vector<char> buffer(size);
	int pos = 0;

	memcpy(buffer.data() + pos, &verticesSize, sizeof(verticesSize));
	pos += sizeof(verticesSize);

	for (unsigned int i = 0; i < verticesSize; i++)
	{
		buffer[pos++] = RGB[i].rgbRed;
		buffer[pos++] = RGB[i].rgbGreen;
		buffer[pos++] = RGB[i].rgbBlue;

		memcpy(buffer.data() + pos, vertices, sizeof(short) * 3);
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
	memcpy(header, (char*)&size, sizeof(size));
	memcpy(header + 4, (char*)&iCompression, sizeof(iCompression));

	char message;

	if (live)
		message = MSG_LAST_FRAME;

	else
		message = MSG_STORED_FRAME;
		
	m_pClientSocket->SendBytes(&message, 1);
	m_pClientSocket->SendBytes((char*)&header, sizeof(int) * 2);
	m_pClientSocket->SendBytes(buffer.data(), size);
}

void LiveScanClient::StoreFrame(k4a_image_t pointcloudImage, cv::Mat* colorImage)
{
	log.LogTrace("Storing Frame");

	unsigned int nVertices = pCapture->nColorFrameHeight * pCapture->nColorFrameWidth;

	int16_t* pointCloudImageData = (int16_t*)(void*)k4a_image_get_buffer(pointcloudImage);
	Point3f invalidPoint = Point3f(0, 0, 0, true);
	Point3f* allVertices = new Point3f[nVertices];
	int goodVerticesCount = 0;

	for (unsigned int vertexIndex = 0; vertexIndex < nVertices; vertexIndex++)
	{
		//As the resizing function doesn't return a valid RGB-Reserved value which indicates that this pixel is invalid,
		//we cut all vertices under a distance of 0.0001mm, as the invalid vertices always have a Z-Value of 0
		if (pointCloudImageData[3 * vertexIndex + 2] >= 0.0001) // TODO: Needed? && colorInDepth->data[vertexIndex] == 255)
		{
			Point3f temp;

			temp.X = pointCloudImageData[3 * vertexIndex + 0] / 1000.0f;
			temp.Y = pointCloudImageData[3 * vertexIndex + 1] / 1000.0f;
			temp.Z = pointCloudImageData[3 * vertexIndex + 2] / 1000.0f;

			if (calibration.bCalibrated)
			{
				temp.X += calibration.worldT[0];
				temp.Y += calibration.worldT[1];
				temp.Z += calibration.worldT[2];
				temp = RotatePoint(temp, calibration.worldR);

				if (temp.X < m_vBounds[0] || temp.X > m_vBounds[3]
					|| temp.Y < m_vBounds[1] || temp.Y > m_vBounds[4]
					|| temp.Z < m_vBounds[2] || temp.Z > m_vBounds[5])
				{
					allVertices[vertexIndex] = invalidPoint;
					continue;
				}
			}

			allVertices[vertexIndex] = temp;
			goodVerticesCount++;
		}

		else
		{
			allVertices[vertexIndex] = invalidPoint;
		}
	}


	delete[] m_vLastFrameVertices;
	delete[] m_vLastFrameRGB;

	m_vLastFrameVertices = new Point3s[goodVerticesCount];
	m_vLastFrameRGB = new RGB[goodVerticesCount];
	int validVerticesShortCounter = 0;

	uchar* colorValues = colorImage->data;

	//Copy all valid vertices into a clean vector 
	for (unsigned int i = 0; i < nVertices; i++)
	{
		if (!allVertices[i].Invalid)
		{
			RGB color;
			color.rgbRed = colorValues[i * 4];
			color.rgbGreen = colorValues[(i * 4) + 1];
			color.rgbBlue = colorValues[(i * 4) + 2];

			m_vLastFrameVertices[validVerticesShortCounter] = allVertices[i];
			m_vLastFrameRGB[validVerticesShortCounter] = color;
			validVerticesShortCounter++;
		}
	}

	m_vLastFrameVerticesSize = validVerticesShortCounter;

	delete[] allVertices;
}

void LiveScanClient::ShowFPS()
{
	if (m_hWnd)
	{
		long difference = std::chrono::duration_cast<std::chrono::milliseconds>(m_tFrameTime - m_tOldFrameTime).count();		
		m_fAverageFPS += (difference - m_fAverageFPS) * 0.3f;
		float fps = 1000 / m_fAverageFPS;

		WCHAR szStatusMessage[64];
		StringCchPrintf(szStatusMessage, _countof(szStatusMessage), L" FPS = %0.2f", fps);

		SetStatusMessage(szStatusMessage, 1000, false);
	}
}

void LiveScanClient::ReadIPFromFile()
{
	log.LogDebug("Reading IP from file: ");

	ifstream file;
	file.open("lastIP.txt");
	if (file.is_open())
	{
		char lastUsedIPAddress[20];
		file.getline(lastUsedIPAddress, 20);
		file.close();
		SetDlgItemTextA(m_hWnd, IDC_IP, lastUsedIPAddress);
		log.LogDebug(lastUsedIPAddress);
	}
}

void LiveScanClient::WriteIPToFile()
{
	log.LogDebug("Writing IP to file: ");

	ofstream file;
	file.open("lastIP.txt");
	char lastUsedIPAddress[20];
	GetDlgItemTextA(m_hWnd, IDC_IP, lastUsedIPAddress, 20);
	file << lastUsedIPAddress;
	file.close();
	
	log.LogDebug(lastUsedIPAddress);
}



bool LiveScanClient::PostSyncPointclouds() {

	log.LogDebug("Starting Post Sync for Pointclouds");

	bool success = true;

	Point3s* emptyPoints = new Point3s[0];
	RGB* emptyColors = new RGB[0];
	int timestamp = 0;

	//We open a new .bin file in which we copy and paste all the frames from the recorded .bin file,
	//but in the right order
	FrameFileWriterReader syncedFileWriter;
	syncedFileWriter.SetRecordingDirPath(m_framesFileWriterReader.GetRecordingDirPath());
	syncedFileWriter.openNewBinFileForWriting(configuration.nGlobalDeviceIndex, "synced");
	m_framesFileWriterReader.openCurrentBinFileForReading();

	for (size_t i = 0; i < m_vFrameID.size(); i++)
	{
		//-1 indicates that this device doesn't have a valid frame for this capture. To keep a good frame timing, we fill in an empty frame
		if (m_vFrameID[i] == -1) {

			if (!syncedFileWriter.writeNextBinaryFrame(emptyPoints,0, emptyColors, 0, configuration.nGlobalDeviceIndex)) {
				success = false;
			}
		}

		else {

			Point3s* points;
			RGB* colors;
			int pointsSize;

			m_framesFileWriterReader.seekBinaryReaderToFrame(m_vFrameID[i]);
			m_framesFileWriterReader.readNextBinaryFrame(points, colors, pointsSize, timestamp);
			if (!syncedFileWriter.writeNextBinaryFrame(points, pointsSize, colors, timestamp, configuration.nGlobalDeviceIndex)) {
				log.LogWarning("Could not write Pointcloud Frame during post sync. Frame ID: ");
				log.LogWarning(to_string(m_vFrameID[i]));
				success = false;
			}

			delete[] points;
			delete[] colors;
		}
	}

	//Close our synced .bin file...
	std::string syncedFilePath = syncedFileWriter.GetBinFilePath();
	syncedFileWriter.closeFileIfOpened();
	m_framesFileWriterReader.closeAndDeleteFile(); //...delete the old .bin file...
	m_framesFileWriterReader.openNewBinFileForReading(syncedFilePath); //... and open it into the default framefilewriter, so that when the server requests stored frames, the client knows where to look
	
	return success;	
}

bool LiveScanClient::PostSyncRawFrames() {

	log.LogDebug("Starting Post Sync for Raw frames");


	bool success = true;
	k4a_image_t emptyDepthFrame;
	k4a_image_create(K4A_IMAGE_FORMAT_DEPTH16, 1, 1, 0, &emptyDepthFrame);

	for (size_t i = 0; i < m_vFrameID.size(); i++)
	{
		//-1 indicates that this device doesn't have a valid frame for this capture. To keep a good frame timing, we fill in an empty frame
		if (m_vFrameID[i] == -1) {
			m_framesFileWriterReader.WriteColorJPGFile(emptyJPEGBuffer->data(), emptyJPEGBuffer->size(), m_vPostSyncedFrameID[i], "synced");
			m_framesFileWriterReader.WriteDepthTiffFile(emptyDepthFrame, m_vPostSyncedFrameID[i], "synced");
		}

		else {
			if (!m_framesFileWriterReader.RenameRawFramePair(m_vFrameID[i], m_vPostSyncedFrameID[i], std::string("synced_"))) {
				log.LogWarning("Could not rename Frame during post sync. Frame ID: ");
				log.LogWarning(to_string(m_vFrameID[i]));
				success = false;
			}
		}		
	}

	return success;
}
