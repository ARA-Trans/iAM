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

        // POST: api/sections
        //[HttpPost]
        public IQueryable<SectionModel> Get(NetworkModel data)
        {
            return section.GetSections(data,db);
        }
    }
}
