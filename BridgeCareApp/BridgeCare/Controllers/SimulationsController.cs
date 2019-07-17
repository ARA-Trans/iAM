﻿using BridgeCare.EntityClasses;
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
        [ResponseType(typeof(SimulationEntity))]
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

        [ModelValidation("Given simulation is not valid")]
        [Route("api/UpdateSimulationName")]
        [HttpPost]
        public IHttpActionResult UpdateSimulationName([FromBody]SimulationModel data)
        {
            simulations.UpdateName(data);
            return Ok();
        }

        [ModelValidation("Given simulation is not valid")]
        [Route("api/DeleteSimulation/{simulationId}")]
        [HttpDelete]
        public IHttpActionResult DeleteSimulation(int simulationId)
        {
            var rowsAffected = simulations.Delete(simulationId);
            if(rowsAffected == -1)
            {
                return NotFound();
            }
            return Ok(simulationId);
        }

        [Route("api/CreateNewSimulation")]
        [HttpPost]
        public IHttpActionResult CreateNewSimulation([FromBody]CreateSimulationDataModel createSimulationData)
        {
            SimulationModel simulationData = simulations.CreateNewSimulation(createSimulationData, db);
            if (simulationData != null && simulationData.SimulationId > 0)
            {
              return Ok(simulationData);
            }
            return NotFound();
        }

        [Route("api/CreateRunnableSimulation")]
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