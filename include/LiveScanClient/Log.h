#pragma once

#include <string>
#include <stdio.h>
#include <sstream>
#include <fstream>


class Log
{
public:
	
	static Log& Get()
	{
		static Log s_Instance;
		return s_Instance;
	}

	enum LOGLEVEL
	{
		LOGLEVEL_NONE,
		LOGLEVEL_ERRORS,
		LOGLEVEL_INFO,
		LOGLEVEL_DEBUG,
		LOGLEVEL_DEBUG_CAPTURE,
		LOGLEVEL_ALL
		
	};

	void LogTrace(std::string message);
	void LogCaptureDebug(std::string message); //Capture Debug is logging stuff for every single picture taken. Might spam the log pretty fast
	void LogDebug(std::string message);
	void LogInfo(std::string message);
	void LogWarning(std::string message);
	void LogError(std::string message);
	void LogFatal(std::string message);

	bool StartLog(std::string serialNumber, LOGLEVEL level, bool openConsole);
	

private:

	Log(){}
	~Log() { CloseLogFile(); }
	Log(const Log&) = delete;

	void AddLogEntry(std::string message, std::string loglevel);
	void WriteAndFlushBuffer();
	void CloseLogFile();

	std::ofstream* logfile = nullptr;
	std::string logBuffer = "";
	LOGLEVEL logLevel = LOGLEVEL_INFO;
	bool writeToConsole = false;
};

