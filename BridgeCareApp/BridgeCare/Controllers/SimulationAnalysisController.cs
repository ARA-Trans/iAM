using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Web.Http;

namespace BridgeCare.Controllers
{
    public class SimulationAnalysisController : ApiController
    {
        private readonly BridgeCareContext db;
        private ISimulationAnalysis analysis;

        public SimulationAnalysisController(ISimulationAnalysis simulationAnalysis, BridgeCareContext context)
        {
            analysis = simulationAnalysis ?? throw new ArgumentNullException(nameof(simulationAnalysis));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        [Route("api/GetScenarioAnalysisData/{simulationId}")]
        [HttpGet]
        public SimulationAnalysisModel GetSimulationAnalysis(int SimulationId) => analysis.GetSimulationAnalysis(SimulationId, db);

        [Route("api/SaveScenarioAnalysisData")]
        [HttpPost]
        public IHttpActionResult UpdateSimulationAnalysis([FromBody]SimulationAnalysisModel model)
        {
            analysis.UpdateSimulationAnalysis(model, db);
            return Ok();
        }
    }
}