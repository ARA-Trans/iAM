using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using BridgeCare;
using BridgeCare.Models;
using BridgeCare.Services;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.DataAccessLayer;
using BridgeCare.ApplicationLog;

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

        // Get: api/projectedattributesbyyear/
        public List<AttributeByYearModel> Get(SimulatedSegmentAddressModel sm)
        {    
            if (sm == null)
                return null;

            List<AttributeByYearModel> returnValues  = attributes.GetProjectedAttributes(sm.SimulationId, sm.NetworkId, sm.SectionId, db);

            return returnValues;
        }


    }
}