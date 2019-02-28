using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

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

        // Get: api/attributeNames
        [HttpGet]
        public HttpResponseMessage<List<AttributeNameModel>> Get() => Ok(attributes.GetAttributeNames(db).ToList());
        
    }
}