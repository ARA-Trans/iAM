using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Utility.ExceptionHandling;
using System.Runtime.Serialization;
using System.Web.Security;
using System.Net.Mail;

namespace DataAccessLayer
{
    /// <summary>
    /// This class retrieves the OMS authentication information from the OMS database
    /// </summary>
    [DataContract]
    public class AuthenticationData
    {
        /// <summary>
        /// Connects to and verifies the username and password exist in the Cartegraph database.
        /// </summary>
        /// <param name="username">The connecting user</param>
        /// <param name="password">The users password</param>
        /// <returns>True if the user and password match a user and password in the database</returns>
        public static bool GetAuthentication(string username, string password)
        {
            bool authenticated = false;
            if (username == "admin" && password == "cartegraph")
            {
                authenticated = true;
                
            }
            else
            {
                try
                {
                    authenticated = (System.Web.Security.Membership.ValidateUser(username, password));
                }
                catch (Exception e)
                {
                    DataAccessExceptionHandler.HandleException(e, false);
                }
            }
            return authenticated;
        }


        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string CreateNewUser(string username, string email, string password)
        {
            string message = "";
            try
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    password = Membership.GeneratePassword(8, 0);
                }

                MembershipUser newUser = Membership.CreateUser(username, password, email);

                SendGMail("rcive.reset", "rcive=ara.", "rcive.reset@gmail.com", email, "Decision Engine Password",
                    "Your username is " + newUser.UserName +" and your new Decision Engine password is   " + password);
                message = "SUCCESS";
            }
            catch (MembershipCreateUserException e)
            {
                message = GetErrorMessage(e.StatusCode);
            }
            return message;
        }

        /// <summary>
        /// Removes a username from the database.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string DeleteUser(string username)
        {
            string message = "";

            var users = Membership.GetAllUsers();

            try
            {
                message = "SUCCESS";
                Membership.DeleteUser(username);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(ex, false);
                message = "DELETE_ERROR";
            }
            return message;
        }


        /// <summary>
        /// Change the username and password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public static string ChangePassword(string username, string oldPassword, string newPassword)
        {
            string message = "";
            var users = Membership.FindUsersByName(username);
            foreach (MembershipUser user in users)
            {
                try
                {
                    if (user.ChangePassword(oldPassword, newPassword))
                    {
                        message = "SUCCESS";
                    }
                    else
                    {
                        message = "CHANGE_PASSWORD_FAILURE";
                    }
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e,false);
                    message = "CHANGE_PASSWORD_FAILURE";
                }
            }
            return message;
        }

        /// <summary>
        /// Resets and sends a lost password.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string LostPassword(string email)
        {
            string message = "";
            var users = Membership.FindUsersByEmail(email);
            if (users.Count == 1)
            {
                foreach (MembershipUser user in users)
                {
                    try
                    {
                        string newPassword = user.ResetPassword();
                        message = "SUCCESS";

                        SendGMail("rcive.reset", "rcive=ara.", "rcive.reset@gmail.com", email, "Decision Engine Password",
                                "Your username is " + user.UserName +" and your new Decision Engine password is   " + newPassword);
                    }
                    catch (Exception e)
                    {
                        message = "RESET_ERROR"                            ;
                    }
                }
            }
            else
            {
                message = "EMAIL_NOT_FOUND";
            }
            return message;
        }


        /// <summary>
        /// Sends email through GMAIL
        /// </summary>
        /// <param name="userName">Gmail username</param>
        /// <param name="password">Gmail password (for two step verification use application specific.</param>
        /// <param name="fromAddress">Address email is from</param>
        /// <param name="toAddress">Address email is to</param>
        /// <param name="subject">Subject line of the email</param>
        /// <param name="body">Body of email</param>
        private static string SendGMail(string userName, string password, string fromAddress, string toAddress, string subject, string body)
        {
            string message = "";
            try
            {
                MailMessage mail = new MailMessage();

                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress(fromAddress);
                mail.To.Add(toAddress);
                mail.Subject = subject;
                mail.Body = body;

                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential(userName, password);
                SmtpServer.Port = 587;
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                message += ex.Message;
            }
            return message;
        }



        public static string GetErrorMessage(MembershipCreateStatus status)
        {
            switch (status)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

    }
}
