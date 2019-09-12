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

        // GET: api/GetScenarios
        [Route("api/GetScenarios")]
        [HttpGet]
        public IQueryable<SimulationModel> GetSimulations()
        {
            return simulations.GetAllSimulations();
        }

        // GET: api/GetScenario/#
        [ModelValidation("Given scenario id is not valid")]
        [ResponseType(typeof(SimulationEntity))]
        [Route("api/GetScenario/{scenarioId}")]
        [HttpGet]
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
            return db.Simulations.Count(e => e.SIMULATIONID == id) > 0;
        }

        [ModelValidation("Given scenario data is not valid")]
        [Route("api/UpdateScenario")]
        [HttpPost]
        public IHttpActionResult UpdateSimulationName([FromBody]SimulationModel data)
        {
            simulations.UpdateName(data);
            return Ok();
        }

        [ModelValidation("Given scenario id is not valid")]
        [Route("api/DeleteScenario/{scenarioId}")]
        [HttpDelete]
        public IHttpActionResult DeleteSimulation(int scenarioId)
        {
            var rowsAffected = simulations.Delete(scenarioId);
            if(rowsAffected == -1)
            {
                return NotFound();
            }
            return Ok(scenarioId);
        }

        [ModelValidation("Given scenario data is not valid")]
        [Route("api/CreateRunnableScenario")]
        [HttpPost]
        public IHttpActionResult CreateRunnableSimulation([FromBody]CreateSimulationDataModel createSimulationData)
        {
            SimulationModel simulationData = simulations.CreateRunnableSimulation(createSimulationData, db);
            if (simulationData != null && simulationData.SimulationId > 0)
            {
                return Ok(simulationData);
            }
            return NotFound();
        }
    }
}