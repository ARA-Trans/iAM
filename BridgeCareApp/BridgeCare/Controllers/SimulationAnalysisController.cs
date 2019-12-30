using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace BridgeCare.Controllers
{
    using SimulationAnalysisGetMethod = Func<int, UserInformationModel, SimulationAnalysisModel>;
    using SimulationAnalysisUpdateMethod = Action<SimulationAnalysisModel, UserInformationModel>;

    public class SimulationAnalysisController : ApiController
    {
        private readonly ISimulationAnalysis repo;
        private readonly BridgeCareContext db;
        private readonly IReadOnlyDictionary<string, SimulationAnalysisGetMethod> SimulationAnalysisGetMethods;
        private readonly IReadOnlyDictionary<string, SimulationAnalysisUpdateMethod> SimulationAnalysisUpdateMethods;

        public SimulationAnalysisController(ISimulationAnalysis simulationAnalysis, BridgeCareContext context)
        {
            repo = simulationAnalysis ?? throw new ArgumentNullException(nameof(simulationAnalysis));
            db = context ?? throw new ArgumentNullException(nameof(context));

            SimulationAnalysisGetMethods = CreateGetMethods();
            SimulationAnalysisUpdateMethods = CreateUpdateMethods();
        }

        /// <summary>
        /// Creates a mapping from user roles to the appropriate methods for getting simulation analyses
        /// </summary>
        private Dictionary<string, SimulationAnalysisGetMethod> CreateGetMethods()
        {
            SimulationAnalysisModel GetAnySimulationAnalysis(int id, UserInformationModel userInformation) =>
                repo.GetAnySimulationAnalysis(id, db);
            SimulationAnalysisModel GetOwnedSimulationAnalysis(int id, UserInformationModel userInformation) =>
                repo.GetOwnedSimulationAnalysis(id, db, userInformation.Name);

            return new Dictionary<string, SimulationAnalysisGetMethod>
            {
                [Role.ADMINISTRATOR] = GetAnySimulationAnalysis,
                [Role.DISTRICT_ENGINEER] = GetOwnedSimulationAnalysis,
                [Role.CWOPA] = GetOwnedSimulationAnalysis,
                [Role.PLANNING_PARTNER] = GetOwnedSimulationAnalysis
            };
        }

        /// <summary>
        /// Creates a mapping from user roles to the appropriate methods for updating simulation analyses
        /// </summary>
        private Dictionary<string, SimulationAnalysisUpdateMethod> CreateUpdateMethods()
        {
            void UpdateSimulationAnalysis(SimulationAnalysisModel model, UserInformationModel userInformation) =>
                repo.UpdateSimulationAnalysis(model, db);
            void PartialUpdateOwnedSimulationAnalysis(SimulationAnalysisModel model, UserInformationModel userInformation) =>
                repo.PartialUpdateOwnedSimulationAnalysis(model, db, userInformation.Name);
            void PartialUpdateOwnedSimulationAnalysisWithoutWeights(SimulationAnalysisModel model, UserInformationModel userInformation) =>
                repo.PartialUpdateOwnedSimulationAnalysis(model, db, userInformation.Name, updateWeighting: false);

            return new Dictionary<string, SimulationAnalysisUpdateMethod>
            {
                [Role.ADMINISTRATOR] = UpdateSimulationAnalysis,
                [Role.DISTRICT_ENGINEER] = PartialUpdateOwnedSimulationAnalysis,
                [Role.CWOPA] = PartialUpdateOwnedSimulationAnalysisWithoutWeights,
                [Role.PLANNING_PARTNER] = PartialUpdateOwnedSimulationAnalysis
            };
        }

        /// <summary>
        /// API endpoint for fetching a simulation's analysis data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetScenarioAnalysisData/{id}")]
        [RestrictAccess]
        public IHttpActionResult GetSimulationAnalysis(int id)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            return Ok(SimulationAnalysisGetMethods[userInformation.Role](id, userInformation));
        }

        /// <summary>
        /// API endpoint for upserting a simulation's analysis data
        /// </summary>
        /// <param name="model">SimulationAnalysisModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveScenarioAnalysisData")]
        [RestrictAccess]
        public IHttpActionResult UpdateSimulationAnalysis([FromBody]SimulationAnalysisModel model)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            SimulationAnalysisUpdateMethods[userInformation.Role](model, userInformation);
            return Ok();
        }
    }
}