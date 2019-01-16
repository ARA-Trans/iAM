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
using BridgeCareCodeFirst;
using BridgeCareCodeFirst.Models;
using BridgeCareCodeFirst.Services;
using BridgeCareCodeFirst.EntityClasses;

namespace BridgeCareCodeFirst.Controllers
{
    public class SimulationsController : ApiController
    {
        private BridgeCareContext db = new BridgeCareContext();
        private SimulationRepository simulationRepository = new SimulationRepository();

        // GET: api/Simulations
        public IQueryable<SimulationModel> GetSIMULATIONS()
        {
            return simulationRepository.GetAllSimulations();
        }

        // GET: api/Simulations/5
        [ResponseType(typeof(SIMULATION))]
        public IHttpActionResult GetSIMULATION(int id)
        {
            // this `id` is network id
            var getResults = simulationRepository.GetSelectedSimulation(id);
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

        private bool SIMULATIONExists(int id)
        {
            return db.SIMULATIONS.Count(e => e.SIMULATIONID == id) > 0;
        }
    }
}