#pragma once
#include "ClientManager.h"
#include <strsafe.h>
#include <shellapi.h> // 


// HOGUE
int g_winWidth = 800;
int g_winHeight = 540;
int g_winX = 0;
int g_winY = 0; 
int g_connectToServerImmediately = 0;

class UI
{
public:

	UI();
	~UI();

	static LRESULT CALLBACK MessageRouter(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
	LRESULT CALLBACK        DlgProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
	int                     Run(HINSTANCE hInstance, int nCmdShow, Log::LOGLEVEL loglevel, bool virtualDevice);

	void Initialize(Log::LOGLEVEL level, bool virtualDevice);

	bool m_bVirtualDevice;

private:

	bool SetStatusMessage(_In_z_ WCHAR* szMessage, DWORD nShowTimeMsec, bool bForce);
	void ManagePreviewWindowInitialization(int width, int height);
	void HandleConnectButton(LPSTR address);
	void HandleAddClientButton();
	void HandleRemoveClientButton();
	void ShowFPS();
	void ShowPreview();
	void ShowStatus();
	void CheckConnection();
	void Update();

	ClientManager* m_cClientManager;

	// Direct2D
	ImageRenderer* m_pD2DImageRenderer;
	ID2D1Factory* m_pD2DFactory;
	PreviewFrame m_pCurrentPreviewFrame;

	bool m_bShowDepth;
	bool m_bWaitForConnection;

	Log& log;
	HWND m_hWnd;
	INT64 m_nLastCounter;
	double m_fFreq;
	INT64 m_nNextStatusTime;
	float m_fAverageFPS;
	cv::Mat m_cvPreviewDisabled;
	cv::Mat* emptyDepthMat;

	int m_nUpdateFPS = 30; //How often should we update the UI visuals, limited to save some CPU 
	std::chrono::milliseconds m_tLastFrameTime;

};











