using AspWebApi.Models;
using AspWebApi.Services;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace AspWebApi.Controllers
{
    public class NetworksController : ApiController
    {
        private BridgeCareEntities db = new BridgeCareEntities();

        private NetworkRepository networkRepository = new NetworkRepository();

        // GET: api/Networks
        public IQueryable<NetworkModel> GetNETWORKS()
        {
            return networkRepository.GetAllNetworks();
        }

        // GET: api/Networks/5
        [ResponseType(typeof(NetworkModel))]
        public IHttpActionResult GetNETWORK(int id)
        {
            var nETWORK = db.NETWORKS.Find(id);

            if (nETWORK == null)
            {
                return NotFound();
            }
            return Ok(networkRepository.GetSelectedNetwork(id));
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