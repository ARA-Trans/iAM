﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using BridgeCare.Models;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;

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
        public IQueryable<SimulationResult> GetSIMULATIONS()
        {
            return simulations.GetAllSimulations();
        }

        // GET: api/Simulations/5
        [ResponseType(typeof(SIMULATION))]
        public IHttpActionResult GetSIMULATION(int id)
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

        private bool SIMULATIONExists(int id)
        {
            return db.SIMULATIONS.Count(e => e.SIMULATIONID == id) > 0;
        }
    }
}