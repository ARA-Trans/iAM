using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    using SimulationGetMethod = Func<UserInformationModel, List<SimulationModel>>;
    using SimulationUpdateMethod = Action<SimulationModel, UserInformationModel>;
    using SimulationRunMethod = Func<SimulationModel, UserInformationModel, Task<string>>;
    using SimulationDeletionMethod = Action<int, UserInformationModel>;
    using SimulationUserUpdateMethod = Action<int, List<SimulationUserModel>, UserInformationModel>;
    using SimulationCloneMethod = Action<int>;

    public class SimulationController : ApiController
    {
        private readonly ISimulation repo;
        private readonly BridgeCareContext db;
        /// <summary>Maps user roles to methods for fetching simulations</summary>
        private readonly IReadOnlyDictionary<string, SimulationGetMethod> SimulationGetMethods;
        /// <summary>Maps user roles to methods for updating simulations</summary>
        private readonly IReadOnlyDictionary<string, SimulationUpdateMethod> SimulationUpdateMethods;
        /// <summary>Maps user roles to methods for running simulations</summary>
        private readonly IReadOnlyDictionary<string, SimulationRunMethod> SimulationRunMethods;
        /// <summary>Maps user roles to methods for deleting simulations</summary>
        private readonly IReadOnlyDictionary<string, SimulationDeletionMethod> SimulationDeletionMethods;
        /// <summary>Maps user roles to methods for setting simulation users</summary>
        private readonly IReadOnlyDictionary<string, SimulationUserUpdateMethod> SimulationUserUpdateMethods;

        public SimulationController(ISimulation repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));

            SimulationGetMethods = CreateGetMethods();
            SimulationUpdateMethods = CreateUpdateMethods();
            SimulationRunMethods = CreateRunMethods();
            SimulationDeletionMethods = CreateDeletionMethods();
            SimulationUserUpdateMethods = CreateUserUpdateMethods();
        }

        /// <summary>
        /// Creates a mapping from user roles to the appropriate methods for getting scenarios
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, SimulationGetMethod> CreateGetMethods()
        {
            List<SimulationModel> GetAllSimulations(UserInformationModel userInformation) =>
                repo.GetSimulations(db);
            List<SimulationModel> GetPermittedSimulations(UserInformationModel userInformation) =>
                repo.GetPermittedSimulations(db, userInformation.Name);

            return new Dictionary<string, SimulationGetMethod>
            {
                [Role.ADMINISTRATOR] = GetAllSimulations,
                [Role.DISTRICT_ENGINEER] = GetAllSimulations,
                [Role.CWOPA] = GetAllSimulations,
                [Role.PLANNING_PARTNER] = GetAllSimulations
            };
        }

        /// <summary>
        /// Creates a mapping from user roles to the appropriate methods for updating scenarios
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, SimulationUpdateMethod> CreateUpdateMethods()
        {
            void UpdateAnySimulation(SimulationModel model, UserInformationModel userInformation) =>
                repo.UpdateAnySimulation(model, db);
            void UpdatePermittedSimulation(SimulationModel model, UserInformationModel userInformation) =>
                repo.UpdatePermittedSimulation(model, db, userInformation.Name);

            return new Dictionary<string, SimulationUpdateMethod>
            {
                [Role.ADMINISTRATOR] = UpdateAnySimulation,
                [Role.DISTRICT_ENGINEER] = UpdatePermittedSimulation,
                [Role.CWOPA] = UpdatePermittedSimulation,
                [Role.PLANNING_PARTNER] = UpdatePermittedSimulation
            };
        }

        private Dictionary<string, SimulationRunMethod> CreateRunMethods()
        {
            Task<string> RunAnySimulation(SimulationModel model, UserInformationModel userInformation) =>
                repo.RunSimulation(model);
            Task<string> RunPermittedSimulation(SimulationModel model, UserInformationModel userInformation) =>
                repo.RunPermittedSimulation(model, db, userInformation.Name);

            return new Dictionary<string, SimulationRunMethod>
            {
                [Role.ADMINISTRATOR] = RunAnySimulation,
                [Role.DISTRICT_ENGINEER] = RunPermittedSimulation,
                [Role.CWOPA] = RunPermittedSimulation,
                [Role.PLANNING_PARTNER] = RunPermittedSimulation
            };
        }

        /// <summary>
        /// Creates a mapping from user roles to the appropriate methods for deleting scenarios
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, SimulationDeletionMethod> CreateDeletionMethods()
        {
            void DeleteAnySimulation(int id, UserInformationModel userInformation) =>
                repo.DeleteAnySimulation(id, db);
            void DeletePermittedSimulation(int id, UserInformationModel userInformation) =>
                repo.DeletePermittedSimulation(id, db, userInformation.Name);

            return new Dictionary<string, SimulationDeletionMethod>
            {
                [Role.ADMINISTRATOR] = DeleteAnySimulation,
                [Role.DISTRICT_ENGINEER] = DeletePermittedSimulation,
                [Role.CWOPA] = DeletePermittedSimulation,
                [Role.PLANNING_PARTNER] = DeletePermittedSimulation
            };
        }

        private Dictionary<string, SimulationUserUpdateMethod> CreateUserUpdateMethods()
        {
            void SetAnySimulationUsers(int id, List<SimulationUserModel> users, UserInformationModel userInformation) =>
                repo.SetAnySimulationUsers(id, users, db);
            void SetPermittedSimulationUsers(int id, List<SimulationUserModel> users, UserInformationModel userInformation) =>
                repo.SetPermittedSimulationUsers(id, users, db, userInformation.Name);

            return new Dictionary<string, SimulationUserUpdateMethod>
            {
                [Role.ADMINISTRATOR] = SetAnySimulationUsers,
                [Role.DISTRICT_ENGINEER] = SetPermittedSimulationUsers,
                [Role.CWOPA] = SetPermittedSimulationUsers,
                [Role.PLANNING_PARTNER] = SetPermittedSimulationUsers
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

        [HttpPost]
        [Route("api/CloneScenario/{id}")]
        [RestrictAccess]
        public IHttpActionResult CloneSimulation(int id)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            return Ok(repo.CloneSimulation(id, db, userInformation.Name));
        }

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
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            var result = await Task.Factory.StartNew(() => SimulationRunMethods[userInformation.Role](model, userInformation));

            if (result.IsCompleted)
                repo.SetSimulationLastRunDate(model.simulationId, db);
            else
                return InternalServerError(new Exception(result.Result));

            return Ok();
        }

        [HttpPost]
        [Route("api/SetScenarioUsers/{id}")]
        public IHttpActionResult SetSimulationUsers(int id, [FromBody]SimulationUserModel[] simulationUsers)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            SimulationUserUpdateMethods[userInformation.Role](id, simulationUsers.ToList(), userInformation);
            return Ok(simulationUsers);
        }
    }
}
