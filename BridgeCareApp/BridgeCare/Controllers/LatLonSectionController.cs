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
    public class LatLonSectionController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly ILatLonSection latlon;

        public LatLonSectionController(ILatLonSection lls,BridgeCareContext context)
        {
            latlon = lls ?? throw new ArgumentNullException(nameof(lls));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Get: api/latlonsection
        public IQueryable<LatLonSectionModel> Get(SectionModel sm)
        {
            return latlon.GetLatLon(sm.NetworkId,db);
        }
    }
}
