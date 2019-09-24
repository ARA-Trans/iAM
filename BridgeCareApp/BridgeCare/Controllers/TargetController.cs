using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Filters;
using BridgeCare.Interfaces;
using BridgeCare.Models;

namespace BridgeCare.Controllers
{
    public class TargetController: ApiController
    {
        private readonly ITarget repo;
        private readonly BridgeCareContext db;

        public TargetController() { }

        public TargetController(ITarget repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// API endpoint for fetching a simulation's target library data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetScenarioTargetLibrary/{id}")]
        [ModelValidation("The scenario id is invalid.")]
        public IHttpActionResult GetSimulationTargetLibrary(int id) =>
            Ok(repo.GetSimulationTargetLibrary(id, db));

        /// <summary>
        /// API endpoint for upserting/deleting a simulation's target library data
        /// </summary>
        /// <param name="model">TargetLibraryModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveScenarioTargetLibrary")]
        [ModelValidation("The target data is invalid.")]
        public IHttpActionResult SaveSimulationTargetLibrary([FromBody]TargetLibraryModel model) => 
            Ok(repo.SaveSimulationTargetLibrary(model, db));
    }
}