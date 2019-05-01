using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    public class SectionController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly ISections section;

        public SectionController(ISections sections, BridgeCareContext context)
        {
            section = sections ?? throw new ArgumentNullException(nameof(sections));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Get: api/section
        [ModelValidation("Given network data is not valid")]
        public IQueryable<SectionModel> Get(NetworkModel data) => section.GetSections(data, db);

        [ModelValidation("Function call not valid")]
        [Route("api/GetSectionID/{networkID}/{BrKey}")]
        [HttpGet]
        public IHttpActionResult GetSectionId(int networkID, int brKey)
        {
            int sectionId = section.GetSectionId(networkID, brKey, db);
            return (sectionId > 0) ? (IHttpActionResult)Ok(sectionId) : NotFound();
        }

        [ModelValidation("Function call not valid")]
        [Route("api/GetBrKey/{networkID}/{SectionID}")]
        [HttpGet]
        public IHttpActionResult GetBrKey(int networkID, int sectionId)
        {
            int brKey = section.GetBrKey(networkID, sectionId, db);
            return (brKey > 0) ? (IHttpActionResult)Ok(brKey) : NotFound();
        }
    }
}