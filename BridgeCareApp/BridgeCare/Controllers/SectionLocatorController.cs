using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    public class SectionLocatorController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly ISectionLocator SectionLocator;

        public SectionLocatorController(ISectionLocator sectionLocator, BridgeCareContext context)
        {
            SectionLocator = sectionLocator ?? throw new ArgumentNullException(nameof(sectionLocator));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Get: api/SectionLocator
        [ModelValidation("Given section data is not valid")]
        public SectionLocationModel Get(SectionModel sectionModel) => SectionLocator.Locate(sectionModel, db);
    }
}