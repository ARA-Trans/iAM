using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace BridgeCare.Controllers
{
    public class NetworksController : ApiController
    {
        private readonly INetwork repo;
        private readonly BridgeCareContext db;

        public NetworksController(INetwork repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        [HttpGet]
        [Route("api/GetNetworks")]
        public IHttpActionResult GetNetworks() => Ok(repo.GetAllNetworks(db));
    }
}