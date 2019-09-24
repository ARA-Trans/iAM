using System;
using System.Web.Http;
using System.Web.Http.Filters;
using BridgeCare.Interfaces;
using BridgeCare.Models;

namespace BridgeCare.Controllers
{
    public class RemainingLifeLimitController : ApiController
    {
        private readonly IRemainingLifeLimit repo;
        private readonly BridgeCareContext db;

        public RemainingLifeLimitController(IRemainingLifeLimit repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// API endpoint for fetching a simulation's remaining life limit library data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetScenarioRemainingLifeLimitLibrary/{id}")]
        [ModelValidation("The scenario id is invalid.")]
        public IHttpActionResult GetSimulationRemainingLifeLimitLibrary(int id) =>
            Ok(repo.GetSimulationRemainingLifeLimitLibrary(id, db));

        /// <summary>
        /// API endpoint for upserting/deleting a simulation's remaining life limit library data
        /// </summary>
        /// <param name="model">RemainingLifeLimitLibraryModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveScenarioRemainingLifeLimitLibrary")]
        public IHttpActionResult SaveSimulationRemainingLifeLimitLibrary([FromBody]RemainingLifeLimitLibraryModel model) =>
            Ok(repo.SaveSimulationRemainingLifeLimitLibrary(model, db));
    }
}