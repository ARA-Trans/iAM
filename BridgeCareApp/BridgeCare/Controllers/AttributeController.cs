using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Http;
using BridgeCare.ApplicationLog;

namespace BridgeCare.Controllers
{
    public class AttributeController : ApiController
    {
        private readonly IAttributeRepo repo;
        private readonly BridgeCareContext db;

        public AttributeController(IAttributeRepo repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        [HttpGet]
        [Route("api/GetAttributes")]
        public IHttpActionResult GetAttributes() => Ok(repo.GetAttributes(db));
    }
}