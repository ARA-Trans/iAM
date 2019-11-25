using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace BridgeCare.Controllers.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RestrictAccessAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext httpContext)
        {
            if (httpContext.Request.Headers.Contains("Authorization"))
            {
                if (httpContext.Request.Headers.Authorization.Parameter == null)
                {
                    return false;
                }
                string accessToken = httpContext.Request.Headers.Authorization.Parameter.ToString();
                string userInfoString = AuthenticationController.GetUserInfoString(accessToken);
                Dictionary<string, string> userInfo = (new JavaScriptSerializer()).Deserialize<Dictionary<string, string>>(userInfoString);
                if (!userInfo.ContainsKey("roles")) {
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}