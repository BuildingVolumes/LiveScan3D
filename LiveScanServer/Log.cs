using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace LiveScanServer
{
    static class Log
    {
        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();

        const string logFilePath = "logs/Log_Server.txt";

        public enum LogLevel { All, DebugCapture, Debug, Normal, None}

        static LogLevel logLevel;

        static FileStream fileStream;
        static StreamWriter streamWriter;


        public static bool StartLog(LogLevel loglevel)
        {
#if DEBUG
            if (loglevel > LogLevel.Debug)
                loglevel = LogLevel.Debug;
#endif

            CloseLog();

            logLevel = loglevel;

            if (logLevel == LogLevel.None)
                return true;

            try
            {
                if (!Directory.Exists("logs"))
                    Directory.CreateDirectory("logs");

                fileStream = new FileStream(logFilePath, FileMode.Create);
                streamWriter = new StreamWriter(fileStream);
            }

            catch(Exception e)
            {
                return false;
            }

            streamWriter.AutoFlush = true;

            if (logLevel <= LogLevel.Debug)
                AllocConsole();

            LogInfo("Logging Started");
            return true;
        }

        public static void CloseLog()
        {
            if(streamWriter != null)
            {
                streamWriter.Close();
                streamWriter = null;
            }

            if(fileStream != null)
            {
                fileStream.Close();
                fileStream = null;
            }
            
        }

        public static void WriteAndFlush(string message, string level)
        {
            if(fileStream != null)
            {
                if(streamWriter != null)
                {
                    string newLine = "[" +DateTime.Now.ToString("HH:mm:ss") + "] " + level + ": " + message;

                    try
                    {
                        streamWriter.WriteLine(newLine);
                    }

                    //We can't write, but hey, no way to log this soooo...just keep the app running
                    catch (Exception e)
                    {}


                    if (logLevel <= LogLevel.Debug)
                        Console.WriteLine(newLine);
                }
            }

            
        }

        public static LogLevel GetLogLevel()
        {
            return logLevel;
        }

        public static void LogFatal(string message)
        {
            if(logLevel <= LogLevel.Normal)
            {
                WriteAndFlush(message, "[FATAL]");
            }
        }

        public static void LogError(string message)
        {
            if (logLevel <= LogLevel.Normal)
            {
                WriteAndFlush(message, "[ERROR]");
            }
        }

        public static void LogWarning(string message)
        {
            if (logLevel <= LogLevel.Normal)
            {
                WriteAndFlush(message, "[WARNING]");
            }
        }

        public static void LogInfo(string message)
        {
            if (logLevel <= LogLevel.Normal)
            {
                WriteAndFlush(message, "[INFO]");
            }
        }

        public static void LogDebug(string message)
        {
            if (logLevel <= LogLevel.Debug)
            {
                WriteAndFlush(message, "[DEBUG]");
            }
        }

        public static void LogDebugCapture(string message)
        {
            if (logLevel <= LogLevel.DebugCapture)
            {
                WriteAndFlush(message, "[DEBUG CAPTURE]");
            }
        }

        public static void LogTrace(string message)
        {
            if (logLevel <= LogLevel.All)
            {
                WriteAndFlush(message, "[TRACE]");
            }
        }

    }
}
