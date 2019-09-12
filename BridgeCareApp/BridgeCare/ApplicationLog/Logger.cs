using System;
using System.Diagnostics;

namespace BridgeCare.ApplicationLog
{
    public static class Logger
    {
        public static void Error(string message, string module)
        {
            WriteEntry(message, " error ", module);
        }

        private static void WriteEntry(string message, string type, string module)
        {
            Trace.WriteLine(
                    string.Format("{0},{1},{2},{3}",
                                  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                  type,
                                  module,
                                  message));
            Trace.Close();
        }
    }
}