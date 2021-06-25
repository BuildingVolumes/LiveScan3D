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
#include <shellapi.h>

std::mutex m_mSocketThreadMutex;

int g_winWidth=800;
int g_winHeight=800;
int g_winX=0;
int g_winY=0;

int APIENTRY wWinMain(
	_In_ HINSTANCE hInstance,
    _In_opt_ HINSTANCE hPrevInstance,
    _In_ LPWSTR lpCmdLine,
    _In_ int nShowCmd
    )
{
    UNREFERENCED_PARAMETER(hPrevInstance);
   // UNREFERENCED_PARAMETER(lpCmdLine);
	//std::cout << lpCmdLine<<std::endl;
	LPWSTR* szArgList;
	int argCount; 
	szArgList = CommandLineToArgvW(GetCommandLine(), &argCount);
	if (argCount == 5) {
		// assume window width, height, x, y
		g_winWidth = _wtoi(szArgList[1]);
		g_winHeight = _wtoi(szArgList[2]);
		g_winX= _wtoi(szArgList[3]);
		g_winY = _wtoi(szArgList[4]);
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
	m_pDrawColor(NULL),
	m_pDepthRGBX(NULL),
	m_pBlankGreyImage(NULL),
	m_pCameraSpaceCoordinates(NULL),
	m_pColorInColorSpace(NULL),
	m_pDepthInColorSpace(NULL),
	m_bCalibrate(false), 
	m_bCollectMarkers(false),
	m_bFilter(false),
	m_bStreamOnlyBodies(false),
	m_bCaptureFrame(false),
	m_bConnected(false),
	m_bConfirmCaptured(false),
	m_bConfirmCalibrated(false),
	m_bConfirmRestartAsMaster(false),
	m_bShowDepth(false),
	m_bSocketThread(true),
	m_bFrameCompression(true),
	m_iCompressionLevel(2),
	m_pClientSocket(NULL),
	m_nFilterNeighbors(10),
	m_fFilterThreshold(0.01f),
	m_bRestartingCamera(false),
	m_bAutoExposureEnabled(true),
	m_nExposureStep(-5),
	m_nExtrinsicsStyle(0), // 0 = no export of extrinsics
	m_nFrameIndex(0)
{
	pCapture = new AzureKinectCapture();

    LARGE_INTEGER qpf = {0};
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
    if (m_pDrawColor)
    {
        delete m_pDrawColor;
        m_pDrawColor = NULL;
    }

	if (pCapture)
	{
		delete pCapture;
		pCapture = NULL;
	}

	if (m_pDepthRGBX)
	{
		delete[] m_pDepthRGBX;
		m_pDepthRGBX = NULL;
	}

	if (m_pBlankGreyImage) 
	{
		delete[] m_pBlankGreyImage;
		m_pBlankGreyImage = NULL;
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
}

int LiveScanClient::Run(HINSTANCE hInstance, int nCmdShow)
{
    MSG       msg = {0};
    WNDCLASS  wc;

	// Dialog custom window class
    ZeroMemory(&wc, sizeof(wc));
    wc.style         = CS_HREDRAW | CS_VREDRAW;
    wc.cbWndExtra    = DLGWINDOWEXTRA;
    wc.hCursor       = LoadCursorW(NULL, IDC_ARROW);
    wc.hIcon         = LoadIconW(hInstance, MAKEINTRESOURCE(IDI_APP));
    wc.lpfnWndProc   = DefDlgProcW;
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

	::SetWindowPos(m_hWnd, HWND_TOP, g_winX,g_winY, g_winWidth,g_winHeight, NULL);
	std::thread t1(&LiveScanClient::SocketThreadFunction, this);
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
	if (!pCapture->bInitialized)
	{
		return;
	}

	//Recording raw frames
	if(configuration.config.color_format == K4A_IMAGE_FORMAT_COLOR_MJPG)
	{
		bool bNewFrameAcquired = pCapture->AquireRawFrame();

		if (!bNewFrameAcquired)
			return;

		if (m_bCaptureFrame) 
		{
			m_framesFileWriterReader.WriteColorJPGFile(k4a_image_get_buffer(pCapture->colorImage), k4a_image_get_size(pCapture->colorImage), m_nFrameIndex);
			m_framesFileWriterReader.WriteDepthTiffFile(pCapture->depthImage, m_nFrameIndex);
			m_nFrameIndex++;

			m_bConfirmCaptured = true;
			m_bCaptureFrame = false;
		}
	}

	//Recording Pointclouds
	if (configuration.config.color_format == K4A_IMAGE_FORMAT_COLOR_BGRA32)
	{
		bool bNewFrameAcquired = pCapture->AquirePointcloudFrame();

		if (!bNewFrameAcquired)
			return;

		pCapture->MapColorFrameToCameraSpace(m_pCameraSpaceCoordinates);

		{
			std::lock_guard<std::mutex> lock(m_mSocketThreadMutex);
			StoreFrame(m_pCameraSpaceCoordinates, pCapture->pColorRGBX, pCapture->vBodies, pCapture->pBodyIndex);

			if (m_bCaptureFrame)
			{
				uint64_t timeStamp = pCapture->GetTimeStamp();
				m_framesFileWriterReader.writeFrame(m_vLastFrameVertices, m_vLastFrameRGB, timeStamp, pCapture->GetDeviceIndex());
				m_bConfirmCaptured = true;
				m_bCaptureFrame = false;
			}
		}

		if (m_bCalibrate)
		{
			std::lock_guard<std::mutex> lock(m_mSocketThreadMutex);
			Point3f* pCameraCoordinates = new Point3f[pCapture->nColorFrameWidth * pCapture->nColorFrameHeight];
			pCapture->MapColorFrameToCameraSpace(pCameraCoordinates);

			bool res = calibration.Calibrate(pCapture->pColorRGBX, pCameraCoordinates, pCapture->nColorFrameWidth, pCapture->nColorFrameHeight);

			delete[] pCameraCoordinates;

			if (res)
			{
				calibration.SaveCalibration(pCapture->serialNumber);
				m_bConfirmCalibrated = true;
				m_bCalibrate = false;
			}
		}
	}	

	if (!m_bShowDepth)
		ShowColor();
	else
		ShowDepth();

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

            // Init Direct2D
            D2D1CreateFactory(D2D1_FACTORY_TYPE_SINGLE_THREADED, &m_pD2DFactory);

            // Get and initialize the default Kinect sensor as standalone
			configuration = *new KinectConfiguration();
			configuration.state = Standalone;
			configuration.sync_offset = 0;
			bool res = pCapture->Initialize(configuration);
			if (res)
			{
				calibration.LoadCalibration(pCapture->serialNumber);
				m_pDepthRGBX = new RGB[pCapture->nColorFrameWidth * pCapture->nColorFrameHeight];
				m_pDepthInColorSpace = new UINT16[pCapture->nColorFrameWidth * pCapture->nColorFrameHeight];
				m_pCameraSpaceCoordinates = new Point3f[pCapture->nColorFrameWidth * pCapture->nColorFrameHeight];
				m_pColorInColorSpace = new RGB[pCapture->nColorFrameWidth * pCapture->nColorFrameHeight];
				pCapture->SetExposureState(true, 0);
			}
			else
			{
				SetStatusMessage(L"Capture device failed to initialize!", 10000, true);
			}

			// Create and initialize a new Direct2D image renderer (take a look at ImageRenderer.h)
			// We'll use this to draw the data we receive from the Kinect to the screen
			HRESULT hr;
			m_pDrawColor = new ImageRenderer();
			hr = m_pDrawColor->Initialize(GetDlgItem(m_hWnd, IDC_VIDEOVIEW), m_pD2DFactory, pCapture->nColorFrameWidth, pCapture->nColorFrameHeight, pCapture->nColorFrameWidth * sizeof(RGB));
			if (FAILED(hr))
			{
				SetStatusMessage(L"Failed to initialize the Direct2D draw device.", 10000, true);
			}

			CreateBlankGrayImage(pCapture->nColorFrameWidth, pCapture->nColorFrameHeight);

			ReadIPFromFile();


        }
        break;
		case WM_SIZING: {
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
				int h2 = w/asp;
				int startB = 80;
				int fixedHeight = 3 * ch + startB;
				
				::SetWindowPos(GetDlgItem(m_hWnd, IDC_BUTTON_CONNECT), HWND_TOP, 0, h - (ch/2) - startB, cw, ch, SWP_NOSIZE);
				::SetWindowPos(GetDlgItem(m_hWnd, IDC_BUTTON_SWITCH), HWND_TOP, 0, h - 2*ch -  (ch / 2) - startB, cw, ch, SWP_NOSIZE);
				
				::SetWindowPos(GetDlgItem(m_hWnd, IDC_IP), HWND_TOP,  cw + cw/2, h - (ch / 2) - startB, cw, ch, SWP_NOSIZE);
				::SetWindowPos(GetDlgItem(m_hWnd, IDC_STATIC), HWND_TOP,  cw + cw/2, h - 2 * ch - (ch / 2) - startB, cw, ch, SWP_NOSIZE);
				::SetWindowPos(GetDlgItem(m_hWnd, IDC_STATUS), HWND_TOP, 0, h - 60, w,ch*2, NULL);

				::SetWindowPos(GetDlgItem(m_hWnd, IDC_VIDEOVIEW), HWND_TOP,0,0,w,h-fixedHeight, NULL);
				
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
				std::lock_guard<std::mutex> lock(m_mSocketThreadMutex);
				if (m_bConnected)
				{
					delete m_pClientSocket;
					m_pClientSocket = NULL;

					m_bConnected = false;
					SetDlgItemTextA(m_hWnd, IDC_BUTTON_CONNECT, "Connect");
				}
				else
				{
					try
					{
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
						SetStatusMessage(L"Failed to connect. Did you start the server?", 10000, true);
					}
				}
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

void LiveScanClient::ShowDepth()
{
	if (configuration.config.color_format == K4A_IMAGE_FORMAT_COLOR_BGRA32)
	{
		// Make sure we've received valid data
		if (m_pDepthRGBX && m_pDepthInColorSpace)
		{
			pCapture->MapDepthFrameToColorSpace(m_pDepthInColorSpace);

			for (int i = 0; i < pCapture->nColorFrameWidth * pCapture->nColorFrameHeight; i++)
			{
				USHORT depth = m_pDepthInColorSpace[i];
				BYTE intensity = static_cast<BYTE>(depth % 256);

				m_pDepthRGBX[i].rgbRed = intensity;
				m_pDepthRGBX[i].rgbGreen = intensity;
				m_pDepthRGBX[i].rgbBlue = intensity;
			}

			// Draw the data with Direct2D
			m_pDrawColor->Draw(reinterpret_cast<BYTE*>(m_pDepthRGBX), pCapture->nColorFrameWidth * pCapture->nColorFrameHeight * sizeof(RGB), pCapture->vBodies);
		}
	}

	else 
	{
		m_pDrawColor->Draw(reinterpret_cast<BYTE*>(m_pBlankGreyImage), pCapture->nColorFrameWidth * pCapture->nColorFrameHeight * sizeof(RGB), pCapture->vBodies);
	}
	
}

void LiveScanClient::ShowColor()
{
	//Draw the preview, as we have the data already in a nice BGRA Format
	if (configuration.config.color_format == K4A_IMAGE_FORMAT_COLOR_BGRA32) 
	{
		// Make sure we've received valid data
		if (pCapture->pColorRGBX)
		{
			// Draw the data with Direct2D
			m_pDrawColor->Draw(reinterpret_cast<BYTE*>(pCapture->pColorRGBX), pCapture->nColorFrameWidth * pCapture->nColorFrameHeight * sizeof(RGB), pCapture->vBodies);
		}
	}
    
	//If we are recording in another image format, we just display a blank grey screen.
	//This saves performance, by not decoding the MJPG Frame into a RGB struct 
	else 
	{
		m_pDrawColor->Draw(reinterpret_cast<BYTE*>(m_pBlankGreyImage), pCapture->nColorFrameWidth * pCapture->nColorFrameHeight * sizeof(RGB), pCapture->vBodies);
	}
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

void LiveScanClient::HandleSocket()
{
	char byteToSend;
	std::lock_guard<std::mutex> lock(m_mSocketThreadMutex);

	if (!m_bConnected)
	{
		return;
	}

	string received = m_pClientSocket->ReceiveBytes();
	for (unsigned int i = 0; i < received.length(); i++)
	{
		//capture a frame
		if (received[i] == MSG_CAPTURE_FRAME)
		{
			m_bCaptureFrame = true;
		}
		//calibrate
		else if (received[i] == MSG_CALIBRATE)
			m_bCalibrate = true;
		else if (received[i] == MSG_COLLECTMARKERS) {
			// HOGUE TODO
			m_bCollectMarkers = true;
		}
		//Restart The Device without changing any settings - must be done after turning on/off temporal sync, and when changing depth mode.
		else if (received[i] == MSG_REINITIALIZE_WITH_CURRENT_SETTINGS) {
			{
				Reinitialize();
			}
		}
		//Enables Temporal sync on this client
		else if (received[i] == MSG_SET_TEMPSYNC_ON) {

			i++; //Get next byte (the sync Offset)
			configuration.sync_offset = received[i];

			//Determine if this device is a subordinate, master, or standalone
			int jackState = pCapture->GetSyncJackState();

			bool res = false;

			switch (jackState)
			{
			case -1:
				configuration.state = Subordinate;

				//Restart this device as Subordinate, with a unique syncOffset (send by the server)
				m_bRestartingCamera = true;

				Reinitialize();
				//Confirm to the server, that we set this device as subordinate
				m_bConfirmTempSyncState = true;
				break;

			case 0:
				configuration.state = Master;

				//Only Close this device, as it needs to wait for all subordinates to start, before starting itself
				m_bRestartingCamera = true;

				res = pCapture->Close();
				if (!res) {
					SetStatusMessage(L"Master device failed to close! Restart Application!", 10000, true);
					return;
				}

				m_bConfirmTempSyncState = true;
				break;

			case 1://Device is Standalone
				configuration.state = Standalone;

				Reinitialize();

				m_bConfirmTempSyncState = true;
				m_bRestartingCamera = false;
				break;
			default:
				break;
			}

		}

		//Sets this device as Standalone
		else if (received[i] == MSG_SET_TEMPSYNC_OFF) {
			configuration.sync_offset = 0;
			configuration.state = Standalone;
			Reinitialize();
			
			m_bConfirmTempSyncState = true;
			m_bRestartingCamera = false;
		}

		else if (received[i] == MSG_SET_DEPTHMODE) {
			i++;
			int depthMode = (int)received[i];

			//cast the integer to the enum.
			//"off" (0) and "passive_IR" (5) are not supported. 
			//see: https://microsoft.github.io/Azure-Kinect-Sensor-SDK/master/group___enumerations_ga3507ee60c1ffe1909096e2080dd2a05d.html
			k4a_depth_mode_t newDepthMode = static_cast<k4a_depth_mode_t>(depthMode);

			//Check if we actually changed anything before restarting.
			if (newDepthMode != configuration.config.depth_mode) {
				configuration.config.depth_mode = newDepthMode;
				Reinitialize();
			}
		}
		else if (received[i] == MSG_SET_CONFIGURATION)
		{
			i++;
			std:string message;
			//TODO: this can be done with substrings, im sure.
			for (int x = 0; x < KinectConfiguration::byteLength; x++)
			{
				message.push_back(received[i+x]);
			}

			i += KinectConfiguration::byteLength;
			configuration.SetFromBytes(message);
			
			//Update the configuration.

			//If it needs to be reinitialized, that will be sent by the server.
			pCapture->SetConfiguration(configuration);
		}

		//Got confirmation from the server that all subs have started, and we can now start the master 
		else if (received[i] == MSG_START_MASTER) {
			if (currentTempSyncState == MASTER) 
			{
				configuration.state = Master;
				configuration.sync_offset = 0;
				bool res = pCapture->Initialize(configuration);
				if (!res) {
					SetStatusMessage(L"Master device failed to initialize! Restart Application!", 10000, true);
					return;
				}

				m_bConfirmRestartAsMaster = true;
				m_bRestartingCamera = false;
			}
		}

		//receive settings
		//TODO: what if packet is split?
		else if (received[i] == MSG_RECEIVE_SETTINGS)
		{
			vector<float> bounds(6);
			i++;
			int nBytes = *(int*)(received.c_str() + i);
			i += sizeof(int);

			for (int j = 0; j < 6; j++)
			{
				bounds[j] = *(float*)(received.c_str() + i);
				i += sizeof(float);
			}

			m_bFilter = (received[i]!=0);
			i++;

			m_nFilterNeighbors = *(int*)(received.c_str() + i);
			i += sizeof(int);

			m_fFilterThreshold = *(float*)(received.c_str() + i);
			i += sizeof(float);

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

			m_bStreamOnlyBodies = (received[i] != 0);
			i += 1;

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

			pCapture->SetExposureState(m_bAutoExposureEnabled, m_nExposureStep);

			int exportFormat = *(int*)(received.c_str() + i);
			i += sizeof(int);

			if (exportFormat == 0) 
			{
				configuration.config.color_format = K4A_IMAGE_FORMAT_COLOR_BGRA32;
				Reinitialize();
			}

			if (exportFormat == 1)
			{
				configuration.config.color_format = K4A_IMAGE_FORMAT_COLOR_MJPG;
				SetStatusMessage(L"NOTICE: Preview will be disabled while recording raw data!", 10000, true);
				Reinitialize();
			}

			m_nExtrinsicsStyle = *(int*)(received.c_str() + i);
			i += sizeof(int);
			//TODO: Implement Extrinsics Export


			//so that we do not lose the next character in the stream
			i--;
		}
		//send configuration
		else if (received[i] == MSG_REQUEST_CONFIGURATION)
		{
			byteToSend = MSG_CONFIGURATION;
			m_pClientSocket->SendBytes(&byteToSend, 1);
			m_pClientSocket->SendBytes(configuration.ToBytes(), KinectConfiguration::byteLength);
		}
		//send stored frame
		else if (received[i] == MSG_REQUEST_STORED_FRAME)
		{
			byteToSend = MSG_STORED_FRAME;
			m_pClientSocket->SendBytes(&byteToSend, 1);

			vector<Point3s> points;
			vector<RGB> colors;
			bool res = m_framesFileWriterReader.readFrame(points, colors);
			if (res == false)
			{
				int size = -1;
				m_pClientSocket->SendBytes((char*)&size, 4);
			} else
				SendFrame(points, colors, m_vLastFrameBody);
		}
		//send last frame
		else if (received[i] == MSG_REQUEST_LAST_FRAME)
		{
			byteToSend = MSG_LAST_FRAME;
			m_pClientSocket->SendBytes(&byteToSend, 1);

			SendFrame(m_vLastFrameVertices, m_vLastFrameRGB, m_vLastFrameBody);
		}
		//receive calibration data
		else if (received[i] == MSG_RECEIVE_CALIBRATION)
		{
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
			m_framesFileWriterReader.closeFileIfOpened();
		}

		else if (received[i] == MSG_REQUEST_SYNC_JACK_STATE) 
		{
			int size = 3;
			char* buffer = new char[size];
			buffer[0] = MSG_SYNC_JACK_STATE;

			buffer[1] = pCapture->GetSyncJackState() + 1; //Gets the current Sync Jack State and adds + 1, as we can't send a negative value

			m_pClientSocket->SendBytes(buffer, size);
			m_bConfirmTempSyncState = false;
		}

		else if (received[i] == MSG_CREATE_DIR) //Creates a dir on the client. Message also marks the start of the recording
		{
			m_nFrameIndex = 0; 

			i++; 
			int stringLength = *(int*)(received.c_str() + i); //Get the length of the following string
			i += sizeof(int);

			std::string dirPath;

		    dirPath.assign(received, i, stringLength); //Recieved is already a string, so we just copy the characters out of it			

			//Confirmation message that we have created a valid new directory on this system
			int size = 2;
			char* buffer = new char[size];
			buffer[0] = MSG_CONFIRM_DIR_CREATION;

			if (m_framesFileWriterReader.CreateRecordDirectory(dirPath, pCapture->GetDeviceIndex())) 
			{
				buffer[1] = 1;
				m_pClientSocket->SendBytes(buffer, size);
			}

			else
			{
				//Tell the server that the directory creation has failed, server will abort the recording
				buffer[1] = 0;
				m_pClientSocket->SendBytes(buffer, size);
			}

			std::vector<uint8_t> calibration_buffer;
			size_t calibration_size = 0;

			//Write the calibration intrinsics to the newly created dir
			if (pCapture->GetIntrinsicsJSON(calibration_buffer, calibration_size)) {
				m_framesFileWriterReader.WriteCalibrationJSON(pCapture->GetDeviceIndex(), calibration_buffer, calibration_size);
			}

		}
	}

	if (m_bConfirmCaptured)
	{
		byteToSend = MSG_CONFIRM_CAPTURED;
		m_pClientSocket->SendBytes(&byteToSend, 1);
		m_bConfirmCaptured = false;
	}

	//Send validation to the server that this device has been set to a specific Sync State 
	if (m_bConfirmTempSyncState)
	{
		int size = 3; //Somehow it doesn't work when sending only two bytes?? (So we send an extra one)
		char* buffer = new char[size];
		buffer[0] = MSG_CONFIRM_TEMP_SYNC_STATUS;

		switch (currentTempSyncState)//TODO: extract to helper method.
		{
		case SUBORDINATE: buffer[1] = 0; break;
		case MASTER: buffer[1] = 1; break;
		case STANDALONE: buffer[1] = 2; break;
		}

		m_pClientSocket->SendBytes(buffer, size);
		m_bConfirmTempSyncState = false;
	}

	//Send validation to the server that the Master camera has started recording again
	if (m_bConfirmRestartAsMaster) 
	{
		byteToSend = MSG_CONFIRM_MASTER_RESTART;
		m_pClientSocket->SendBytes(&byteToSend, 1);
		m_bConfirmRestartAsMaster = false;
	}

	// THIS IS WHERE IT SENDS THE CALIBRATION INFO TO THE SERVER!
	if (m_bConfirmCalibrated)
	{
		int size = (9 + 3) * sizeof(float) + sizeof(int) + 1;
		char *buffer = new char[size];
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
	if (m_bCollectMarkers) {
		// send the current marker data if it exists
		// HOGUE TODO
	}
}

/// <summary>
/// Reinitialize. Must be called after changing depthMode or afer changing temporal sync mode.
/// </summary>
void LiveScanClient::Reinitialize()
{
	//override for when turning off temporal sync, as the jackState is set to something else and needs to be reset to this.

	bool res = false;
	res = pCapture->Close();
	if (!res) {
		SetStatusMessage(L"device failed to close! Restart Application!", 10000, true);
		return;
	}

	res = pCapture->Initialize(configuration);
	if (!res) {
		SetStatusMessage(L"device failed to reinitialize! Restart Application!", 10000, true);
		return;
	}

	CreateBlankGrayImage(pCapture->nColorFrameWidth, pCapture->nColorFrameHeight);
}

void LiveScanClient::SendFrame(vector<Point3s> vertices, vector<RGB> RGB, vector<Body> body)
{
	int size = RGB.size() * (3 + 3 * sizeof(short)) + sizeof(int);

	vector<char> buffer(size);
	char *ptr2 = (char*)vertices.data();
	int pos = 0;

	int nVertices = RGB.size();
	memcpy(buffer.data() + pos, &nVertices, sizeof(nVertices));
	pos += sizeof(nVertices);

	for (unsigned int i = 0; i < RGB.size(); i++)
	{
		buffer[pos++] = RGB[i].rgbRed;
		buffer[pos++] = RGB[i].rgbGreen;
		buffer[pos++] = RGB[i].rgbBlue;

		memcpy(buffer.data() + pos, ptr2, sizeof(short)* 3);
		ptr2 += sizeof(short) * 3;
		pos += sizeof(short) * 3;
	}

	int nBodies = body.size();
	size += sizeof(nBodies);
	for (int i = 0; i < nBodies; i++)
	{
		size += sizeof(body[i].bTracked);
		int nJoints = body[i].vJoints.size();
		size += sizeof(nJoints);
		size += nJoints * (3 * sizeof(float) + 2 * sizeof(int));
		size += nJoints * 2 * sizeof(float);
	}
	buffer.resize(size);

	memcpy(buffer.data() + pos, &nBodies, sizeof(nBodies));
	pos += sizeof(nBodies);

	for (int i = 0; i < nBodies; i++)
	{
		memcpy(buffer.data() + pos, &body[i].bTracked, sizeof(body[i].bTracked));
		pos += sizeof(body[i].bTracked);

		int nJoints = body[i].vJoints.size();
		memcpy(buffer.data() + pos, &nJoints, sizeof(nJoints));
		pos += sizeof(nJoints);

		for (int j = 0; j < nJoints; j++)
		{
			////Joint
			//memcpy(buffer.data() + pos, &body[i].vJoints[j].JointType, sizeof(JointType));
			//pos += sizeof(JointType);
			//memcpy(buffer.data() + pos, &body[i].vJoints[j].TrackingState, sizeof(TrackingState));
			//pos += sizeof(TrackingState);
			////Joint position
			//memcpy(buffer.data() + pos, &body[i].vJoints[j].Position.X, sizeof(float));
			//pos += sizeof(float);
			//memcpy(buffer.data() + pos, &body[i].vJoints[j].Position.Y, sizeof(float));
			//pos += sizeof(float);
			//memcpy(buffer.data() + pos, &body[i].vJoints[j].Position.Z, sizeof(float));
			//pos += sizeof(float);

			////JointInColorSpace
			//memcpy(buffer.data() + pos, &body[i].vJointsInColorSpace[j].X, sizeof(float));
			//pos += sizeof(float);
			//memcpy(buffer.data() + pos, &body[i].vJointsInColorSpace[j].Y, sizeof(float));
			//pos += sizeof(float);
		}
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

	m_pClientSocket->SendBytes((char*)&header, sizeof(int) * 2);
	m_pClientSocket->SendBytes(buffer.data(), size);
}

void LiveScanClient::StoreFrame(Point3f *vertices, RGB *colorInDepth, vector<Body> &bodies, BYTE* bodyIndex)
{
	unsigned int nVertices = pCapture->nColorFrameHeight * pCapture->nColorFrameWidth;

	//To save some processing cost, we allocate a full frame size (nVertices) of a Point3f Vector beforehand
	//instead of using push_back for each vertice. Even though we have to copy the vertices into a clean array
	//later and it uses a little bit more RAM, this gives us a nice speed increase for this function, around 25-50%
	Point3f invalidPoint = Point3f(0, 0, 0, true);
	vector<Point3f> AllVertices(nVertices);
	int goodVerticesCount = 0;

	for (unsigned int vertexIndex = 0; vertexIndex < nVertices; vertexIndex++)
	{
		if (m_bStreamOnlyBodies && bodyIndex[vertexIndex] >= bodies.size())
			continue;

		//As the resizing function doesn't return a valid RGB-Reserved value which indicates that this pixel is invalid,
		//we cut all vertices under a distance of 0.0001mm, as the invalid vertices always have a Z-Value of 0
		if (vertices[vertexIndex].Z >= 0.0001 && colorInDepth[vertexIndex].rgbReserved == 255)
		{
			Point3f temp = vertices[vertexIndex];
			RGB tempColor = colorInDepth[vertexIndex];
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
					AllVertices[vertexIndex] = invalidPoint;
					continue;
				}
					
			}

			AllVertices[vertexIndex] = temp;
			goodVerticesCount++;
		}

		else 
		{
			AllVertices[vertexIndex] = invalidPoint;
		}
	}

	vector<Body> tempBodies = bodies;

	//for (unsigned int i = 0; i < tempBodies.size(); i++)
	//{
	//	for (unsigned int j = 0; j < tempBodies[i].vJoints.size(); j++)
	//	{
	//		if (calibration.bCalibrated)
	//		{
	//			tempBodies[i].vJoints[j].Position.X += calibration.worldT[0];
	//			tempBodies[i].vJoints[j].Position.Y += calibration.worldT[1];
	//			tempBodies[i].vJoints[j].Position.Z += calibration.worldT[2];

	//			Point3f tempPoint(tempBodies[i].vJoints[j].Position.X, tempBodies[i].vJoints[j].Position.Y, tempBodies[i].vJoints[j].Position.Z);

	//			tempPoint = RotatePoint(tempPoint, calibration.worldR);

	//			tempBodies[i].vJoints[j].Position.X = tempPoint.X;
	//			tempBodies[i].vJoints[j].Position.Y = tempPoint.Y;
	//			tempBodies[i].vJoints[j].Position.Z = tempPoint.Z;
	//		}
	//	}
	//}

	vector<Point3f> goodVertices(goodVerticesCount);
	vector<RGB> goodColorPoints(goodVerticesCount);
	int goodVerticesShortCounter = 0;

	//Copy all valid vertices into a clean vector 
	for (unsigned int i = 0; i < AllVertices.size(); i++)
	{
		if (!AllVertices[i].Invalid) 
		{
			goodVertices[goodVerticesShortCounter] = AllVertices[i];
			goodColorPoints[goodVerticesShortCounter] = colorInDepth[i];
			goodVerticesShortCounter++;
		}
	}

	if (m_bFilter)
		filter(goodVertices, goodColorPoints, m_nFilterNeighbors, m_fFilterThreshold);


	vector<Point3s> goodVerticesShort(goodVertices.size());
	
	for (size_t i = 0; i < goodVertices.size(); i++)
	{
		goodVerticesShort[i] = goodVertices[i];
	}

	m_vLastFrameBody = tempBodies;
	m_vLastFrameVertices = goodVerticesShort;
	m_vLastFrameRGB = goodColorPoints;
}

void LiveScanClient::ShowFPS()
{
	if (m_hWnd)
	{
		double fps = 0.0;

		LARGE_INTEGER qpcNow = { 0 };
		if (m_fFreq)
		{
			if (QueryPerformanceCounter(&qpcNow))
			{
				if (m_nLastCounter)
				{
					m_nFramesSinceUpdate++;
					fps = m_fFreq * m_nFramesSinceUpdate / double(qpcNow.QuadPart - m_nLastCounter);
				}
			}
		}

		WCHAR szStatusMessage[64];
		StringCchPrintf(szStatusMessage, _countof(szStatusMessage), L" FPS = %0.2f", fps);

		if (SetStatusMessage(szStatusMessage, 1000, false))
		{
			m_nLastCounter = qpcNow.QuadPart;
			m_nFramesSinceUpdate = 0;
		}
	}
}

void LiveScanClient::ReadIPFromFile()
{
	ifstream file;
	file.open("lastIP.txt");
	if (file.is_open())
	{
		char lastUsedIPAddress[20];
		file.getline(lastUsedIPAddress, 20);
		file.close();
		SetDlgItemTextA(m_hWnd, IDC_IP, lastUsedIPAddress);
	}
}

void LiveScanClient::WriteIPToFile()
{
	ofstream file;
	file.open("lastIP.txt");
	char lastUsedIPAddress[20];
	GetDlgItemTextA(m_hWnd, IDC_IP, lastUsedIPAddress, 20);
	file << lastUsedIPAddress;
	file.close();
}

void LiveScanClient::CreateBlankGrayImage(const int width, const int height) 
{
	if (m_pBlankGreyImage) //Cleanup the previous image
	{
		delete[] m_pBlankGreyImage;
		m_pBlankGreyImage = NULL;
	}

	int imageSize = width * height;
	RGB greyPixel;
	greyPixel.rgbBlue = 50;
	greyPixel.rgbGreen = 50;
	greyPixel.rgbRed = 50;

	static RGB* greyFrame = new RGB[imageSize];

	for (size_t i = 0; i < imageSize; i++)
	{
		greyFrame[i] = greyPixel;
	}

	m_pBlankGreyImage = greyFrame;
}
