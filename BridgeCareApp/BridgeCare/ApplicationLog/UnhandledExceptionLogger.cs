using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace BridgeCare.ApplicationLog
{
    public class UnhandledExceptionLogger : ExceptionLogger
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UnhandledExceptionLogger));
        public override void Log(ExceptionLoggerContext context)
        {
            var logMessage = context.Exception;
            log.Error(logMessage);
        }
    }
}