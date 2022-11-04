#include "Log.h"
#include <ctime>
#include "iostream"

bool Log::StartLog(std::string serialNumber, LOGLEVEL level, bool openConsole) 
{
	CloseLogFile();

	std::string filename = "Log_Client_";
	filename += serialNumber.c_str();
	filename += ".txt";
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

	LogInfo("Start of Logging");

	return true;
}

void Log::LogTrace(std::string message)
{
	if (logLevel >= LOGLEVEL_ALL)
		AddLogEntry(message, "[TRACE]");	
}

void Log::LogCaptureDebug(std::string message)
{
	if (logLevel >= LOGLEVEL_DEBUG_CAPTURE)
		AddLogEntry(message, "[CAPTURE DEBUG]");
}

void Log::LogDebug(std::string message)
{
	if (logLevel >= LOGLEVEL_DEBUG)
		AddLogEntry(message, "[DEBUG]");
}

void Log::LogInfo(std::string message)
{
	if (logLevel >= LOGLEVEL_INFO)
		AddLogEntry(message, "[INFO]");
}

void Log::LogWarning(std::string message) 
{
	if (logLevel >= LOGLEVEL_ERRORS)
		AddLogEntry(message, "[WARNING]");
}

void Log::LogError(std::string message)
{
	if (logLevel >= LOGLEVEL_ERRORS)
		AddLogEntry(message, "[ERROR]");
}

void Log::LogFatal(std::string message)
{
	if (logLevel >= LOGLEVEL_ERRORS)
		AddLogEntry(message, "[FATAL]");
}

void Log::AddLogEntry(std::string message, std::string loglevel) 
{
	time_t t = time(0);
	struct tm* now = localtime(&t);

	std::ostringstream sstream;
	sstream << "[" << now->tm_hour << ":" << now->tm_min << ":" << now->tm_sec << "] " << loglevel << ": " << message << std::endl;
	logBuffer += sstream.str();

	WriteAndFlushBuffer();
}

void Log::WriteAndFlushBuffer() 
{
	if (logfile) 
	{
		if (!logfile->fail() && logfile->is_open())
		{
			*logfile << logBuffer;
			logfile->flush();
		}

		if (writeToConsole)
			printf(logBuffer.c_str());

		logBuffer = "";
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