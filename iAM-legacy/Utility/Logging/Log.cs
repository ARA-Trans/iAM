using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Utility.Logging
{
    public static class Log
    {
        //private static LogWriter m_writer;

        static Log()
        {
            try
            {
             //   m_writer = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();
            }
            catch(Exception e)
            {
               // string ex = e.Message;
            
            }
        }

        public static void Write(string msg)
        {
            //m_writer.Write(msg, "General");
        }

        public static void Write(string msg, int priority)
        {
            //m_writer.Write(msg, "General", priority);
        }

        public static void Write(string msg, int priority, int eventId)
        {
            //m_writer.Write(msg, "General", priority, eventId);
        }

        public static void Write(string msg, int priority, int eventId, string title)
        {
            //m_writer.Write(msg, "General", priority, eventId, System.Diagnostics.TraceEventType.Information, title);
        }
    }
}
