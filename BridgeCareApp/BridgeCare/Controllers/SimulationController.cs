using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    public class SimulationController : ApiController
    {
        private readonly ISimulation repo;
        private readonly BridgeCareContext db;

        public SimulationController(ISimulation repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// API endpoint for fetching all simulations
        /// </summary>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetScenarios")]
        [RestrictAccess]
        public IHttpActionResult GetSimulations()
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            if (userInformation.Role == Role.ADMINISTRATOR || userInformation.Role == Role.CWOPA)
            {
                return Ok(repo.GetSimulations(db));
            } else
            {
                return Ok(repo.GetSimulations(db, userInformation));
            }
        }

        /// <summary>
        /// API endpoint for creating a simulation
        /// </summary>
        /// <param name="model">CreateSimulationDataModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/CreateScenario")]
        [ModelValidation("The scenario data is invalid.")]
        [RestrictAccess]
        public IHttpActionResult CreateSimulation([FromBody]CreateSimulationDataModel model) =>
            Ok(repo.CreateSimulation(model, db));

        /// <summary>
        /// API endpoint for updating a simulation
        /// </summary>
        /// <param name="model">SimulationModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/UpdateScenario")]
        [ModelValidation("The scenario data is invalid.")]
        [RestrictAccess(Role.ADMINISTRATOR, Role.DISTRICT_ENGINEER)]
        public IHttpActionResult UpdateSimulation([FromBody]SimulationModel model)
        {
            repo.UpdateSimulation(model, db);
            return Ok();
        }

        /// <summary>
        /// API endpoint for deleting a simulation
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpDelete]
        [Route("api/DeleteScenario/{id}")]
        [ModelValidation("The scenario data is invalid.")]
        [RestrictAccess(Role.ADMINISTRATOR, Role.DISTRICT_ENGINEER)]
        public IHttpActionResult DeleteSimulation(int id)
        {
            repo.DeleteSimulation(id, db);
            return Ok();
        }

        /// <summary>
        /// API endpoint for running a simulation
        /// </summary>
        /// <param name="model">SimulationModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/RunSimulation")]
        [RestrictAccess]
        public async Task<IHttpActionResult> RunSimulation([FromBody]SimulationModel model)
        {
            var result = await Task.Factory.StartNew(() => repo.RunSimulation(model));

            if (result.IsCompleted)
                repo.SetSimulationLastRunDate(model.SimulationId, db);
            else
                return InternalServerError(new Exception(result.Result));

            return Ok();
        }
    }
}