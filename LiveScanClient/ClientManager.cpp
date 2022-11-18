#include "ClientManager.h"

ClientManager::ClientManager(Log::LOGLEVEL level, bool virtualDevice)
{
	m_eLogLevel = level;
	m_bVirtualDevice = virtualDevice;
}

ClientManager::~ClientManager()
{
	for (size_t i = 0; i < m_vClientThreads.size(); i++)
	{
		m_vClients[i]->CloseClient();
		m_vClientThreads[i].join();
	}

	for (size_t i = 0; i < m_vClients.size(); i++)
	{
		delete m_vClients[i];
	}

	m_vClientThreads.clear();
	m_vClients.clear();
}

void ClientManager::AddClient()
{
	//Create a new client and start it's update function in a new thread
	LiveScanClient* client = new LiveScanClient();
	std::thread clientThread(&LiveScanClient::RunClient, client, m_eLogLevel, m_bVirtualDevice);

	//Save the new client and its thread so we can access it later
	m_vClients.push_back(std::move(client));
	m_vClientThreads.push_back(std::move(clientThread));
}

void ClientManager::RemoveClient(int index)
{
	m_vClients[index]->CloseClient();
	m_vClientThreads[index].join();

	delete m_vClients[index];
	m_vClientThreads.erase(m_vClientThreads.begin() + index);
	m_vClients.erase(m_vClients.begin() + index);

}

void ClientManager::ConnectClient(std::string adress, int index)
{
	m_vClients[index]->Connect(adress);
}


float ClientManager::GetClientFPS(int index)
{
	return m_vClients[index]->GetFPSTS();
}

std::string ClientManager::GetClientIP(int index)
{
	return std::string();
}

bool ClientManager::GetClientConnected(int index)
{
	return m_vClients[index]->GetConnectedTS();
}

PreviewFrame ClientManager::GetClientColor(int index)
{
	return m_vClients[index]->GetColorTS();
}

PreviewFrame ClientManager::GetClientDepth(int index)
{
	return m_vClients[index]->GetDepthTS();
}

StatusMessage ClientManager::GetClientStatusMessage(int index)
{
	return m_vClients[index]->GetStatusMessageTS();
}

DeviceStatus ClientManager::GetClientDeviceStatus(int index)
{
	return m_vClients[index]->GetDeviceStatusTS();
}
