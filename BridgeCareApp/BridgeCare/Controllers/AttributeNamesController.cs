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

namespace BridgeCare.Controllers
{
    public class AttributeNamesController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly IAttributeNames attributes;
        public AttributeNamesController(IAttributeNames att,BridgeCareContext context)
        {
            attributes = att ?? throw new ArgumentNullException(nameof(att));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Get: api/attributenames
        public List<string> Get()
        {
            IQueryable < AttributeNameModel > returned = attributes.GetAttributeNames(db);
            List<string> toreturn = new List<string>();
            foreach (AttributeNameModel nm in returned)
            {
                toreturn.Add(nm.Name);
            }
            return toreturn; 
        }
    }
}