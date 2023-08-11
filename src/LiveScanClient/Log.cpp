#include "Log.h"


bool Log::StartLog(int clientInstance, LOGLEVEL level)
{
#ifdef _DEBUG
	if (level < LOGLEVEL_DEBUG)
		level = LOGLEVEL_DEBUG;
#endif

	CloseLogFile();

	clientNumber = clientInstance;

	if (!std::filesystem::exists("logs/"))
	{
		if (!std::filesystem::create_directory("logs/"))
			return false;
	}

	std::string filename = "logs/Log_Client_" + std::to_string(clientNumber) + ".txt";
	logfile = new std::ofstream;
	logfile->open(filename, std::ofstream::trunc);
	logLevel = level;
	writeToConsole = level >= LOGLEVEL_DEBUG;

	if (logfile->fail())
	{
		return false;
	}


	if (writeToConsole)
	{
		AllocConsole();
		freopen("CONOUT$", "w", stdout);
		freopen("CONOUT$", "w", stderr);
	}

	return true;
}

/// <summary>
/// The log is being accessed by multiple Threads. To keep the performance high and avoid too many mutex locks, each thread has it's own
/// message buffer. The buffers are periodically read out and flushed
/// </summary>
/// <param name="message"></param>
void Log::RegisterBuffer(LogBuffer* buffer)
{
	std::lock_guard<std::mutex> regMutex(registerMutex);
	buffers.push_back(buffer);
}

/// <summary>
/// When the Object containing LogBuffer is being destroyed, always unregister the logbuffer 
/// before destroying the object, otherwise a nullpointer exeption may occur 
/// </summary>
/// <param name="buffer"></param>
void Log::UnRegisterBuffer(LogBuffer* buffer)
{
	std::lock_guard<std::mutex> regMutex(registerMutex);

	for (size_t i = 0; i < buffers.size(); i++)
	{
		if (buffers[i] == buffer)
			buffers.erase(buffers.begin() + i);
	}
}

/// <summary>
/// Gets the messsages from all registered log buffers and writes them to a file, as well as print them to the console
/// </summary>
void Log::PrintAllMessages()
{
	std::string allMessages = "";

	for (size_t i = 0; i < buffers.size(); i++)
	{
		std::vector<std::string> messages = buffers[i]->GetMessageBuffer();
		buffers[i]->ClearMessageBuffer();

		for (size_t j = 0; j < messages.size(); j++)
		{
			allMessages += messages[j];
		}
	}

	WriteAndFlushBuffer(allMessages);
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

/// <summary>
/// Returns all the messages in the buffer, always use ClearMessageBuffer() afterwards
/// </summary>
/// <returns></returns>
std::vector <std::string> LogBuffer::GetMessageBuffer()
{
	std::lock_guard<std::mutex> regMutex(bufferMutex);
	return messages;
}

/// <summary>
/// Clears a message buffer, should always be used after getting messages
/// </summary>
void LogBuffer::ClearMessageBuffer()
{
	std::lock_guard<std::mutex> regMutex(bufferMutex);
	messages.clear();
}

/// <summary>
/// Changes the name which should be used in the log display
/// </summary>
/// <param name="newName"></param>
void LogBuffer::ChangeName(std::string newName)
{
	name = newName;
}

/// <summary>
/// For logging the smallest things
/// </summary>
/// <param name="message"></param>
void LogBuffer::LogTrace(std::string message)
{
	if (logLevel >= Log::LOGLEVEL_ALL)
		AddLogEntry(message, "[TRACE]");
}

/// <summary>
/// For logs that are occuring each frame
/// </summary>
/// <param name="message"></param>
void LogBuffer::LogCaptureDebug(std::string message)
{
	if (logLevel >= Log::LOGLEVEL_DEBUG_CAPTURE)
		AddLogEntry(message, "[CAPTURE DEBUG]");
}

/// <summary>
/// Logs that should only be used while debugging
/// </summary>
/// <param name="message"></param>
void LogBuffer::LogDebug(std::string message)
{
	if (logLevel >= Log::LOGLEVEL_DEBUG)
		AddLogEntry(message, "[DEBUG]");
}

/// <summary>
/// Common and important logs that should also always be logged in the end user runtime
/// </summary>
/// <param name="message"></param>
void LogBuffer::LogInfo(std::string message)
{
	if (logLevel >= Log::LOGLEVEL_INFO)
		AddLogEntry(message, "[INFO]");
}

/// <summary>
/// Logs for smaller errors, where the program can continue execution normally.
/// Will be logged in the end user runtime
/// </summary>
/// <param name="message"></param>
void LogBuffer::LogWarning(std::string message)
{
	if (logLevel >= Log::LOGLEVEL_ERRORS)
		AddLogEntry(message, "[WARNING]");
}

/// <summary>
/// Logs for errors which might have a big impact on the execution, but the program
/// can recover from. Will be logged in the end user runtime
/// </summary>
/// <param name="message"></param>
void LogBuffer::LogError(std::string message)
{
	if (logLevel >= Log::LOGLEVEL_ERRORS)
		AddLogEntry(message, "[ERROR]");
}

/// <summary>
/// Should only be used for a log message after which the program can't proceed with the execution.
/// Will be logged in the end user runtime
/// </summary>
/// <param name="message"></param>
void LogBuffer::LogFatal(std::string message)
{
	if (logLevel >= Log::LOGLEVEL_ERRORS)
		AddLogEntry(message, "[FATAL]");
}

/// <summary>
/// Adds the log message to the message buffer, with the name, time severity level.
/// </summary>
/// <param name="message"></param>
/// <param name="loglevel"></param>
void LogBuffer::AddLogEntry(std::string message, std::string loglevel)
{
	time_t t = time(0);
	struct tm* now = localtime(&t);

	std::string newMessage = std::string("[") + name + std::string("]");

	std::string hour = std::to_string(now->tm_hour);
	std::string min = std::to_string(now->tm_min);
	std::string sec = std::to_string(now->tm_sec);

	//Pad values
	if (hour.size() == 1)
		hour = "0" + hour;
	if (min.size() == 1)
		min = "0" + min;
	if (sec.size() == 1)
		sec = "0" + sec;


	newMessage += "[" + hour + ":" + min + ":" + sec + "] ";
	newMessage += loglevel + ": " + message + "\n";

	std::lock_guard<std::mutex> regMutex(bufferMutex);
	messages.push_back(newMessage);

}