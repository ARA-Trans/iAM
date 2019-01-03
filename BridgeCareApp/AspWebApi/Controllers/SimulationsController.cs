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
using AspWebApi;
using AspWebApi.Models;
using AspWebApi.Services;

namespace AspWebApi.Controllers
{
    public class SimulationsController : ApiController
    {
        private BridgeCareEntities db = new BridgeCareEntities();

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
            //SIMULATION sIMULATION = simulationRepository.FindWithKey(id);
            //if (sIMULATION == null)
            //{
            //    return NotFound();
            //}
            var getResults = simulationRepository.GetSelectedSimulation(id);
            if(getResults == null)
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