using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Web.Http;

namespace BridgeCare.Controllers
{
    public class SimulationAnalysisController : ApiController
    {
        private readonly ISimulationAnalysis repo;
        private readonly BridgeCareContext db;

        public SimulationAnalysisController(ISimulationAnalysis simulationAnalysis, BridgeCareContext context)
        {
            repo = simulationAnalysis ?? throw new ArgumentNullException(nameof(simulationAnalysis));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// API endpoint for fetching a simulation's analysis data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetScenarioAnalysisData/{id}")]
        [Filters.RestrictAccess]
        public IHttpActionResult GetSimulationAnalysis(int id) =>
            Ok(repo.GetSimulationAnalysis(id, db));

        /// <summary>
        /// API endpoint for upserting a simulation's analysis data
        /// </summary>
        /// <param name="model">SimulationAnalysisModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveScenarioAnalysisData")]
        [Filters.RestrictAccess("PD-BAMS-Administrator", "PD-BAMS-DBEngineer")]
        public IHttpActionResult UpdateSimulationAnalysis([FromBody]SimulationAnalysisModel model)
        {
            repo.UpdateSimulationAnalysis(model, db);
            return Ok();
        }
    }
}