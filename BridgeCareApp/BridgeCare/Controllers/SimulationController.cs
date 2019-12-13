using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    using SimulationGetMethod = Func<ISimulation, BridgeCareContext, UserInformationModel, List<SimulationModel>>;
    using SimulationUpdateMethod = Action<ISimulation, SimulationModel, BridgeCareContext, UserInformationModel>;
    using SimulationDeletionMethod = Action<ISimulation, int, BridgeCareContext, UserInformationModel>;

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
        /// Maps roles to methods for fetching simulations
        /// </summary>
        private readonly IReadOnlyDictionary<string, SimulationGetMethod> SimulationGetMethods = new Dictionary<string, SimulationGetMethod>
        {
            [Role.ADMINISTRATOR] = (repo, db, userInformation) => repo.GetSimulations(db),
            [Role.DISTRICT_ENGINEER] = (repo, db, userInformation) => repo.GetOwnedSimulations(db, userInformation.Name),
            [Role.CWOPA] = (repo, db, userInformation) => repo.GetSimulations(db),
            [Role.PLANNING_PARTNER] = (repo, db, userInformation) => repo.GetOwnedSimulations(db, userInformation.Name)
        };

        /// <summary>
        /// Maps roles to methods for updating simulations
        /// </summary>
        private readonly IReadOnlyDictionary<string, SimulationUpdateMethod> SimulationUpdateMethods = new Dictionary<string, SimulationUpdateMethod>
        {
            [Role.ADMINISTRATOR] = (repo, model, db, userInformation) => repo.UpdateSimulation(model, db),
            [Role.DISTRICT_ENGINEER] = (repo, model, db, userInformation) => repo.UpdateOwnedSimulation(model, db, userInformation),
            [Role.CWOPA] = (repo, model, db, userInformation) => repo.UpdateOwnedSimulation(model, db, userInformation),
            [Role.PLANNING_PARTNER] = (repo, model, db, userInformation) => repo.UpdateOwnedSimulation(model, db, userInformation)
        };

        /// <summary>
        /// Maps roles to methods for deleting simulations
        /// </summary>
        private readonly IReadOnlyDictionary<string, SimulationDeletionMethod> SimulationDeleteMethods = new Dictionary<string, SimulationDeletionMethod>
        {
            [Role.ADMINISTRATOR] = (repo, id, db, userInformation) => repo.DeleteSimulation(id, db),
            [Role.DISTRICT_ENGINEER] = (repo, id, db, userInformation) => repo.DeleteOwnedSimulation(id, db, userInformation),
            [Role.CWOPA] = (repo, id, db, userInformation) => repo.DeleteOwnedSimulation(id, db, userInformation),
            [Role.PLANNING_PARTNER] = (repo, id, db, userInformation) => repo.DeleteOwnedSimulation(id, db, userInformation)
        };

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
            return Ok(SimulationGetMethods[userInformation.Role](repo, db, userInformation));
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
        [RestrictAccess]
        public IHttpActionResult UpdateSimulation([FromBody]SimulationModel model)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            SimulationUpdateMethods[userInformation.Role](repo, model, db, userInformation);
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
        [RestrictAccess]
        public IHttpActionResult DeleteSimulation(int id)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            SimulationDeleteMethods[userInformation.Role](repo, id, db, userInformation);
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