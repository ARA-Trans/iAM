using BridgeCare.Models;
using BridgeCare.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BridgeCare.Controllers
{
    public class UserCriteriaController : ApiController
    {
        [HttpGet]
        [Route("api/GetUserCriteria")]
        [RestrictAccess]
        public IHttpActionResult GetUserCriteria()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("api/GetAllUserCriteria")]
        [RestrictAccess(Role.ADMINISTRATOR)]
        public IHttpActionResult GetAllUserCriteria()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("api/SetUserCriteria")]
        [RestrictAccess(Role.ADMINISTRATOR)]
        public IHttpActionResult SetUserCriteria([FromBody] UserCriteriaModel userCriteria)
        {
            throw new NotImplementedException();
        }
    }
}
