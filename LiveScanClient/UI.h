#pragma once
#include "ClientManager.h"
#include <strsafe.h>
#include <shellapi.h>
#include <codecvt>


// HOGUE
int g_winWidth = 800;
int g_winHeight = 540;
int g_winX = 0;
int g_winY = 0; 
int g_connectToServerImmediately = 0;

struct ClientUI
{
	TCITEM uiTab;
	HWND tabCloseButton;
	std::wstring tabName;
	DeviceStatus deviceStatus;
};

class UI
{
public:

	UI();
	~UI();

	static LRESULT CALLBACK MessageRouter(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
	LRESULT CALLBACK        DlgProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
	int                     Run(HINSTANCE hInstance, int nCmdShow, Log::LOGLEVEL loglevel, bool virtualDevice);

	bool Initialize(Log::LOGLEVEL level, bool virtualDevice, HWND hWnd);

	bool m_bVirtualDevice;

private:

	bool SetStatusMessage(_In_z_ WCHAR* szMessage, DWORD nShowTimeMsec, bool bForce);
	void ManagePreviewWindowInitialization(int width, int height);
	void HandleConnectButton(LPSTR address);
	void HandleConnectAllButton(LPSTR adress);
	void HandleTabSelection(int index);
	void AddClient();
	void RemoveClient(int index);
	void ShowFPS();
	void ShowPreview();
	void ShowStatus();
	void CheckConnection();
	void Update(bool force);
	void UpdateUILayout();
	void UpdateDeviceStatus();

	ClientManager* m_cClientManager;
	int processID = 0;

	// Direct2D
	ImageRenderer* m_pD2DImageRenderer;
	ID2D1Factory* m_pD2DFactory;
	PreviewFrame m_pCurrentPreviewFrame;

	cv::Mat m_pImgStarting;
	cv::Mat m_pImgCrash;
	cv::Mat m_pImgCameraError;
	cv::Mat m_pImgPreviewDeactivated;

	cv::Mat m_pImgStartingResized;
	cv::Mat m_pImgCrashResized;
	cv::Mat m_pImgCameraErrorResized;
	cv::Mat m_pImgPreviewDeactivatedResized;

	std::vector<ClientUI> tabs;

	bool m_bShowDepth;

	Log log;
	LogBuffer logBuffer;
	HWND m_hWnd;
	INT64 m_nLastCounter;
	double m_fFreq;
	INT64 m_nNextStatusTime;
	float m_fAverageFPS;
	cv::Mat* emptyDepthMat;

	int m_nTabSelected;
	TCITEM m_uiPlusTab;
	std::wstring m_sCloseButtonSpace = L"        \0";

	int m_nUpdateFPS = 30; //How often should we update the UI visuals, limited to save some CPU 
	std::chrono::milliseconds m_tLastFrameTime;
};















