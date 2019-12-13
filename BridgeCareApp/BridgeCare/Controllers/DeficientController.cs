using System;
using System.Web.Http;
using System.Web.Http.Filters;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;

namespace BridgeCare.Controllers
{
    public class DeficientController: ApiController
    {
        private readonly IDeficient repo;
        private readonly BridgeCareContext db;

        public DeficientController() { }

        public DeficientController(IDeficient repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// API endpoint for fetching a simulation's deficient library data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetScenarioDeficientLibrary/{id}")]
        [ModelValidation("The scenario id is invalid.")]
        [RestrictAccess]
        public IHttpActionResult GetSimulationDeficientLibrary(int id)
            => Ok(repo.GetSimulationDeficientLibrary(id, db));

        /// <summary>
        /// API endpoint for upserting/deleting a simulation's deficient library data
        /// </summary>
        /// <param name="model">DeficientLibraryModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveScenarioDeficientLibrary")]
        [ModelValidation("The deficient data is invalid.")]
        [RestrictAccess(Role.ADMINISTRATOR, Role.DISTRICT_ENGINEER)]
        public IHttpActionResult SaveSimulationDeficientLibrary([FromBody] DeficientLibraryModel model)
            => Ok(repo.SaveSimulationDeficientLibrary(model, db));
    }
}