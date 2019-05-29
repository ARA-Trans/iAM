using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
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
        [HttpGet]
        [Route("AuthenticateUser")]
        [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]
        public IHttpActionResult GetUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                WindowsIdentity identity = HttpContext.Current.Request.LogonUserIdentity;
                UserInformationModel userInformation = new UserInformationModel
                {
                  Name = identity.Name,
                  Id = identity.User.ToString()
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
