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
        private readonly BridgeCareContext db;
        private readonly INetwork networks;

        public NetworksController(INetwork networkRepository, BridgeCareContext context)
        {
            networks = networkRepository ?? throw new ArgumentNullException(nameof(networkRepository));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/NETWORKs
        public IQueryable<NetworkModel> GetNetworks()
        {
            return networks.GetAllNetworks();
        }

        // GET: api/NETWORKs/5
        [ResponseType(typeof(NetworkEntity))]
        public IHttpActionResult GetNetwork(int id)
        {
            var network = db.NETWORKS.Find(id);
            if (network == null)
            {
                return NotFound();
            }

            return Ok(network);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NetworkExists(int id)
        {
            return db.NETWORKS.Count(e => e.NETWORKID == id) > 0;
        }
    }
}