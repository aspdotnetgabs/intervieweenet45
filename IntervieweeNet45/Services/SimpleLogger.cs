using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace IntervieweeNet45.Services
{
    public interface ISimpleLogger
    {
        void LogDebug(string text);
        void LogError(string text);
        void LogInformation(string text);
        void LogTrace(string text);
        void LogWarning(string text);
    }


    public class SimpleLogger : ISimpleLogger
    {
        public string DatetimeFormat;
        public string Filename;

        /// <summary>
        /// Initialize a new instance of SimpleLogger class.
        /// Log file will be created automatically if not yet exists, else it can be either a fresh new file or append to the existing file.
        /// Default is create a fresh new log file.
        /// </summary>
        /// <param name="append">True to append to existing log file, False to overwrite and create new log file</param>
        public SimpleLogger()
        {
            DatetimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
            string Date = DateTime.Now.ToString("yyyyMMdd");
            //Filename = Path.Combine(_env.ContentRootPath, "Logs", "log-" + Date + ".txt");
            Filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Logs\log-" + Date + ".txt");

            if (!System.IO.File.Exists(Filename))
            {
                (new FileInfo(Filename)).Directory.Create();
                // Log file header line
                string logHeader = Filename + " is created." + Environment.NewLine;
                WriteLine(DateTime.Now.ToString(DatetimeFormat) + " " + logHeader, false);
            }
        }

        /// <summary>
        /// Log a debug message
        /// </summary>
        /// <param name="text">Message</param>
        public void LogDebug(string text)
        {
            WriteFormattedLog(LogLevel.DEBUG, text);
        }

        /// <summary>
        /// Log an error message
        /// </summary>
        /// <param name="text">Message</param>
        public void LogError(string text)
        {
            WriteFormattedLog(LogLevel.ERROR, text);
        }

        /// <summary>
        /// Log a fatal error message
        /// </summary>
        /// <param name="text">Message</param>
        public void LogFatal(string text)
        {
            WriteFormattedLog(LogLevel.FATAL, text);
        }

        /// <summary>
        /// Log an info message
        /// </summary>
        /// <param name="text">Message</param>
        public void LogInformation(string text)
        {
            WriteFormattedLog(LogLevel.INFO, text);
        }

        /// <summary>
        /// Log a trace message
        /// </summary>
        /// <param name="text">Message</param>
        public void LogTrace(string text)
        {
            WriteFormattedLog(LogLevel.TRACE, text);
        }

        /// <summary>
        /// Log a waning message
        /// </summary>
        /// <param name="text">Message</param>
        public void LogWarning(string text)
        {
            WriteFormattedLog(LogLevel.WARNING, text);
        }

        /// <summary>
        /// Format a log message based on log level
        /// </summary>
        /// <param name="level">Log level</param>
        /// <param name="text">Log message</param>
        private void WriteFormattedLog(LogLevel level, string text)
        {
            string pretext;
            switch (level)
            {
                case LogLevel.TRACE: pretext = DateTime.Now.ToString(DatetimeFormat) + " [TRACE]   "; break;
                case LogLevel.INFO: pretext = DateTime.Now.ToString(DatetimeFormat) + " [INFO]    "; break;
                case LogLevel.DEBUG: pretext = DateTime.Now.ToString(DatetimeFormat) + " [DEBUG]   "; break;
                case LogLevel.WARNING: pretext = DateTime.Now.ToString(DatetimeFormat) + " [WARNING] "; break;
                case LogLevel.ERROR: pretext = DateTime.Now.ToString(DatetimeFormat) + " [ERROR]   "; break;
                case LogLevel.FATAL: pretext = DateTime.Now.ToString(DatetimeFormat) + " [FATAL]   "; break;
                default: pretext = ""; break;
            }

            WriteLine(pretext + text + Environment.NewLine);
        }

        /// <summary>
        /// Write a line of formatted log message into a log file
        /// </summary>
        /// <param name="text">Formatted log message</param>
        /// <param name="append">True to append, False to overwrite the file</param>
        /// <exception cref="System.IO.IOException"></exception>
        public void WriteLine(string text, bool append = true)
        {
            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    if (append)
                        File.AppendAllText(Filename, text);
                    else
                        File.WriteAllText(Filename, text);
                }
                catch { }
            }
        }

        /// <summary>
        /// Supported log level
        /// </summary>
        [Flags]
        private enum LogLevel
        {
            TRACE,
            INFO,
            DEBUG,
            WARNING,
            ERROR,
            FATAL
        }
    }

}

