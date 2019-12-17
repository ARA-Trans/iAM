using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    using PerformanceLibraryGetMethod = Func<int, UserInformationModel, PerformanceLibraryModel>;
    using PerformanceLibrarySaveMethod = Func<PerformanceLibraryModel, UserInformationModel, PerformanceLibraryModel>;

    public class PerformanceLibraryController : ApiController
    {
        private readonly IPerformanceLibrary repo;
        private readonly BridgeCareContext db;
        /// <summary>Maps user roles to methods for getting performance libraries.</summary>
        private readonly IReadOnlyDictionary<string, PerformanceLibraryGetMethod> PerformanceLibraryGetMethods;
        /// <summary>Maps user roles to methods for saving performance libraries.</summary>
        private readonly IReadOnlyDictionary<string, PerformanceLibrarySaveMethod> PerformanceLibrarySaveMethods;

        public PerformanceLibraryController(IPerformanceLibrary repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));

            PerformanceLibraryGetMethods = new Dictionary<string, PerformanceLibraryGetMethod>
            {
                [Role.ADMINISTRATOR] = (id, userInformation) => repo.GetSimulationPerformanceLibrary(id, db),
                [Role.DISTRICT_ENGINEER] = (id, userInformation) => repo.GetOwnedSimulationPerformanceLibrary(id, db, userInformation.Name),
                [Role.CWOPA] = (id, userInformation) => repo.GetOwnedSimulationPerformanceLibrary(id, db, userInformation.Name),
                [Role.PLANNING_PARTNER] = (id, userInformation) => repo.GetOwnedSimulationPerformanceLibrary(id, db, userInformation.Name)
            };
            PerformanceLibrarySaveMethods = new Dictionary<string, PerformanceLibrarySaveMethod>
            {
                [Role.ADMINISTRATOR] = (model, userInformation) => repo.SaveSimulationPerformanceLibrary(model, db),
                [Role.DISTRICT_ENGINEER] = (model, userInformation) => repo.SaveOwnedSimulationPerformanceLibrary(model, db, userInformation.Name)
            };
        }

        /// <summary>
        /// API endpoint for fetching a simulation's performance library data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetScenarioPerformanceLibrary/{id}")]
        [ModelValidation("The scenario id is invalid.")]
        [RestrictAccess]
        public IHttpActionResult GetSimulationPerformanceLibrary(int id)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            return Ok(PerformanceLibraryGetMethods[userInformation.Role](id, userInformation));
        }

        /// <summary>
        /// API endpoint for upserting/deleting a simulation's performance library data
        /// </summary>
        /// <param name="model">PerformanceLibraryModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveScenarioPerformanceLibrary")]
        [ModelValidation("The performance data is invalid.")]
        [RestrictAccess(Role.ADMINISTRATOR, Role.DISTRICT_ENGINEER)]
        public IHttpActionResult SaveSimulationPerformanceLibrary([FromBody]PerformanceLibraryModel model)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            return Ok(PerformanceLibrarySaveMethods[userInformation.Role](model, userInformation));
        }
    }
}