using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace BridgeCare.Controllers
{
    public class AttributeController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly IAttributeNames attributes;

        public AttributeController(IAttributeNames attributeNames, BridgeCareContext context)
        {
            attributes = attributeNames ?? throw new ArgumentNullException(nameof(attributeNames));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        [Route("api/GetAttributes")]
        [HttpGet]
        public List<AttributeModel> GetAttributes() => attributes.GetAttributes(db).ToList();
    }
}