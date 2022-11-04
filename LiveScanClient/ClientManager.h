#pragma once
#include "liveScanClient.h"

class ClientManager
{
public:

	ClientManager(Log::LOGLEVEL level, bool virtualDevice);
	~ClientManager();
	
	void AddClient();
	void RemoveClient(int index);
	void SelectActiveClient(int index);
	void ConnectClient(std::string adress, int index);

	float GetClientFPS(int index);
	std::string GetClientIP(int index);
	bool GetClientConnected(int index);
	PreviewFrame GetClientColor(int index);
	PreviewFrame GetClientDepth(int index);
	StatusMessage GetClientStatusMessage(int index);

	int m_nActiveClientIndex;

private:

	std::vector<LiveScanClient*> m_vClients;
	std::vector<std::thread> m_vClientThreads;

	Log::LOGLEVEL m_eLogLevel;
	bool m_bVirtualDevice;

};

