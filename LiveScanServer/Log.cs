using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace LiveScanServer
{
    public static class Log
    {
        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();

        static string logFilePath = "Log";

        public enum LogLevel { All, DebugCapture, Debug, Normal, None }

        static LogLevel logLevel;

        static List<string> logBuffer;
        static FileStream fileStream;
        static StreamWriter streamWriter;
        static object writeLock = new object();

        static bool closeRequested = false;
        static bool setup = false;

        /// <summary>
        /// Initiate the logger. You need to call RunLog() in it's own thread afterwards to start logging
        /// </summary>
        /// <param name="loglevel"></param>
        /// <returns></returns>
        public static bool SetupLog(LogLevel loglevel, string logFileName)
        {
#if DEBUG
            if (loglevel > LogLevel.Debug)
                loglevel = LogLevel.Debug;
#endif
            logLevel = loglevel;

            if (logLevel == LogLevel.None)
                return true;

            logFilePath = "logs/" + logFileName + ".txt";

            try
            {
                lock (writeLock)
                {
                    if (!Directory.Exists("logs"))
                        Directory.CreateDirectory("logs");

                    fileStream = new FileStream(logFilePath, FileMode.Create);
                    streamWriter = new StreamWriter(fileStream);
                }                
            }

            catch (Exception e)
            {
                return false;
            }

            closeRequested = false;
            streamWriter.AutoFlush = true;
            logBuffer = new List<string>();

            if (logLevel <= LogLevel.Debug)
                AllocConsole();

            setup = true;

            LogInfo("Logging Started");
            return true;
        }

        /// <summary>
        /// Periodically writes the log to disk/console when new messages have appeared in the log buffer
        /// Should be run in it's own thread.
        /// </summary>
        public static void RunLog()
        {
            if (!setup)
                throw new Exception("Log has not been setup yet!");

            while (!closeRequested)
            {
                if (logBuffer.Count > 0)
                {
                    WriteLogBuffer();
                }

                Thread.Sleep(1);
            }

            DisposeLog();
        }

        public static void WriteLogBuffer()
        {
            List<string> newLogs = new List<string>();

            lock (logBuffer)
            {
                //Copy the log to a temporary list, so that we don't lock the buffer for too long
                //during a possibly time-taking write
                newLogs = new List<string>(logBuffer);
                logBuffer.Clear();
            }

            WriteAndFlush(newLogs);
        }

        static void WriteAndFlush(List<string> buffer)
        {
            lock (writeLock)
            {
                if (fileStream != null)
                {
                    if (streamWriter != null)
                    {
                        string toWrite = "";

                        for (int i = 0; i < buffer.Count; i++)
                            toWrite += "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + ": " + buffer[i] + '\n';

                        try
                        {
                            streamWriter.Write(toWrite);
                        }

                        //Write Error, but hey, no way to log this. Soooo...just keep the app running
                        catch (Exception e)
                        { }

                        if (logLevel <= LogLevel.Debug)
                            Console.Write(toWrite);
                    }
                }
            }        
        }

        public static LogLevel GetLogLevel()
        {
            return logLevel;
        }

        static void AddToLogBuffer(string message)
        {
            lock (logBuffer)
                logBuffer.Add(message);
        }

        public static void LogFatal(string message)
        {
            if (logLevel <= LogLevel.Normal)
            {
                AddToLogBuffer("[FATAL] " + message);
                WriteLogBuffer(); //Always write immediatly, as the app might crash after this
            }
        }

        public static void LogError(string message)
        {
            if (logLevel <= LogLevel.Normal)
            {
                AddToLogBuffer("[ERROR] " + message);
                WriteLogBuffer(); //Also write immediatly, if the app becomes unstable
            }
        }

        public static void LogWarning(string message)
        {
            if (logLevel <= LogLevel.Normal)
            {
                AddToLogBuffer("[WARNING] " + message);
            }
        }

        public static void LogInfo(string message)
        {
            if (logLevel <= LogLevel.Normal)
            {
                AddToLogBuffer("[INFO] " + message);
            }
        }

        public static void LogDebug(string message)
        {
            if (logLevel <= LogLevel.Debug)
            {
                AddToLogBuffer("[DEBUG] " + message);
            }
        }

        public static void LogDebugCapture(string message)
        {
            if (logLevel <= LogLevel.DebugCapture)
            {
                AddToLogBuffer("[DEBUG CAPTURE] " + message);
            }
        }

        public static void LogTrace(string message)
        {
            if (logLevel <= LogLevel.All)
            {
                AddToLogBuffer("[TRACE]" + message);
            }
        }

        /// <summary>
        /// Request to close the log. Finishes the remaining write operations,
        /// and file operations. After calling this you can join the log thread.
        /// </summary>
        public static void CloseLog()
        {
            closeRequested = true;
        }

        private static void DisposeLog()
        {
            setup = false;

            lock (writeLock)
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter = null;
                }

                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream = null;
                }
            }         

        }

    }
}
