using BridgeCare.Interfaces;
using System;
using System.Web.Http;

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

        /// <summary>
        /// API endpoint for fetching all networks
        /// </summary>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetNetworks")]
        public IHttpActionResult GetNetworks() => Ok(repo.GetAllNetworks(db));
    }
}