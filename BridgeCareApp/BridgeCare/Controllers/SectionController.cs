using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
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
    }
}