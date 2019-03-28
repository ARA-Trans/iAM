using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BridgeCare.Controllers
{
    [Authorize]
    [RoutePrefix("auth")]
    public class WindowsAuthorizationController : ApiController
    {
        [HttpGet]
        [Route("getuser")]
        [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]
        public IHttpActionResult GetUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                WindowsIdentity identity = HttpContext.Current.Request.LogonUserIdentity;
                List<string> userInformation = new List<string>
                {
                    $"{identity.Name}",
                    $"{identity.User}"
                };
                return Ok(userInformation);
            }
            else
            {
                return BadRequest("Not authenticated");
            }
        }
    }
}
