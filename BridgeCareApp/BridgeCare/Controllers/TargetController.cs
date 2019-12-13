using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Filters;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;

namespace BridgeCare.Controllers
{
    using TargetLibraryGetMethod = Func<int, UserInformationModel, TargetLibraryModel>;
    using TargetLibrarySaveMethod = Func<TargetLibraryModel, UserInformationModel, TargetLibraryModel>;

    public class TargetController: ApiController
    {
        private readonly ITarget repo;
        private readonly BridgeCareContext db;
        /// <summary>Maps user roles to methods for fetching a target library</summary>
        private readonly IReadOnlyDictionary<string, TargetLibraryGetMethod> TargetLibraryGetMethods;
        /// <summary>Maps user roles to methods for saving a target library</summary>
        private readonly IReadOnlyDictionary<string, TargetLibrarySaveMethod> TargetLibrarySaveMethods;

        public TargetController() { }

        public TargetController(ITarget repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));

            TargetLibraryGetMethods = new Dictionary<string, TargetLibraryGetMethod>
            {
                [Role.ADMINISTRATOR] = (id, userInformation) => repo.GetSimulationTargetLibrary(id, db),
                [Role.DISTRICT_ENGINEER] = (id, userInformation) => repo.GetOwnedSimulationTargetLibrary(id, db, userInformation.Name),
                [Role.CWOPA] = (id, userInformation) => repo.GetOwnedSimulationTargetLibrary(id, db, userInformation.Name),
                [Role.PLANNING_PARTNER] = (id, userInformation) => repo.GetOwnedSimulationTargetLibrary(id, db, userInformation.Name)
            };
            TargetLibrarySaveMethods = new Dictionary<string, TargetLibrarySaveMethod>
            {
                [Role.ADMINISTRATOR] = (model, userInformation) => repo.SaveSimulationTargetLibrary(model, db),
                [Role.DISTRICT_ENGINEER] = (model, userInformation) => repo.SaveOwnedSimulationTargetLibrary(model, db, userInformation.Name),
                [Role.CWOPA] = (model, userInformation) => repo.SaveOwnedSimulationTargetLibrary(model, db, userInformation.Name),
                [Role.PLANNING_PARTNER] = (model, userInformation) => repo.SaveOwnedSimulationTargetLibrary(model, db, userInformation.Name)
            };
        }

        /// <summary>
        /// API endpoint for fetching a simulation's target library data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetScenarioTargetLibrary/{id}")]
        [ModelValidation("The scenario id is invalid.")]
        [RestrictAccess]
        public IHttpActionResult GetSimulationTargetLibrary(int id)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            return Ok(TargetLibraryGetMethods[userInformation.Role](id, userInformation));
        }

        /// <summary>
        /// API endpoint for upserting/deleting a simulation's target library data
        /// </summary>
        /// <param name="model">TargetLibraryModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveScenarioTargetLibrary")]
        [ModelValidation("The target data is invalid.")]
        [RestrictAccess]
        public IHttpActionResult SaveSimulationTargetLibrary([FromBody]TargetLibraryModel model)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            return Ok(TargetLibrarySaveMethods[userInformation.Role](model, userInformation));
        }
    }
}