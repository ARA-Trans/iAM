using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace BridgeCare.Controllers
{
    public class ProjectedAttributesByYearController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly IAttributeByYear attributes;

        public ProjectedAttributesByYearController(IAttributeByYear aby, BridgeCareContext context)
        {
            attributes = aby ?? throw new ArgumentNullException(nameof(aby));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        // usage
        // Get: api/projectedattributesbyyear/
        // body (example)
        // {
        //"sectionId":1000012,
        //"networkId":13,
        //"simulationId":9
        // }
        public List<AttributeByYearModel> Get(SimulatedSegmentAddressModel segmentAddressModel)
        {
            return attributes.GetProjectedAttributes(segmentAddressModel, db);
        }
    }
}