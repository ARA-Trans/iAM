using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Controllers;
using BridgeCare.Controllers;

namespace BridgeCare.Security
{
    /// <summary>
    ///     Restricts access to an API endpoint, only allowing requests with a valid access token to
    ///     be processed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RestrictAccessAttribute : AuthorizeAttribute
    {
        private readonly Func<string, bool> ValidateRole;

        /// <summary>
        ///     Only users with the provided roles will be able to access the endpoint. If no roles
        ///     are listed, all authenticated users will be able to access it.
        /// </summary>
        /// <param name="roles">Permitted roles</param>
        public RestrictAccessAttribute(params string[] roles) : base()
        {
            ValidateRole = role => roles.Contains(role);
        }

        public RestrictAccessAttribute() : base()
        {
            ValidateRole = role => true;
        }

        protected override bool IsAuthorized(HttpActionContext httpContext)
        {
            if (!TryGetAuthorization(httpContext.Request.Headers, out string accessToken))
            {
                return false;
            }

            var userInformationDictionary = AuthenticationController.GetUserInfoDictionary(accessToken);

            if (!userInformationDictionary.ContainsKey("roles"))
            {
                throw new UnauthorizedAccessException("User has no roles assigned.");
            }

            var userInformation = ESECSecurity.GetUserInformation(userInformationDictionary);

            // Some API endpoints need this user information, so it is inserted into
            // the request here before they process it
            httpContext.Request.Headers.Clear();

            httpContext.Request.Headers.Add("Role", userInformation.Role);
            httpContext.Request.Headers.Add("Name", userInformation.Name);
            httpContext.Request.Headers.Add("Email", userInformation.Email);

            return ValidateRole(userInformation.Role);
        }

        /// <summary>
        ///     Attempts to get an authorization parameter from an HTTP request's headers
        /// </summary>
        /// <param name="headers">Request headers</param>
        /// <returns>Returns true if successful</returns>
        private static bool TryGetAuthorization(HttpRequestHeaders headers, out string authorization)
        {
            if (!headers.Contains("Authorization"))
            {
                authorization = "";
                return false;
            }
            if (headers.Authorization.Parameter == null)
            {
                authorization = "";
                return false;
            }
            authorization = headers.Authorization.Parameter.ToString();
            return true;
        }
    }
}
