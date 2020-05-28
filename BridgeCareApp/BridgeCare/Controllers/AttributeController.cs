using BridgeCare.Interfaces;
using BridgeCare.Security;
using System;
using System.Web.Http;

namespace BridgeCare.Controllers
{
    public class AttributeController : ApiController
    {
        private readonly IAttributeRepo repo;
        private readonly BridgeCareContext db;

        public AttributeController(IAttributeRepo repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// API endpoint for fetching all attributes
        /// </summary>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetAttributes")]
        [RestrictAccess]
        public IHttpActionResult GetAttributes() => Ok(repo.GetAttributes(db));

        [HttpGet]
        [Route("api/GetAttributeSelectValues")]
        [RestrictAccess]
        public IHttpActionResult GetAttributeSelectValues([FromUri] int networkId, [FromUri] string attribute) => Ok(repo.GetAttributeSelectValues(networkId, attribute, db));
    }
}
