using System;
using System.Collections.Generic;
using System.Web.Http;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;

namespace BridgeCare.Controllers
{
    using SimulationAnalysisGetMethod = Func<int, UserInformationModel, SimulationAnalysisModel>;
    using SimulationAnalysisUpdateMethod = Action<SimulationAnalysisModel, UserInformationModel>;

    public class SimulationAnalysisController : ApiController
    {
        private readonly BridgeCareContext db;

        private readonly ISimulationAnalysis repo;

        private readonly IReadOnlyDictionary<string, SimulationAnalysisGetMethod> SimulationAnalysisGetMethods;

        private readonly IReadOnlyDictionary<string, SimulationAnalysisUpdateMethod> SimulationAnalysisUpdateMethods;

        /// <summary>
        ///     Creates a mapping from user roles to the appropriate methods for getting simulation analyses
        /// </summary>
        private Dictionary<string, SimulationAnalysisGetMethod> CreateGetMethods()
        {
            SimulationAnalysisModel GetAnySimulationAnalysis(int id, UserInformationModel userInformation) =>
                repo.GetAnySimulationAnalysis(id, db);
            SimulationAnalysisModel GetPermittedSimulationAnalysis(int id, UserInformationModel userInformation) =>
                repo.GetPermittedSimulationAnalysis(id, db, userInformation.Name);

            return new Dictionary<string, SimulationAnalysisGetMethod>
            {
                [Role.ADMINISTRATOR] = GetAnySimulationAnalysis,
                [Role.DISTRICT_ENGINEER] = GetPermittedSimulationAnalysis,
                [Role.CWOPA] = GetPermittedSimulationAnalysis,
                [Role.PLANNING_PARTNER] = GetPermittedSimulationAnalysis
            };
        }

        /// <summary>
        ///     Creates a mapping from user roles to the appropriate methods for updating simulation analyses
        /// </summary>
        private Dictionary<string, SimulationAnalysisUpdateMethod> CreateUpdateMethods()
        {
            void UpdateSimulationAnalysis(SimulationAnalysisModel model, UserInformationModel userInformation) =>
                repo.UpdateSimulationAnalysis(model, db);
            void PartialUpdatePermittedSimulationAnalysis(SimulationAnalysisModel model, UserInformationModel userInformation) =>
                repo.PartialUpdatePermittedSimulationAnalysis(model, db, userInformation.Name);
            void PartialUpdatePermittedSimulationAnalysisWithoutWeights(SimulationAnalysisModel model, UserInformationModel userInformation) =>
                repo.PartialUpdatePermittedSimulationAnalysis(model, db, userInformation.Name, updateWeighting: false);

            return new Dictionary<string, SimulationAnalysisUpdateMethod>
            {
                [Role.ADMINISTRATOR] = UpdateSimulationAnalysis,
                [Role.DISTRICT_ENGINEER] = PartialUpdatePermittedSimulationAnalysis,
                [Role.CWOPA] = PartialUpdatePermittedSimulationAnalysisWithoutWeights,
                [Role.PLANNING_PARTNER] = PartialUpdatePermittedSimulationAnalysis
            };
        }

        public SimulationAnalysisController(ISimulationAnalysis simulationAnalysis, BridgeCareContext context)
        {
            repo = simulationAnalysis ?? throw new ArgumentNullException(nameof(simulationAnalysis));
            db = context ?? throw new ArgumentNullException(nameof(context));

            SimulationAnalysisGetMethods = CreateGetMethods();
            SimulationAnalysisUpdateMethods = CreateUpdateMethods();
        }

        /// <summary>
        ///     API endpoint for fetching a simulation's analysis data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetScenarioAnalysisData/{id}")]
        [RestrictAccess]
        public IHttpActionResult GetSimulationAnalysis(int id)
        {
            UserInformationModel userInformation = ESECSecurity.GetUserInformation(Request);
            return Ok(SimulationAnalysisGetMethods[userInformation.Role](id, userInformation));
        }

        /// <summary>
        ///     API endpoint for upserting a simulation's analysis data
        /// </summary>
        /// <param name="model">SimulationAnalysisModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveScenarioAnalysisData")]
        [RestrictAccess]
        public IHttpActionResult UpdateSimulationAnalysis([FromBody]SimulationAnalysisModel model)
        {
            UserInformationModel userInformation = ESECSecurity.GetUserInformation(Request);
            SimulationAnalysisUpdateMethods[userInformation.Role](model, userInformation);
            return Ok();
        }
    }
}
