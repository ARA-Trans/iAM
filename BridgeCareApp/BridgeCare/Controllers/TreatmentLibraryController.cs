using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    using TreatmentLibraryGetMethod = Func<int, UserInformationModel, TreatmentLibraryModel>;
    using TreatmentLibrarySaveMethod = Func<TreatmentLibraryModel, UserInformationModel, TreatmentLibraryModel>;

    public class TreatmentLibraryController : ApiController
    {
        private readonly ITreatmentLibrary repo;
        private readonly BridgeCareContext db;
        /// <summary>Maps user roles to methods for getting treatment libraries.</summary>
        private readonly IReadOnlyDictionary<string, TreatmentLibraryGetMethod> TreatmentLibraryGetMethods;
        /// <summary>Maps user roles to methods for saving treatment libraries.</summary>
        private readonly IReadOnlyDictionary<string, TreatmentLibrarySaveMethod> TreatmentLibrarySaveMethods;

        public TreatmentLibraryController(ITreatmentLibrary repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));

            TreatmentLibraryGetMethods = new Dictionary<string, TreatmentLibraryGetMethod>
            {
                [Role.ADMINISTRATOR] = (id, userInformation) => repo.GetSimulationTreatmentLibrary(id, db),
                [Role.DISTRICT_ENGINEER] = (id, userInformation) => repo.GetOwnedSimulationTreatmentLibrary(id, db, userInformation.Name),
                [Role.CWOPA] = (id, userInformation) => repo.GetOwnedSimulationTreatmentLibrary(id, db, userInformation.Name),
                [Role.PLANNING_PARTNER] = (id, userInformation) => repo.GetOwnedSimulationTreatmentLibrary(id, db, userInformation.Name)
            };
            TreatmentLibrarySaveMethods = new Dictionary<string, TreatmentLibrarySaveMethod>
            {
                [Role.ADMINISTRATOR] = (model, userInformation) => repo.SaveSimulationTreatmentLibrary(model, db),
                [Role.DISTRICT_ENGINEER] = (model, userInformation) => repo.SaveOwnedSimulationTreatmentLibrary(model, db, userInformation.Name)
            };
        }

        /// <summary>
        /// API endpoint for fetching a simulation's treatment library data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetScenarioTreatmentLibrary/{id}")]
        [ModelValidation("The scenario id is invalid.")]
        [RestrictAccess]
        public IHttpActionResult GetSimulationTreatmentLibrary(int id)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            return Ok(TreatmentLibraryGetMethods[userInformation.Role](id, userInformation));
        }

        /// <summary>
        /// API endpoint for upserting/deleting a simulation's treatment library data
        /// </summary>
        /// <param name="model">TreatmentLibraryModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveScenarioTreatmentLibrary")]
        [RestrictAccess(Role.ADMINISTRATOR, Role.DISTRICT_ENGINEER)]
        public IHttpActionResult SaveSimulationTreatmentLibrary([FromBody]TreatmentLibraryModel model)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            return Ok(TreatmentLibrarySaveMethods[userInformation.Role](model, userInformation));
        }
    }
}