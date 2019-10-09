using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    public class TreatmentLibraryController : ApiController
    {
        private readonly ITreatmentLibrary repo;
        private readonly BridgeCareContext db;

        public TreatmentLibraryController(ITreatmentLibrary repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// API endpoint for fetching a simulation's treatment library data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetScenarioTreatmentLibrary/{id}")]
        [ModelValidation("The scenario id is invalid.")]
        public IHttpActionResult GetSimulationTreatmentLibrary(int id) =>
            Ok(repo.GetSimulationTreatmentLibrary(id, db));

        /// <summary>
        /// API endpoint for upserting/deleting a simulation's treatment library data
        /// </summary>
        /// <param name="model">TreatmentLibraryModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveScenarioTreatmentLibrary")]
        public IHttpActionResult SaveSimulationTreatmentLibrary([FromBody]TreatmentLibraryModel model) =>
            Ok(repo.SaveSimulationTreatmentLibrary(model, db));
    }
}