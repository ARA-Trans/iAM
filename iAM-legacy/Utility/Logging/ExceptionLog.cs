using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Utility.Logging
{
    internal static class ExceptionLog
    {
        //private static LogWriter m_writer;

        static ExceptionLog()
        {
          //  m_writer = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();
        }

        public static void Write(string msg)
        {
            //m_writer.Write(msg, "Exception", 0, 0, System.Diagnostics.TraceEventType.Warning);
        }
    }
}
