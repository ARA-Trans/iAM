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
    using SimulationGetMethod = Func<UserInformationModel, List<SimulationModel>>;
    using SimulationUpdateMethod = Action<SimulationModel, UserInformationModel>;
    using SimulationDeletionMethod = Action<int, UserInformationModel>;

    public class SimulationController : ApiController
    {
        private readonly ISimulation repo;
        private readonly BridgeCareContext db;
        /// <summary>Maps user roles to methods for fetching simulations</summary>
        private readonly IReadOnlyDictionary<string, SimulationGetMethod> SimulationGetMethods;
        /// <summary>Maps user roles to methods for updating simulations</summary>
        private readonly IReadOnlyDictionary<string, SimulationUpdateMethod> SimulationUpdateMethods;
        /// <summary>Maps user roles to methods for deleting simulations</summary>
        private readonly IReadOnlyDictionary<string, SimulationDeletionMethod> SimulationDeletionMethods;

        public SimulationController(ISimulation repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            
            SimulationGetMethods = new Dictionary<string, SimulationGetMethod>
            {
                [Role.ADMINISTRATOR] = (userInformation) => repo.GetSimulations(db),
                [Role.DISTRICT_ENGINEER] = (userInformation) => repo.GetOwnedSimulations(db, userInformation.Name),
                [Role.CWOPA] = (userInformation) => repo.GetSimulations(db),
                [Role.PLANNING_PARTNER] = (userInformation) => repo.GetOwnedSimulations(db, userInformation.Name)
            };
            SimulationUpdateMethods = new Dictionary<string, SimulationUpdateMethod>
            {
                [Role.ADMINISTRATOR] = (model, userInformation) => repo.UpdateSimulation(model, db),
                [Role.DISTRICT_ENGINEER] = (model, userInformation) => repo.UpdateOwnedSimulation(model, db, userInformation),
                [Role.CWOPA] = (model, userInformation) => repo.UpdateOwnedSimulation(model, db, userInformation),
                [Role.PLANNING_PARTNER] = (model, userInformation) => repo.UpdateOwnedSimulation(model, db, userInformation)
            };

            SimulationDeletionMethods = new Dictionary<string, SimulationDeletionMethod>
            {
                [Role.ADMINISTRATOR] = (id, userInformation) => repo.DeleteSimulation(id, db),
                [Role.DISTRICT_ENGINEER] = (id, userInformation) => repo.DeleteOwnedSimulation(id, db, userInformation),
                [Role.CWOPA] = (id, userInformation) => repo.DeleteOwnedSimulation(id, db, userInformation),
                [Role.PLANNING_PARTNER] = (id, userInformation) => repo.DeleteOwnedSimulation(id, db, userInformation)
            };
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
            return Ok(SimulationGetMethods[userInformation.Role](userInformation));
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
            SimulationUpdateMethods[userInformation.Role](model, userInformation);
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
            SimulationDeletionMethods[userInformation.Role](id, userInformation);
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