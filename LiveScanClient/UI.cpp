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
	m_nTabSelected(0)

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

bool UI::Initialize(Log::LOGLEVEL level, bool virtualDevice, HWND hWnd)
{
	//Create/Check system-wide mutexes to see if another Livescan Instance
	//is already running and save our Instance ID. This is used so saved files
	//(log files for example) don't overwrite each other	
	bool searchForID = true;
	while (searchForID)
	{
		std::wstring mutexNameWStr = L"LiveScanClientProcess_" + to_wstring(processID);
		LPWSTR mutexName = const_cast<wchar_t*>(mutexNameWStr.c_str());
		HANDLE mutex = CreateMutex(NULL, FALSE, mutexName);

		switch (GetLastError())
		{
		case ERROR_SUCCESS:
			searchForID = false;
			// Process was not running already, we got our ID!
			break;
		case ERROR_ALREADY_EXISTS:
			// Process is running already, we increment the ID
			CloseHandle(mutex);
			processID++;
			break;
		default:
			// Error occured, not sure whether process is running already.
			break;
		}
	}

	//If needed, change loglevel here for the whole application
	if (!log.StartLog(processID, Log::LOGLEVEL_INFO))
	{
		MessageBox(NULL, L"Failed to create log file, please restart application!", L"Fatal Error", MB_ICONERROR | MB_OK);
		return false;
	}

	log.RegisterBuffer(&logBuffer);
	logBuffer.ChangeSerial("LiveScan Client");

	//Setup directories
	if (!std::filesystem::exists("temp/"))
	{
		if (!std::filesystem::create_directory("temp/"))
		{
			logBuffer.LogFatal("Failed to setup directories! Please restart application!");
			MessageBox(NULL, L"Failed to setup directories! Please restart application!", L"Fatal Error", MB_ICONERROR | MB_OK);
			return false;

		}
	}

	// Init Direct2D
	if (D2D1CreateFactory(D2D1_FACTORY_TYPE_SINGLE_THREADED, &m_pD2DFactory) != S_OK)
	{
		logBuffer.LogFatal("Failed to intialized D2DDevice");
		MessageBox(NULL, L"Error creating D2D device! Please restart application", L"Fatal Error", MB_ICONERROR | MB_OK);
		return false;
	}

	try
	{
		m_pImgCameraError = cv::imread("resources/images/Status_CameraError.png");
		m_pImgStarting = cv::imread("resources/images/Status_CameraStarting.png");
		m_pImgCrash = cv::imread("resources/images/Status_Crash.png");
		m_pImgPreviewDeactivated = cv::imread("resources/images/Status_PreviewDeactivated.png");
	}

	catch (cv::Exception e)
	{
		logBuffer.LogFatal(e.what());
		MessageBox(NULL, L"Failed to load image resources! Please restart application", L"Fatal Error", MB_ICONERROR | MB_OK);
		return false;
	}
	

	m_cClientManager = new ClientManager(&log, virtualDevice);
	

	//Add our first client tab and a tab which acts as a "add tab" button
	LPWSTR addTabText = L"  +";	
	m_uiPlusTab.pszText = addTabText;
	m_uiPlusTab.cchTextMax = sizeof(addTabText);
	m_uiPlusTab.iImage = -1;
	m_uiPlusTab.mask = TCIF_TEXT;
	TabCtrl_InsertItem(GetDlgItem(hWnd, IDC_TAB), 0, &m_uiPlusTab);

	AddClient();	

	return true;
}

void UI::Update(bool force)
{
	std::chrono::milliseconds nowTime = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::steady_clock::now().time_since_epoch());
	std::chrono::milliseconds msSinceLastUpdate = nowTime - m_tLastFrameTime;

	if (msSinceLastUpdate.count() > 1000 / m_nUpdateFPS || force)
	{
		ShowFPS();
		ShowPreview();
		ShowStatus();
		CheckConnection();
		UpdateDeviceStatus();
		log.PrintAllMessages();

		m_tLastFrameTime = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::steady_clock::now().time_since_epoch());
	}	
}

void UI::ShowPreview()
{
	if (m_pCurrentPreviewFrame.picture != NULL)
		delete[] m_pCurrentPreviewFrame.picture;

	if(!m_bShowDepth)
		m_pCurrentPreviewFrame = m_cClientManager->GetClientColor(m_nTabSelected);
	else
		m_pCurrentPreviewFrame = m_cClientManager->GetClientDepth(m_nTabSelected);

	//The camera my not have been initialized, so we use default viewport values instead
	if (m_pCurrentPreviewFrame.width == 0)
		m_pCurrentPreviewFrame.width = 1280;
	if (m_pCurrentPreviewFrame.height == 0)
		m_pCurrentPreviewFrame.height = 720;

	ManagePreviewWindowInitialization(m_pCurrentPreviewFrame.width, m_pCurrentPreviewFrame.height);

	if (tabs.size() > 0)
	{
		if (!m_pCurrentPreviewFrame.previewDisabled)
		{
			switch (tabs[m_nTabSelected].deviceStatus.status)
			{
			case STATUS_STARTING:
				m_pD2DImageRenderer->Draw(reinterpret_cast<BYTE*>(m_pImgStartingResized.data), long(m_pImgStartingResized.total() * m_pImgStartingResized.elemSize()));
				break;
			case STATUS_CAMERA_ERROR:
				m_pD2DImageRenderer->Draw(reinterpret_cast<BYTE*>(m_pImgCameraErrorResized.data), long(m_pImgCameraErrorResized.total() * m_pImgCameraErrorResized.elemSize()));
				break;
			case STATUS_RUNNING:
				if (m_pCurrentPreviewFrame.picture != NULL)
				{
					m_pD2DImageRenderer->Draw(reinterpret_cast<uint8_t*>(m_pCurrentPreviewFrame.picture), m_pCurrentPreviewFrame.width * m_pCurrentPreviewFrame.height * sizeof(RGBA));
				}
				break;
			case STATUS_CRASH:
				m_pD2DImageRenderer->Draw(reinterpret_cast<BYTE*>(m_pImgCrashResized.data), long(m_pImgCrashResized.total() * m_pImgCrashResized.elemSize()));
				break;
			case STATUS_TERMINATED_NORMALLY:
				m_pD2DImageRenderer->Draw(reinterpret_cast<BYTE*>(m_pImgCrashResized.data), long(m_pImgCrashResized.total() * m_pImgCrashResized.elemSize())); //TODO: What happens here?
				break;
			}
		}

		else
		{
			m_pD2DImageRenderer->Draw(reinterpret_cast<BYTE*>(m_pImgPreviewDeactivated.data), long(m_pImgPreviewDeactivated.total() * m_pImgPreviewDeactivated.elemSize()));
		}

		
	}
	
}

void UI::ShowFPS()
{
	m_fAverageFPS = m_cClientManager->GetClientFPS(m_nTabSelected);
	WCHAR szStatusMessage[64];
	StringCchPrintf(szStatusMessage, _countof(szStatusMessage), L" FPS = %0.0f", m_fAverageFPS);
	SetStatusMessage(szStatusMessage, 1000, false);
}

void UI::ShowStatus()
{
	StatusMessage status = m_cClientManager->GetClientStatusMessage(m_nTabSelected);

	if (status.time > 0)
	{
		SetStatusMessage(reinterpret_cast<WCHAR*>(status.message.data()), status.time, status.priority);
	}	
}

void UI::CheckConnection()
{
	if(m_cClientManager->GetClientConnected(m_nTabSelected))
		SetDlgItemTextA(m_hWnd, IDC_BUTTON_CONNECT, "Disconnect");

	else
		SetDlgItemTextA(m_hWnd, IDC_BUTTON_CONNECT, "Connect");
}

void UI::UpdateDeviceStatus()
{
	for (size_t i = 0; i < tabs.size(); i++)
	{
		DeviceStatus newStatus = m_cClientManager->GetClientDeviceStatus(i);

		if (tabs[i].deviceStatus.name != newStatus.name)
		{			
			std::wstring newName = std::wstring_convert<std::codecvt_utf8<wchar_t>>().from_bytes(newStatus.name);
			newName += L"        \0";
			tabs[i].tabName = newName;
			tabs[i].uiTab.pszText = const_cast<wchar_t*>(tabs[i].tabName.c_str());
			tabs[i].uiTab.cchTextMax = tabs[i].tabName.size();
			TabCtrl_SetItem(GetDlgItem(m_hWnd, IDC_TAB), i, &tabs[i].uiTab);

			UpdateUILayout();
		}

		tabs[i].deviceStatus = newStatus;
	}
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
		logBuffer.LogInfo("Initializing Preview Window");

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
			logBuffer.LogFatal("Failed to initialize the Direct2D Draw device");
			MessageBox(NULL, L"Failed to initialize the Direct2D Draw device", L"Fatal Error", MB_ICONERROR | MB_OK);

		}

		//Initialize Preview Resources
		try
		{
			m_pImgCameraErrorResized = cv::Mat(height, width, CV_8UC4);
			m_pImgStartingResized = cv::Mat(height, width, CV_8UC4);
			m_pImgCrashResized = cv::Mat(height, width, CV_8UC4);
			m_pImgPreviewDeactivatedResized = cv::Mat(height, width, CV_8UC4);

			cv::resize(m_pImgCameraError, m_pImgCameraErrorResized, cv::Size(width, height), cv::INTER_AREA);
			cv::cvtColor(m_pImgCameraErrorResized, m_pImgCameraErrorResized, cv::COLOR_BGR2BGRA);

			cv::resize(m_pImgStarting, m_pImgStartingResized, cv::Size(width, height), cv::INTER_AREA);
			cv::cvtColor(m_pImgStartingResized, m_pImgStartingResized, cv::COLOR_BGR2BGRA);

			cv::resize(m_pImgCrash, m_pImgCrashResized, cv::Size(width, height), cv::INTER_AREA);
			cv::cvtColor(m_pImgCrashResized, m_pImgCrashResized, cv::COLOR_BGR2BGRA);

			cv::resize(m_pImgPreviewDeactivated, m_pImgPreviewDeactivatedResized, cv::Size(width, height), cv::INTER_AREA);
			cv::cvtColor(m_pImgPreviewDeactivatedResized, m_pImgPreviewDeactivatedResized, cv::COLOR_BGR2BGRA);
		}

		catch (cv::Exception e)
		{
			logBuffer.LogFatal(e.what());
			MessageBox(NULL, L"Failed to rescale resources, please restart client!", L"Fatal Error", MB_ICONERROR | MB_OK);
			return;
		}		
	}
}

void UI::HandleConnectButton(LPSTR address)
{
	m_cClientManager->ConnectClient(address, m_nTabSelected);
	m_bWaitForConnection = true;
}

void UI::HandleTabSelection(int index)
{
	//The last tab is always the add tab button
	if (index == tabs.size())
		AddClient();

	m_nTabSelected = index;
	m_cClientManager->SetActiveClient(index);


}

void UI::AddClient()
{	
	//Create a new Livescan Instance
	m_cClientManager->AddClient();
	ClientUI newClientUI;
	tabs.push_back(newClientUI);

	ClientUI* tab = &tabs[tabs.size() - 1];

	//Create Name for client
	std::wstring name = L"Loading Device..."; //Default name which should be updated to the Kinect Serial Number or nickname after it has initialized];
	name += m_sCloseButtonSpace; //Add 4 spaces to the name to make place for the Close Button
	tab->tabName = name;

	//Create Tab on UI 	
	tab->uiTab.pszText = const_cast<wchar_t*>(tab->tabName.c_str()); //We need to gurantee that the wstring exists longer than the tab
	tab->uiTab.cchTextMax = tab->tabName.size();
	tab->uiTab.iImage = -1;
	tab->uiTab.mask = TCIF_TEXT;
	TabCtrl_InsertItem(GetDlgItem(m_hWnd, IDC_TAB), tabs.size() - 1, &tab->uiTab);

	//Create Close Button for Tab. Position will be handled by the layout function, as it is dynamic
	HWND hwndButton = CreateWindow(L"BUTTON", L"X", WS_VISIBLE | WS_CHILD | BS_DEFPUSHBUTTON, 0, 0, 0, 0, m_hWnd, NULL, (HINSTANCE)GetWindowLongPtr(m_hWnd, GWLP_HINSTANCE),NULL);
	tab->tabCloseButton = hwndButton;

	UpdateUILayout(); //Update the Layout, so that the new elements will be positioned correctly

	//"Open" the newly created tab
	m_nTabSelected = tabs.size() - 1;
	TabCtrl_SetCurSel(GetDlgItem(m_hWnd, IDC_TAB), m_nTabSelected);

	//Only allow max of 8 clients per LiveScanClientWindow
	//This is a UI restrictions, as we can't fit more tabs into the window
	if (tabs.size() == 8)
	{
		TabCtrl_DeleteItem(GetDlgItem(m_hWnd, IDC_TAB), 8);
	}
}

void UI::RemoveClient(int index)
{
	//Save the currently selected tab index
	m_nTabSelected = TabCtrl_GetCurSel(GetDlgItem(m_hWnd, IDC_TAB));

	TabCtrl_DeleteItem(GetDlgItem(m_hWnd, IDC_TAB), index);
	DestroyWindow(tabs[index].tabCloseButton);
	tabs.erase(tabs.begin() + index);

	if (tabs.size() == 7)
	{
		TabCtrl_InsertItem(GetDlgItem(m_hWnd, IDC_TAB), tabs.size(), &m_uiPlusTab);
	}

	//Now that the tabs list has changed through the removal, we need to figure out
	// where the previously selected tab is now located 

	//In case the deleted tab was our current tab, or was to the left of it, we shuffle one tab to the left
	if (m_nTabSelected == index || m_nTabSelected > index)
	{
		m_nTabSelected = m_nTabSelected - 1;
		if (m_nTabSelected < 0)
			m_nTabSelected = 0;
	}

	TabCtrl_SetCurSel(GetDlgItem(m_hWnd, IDC_TAB), m_nTabSelected);

	UpdateUILayout();

	m_cClientManager->RemoveClient(index);
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


//Win32 API doesn't handle rescaling automatically, so we have to position
//and resize every UI element ourselves here
void UI::UpdateUILayout()
{
	RECT r;
	::GetWindowRect(m_hWnd, &r);

	int minWindowWidth = 1024;

	int windowWidth = abs(r.right - r.left);
	int windowHeight = abs(r.bottom - r.top);

	if (windowWidth < minWindowWidth)
		windowWidth = minWindowWidth;

	int buttonWidth = 100;
	int buttonHeight = 20;

	int tabheight = 24;
	int tabScrollReserved = 20;

	int controlPanelHeight = 40;
	int statusBarHeight = 20;

	int paddingVertical = 5;
	int paddingHorizontal = 7;

	float aspectRatio = 1920 / 1080; // 16:9
	int startB = 80;
	int fixedHeight = 3 * buttonHeight + startB;

	int heightFixedRatio = (windowWidth / 1.77);
	int totalWindowHeight = tabheight + heightFixedRatio + controlPanelHeight + statusBarHeight + 40;


	::SetWindowPos(m_hWnd, HWND_TOP, 0, 0, windowWidth, totalWindowHeight, SWP_NOMOVE);

	::SetWindowPos(GetDlgItem(m_hWnd, IDC_TAB), HWND_TOP, 0, 0, windowWidth - tabScrollReserved, tabheight, NULL);

	int allTabSize = 0;

	for (size_t i = 0; i < tabs.size(); i++)
	{
		HDC hdc = GetDC(m_hWnd);
		HFONT current_font = (HFONT)SendMessage(m_hWnd, WM_GETFONT, 0, 0);
		HGDIOBJ old_font = SelectObject(hdc, current_font);
		SIZE sizeText;
		SIZE sizeButtonSpace;
		GetTextExtentPoint32(hdc, tabs[i].uiTab.pszText, tabs[i].uiTab.cchTextMax, &sizeText);
		GetTextExtentPoint32(hdc, m_sCloseButtonSpace.c_str(), m_sCloseButtonSpace.size(), &sizeButtonSpace);
		SelectObject(hdc, old_font);
		ReleaseDC(m_hWnd, hdc);

		//Get size in pixels of tab name, so we know where to place the close button
		int tabPadding = 12;
		int tabNameSize = sizeText.cx - sizeButtonSpace.cx + tabPadding;
		::SetWindowPos(tabs[i].tabCloseButton, HWND_TOP, allTabSize + tabNameSize, 2, 20, 20, NULL);
		allTabSize += sizeText.cx + tabPadding;

		//Hide the close button for the first tab
		if (i == 0 && tabs.size() == 1)
			ShowWindow(tabs[i].tabCloseButton, SW_HIDE);

		else
			ShowWindow(tabs[i].tabCloseButton, SW_SHOW);
	}	

	::SetWindowPos(GetDlgItem(m_hWnd, IDC_VIDEOVIEW), HWND_TOP, 0, tabheight, windowWidth, heightFixedRatio, NULL);

	//Centered height of all buttons in control panel
	int buttonBarPosY = tabheight + heightFixedRatio + controlPanelHeight / 2 - buttonHeight / 2;

	::SetWindowPos(GetDlgItem(m_hWnd, IDC_STATIC), HWND_TOP, paddingHorizontal, buttonBarPosY + 2, buttonWidth, buttonHeight, NULL);
	::SetWindowPos(GetDlgItem(m_hWnd, IDC_IP), HWND_TOP, paddingHorizontal + buttonWidth, buttonBarPosY, buttonWidth, buttonHeight, NULL);
	::SetWindowPos(GetDlgItem(m_hWnd, IDC_BUTTON_CONNECT), HWND_TOP, paddingHorizontal * 3 + buttonWidth * 2, buttonBarPosY, buttonWidth, buttonHeight, NULL);

	::SetWindowPos(GetDlgItem(m_hWnd, IDC_BUTTON_SWITCH), HWND_TOP, windowWidth - paddingHorizontal * 3 - buttonWidth, buttonBarPosY, buttonWidth, buttonHeight, NULL);

	::SetWindowPos(GetDlgItem(m_hWnd, IDC_STATUS), HWND_TOP, 0, totalWindowHeight - statusBarHeight * 3, windowWidth, statusBarHeight, NULL);



}

//++++Windows 32 API Entry+++++

//This section here handles the window creation, communication and destruction

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
	SetWindowPos(m_hWnd, HWND_TOP, g_winX, g_winY, g_winWidth, g_winHeight, NULL);

	if (!Initialize(loglevel, virtualDevice, m_hWnd))
		return -1;

	// Main message loop
	while (WM_QUIT != msg.message)
	{
			Update(false);

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
		break;
	}
	case WM_SIZING: {// HOGUE
			/*RECT r;
			::GetWindowRect(m_hWnd, &r);
			int w = abs(r.right - r.left);
			int h = abs(r.bottom - r.top);
			::SetWindowPos(m_hWnd, HWND_TOP, 0, 0, w, (w / 1.77) + 200, NULL);*/
	}
	case WM_SIZE: {
		UpdateUILayout();
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

		//Static buttons
		if (BN_CLICKED == HIWORD(wParam))
		{
			if (IDC_BUTTON_CONNECT == LOWORD(wParam))
			{
				char address[20];
				GetDlgItemTextA(m_hWnd, IDC_IP, address, 20);
				HandleConnectButton(address);
			}

			if (IDC_BUTTON_SWITCH == LOWORD(wParam))
			{
				m_bShowDepth = !m_bShowDepth;
				m_cClientManager->SetPreviewMode(m_bShowDepth);

				if (m_bShowDepth)
				{
					SetDlgItemTextA(m_hWnd, IDC_BUTTON_SWITCH, "Show color");
				}
				else
				{
					SetDlgItemTextA(m_hWnd, IDC_BUTTON_SWITCH, "Show depth");
				}
			}
		}

		//Dynamically created buttons
		for (size_t i = 0; i < tabs.size(); i++)
		{
			if (HWND(lParam) == tabs[i].tabCloseButton)
			{
				RemoveClient(i);
			}
		}

		

		break;

	case WM_NOTIFY:
		switch (((LPNMHDR)lParam)->code)
		{
		case TCN_SELCHANGE:
			if (((LPNMHDR)lParam)->idFrom == IDC_TAB)
			{
				int selectedTab = TabCtrl_GetCurSel(GetDlgItem(hWnd, IDC_TAB));
				HandleTabSelection(selectedTab);
				return TRUE;
			}
			break;
		}
	}

	return FALSE;
}



