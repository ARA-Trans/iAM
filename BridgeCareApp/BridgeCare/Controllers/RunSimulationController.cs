using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace BridgeCare.Controllers
{
    public class RunSimulationController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly IRunSimulation simulation;

        public RunSimulationController(IRunSimulation simulations, BridgeCareContext context)
        {
            simulation = simulations ?? throw new ArgumentNullException(nameof(simulations));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        // POST: api/RunSimulation
        public async Task<IHttpActionResult> Post([FromBody]SimulationModel data)
        {
            var result = await Task.Factory.StartNew(() => { return simulation.Start(data); });
            if (result.IsCompleted)
            {
                simulation.SetLastRunDate(data.SimulationId, db);
                return Ok(result);
            }
            return NotFound();
        }
    }
}