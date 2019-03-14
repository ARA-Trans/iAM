using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace BridgeCare.Controllers
{
    public class RunSimulationController : ApiController
    {
        private readonly IRunSimulation simulation;

        public RunSimulationController(IRunSimulation simulations)
        {
            simulation = simulations ?? throw new ArgumentNullException(nameof(simulations));
        }

        // POST: api/RunSimulation
        public async Task<IHttpActionResult> Post([FromBody]SimulationModel data)
        {
            var result = await Task.Factory.StartNew(() => { return simulation.Start(data); });
            if (result.IsCompleted)
            {
                return Ok(result);
            }
            return NotFound();
        }
    }
}