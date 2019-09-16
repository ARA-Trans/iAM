using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Utility.ExceptionHandling
{
    public static class UserInterfaceExceptionHandler
    {

        /// <summary>
        /// UI layer catch and log only the UI layer exception
        /// Show the user friendly UI exception form with the given msg and exception details. 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="msg"></param>
        public static void HandleException(Exception ex, bool isShowUser, string msg)
        {
            string addonMsg = "User Interface Layer Exception: ";

            // check if it's the business logic layer exception
            if (!ex.Message.Contains("Business Logic Layer Exception: "))
                Logging.ExceptionLog.Write(addonMsg + ex.ToString());

            if (isShowUser)
            {
                UIExceptionForm f = new UIExceptionForm(ex, msg);
                f.ShowDialog();
            }
        }
    }
}
