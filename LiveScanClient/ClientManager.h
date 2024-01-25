#pragma once
#include "liveScanClient.h"

class ClientManager
{
public:

	ClientManager(Log* logger, bool virtualDevice);
	~ClientManager();
	
	void AddClient();
	void RemoveClient(int index);
	void ConnectClient(std::string adress, int index);
	void ConnectAllClients(bool connect, std::string adress);
	void SetActiveClient(int index);
	void SetPreviewMode(bool depth);

	float GetClientFPS(int index);
	std::string GetClientIP(int index);
	bool GetClientConnected(int index);
	bool GetAllClientsConnected();
	PreviewFrame GetClientColor(int index);
	PreviewFrame GetClientDepth(int index);
	StatusMessage GetClientStatusMessage(int index);
	DeviceStatus GetClientDeviceStatus(int index);

private:

	std::vector<LiveScanClient*> m_vClients;
	std::vector<std::thread> m_vClientThreads;

	Log* log;
	bool m_bVirtualDevice;
	
	//Current settings
	bool showDepth;
	int activeClient;

};

