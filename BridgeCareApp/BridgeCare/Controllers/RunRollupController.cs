using BridgeCare.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BridgeCare.Models;

namespace BridgeCare.Controllers
{
    public class RunRollupController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly IRunRollup rollup;

        public RunRollupController(IRunRollup rollupNetwork, BridgeCareContext context)
        {
            rollup = rollupNetwork ?? throw new ArgumentNullException(nameof(rollupNetwork));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        // POST: api/RunRollup
        public async Task<IHttpActionResult> Post([FromBody]SimulationModel data)
        {
            var result = await Task.Factory.StartNew(() => rollup.Start(data));
            if (result.IsCompleted)
            {
                rollup.SetLastRunDate(data.SimulationId, db);
                return Ok();
            }
            return NotFound();
        }
    }
}
