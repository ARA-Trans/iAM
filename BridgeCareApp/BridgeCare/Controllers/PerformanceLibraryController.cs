using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    public class PerformanceLibraryController : ApiController
    {
        private readonly IPerformanceLibrary repo;
        private readonly BridgeCareContext db;

        public PerformanceLibraryController(IPerformanceLibrary repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// API endpoint for fetching a simulation's performance library data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetScenarioPerformanceLibrary/{id}")]
        [ModelValidation("The scenario id is invalid.")]
        [Filters.RestrictAccess]
        public IHttpActionResult GetSimulationPerformanceLibrary(int id) =>
            Ok(repo.GetSimulationPerformanceLibrary(id, db));

        /// <summary>
        /// API endpoint for upserting/deleting a simulation's performance library data
        /// </summary>
        /// <param name="model">PerformanceLibraryModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveScenarioPerformanceLibrary")]
        [ModelValidation("The performance data is invalid.")]
        [Filters.RestrictAccess("PD-BAMS-Administrator", "PD-BAMS-DBEngineer")]
        public IHttpActionResult SaveSimulationPerformanceLibrary([FromBody]PerformanceLibraryModel model)
        {
            var performanceLibraryModel = repo.SaveSimulationPerformanceLibrary(model, db);
            return Ok(performanceLibraryModel);
        }
    }
}