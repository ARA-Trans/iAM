using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    public class PerformanceLibraryController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly IPerformanceLibrary performanceLibrary;

        public PerformanceLibraryController(IPerformanceLibrary performanceInterface, BridgeCareContext context)
        {
            performanceLibrary = performanceInterface ?? throw new ArgumentNullException(nameof(performanceInterface));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Get: api/GetScenarioPerformanceLibrary
        /// </summary>
        /// <param name="selectedScenarioId"></param>
        /// <returns>PerformanceLibraryModel</returns>
        [ModelValidation("Given simulation data is not valid")]
        [Route("api/GetScenarioPerformanceLibrary/{selectedScenarioId}")]
        [HttpGet]
        public PerformanceLibraryModel Get(int selectedScenarioId) => performanceLibrary.GetScenarioPerformanceLibrary(selectedScenarioId, db);

        /// <summary>
        /// Post: api/SaveScenarioPerformanceLibrary
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Updated data model</returns>
        [ModelValidation("Given performance scenario data is not valid")]
        [Route("api/SaveScenarioPerformanceLibrary")]
        [HttpPost]
        public IHttpActionResult Post([FromBody]PerformanceLibraryModel data)
        {
            var performanceLibraryModel = performanceLibrary.SaveScenarioPerformanceLibrary(data, db);
            return Ok(performanceLibraryModel);
        }
    }
}