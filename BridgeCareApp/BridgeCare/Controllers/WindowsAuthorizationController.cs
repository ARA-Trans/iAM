using System.Security.Authentication;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using BridgeCare.Models;

namespace BridgeCare.Controllers
{
    [Authorize]
    [RoutePrefix("auth")]
    public class WindowsAuthorizationController : ApiController
    {
        /// <summary>
        /// API endpoint for authenticating users using windows identity
        /// Throws an AuthenticationException if a user is not authenticated or cannot be verified
        /// </summary>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("AuthenticateUser")]
        [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]
        public IHttpActionResult GetUser()
        {
            if (!User.Identity.IsAuthenticated)
                throw new AuthenticationException("User failed authentication.");

            var windowsIdentity = HttpContext.Current.Request.LogonUserIdentity;

            if (windowsIdentity == null)
                throw new AuthenticationException("System cannot determine user identity.");

            return Ok(new UserInformationModel(windowsIdentity));
        }
    }
}
