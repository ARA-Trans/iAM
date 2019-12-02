using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    public class PriorityController : ApiController
    {
        private readonly IPriority repo;
        private readonly BridgeCareContext db;

        public PriorityController() { }

        public PriorityController(IPriority repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// API endpoint for fetching a simulation's priority library data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetScenarioPriorityLibrary/{id}")]
        [ModelValidation("The scenario id is invalid.")]
        [Filters.RestrictAccess("PD-BAMS-PlanningPartner")]
        public IHttpActionResult GetSimulationPriorityLibrary(int id) =>
            Ok(repo.GetSimulationPriorityLibrary(id, db));

        /// <summary>
        /// API endpoint for upserting/deleting a simulation's priority library data
        /// </summary>
        /// <param name="model">PriorityLibraryModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveScenarioPriorityLibrary")]
        [ModelValidation("The priority data is invalid.")]
        [Filters.RestrictAccess("admin")]
        public IHttpActionResult SaveSimulationPriorityLibrary([FromBody]PriorityLibraryModel model) =>
            Ok(repo.SaveSimulationPriorityLibrary(model, db));
    }
}