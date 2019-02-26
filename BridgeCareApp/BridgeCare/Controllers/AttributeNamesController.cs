using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace BridgeCare.Controllers
{
    public class AttributeNamesController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly IAttributeNames attributes;

        public AttributeNamesController(IAttributeNames attributeNames, BridgeCareContext context)
        {
            attributes = attributeNames ?? throw new ArgumentNullException(nameof(attributeNames));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Get: api/attributenames
        public List<string> Get()
        {
            IQueryable<AttributeNameModel> returned = attributes.GetAttributeNames(db);
            List<string> toReturn = new List<string>();
            foreach (AttributeNameModel nameModel in returned)
            {
                toReturn.Add(nameModel.Name);
            }
            return toReturn;
        }
    }
}