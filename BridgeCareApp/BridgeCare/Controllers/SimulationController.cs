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

            List<SimulationModel> GetAllSimulations(UserInformationModel userInformation) => 
                repo.GetSimulations(db);
            List<SimulationModel> GetOwnedSimulations(UserInformationModel userInformation) => 
                repo.GetOwnedSimulations(db, userInformation.Name);

            void UpdateAnySimulation(SimulationModel model, UserInformationModel userInformation) => 
                repo.UpdateSimulation(model, db);
            void UpdateOwnedSimulation(SimulationModel model, UserInformationModel userInformation) => 
                repo.UpdateOwnedSimulation(model, db, userInformation);

            void DeleteAnySimulation(int id, UserInformationModel userInformation) => 
                repo.DeleteSimulation(id, db);
            void DeleteOwnedSimulation(int id, UserInformationModel userInformation) => 
                repo.DeleteOwnedSimulation(id, db, userInformation);

            SimulationGetMethods = new Dictionary<string, SimulationGetMethod>
            {
                [Role.ADMINISTRATOR] = GetAllSimulations,
                [Role.DISTRICT_ENGINEER] = GetOwnedSimulations,
                [Role.CWOPA] = GetAllSimulations,
                [Role.PLANNING_PARTNER] = GetOwnedSimulations
            };
            SimulationUpdateMethods = new Dictionary<string, SimulationUpdateMethod>
            {
                [Role.ADMINISTRATOR] = UpdateAnySimulation,
                [Role.DISTRICT_ENGINEER] = UpdateOwnedSimulation,
                [Role.CWOPA] = UpdateOwnedSimulation,
                [Role.PLANNING_PARTNER] = UpdateOwnedSimulation
            };
            SimulationDeletionMethods = new Dictionary<string, SimulationDeletionMethod>
            {
                [Role.ADMINISTRATOR] = DeleteAnySimulation,
                [Role.DISTRICT_ENGINEER] = DeleteOwnedSimulation,
                [Role.CWOPA] = DeleteOwnedSimulation,
                [Role.PLANNING_PARTNER] = DeleteOwnedSimulation
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