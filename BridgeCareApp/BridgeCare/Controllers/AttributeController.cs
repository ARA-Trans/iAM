using BridgeCare.Interfaces;
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
        public IHttpActionResult GetAttributes() => Ok(repo.GetAttributes(db));
    }
}