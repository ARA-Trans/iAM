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
    //retrieves all attributes year and value pairsfor give sectionId 
    public class HistoricalAttributesByYearController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly IAttributeByYear attributes;
        public HistoricalAttributesByYearController(IAttributeByYear aby, BridgeCareContext context)
        {
            attributes = aby ?? throw new ArgumentNullException(nameof(aby));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Get: api/historicalattributesbyyear/
        public List<AttributeByYearModel> Get(SectionModel sm)
        {    
            if (sm == null)
                return null;

            List <AttributeByYearModel> returnValues  = attributes.GetHistoricalAttributes(sm.NetworkId, sm.SectionId, db);
           
            return returnValues;
        }


    }
}