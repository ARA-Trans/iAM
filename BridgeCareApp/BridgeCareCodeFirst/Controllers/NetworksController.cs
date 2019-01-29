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
        public IQueryable<Network> GetNETWORKS()
        {
            return networks.GetAllNetworks();
        }

        // GET: api/NETWORKs/5
        [ResponseType(typeof(NETWORK))]
        public IHttpActionResult GetNETWORK(int id)
        {
            NETWORK nETWORK = db.NETWORKS.Find(id);
            if (nETWORK == null)
            {
                return NotFound();
            }

            return Ok(nETWORK);
        }

        // PUT: api/NETWORKs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutNETWORK(int id, NETWORK nETWORK)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != nETWORK.NETWORKID)
            {
                return BadRequest();
            }

            db.Entry(nETWORK).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NETWORKExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/NETWORKs
        [ResponseType(typeof(NETWORK))]
        public IHttpActionResult PostNETWORK(NETWORK nETWORK)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.NETWORKS.Add(nETWORK);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = nETWORK.NETWORKID }, nETWORK);
        }

        // DELETE: api/NETWORKs/5
        [ResponseType(typeof(NETWORK))]
        public IHttpActionResult DeleteNETWORK(int id)
        {
            NETWORK nETWORK = db.NETWORKS.Find(id);
            if (nETWORK == null)
            {
                return NotFound();
            }

            db.NETWORKS.Remove(nETWORK);
            db.SaveChanges();

            return Ok(nETWORK);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NETWORKExists(int id)
        {
            return db.NETWORKS.Count(e => e.NETWORKID == id) > 0;
        }
    }
}