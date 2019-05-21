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
        /// Get: api/performance
        /// </summary>
        [ModelValidation("Given simulation data is not valid")]
        [Route("api/GetScenarioPerformanceLibrary/{selectedScenarioId}")]
        [HttpGet]
        public PerformanceLibraryModel Get(int selectedScenarioId) => performanceLibrary.GetScenarioPerformanceLibrary(selectedScenarioId, db);

        [ModelValidation("Given performance scenario data is not valid")]
        [Route("api/SaveScenarioPerformanceLibrary")]
        [HttpPost]
        public IHttpActionResult Post([FromBody]PerformanceLibraryModel data)
        {
            performanceLibrary.SaveScenarioPerformanceLibrary(data, db);
            return Ok();
        }
    }
}