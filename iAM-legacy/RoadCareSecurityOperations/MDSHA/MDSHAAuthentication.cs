#define AT_MDSHA // Leave this here; see below

#undef AT_MDSHA // Comment/uncomment this to reflect build situation

#if AT_MDSHA
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoadCareSecurityOperations.Properties;
using RoadCareSecurityOperations.AuthenticateCS;
using DatabaseManager;
using System.IO;
using System.Windows.Forms;
#endif

namespace RoadCareSecurityOperations.MDSHA
{
    public class AuthenticationResult
    {
#if AT_MDSHA
        private bool successful;
        private string message;

        public AuthenticationResult(bool success, string attemptMessage)
        {
            successful = success;
            message = attemptMessage;
        }

        public bool Successful
        {
            get
            {
                return successful;
            }
        }

        public string Message
        {
            get
            {
                return message;
            }
        }
#endif
    }

#if AT_MDSHA
    public static class MDSHAAuthentication
    {
        private static SecOps Operations = null;
        public static AuthenticationResult Authenticate(SecOps ops)
        {
            AuthenticationResult result = new AuthenticationResult(false, "Unattempted Login!");
            if (ops != null)
            {
                Operations = ops;
            }
            else
            {
                throw new ArgumentException("Security Operations object cannot be null.");
            }

            string authMessage;
            AuthenticateCS.AuthenticateCS serv = new AuthenticateCS.AuthenticateCS();
            serv.Url = GetAuthenticationURL();
#if TESTING
            clsLoginResultCS attemptResult = new clsLoginResultCS();
            attemptResult.result = 1;
            attemptResult.accessRole = "RW";
#else
            clsLoginResultCS attemptResult = serv.LoginIntranetCS(Environment.UserName, Settings.Default.appId);
#endif


            if (attemptResult.result == 0)
            {
                authMessage = "Authentication failed: " + attemptResult.errMessage;
                result = new AuthenticationResult(false, authMessage);
            }
            else
            {
                authMessage = "Authentication succeeded: " + attemptResult.errMessage + " ";
                if (attemptResult.accessRole.ToUpper() == "R")
                {
                    Operations.ReadOnly = true;
                }

                result = RoadCareLogin(attemptResult);
            }

            return result;
        }

        private static string GetAuthenticationURL()
        {
            string URL = "";
            bool found = false;
            TextReader tr = null;
            tr = new StreamReader(".\\MDSHA.ini");

            for (string line = tr.ReadLine(); line != null; line = tr.ReadLine())
            {
                line = line.Replace(" ", "");
                int foundIndex = line.IndexOf("AuthURL=");
                if (foundIndex > -1)
                {
                    found = true;
                    URL = line.Substring(foundIndex + 8);
                    break;
                }
            }

            if (!found)
            {
                URL = "";
            }

            return URL;
        }

        private static AuthenticationResult RoadCareLogin(clsLoginResultCS attemptResult)
        {
            bool error = false;
            string message = "";
            string encyptedConnString = attemptResult.dbConnectString;
            const string key = "SHAN-RC-20-GUA-05";

            string plainConnString = null;
            try
            {
                plainConnString = Decryptor.Decrypt(encyptedConnString, key);
            }
            catch (Exception ex)
            {
                plainConnString = null;
                error = true;
                message = "Error: Unable to decrypt connection string: " + ex.Message;
            }

            ConnectionParameters cp = null;
#if TESTING
            plainConnString = "Data Source = (DESCRIPTION = (CID = GTU_APP)(ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = trans-tester)(PORT = 1521)))(CONNECT_DATA = (SID = MDSHA)(SERVER = DEDICATED))); User Id = DBO_MDSHA_FIX; Password = DBO_MDSHA_FIX;".ToLower();
            error = false;
            message = "";
#else
#endif
            if (plainConnString != null)
            {
                cp = new ConnectionParameters("ORACLE", plainConnString, true);
                try
                {
                    DBMgr.NativeConnectionParameters = cp;
                }
                catch (Exception exc)
                {
                    message = "Error connecting to the database: " + exc.Message;
                    error = true;
                }

                if (!error)
                {
                    Operations.CheckFreshInstall();

                    try
                    {
                        Operations.SetCurrentUser(Environment.UserName, "");
                    }
                    catch (Exception ex)
                    {
                        message = "Error logging in: " + ex.Message;
                        error = true;
                    }
                }

                if (!error)
                {

                    if (!Operations.IsAuthenticated)
                    {
                        message = "User failed to Login.";
                        error = true;
                    }
                }
            }
            else if (message == "")
            {
                //this else block is also necessary in addition to the try/catch block above because a change
                //has been proposed where the function will not throw an exception and just return a null instead.
                error = true;
                message = "Error: Unable to decrypt connection string.";
            }

            if (!error)
            {
                message = "";
            }

            AuthenticationResult res = new AuthenticationResult(!error, message);

            return res;
        }
    }
#endif
}
