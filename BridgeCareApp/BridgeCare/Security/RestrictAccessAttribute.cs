using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace BridgeCare.Security
{
    /// <summary>
    /// Restricts access to an API endpoint, only allowing requests with a valid access token to be processed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RestrictAccessAttribute : AuthorizeAttribute
    {
        //private static readonly RsaSecurityKey ESECPublicKey = GetPublicKey();
        private readonly Func<string, bool> ValidateRole;

        /// <summary>
        /// Only users with the provided roles will be able to access the endpoint.
        /// If no roles are listed, all authenticated users will be able to access it.
        /// </summary>
        /// <param name="roles">Permitted roles</param>
        public RestrictAccessAttribute(params string[] roles) : base()
        {
            ValidateRole = (role) =>
            {
                return roles.Contains(role);
            };
        }

        public RestrictAccessAttribute() : base()
        {
            ValidateRole = (role) => true;
        }

        protected override bool IsAuthorized(HttpActionContext httpContext)
        {
            if (!TryGetAuthorization(httpContext.Request.Headers, out string idToken))
            {
                return false;
            }

            Models.UserInformationModel userInformation = JWTParse.GetUserInformation(idToken);

            return ValidateRole(userInformation.Role);
        }

        /// <summary>
        /// Attempts to get an authorization parameter from an HTTP request's headers
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