using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.ExceptionHandling
{
    public static class DataAccessExceptionHandler
    {
        /// <summary>
        /// The data access layer will log the exception in the application "Errors&Warnings.log" file,
        /// and throw a new exception to the upper layer with the original one as the inner exception
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="isThrow">flag throw to upper layer or not</param>
        public static void HandleException(Exception ex, bool isThrow)
        {
            HandleException(ex, ex.Message, isThrow);
        }

        public static void HandleException(Exception ex, string customMsg, bool isThrow)
        {
            Logging.ElmahLog.Write(ex, customMsg);
            if (isThrow)
                throw ex;
        }
    }
}
