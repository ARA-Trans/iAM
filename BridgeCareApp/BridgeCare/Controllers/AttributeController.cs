using BridgeCare.Interfaces;
using BridgeCare.Security;
using System;
using System.Web.Http;
using BridgeCare.Models;

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

        [HttpPost]
        [Route("api/GetAttributesSelectValues")]
        [RestrictAccess]
        public IHttpActionResult GetAttributeSelectValues([FromBody] NetworkAttributes model) => Ok(repo.GetAttributeSelectValues(model, db));
    }
}
