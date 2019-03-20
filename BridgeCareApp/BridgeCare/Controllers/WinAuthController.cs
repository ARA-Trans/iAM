using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BridgeCare.Controllers
{
    [Authorize]
    [RoutePrefix("auth")]
    public class WinAuthController : ApiController
    {
        [HttpGet]
        [Route("getuser")]
        [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]
        public IHttpActionResult GetUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Ok($"{User.Identity.Name}");
            }
            else
            {
                return BadRequest("Not authenticated");
            }
        }
    }
}
