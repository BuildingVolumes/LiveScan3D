#include "Log.h"


int Log::StartLog(int clientInstance, LOGLEVEL level, bool openConsole) 
{
	CloseLogFile();

	clientNumber = clientInstance;

	std::string filename = "Log_Client_" + std::to_string(clientNumber) + ".txt";
	logfile = new std::ofstream;
	logfile->open(filename, std::ofstream::trunc);
	logLevel = level;
	writeToConsole = openConsole;

	if (logfile->fail())
	{
		return false;
	}


	if (writeToConsole) {
		AllocConsole();
		freopen("CONOUT$", "w", stdout);
		freopen("CONOUT$", "w", stderr);
	}

	//Main thread buffer
	LogBuffer mainBuffer;
	logBuffers.push_back(mainBuffer);

	LogInfo(0, "Start of Logging");

	return 0;
}

/// <summary>
/// The log is being accessed by multiple Threads. To keep the performance high and avoid to many mutex locks, each thread will be assinged its
/// own message buffer. The buffer is then periodically read and flushed
/// </summary>
/// <param name="message"></param>
int Log::RegisterLog()
{
	std::lock_guard<std::mutex> regMutex(registerMutex);

	LogBuffer newBuffer;
	logBuffers.push_back(newBuffer);
	logBuffers[logBuffers.size() - 1].messages.push_back("[Client: " + std::to_string(logBuffers.size()) + "] " + "Starting Log");
	return logBuffers.size();
}

void Log::ChangeName(int id, std::string name)
{
	logBuffers[id].mutex.lock();
	logBuffers[id].name = name;
	logBuffers[id].mutex.unlock();
}


void Log::LogTrace(int id, std::string message)
{
	logBuffers[id].mutex.lock();

	if (logLevel >= LOGLEVEL_ALL)
		AddLogEntry(id, message, "[TRACE]");

	logBuffers[id].mutex.unlock();
}

void Log::LogCaptureDebug(int id, std::string message)
{
	logBuffers[id].mutex.lock();

	if (logLevel >= LOGLEVEL_DEBUG_CAPTURE)
		AddLogEntry(id, message, "[CAPTURE DEBUG]");

	logBuffers[id].mutex.unlock();
}

void Log::LogDebug(int id, std::string message)
{
	logBuffers[id].mutex.lock();

	if (logLevel >= LOGLEVEL_DEBUG)
		AddLogEntry(id, message, "[DEBUG]");
	
	logBuffers[id].mutex.unlock();

}

void Log::LogInfo(int id, std::string message)
{
	logBuffers[id].mutex.lock();

	if (logLevel >= LOGLEVEL_INFO)
		AddLogEntry(id, message, "[INFO]");

	logBuffers[id].mutex.unlock();
}

void Log::LogWarning(int id, std::string message)
{
	logBuffers[id].mutex.lock();

	if (logLevel >= LOGLEVEL_ERRORS)
		AddLogEntry(id, message, "[WARNING]");

	logBuffers[id].mutex.unlock();
}

void Log::LogError(int id, std::string message)
{
	logBuffers[id].mutex.lock();

	if (logLevel >= LOGLEVEL_ERRORS)
		AddLogEntry(id, message, "[ERROR]");

	logBuffers[id].mutex.unlock();
}

void Log::LogFatal(int id, std::string message)
{
	logBuffers[id].mutex.lock();

	if (logLevel >= LOGLEVEL_ERRORS)
		AddLogEntry(id, message, "[FATAL]");

	logBuffers[id].mutex.unlock();
}

void Log::AddLogEntry(int id, std::string message, std::string loglevel)
{
	time_t t = time(0);
	struct tm* now = localtime(&t);

	logBuffers[id].mutex.lock();
	std::string newMessage = "[Client: " + logBuffers[id].name + "]";
	logBuffers[id].mutex.unlock();

	newMessage += "[" + std::to_string(now->tm_hour) + ":" + std::to_string(now->tm_min) + ":" + std::to_string(now->tm_sec) + "] ";
	newMessage += loglevel + ": " + message + "\n";

	logBuffers[id].mutex.lock();
	logBuffers[id].messages.push_back(newMessage);
	logBuffers[id].mutex.unlock();
}

void Log::PullMessages()
{
	std::string messages = "";

	for (size_t i = 0; i < logBuffers.size(); i++)
	{
		logBuffers[i].mutex.lock();

		for (size_t j = 0; j < logBuffers[j].messages.size(); j++)
		{
			messages += logBuffers[i].messages[j];
		}

		logBuffers[i].messages.clear();
		logBuffers[i].mutex.unlock();

	}

	WriteAndFlushBuffer(messages);
}

void Log::WriteAndFlushBuffer(std::string text) 
{
	if (logfile) 
	{
		if (!logfile->fail() && logfile->is_open())
		{
			*logfile << text;
			logfile->flush();
		}

		if (writeToConsole)
			printf(text.c_str());
	}
}

void Log::CloseLogFile() 
{
	if (logfile)
	{		
		logfile->flush();
		logfile->close();

		delete logfile;
		logfile = NULL;
	}
}