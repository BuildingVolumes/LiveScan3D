#pragma once

#include "stdafx.h"
#include <string>
#include <stdio.h>
#include <sstream>
#include <fstream>
#include <vector>
#include <mutex>
#include <ctime>
#include "iostream"

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

	int StartLog(int clientInstance, LOGLEVEL level, bool openConsole);
	int RegisterLog();
	void ChangeName(int id, std::string name);
	void PullMessages();

	void LogTrace(int id, std::string message);
	void LogCaptureDebug(int id, std::string message); //Capture Debug is logging stuff for every single picture taken. Might spam the log pretty fast
	void LogDebug(int id, std::string message);
	void LogInfo(int id, std::string message);
	void LogWarning(int id, std::string message);
	void LogError(int id, std::string message);
	void LogFatal(int id, std::string message);

	

private:

	

	void AddLogEntry(int id, std::string message, std::string loglevel);
	void WriteAndFlushBuffer(std::string text);
	void CloseLogFile();

	std::mutex registerMutex;
	int clientNumber = 0;
	std::vector<LogBuffer> logBuffers;
	std::ofstream* logfile = nullptr;
	LOGLEVEL logLevel = LOGLEVEL_INFO;
	bool writeToConsole = false;
};

struct LogBuffer
{
	std::vector<std::string> messages;
	std::mutex mutex;
	std::string name = "Unknown";
};

