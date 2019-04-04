using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    public class TreatmentController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly ITreatment treatment;

        public TreatmentController(IPerformance performanceInterface, BridgeCareContext context)
        {
            treatment = performanceInterface ?? throw new ArgumentNullException(nameof(performanceInterface));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Get: api/performance
        /// </summary>
        [ModelValidation("Given simulation data is not valid")]
        [HttpGet]
        public IQueryable<PerformanceScenarioModel> Get(SimulationModel data) => performance.GetPerformance(data, db);

        [ModelValidation("Given performance scenario data is not valid")]
        [Route("api/UpdatePerformance")]
        [HttpPost]
        public IHttpActionResult Post([FromBody]PerformanceScenarioModel data)
        {
            treatment.UpdatePerformanceScenario(data, db);
            return Ok();
        }
    }
}