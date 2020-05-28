using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;
using System;
using System.Linq;
using System.Web.Http;

namespace BridgeCare.Controllers
{
    public class UserCriteriaController : ApiController
    {
        private readonly IUserCriteria repo;
        private readonly BridgeCareContext db;

        public UserCriteriaController(IUserCriteria repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// API endpoint for fetching the current user's criteria settings.
        /// Uses the authorization token to determine user identity
        /// </summary>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetUserCriteria")]
        [RestrictAccess]
        public IHttpActionResult GetUserCriteria()
        {
            var userInformation = ESECSecurity.GetUserInformation(Request);
            return Ok(repo.GetOwnUserCriteria(db, userInformation));
        }

        /// <summary>
        /// API endpoint for fetching all users' criteria settings.
        /// </summary>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetAllUserCriteria")]
        [RestrictAccess(Role.ADMINISTRATOR)]
        public IHttpActionResult GetAllUserCriteria() => Ok(repo.GetAllUserCriteria(db));

        /// <summary>
        /// API endpoint for modifying a user's criteria settings
        /// </summary>
        /// <param name="userCriteria">UserCriteriaModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SetUserCriteria")]
        [RestrictAccess(Role.ADMINISTRATOR)]
        public IHttpActionResult SetUserCriteria([FromBody] UserCriteriaModel userCriteria)
        {
            repo.SaveUserCriteria(userCriteria, db);
            return Ok();
        }
    }
}
