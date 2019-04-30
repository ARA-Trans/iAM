using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    public class SimulationsController : ApiController
    {
        private readonly BridgeCareContext db;
        private ISimulation simulations;

        public SimulationsController(ISimulation simulation, BridgeCareContext context)
        {
            simulations = simulation ?? throw new ArgumentNullException(nameof(simulation));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/Simulations
        public IQueryable<SimulationModel> GetSimulations()
        {
            return simulations.GetAllSimulations();
        }

        // GET: api/Simulations/5
        [ResponseType(typeof(SIMULATION))]
        public IHttpActionResult GetSimulation(int id)
        {
            // this `id` is network id
            var getResults = simulations.GetSelectedSimulation(id);
            if (getResults == null)
            {
                return NotFound();
            }
            return Ok(getResults);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SimulationExists(int id)
        {
            return db.SIMULATIONS.Count(e => e.SIMULATIONID == id) > 0;
        }

        [ModelValidation("Given simulation is not valid")]
        [Route("api/UpdateSimulationName")]
        [HttpPost]
        public IHttpActionResult UpdateSimulationName([FromBody]SimulationModel data)
        {
            simulations.UpdateName(data, db);
            return Ok();
        }

        [Route("api/CreateNewSimulation/{networkID}/{simulationName}")]
        [HttpPost]
        public IHttpActionResult CreateNewSimulation(int networkID, string simulationName)
        {
            int newSimulationId = simulations.CreateNewSimulation(networkID,simulationName, db);
            if (newSimulationId > 0)
                return Ok(newSimulationId);
            else
                return NotFound();
        }
    }
}