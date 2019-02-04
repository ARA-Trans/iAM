using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
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
        public void Post([FromBody]SimulationModel data)
        {
            try
            {
                simulation.Start(data);
            }
            catch(Exception ex)
            {
                HandleException.GeneralError(ex);
            }
        }
    }
}
