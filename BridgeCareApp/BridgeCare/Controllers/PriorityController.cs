using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    using PriorityLibraryGetMethod = Func<int, UserInformationModel, PriorityLibraryModel>;
    using PriorityLibrarySaveMethod = Func<PriorityLibraryModel, UserInformationModel, PriorityLibraryModel>;

    public class PriorityController : ApiController
    {
        private readonly IPriority repo;
        private readonly BridgeCareContext db;
        /// <summary>Maps user roles to methods for getting performance libraries.</summary>
        private readonly IReadOnlyDictionary<string, PriorityLibraryGetMethod> PriorityLibraryGetMethods;
        /// <summary>Maps user roles to methods for saving performance libraries.</summary>
        private readonly IReadOnlyDictionary<string, PriorityLibrarySaveMethod> PriorityLibrarySaveMethods;

        public PriorityController() { }

        public PriorityController(IPriority repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));

            PriorityLibraryGetMethods = new Dictionary<string, PriorityLibraryGetMethod>
            {
                [Role.ADMINISTRATOR] = (id, userInformation) => repo.GetSimulationPriorityLibrary(id, db),
                [Role.DISTRICT_ENGINEER] = (id, userInformation) => repo.GetOwnedSimulationPriorityLibrary(id, db, userInformation.Name),
                [Role.CWOPA] = (id, userInformation) => repo.GetOwnedSimulationPriorityLibrary(id, db, userInformation.Name),
                [Role.PLANNING_PARTNER] = (id, userInformation) => repo.GetOwnedSimulationPriorityLibrary(id, db, userInformation.Name)
            };
            PriorityLibrarySaveMethods = new Dictionary<string, PriorityLibrarySaveMethod>
            {
                [Role.ADMINISTRATOR] = (model, userInformation) => repo.SaveSimulationPriorityLibrary(model, db),
                [Role.DISTRICT_ENGINEER] = (model, userInformation) => repo.SaveOwnedSimulationPriorityLibrary(model, db, userInformation.Name)
            };
        }

        /// <summary>
        /// API endpoint for fetching a simulation's priority library data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetScenarioPriorityLibrary/{id}")]
        [ModelValidation("The scenario id is invalid.")]
        [RestrictAccess]
        public IHttpActionResult GetSimulationPriorityLibrary(int id)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            return Ok(PriorityLibraryGetMethods[userInformation.Role](id, userInformation));
        }

        /// <summary>
        /// API endpoint for upserting/deleting a simulation's priority library data
        /// </summary>
        /// <param name="model">PriorityLibraryModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveScenarioPriorityLibrary")]
        [ModelValidation("The priority data is invalid.")]
        [RestrictAccess(Role.ADMINISTRATOR, Role.DISTRICT_ENGINEER)]
        public IHttpActionResult SaveSimulationPriorityLibrary([FromBody]PriorityLibraryModel model)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            return Ok(PriorityLibrarySaveMethods[userInformation.Role](model, userInformation));
        }
    }
}