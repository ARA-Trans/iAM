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
        [ResponseType(typeof(NETWORK))]
        public IHttpActionResult GetNetwork(int id)
        {
            NETWORK network = db.NETWORKS.Find(id);
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