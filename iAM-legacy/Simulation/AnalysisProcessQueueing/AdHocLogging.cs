using System;
using System.Diagnostics;
using System.IO;

namespace Simulation.AnalysisProcessQueueing
{
    /// <summary>
    ///     When "print-line style" debugging is required, do it cleanly with
    ///     this.
    /// </summary>
    public static class AdHocLogging
    {
        private static bool FirstLog = true;
        private static string LogFolderPath;

        public static void Log(object logKey, string logMessage = null)
        {
            if (FirstLog)
            {
                FirstLog = false;
                var tempPath = Path.GetTempPath();
                var logFolderName = Path.GetRandomFileName();
                LogFolderPath = Path.Combine(tempPath, logFolderName);
                _ = Directory.CreateDirectory(LogFolderPath);
                _ = Process.Start(LogFolderPath);
            }

            var fileName = logKey + ".log";
            var logPath = Path.Combine(LogFolderPath, fileName);
            var logText = DateTime.Now.ToString("s") + "\t" + (logMessage ?? "No message provided.") + Environment.NewLine;
            File.AppendAllText(logPath, logText);
        }
    }
}
