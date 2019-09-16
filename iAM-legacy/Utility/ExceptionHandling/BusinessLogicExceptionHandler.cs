using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.ExceptionHandling
{
    public static class BusinessLogicExceptionHandler
    {
        /// <summary>
        /// The business logic layer catch and log the exception
        /// Includes checking the original exception source(From data access layer or not)
        /// Then throw new exception to the UI layer with the given message
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="msg2UI">Customized message to the upper layer, null if not throw</param>
        /// <param name="isThrow">Flag of throwing or not</param>
        public static void HandleException(Exception ex,  bool isThrow, string msg2UI)
        {
            HandleException(ex, ex.Message, isThrow, msg2UI);
        }

        /// <summary>
        /// The business logic layer catch and log the exception
        /// Includes checking the original exception source(From data access layer or not)
        /// Then throw new exception to the UI layer with the original exception message
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="isThrow">Flag of throwing or not</param>
        public static void HandleException(Exception ex, bool isThrow)
        {
            HandleException(ex, isThrow, ex.Message);
        }

        public static void HandleException(Exception ex, string customLogMsg, bool isThrow, string msg2UI)
        {
            string addonMsg = "Business Logic Layer Exception: ";

            // check if the exception comes from the data access layer
            if (!ex.Message.Contains("Data Access Layer Exception: "))
            {
                if (customLogMsg != ex.Message)
                    Logging.ExceptionLog.Write(addonMsg + ex.ToString() + "\r\n Custom Message: " + customLogMsg);
                else
                    Logging.ExceptionLog.Write(addonMsg + ex.ToString());
            }

            // thow to the UI layer
            if (isThrow)
                throw new Exception(addonMsg + msg2UI, ex);
        }
    }
}
