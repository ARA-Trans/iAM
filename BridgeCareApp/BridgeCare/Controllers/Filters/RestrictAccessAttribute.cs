using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Web.Http.Controllers;

namespace BridgeCare.Controllers.Filters
{
    /// <summary>
    /// Restricts access to an API endpoint, only allowing requests with a valid access token to be processed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RestrictAccessAttribute : AuthorizeAttribute
    {
        private string[] PermittedRoles { get; set; }

        /// <summary>
        /// Only users with the provided roles will be able to access the endpoint.
        /// If no roles are listed, all authenticated users will be able to access it.
        /// </summary>
        /// <param name="roles">Permitted roles</param>
        public RestrictAccessAttribute (params string[] roles) : base()
        {
            this.PermittedRoles = roles;
        }

        protected override bool IsAuthorized(HttpActionContext httpContext)
        {
            if (!TryGetAuthorization(httpContext.Request.Headers, out string accessToken))
            {
                return false;
            }
            Dictionary<string, string> userInfo = AuthenticationController.GetUserInfoDictionary(accessToken);
            if (!userInfo.ContainsKey("roles")) 
            {
                return false;
            }
            if (PermittedRoles.Length == 0)
            {
                return true;
            }
            string role = ParseRoleResponse(userInfo["roles"]);
            return PermittedRoles.Contains(role);
        }

        /// <summary>
        /// Given the LDAP-formatted "roles" string from ESEC, extracts the role
        /// </summary>
        /// <param name="roleResponse">LDAP-formatted response</param>
        /// <returns>Role</returns>
        private static string ParseRoleResponse(string roleResponse)
        {
            string firstSegment = roleResponse.Split(',')[0];
            string role = firstSegment.Substring(3);
            return role;
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