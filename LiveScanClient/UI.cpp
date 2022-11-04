#include "UI.h"

UI::UI() :
	m_hWnd(NULL),
	m_nLastCounter(0),
	m_fFreq(0),
	m_nNextStatusTime(0LL),
	m_pD2DFactory(NULL),
	m_pD2DImageRenderer(NULL),
	m_fAverageFPS(30.0),
	m_bVirtualDevice(0),
	m_bShowDepth(0),
	log(log.Get())

{
	LARGE_INTEGER qpf = { 0 };
	if (QueryPerformanceFrequency(&qpf))
	{
		m_fFreq = double(qpf.QuadPart);
	}
}

UI::~UI()
{
	// clean up Direct2D renderer
	if (m_pD2DImageRenderer)
	{
		delete m_pD2DImageRenderer;
		m_pD2DImageRenderer = NULL;
	}

	// clean up Direct2D
	SafeRelease(m_pD2DFactory);
}

void UI::Initialize(Log::LOGLEVEL level, bool virtualDevice)
{
	log.StartLog("Client", Log::LOGLEVEL_INFO, false);

	m_cClientManager = new ClientManager(level, virtualDevice);
	m_cClientManager->AddClient();

	// Init Direct2D
	D2D1CreateFactory(D2D1_FACTORY_TYPE_SINGLE_THREADED, &m_pD2DFactory);
}

void UI::Update()
{
	std::chrono::milliseconds nowTime = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch());
	std::chrono::milliseconds msSinceLastUpdate = nowTime - m_tLastFrameTime;

	if (msSinceLastUpdate.count() > 1000 / m_nUpdateFPS)
	{
		ShowFPS();
		ShowPreview();
		ShowStatus();
		CheckConnection();

		m_tLastFrameTime = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch());
	}	
}

void UI::ShowPreview()
{
	if (m_pCurrentPreviewFrame.picture != NULL)
		delete[] m_pCurrentPreviewFrame.picture;

	if(!m_bShowDepth)
		m_pCurrentPreviewFrame = m_cClientManager->GetClientColor(m_cClientManager->m_nActiveClientIndex);
	else
		m_pCurrentPreviewFrame = m_cClientManager->GetClientDepth(m_cClientManager->m_nActiveClientIndex);

	if (m_pCurrentPreviewFrame.picture != NULL && m_pCurrentPreviewFrame.height * m_pCurrentPreviewFrame.width != 0)
	{
		ManagePreviewWindowInitialization(m_pCurrentPreviewFrame.width, m_pCurrentPreviewFrame.height);
		m_pD2DImageRenderer->Draw(reinterpret_cast<uint8_t*>(m_pCurrentPreviewFrame.picture), m_pCurrentPreviewFrame.width * m_pCurrentPreviewFrame.height * sizeof(RGBA));
		//m_pD2DImageRenderer->Draw(reinterpret_cast<BYTE*>(m_cvPreviewDisabled.data), long(m_cvPreviewDisabled.total() * m_cvPreviewDisabled.elemSize()));
	}
}

void UI::ShowFPS()
{
	m_fAverageFPS = m_cClientManager->GetClientFPS(0);
	WCHAR szStatusMessage[64];
	StringCchPrintf(szStatusMessage, _countof(szStatusMessage), L" FPS = %0.0f", m_fAverageFPS);
	SetStatusMessage(szStatusMessage, 1000, false);
}

void UI::ShowStatus()
{
	StatusMessage status = m_cClientManager->GetClientStatusMessage(m_cClientManager->m_nActiveClientIndex);

	if (status.time > 0)
	{
		SetStatusMessage(reinterpret_cast<WCHAR*>(status.message.data()), status.time, status.priority);
	}	
}

void UI::CheckConnection()
{
	if(m_cClientManager->GetClientConnected(m_cClientManager->m_nActiveClientIndex))
		SetDlgItemTextA(m_hWnd, IDC_BUTTON_CONNECT, "Disconnect");

	else
		SetDlgItemTextA(m_hWnd, IDC_BUTTON_CONNECT, "Connect");
}


/// <summary>
/// Create and initialize a new Direct2D image renderer (take a look at ImageRenderer.h).
/// We'll use this to draw the data we receive from the Kinect to the screen
/// </summary>
/// <returns></returns>
void UI::ManagePreviewWindowInitialization(int width, int height)
{
	bool initializationNeeded = false;

	if (m_pD2DImageRenderer != NULL)
	{
		//If our image height/width has changed, we need to reinitialize our D2D Renderer
		if (width * height != m_pD2DImageRenderer->GetRenderHeight() * m_pD2DImageRenderer->GetRenderWidth())
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

		HRESULT hr;
		m_pD2DImageRenderer = new ImageRenderer();
		hr = m_pD2DImageRenderer->Initialize(GetDlgItem(m_hWnd, IDC_VIDEOVIEW), m_pD2DFactory, width, height, width * sizeof(RGBA));
		if (FAILED(hr))
		{
			log.LogFatal("Failed to initialize the Direct2D Draw device");
			SetStatusMessage(L"Failed to initialize the Direct2D draw device.", 10000, true);
		}

		//Initialize Preview Resources
		try
		{
			m_cvPreviewDisabled = cv::imread("resources/preview_disabled.png");
			cv::resize(m_cvPreviewDisabled, m_cvPreviewDisabled, cv::Size(width, height), cv::INTER_AREA);
			cv::cvtColor(m_cvPreviewDisabled, m_cvPreviewDisabled, cv::COLOR_BGR2BGRA);
		}

		catch (cv::Exception e)
		{
			log.LogFatal(e.what());
			SetStatusMessage(L"Failed to load resources", 10000, true);
			return;
		}		
	}
}


void UI::HandleConnectButton(LPSTR address)
{
	m_cClientManager->ConnectClient(address, m_cClientManager->m_nActiveClientIndex);
	m_bWaitForConnection = true;
}

void UI::HandleAddClientButton()
{
	m_cClientManager->AddClient();
}

void UI::HandleRemoveClientButton()
{
	m_cClientManager->RemoveClient(0);
}

bool UI::SetStatusMessage(_In_z_ WCHAR* szMessage, DWORD nShowTimeMsec, bool bForce)
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



//++++Windows API Entry+++++

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

	Log::LOGLEVEL loglevel = Log::LOGLEVEL_INFO;
	int virtualClient = 0;

	if (argCount > 1)
	{
		if (wcscmp(LPWSTR(L"-debug"), (szArgList[1])) == 0)
			loglevel = Log::LOGLEVEL_DEBUG;

		if (wcscmp(LPWSTR(L"-debugcapture"), (szArgList[1])) == 0 || wcscmp(LPWSTR(L"-debugCapture"), (szArgList[1])) == 0)
			loglevel = Log::LOGLEVEL_DEBUG_CAPTURE;

		if (wcscmp(LPWSTR(L"-debugall"), (szArgList[1])) == 0 || wcscmp(LPWSTR(L"-debugAll"), (szArgList[1])) == 0)
			loglevel = Log::LOGLEVEL_ALL;
	}

	if (argCount > 2)
	{
		//If this client should be instantiated as virtual client for testing purposes (0 = false, 1 = true)
	}

	if (argCount >= 7)
	{
		// assume window width, height, x, y
		g_winWidth = _wtoi(szArgList[3]);
		g_winHeight = _wtoi(szArgList[4]);
		g_winX = _wtoi(szArgList[5]);
		g_winY = _wtoi(szArgList[6]);

		if (argCount >= 8)
			g_connectToServerImmediately = _wtoi(szArgList[7]);
	}

	UI application;

	bool virtualDevice = false;
	if (virtualClient == 1)
		virtualDevice = true;

	application.Run(hInstance, nShowCmd, loglevel, virtualDevice);
}

int UI::Run(HINSTANCE hInstance, int nCmdShow, Log::LOGLEVEL loglevel, bool virtualDevice)
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


	//For build convinience, we can build a seperate .exe with virtual device always enabled
#if _VIRTUAL_DEVICE
	virtualDevice = true;
#endif

	if (!RegisterClassW(&wc))
	{
		return 0;
	}

	// Create main application window
	HWND hWndApp = CreateDialogParamW(
		NULL,
		MAKEINTRESOURCE(IDD_APP),
		NULL,
		(DLGPROC)UI::MessageRouter,
		reinterpret_cast<LPARAM>(this));

	// Show window
	ShowWindow(hWndApp, nCmdShow);

	// HOGUE
	::SetWindowPos(m_hWnd, HWND_TOP, g_winX, g_winY, g_winWidth, g_winHeight, NULL);

	Initialize(loglevel, virtualDevice);

	// Main message loop
	while (WM_QUIT != msg.message)
	{
		Update();

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


	return static_cast<int>(msg.wParam);
}

LRESULT CALLBACK UI::MessageRouter(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	UI* pThis = NULL;

	if (WM_INITDIALOG == uMsg)
	{
		pThis = reinterpret_cast<UI*>(lParam);
		SetWindowLongPtr(hWnd, GWLP_USERDATA, reinterpret_cast<LONG_PTR>(pThis));
	}
	else
	{
		pThis = reinterpret_cast<UI*>(::GetWindowLongPtr(hWnd, GWLP_USERDATA));
	}

	if (pThis)
	{
		return pThis->DlgProc(hWnd, uMsg, wParam, lParam);
	}

	return 0;
}

LRESULT CALLBACK UI::DlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	UNREFERENCED_PARAMETER(wParam);
	UNREFERENCED_PARAMETER(lParam);

	switch (message)
	{
	case WM_INITDIALOG:
	{
		// Bind application window handle
		m_hWnd = hWnd;
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
		DestroyWindow(hWnd);
		break;
	case WM_DESTROY:
		
		//TODO: Destroy all resources here
		
		delete m_cClientManager;

		// Quit the main message pump
		PostQuitMessage(0);
		break;

		// Handle button press
	case WM_COMMAND:
		if (IDC_BUTTON_CONNECT == LOWORD(wParam) && BN_CLICKED == HIWORD(wParam))
		{
			char address[20];
			GetDlgItemTextA(m_hWnd, IDC_IP, address, 20);
			HandleConnectButton(address);
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

