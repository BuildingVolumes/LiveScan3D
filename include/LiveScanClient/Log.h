#pragma once

#include "stdafx.h"
#include <string>
#include <stdio.h>
#include <fstream>
#include <vector>
#include <mutex>
#include <ctime>
#include <iostream>

struct LogBuffer;

class Log
{
public:

	Log() {}
	~Log() { CloseLogFile(); }

	enum LOGLEVEL
	{
		LOGLEVEL_NONE,
		LOGLEVEL_ERRORS,
		LOGLEVEL_INFO,
		LOGLEVEL_DEBUG,
		LOGLEVEL_DEBUG_CAPTURE,
		LOGLEVEL_ALL		
	};

	bool StartLog(int clientInstance, LOGLEVEL level);
	void PrintAllMessages();
	void RegisterBuffer(LogBuffer* buffer);
	void UnRegisterBuffer(LogBuffer* buffer);
	void WriteAndFlushBuffer(std::string text);
	void CloseLogFile();
	

private:	

	

	std::vector<LogBuffer*> buffers;
	std::mutex registerMutex;
	int clientNumber = 0;
	std::ofstream* logfile = nullptr;
	LOGLEVEL logLevel = LOGLEVEL_INFO;
	bool writeToConsole = false;
};

class LogBuffer
{
public:

	void ChangeName(std::string name);
	std::vector<std::string> GetMessageBuffer();
	void ClearMessageBuffer();

	void LogTrace(std::string message);
	void LogCaptureDebug(std::string message); 
	void LogDebug(std::string message);
	void LogInfo(std::string message);
	void LogWarning(std::string message);
	void LogError(std::string message);
	void LogFatal(std::string message);

	std::mutex bufferMutex;

private:

	void AddLogEntry(std::string message, std::string loglevel);

	std::vector<std::string> messages;
	std::string name = "Unknown";
	Log::LOGLEVEL logLevel = Log::LOGLEVEL_INFO;
};

