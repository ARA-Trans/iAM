using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Linq;
using System.Web.Http;

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

        public IQueryable<SectionModel> Get(NetworkModel data)
        {
            return section.GetSections(data, db);
        }
    }
}