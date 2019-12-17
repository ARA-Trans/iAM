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

            TreatmentLibraryModel GetAnyLibrary(int id, UserInformationModel userInformation) => 
                repo.GetAnySimulationTreatmentLibrary(id, db);

            TreatmentLibraryModel GetOwnedLibrary(int id, UserInformationModel userInformation) => 
                repo.GetOwnedSimulationTreatmentLibrary(id, db, userInformation.Name);

            TreatmentLibraryModel SaveAnyLibrary(TreatmentLibraryModel model, UserInformationModel userInformation) => 
                repo.SaveAnySimulationTreatmentLibrary(model, db);

            TreatmentLibraryModel SaveOwnedLibrary(TreatmentLibraryModel model, UserInformationModel userInformation) => 
                repo.SaveOwnedSimulationTreatmentLibrary(model, db, userInformation.Name);

            TreatmentLibraryGetMethods = new Dictionary<string, TreatmentLibraryGetMethod>
            {
                [Role.ADMINISTRATOR] = GetAnyLibrary,
                [Role.DISTRICT_ENGINEER] = GetOwnedLibrary,
                [Role.CWOPA] = GetAnyLibrary,
                [Role.PLANNING_PARTNER] = GetOwnedLibrary
            };
            TreatmentLibrarySaveMethods = new Dictionary<string, TreatmentLibrarySaveMethod>
            {
                [Role.ADMINISTRATOR] = SaveAnyLibrary,
                [Role.DISTRICT_ENGINEER] = SaveOwnedLibrary
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