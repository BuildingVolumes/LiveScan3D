#include "ClientManager.h"

ClientManager::ClientManager(Log* logger, bool virtualDevice)
{
	log = logger;
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

/// <summary>
/// Gets the amount of sensors connected to this PC that could be opened
/// </summary>
/// <returns></returns>
int ClientManager::GetAvailableSensors()
{
	return k4a_device_get_installed_count();
}

void ClientManager::AddClient()
{
	//Create a new client and start it's update function in a new thread
	LiveScanClient* client = new LiveScanClient();
	std::thread clientThread(&LiveScanClient::RunClient, client, log, m_bVirtualDevice);

	//Save the new client and its thread so we can access it later
	m_vClients.push_back(std::move(client));
	m_vClientThreads.push_back(std::move(clientThread));

	//Supply with current settings
	client->SetPreviewMode(showDepth);
	client->SetClientActive(true);
	activeClient = m_vClients.size() - 1;

}

void ClientManager::RemoveClient(int index)
{
	m_vClients[index]->CloseClient();
	m_vClientThreads[index].join();

	delete m_vClients[index];
	m_vClientThreads.erase(m_vClientThreads.begin() + index);
	m_vClients.erase(m_vClients.begin() + index);

	if (activeClient == index)
	{
		activeClient = m_vClients.size() - 1;
		m_vClients[activeClient]->SetClientActive(true);
	}
}

void ClientManager::ConnectClient(std::string adress, int index)
{
	if(GetClientConnected(index))
		m_vClients[index]->Disconnect();
	else
		m_vClients[index]->Connect(adress);
}

void ClientManager::ConnectAllClients(bool connect, std::string adress)
{
	for (int i = 0; i < m_vClients.size(); i++)
	{
		if (connect)
			m_vClients[i]->Connect(adress);
		else
			m_vClients[i]->Disconnect();
	}
}

void ClientManager::SetActiveClient(int index)
{
	activeClient = index;

	for (int i = 0; i < m_vClients.size(); i++)
	{
		if (i == index)
			m_vClients[i]->SetClientActive(true);
		else
			m_vClients[i]->SetClientActive(false);
	}
}

void ClientManager::SetPreviewMode(bool depth)
{
	showDepth = depth;

	for (int i = 0; i < m_vClients.size(); i++)
	{
		m_vClients[i]->SetPreviewMode(depth);
	}
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

bool ClientManager::GetAllClientsConnected()
{
	bool allConnected = true;

	for (int i = 0; i < m_vClients.size(); i++)
		if (!m_vClients[i]->GetConnectedTS())
			allConnected = false;

	return allConnected;
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
