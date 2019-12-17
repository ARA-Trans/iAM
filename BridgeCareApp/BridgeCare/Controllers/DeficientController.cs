using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Filters;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;

namespace BridgeCare.Controllers
{
    using DeficientLibraryGetMethod = Func<int, UserInformationModel, DeficientLibraryModel>;
    using DeficientLibrarySaveMethod = Func<DeficientLibraryModel, UserInformationModel, DeficientLibraryModel>;

    public class DeficientController: ApiController
    {
        private readonly IDeficient repo;
        private readonly BridgeCareContext db;
        /// <summary>Maps user roles to methods for fetching a target library</summary>
        private readonly IReadOnlyDictionary<string, DeficientLibraryGetMethod> DeficientLibraryGetMethods;
        /// <summary>Maps user roles to methods for saving a target library</summary>
        private readonly IReadOnlyDictionary<string, DeficientLibrarySaveMethod> DeficientLibrarySaveMethods;

        public DeficientController() { }

        public DeficientController(IDeficient repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));

            DeficientLibraryModel GetAnyLibrary(int id, UserInformationModel userInformation) =>
                repo.GetAnySimulationDeficientLibrary(id, db);

            DeficientLibraryModel GetOwnedLibrary(int id, UserInformationModel userInformation) =>
                repo.GetOwnedSimulationDeficientLibrary(id, db, userInformation.Name);

            DeficientLibraryModel SaveAnyLibrary(DeficientLibraryModel model, UserInformationModel userInformation) =>
                repo.SaveAnySimulationDeficientLibrary(model, db);

            DeficientLibraryModel SaveOwnedLibrary(DeficientLibraryModel model, UserInformationModel userInformation) =>
                repo.SaveOwnedSimulationDeficientLibrary(model, db, userInformation.Name);

            DeficientLibraryGetMethods = new Dictionary<string, DeficientLibraryGetMethod>
            {
                [Role.ADMINISTRATOR] = GetAnyLibrary,
                [Role.DISTRICT_ENGINEER] = GetOwnedLibrary,
                [Role.CWOPA] = GetAnyLibrary,
                [Role.PLANNING_PARTNER] = GetOwnedLibrary
            };
            DeficientLibrarySaveMethods = new Dictionary<string, DeficientLibrarySaveMethod>
            {
                [Role.ADMINISTRATOR] = SaveAnyLibrary,
                [Role.DISTRICT_ENGINEER] = SaveOwnedLibrary,
                [Role.CWOPA] = SaveOwnedLibrary,
                [Role.PLANNING_PARTNER] = SaveOwnedLibrary
            };
        }

        /// <summary>
        /// API endpoint for fetching a simulation's deficient library data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetScenarioDeficientLibrary/{id}")]
        [ModelValidation("The scenario id is invalid.")]
        [RestrictAccess]
        public IHttpActionResult GetSimulationDeficientLibrary(int id)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            return Ok(DeficientLibraryGetMethods[userInformation.Role](id, userInformation));
        }

        /// <summary>
        /// API endpoint for upserting/deleting a simulation's deficient library data
        /// </summary>
        /// <param name="model">DeficientLibraryModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveScenarioDeficientLibrary")]
        [ModelValidation("The deficient data is invalid.")]
        [RestrictAccess(Role.ADMINISTRATOR, Role.DISTRICT_ENGINEER)]
        public IHttpActionResult SaveSimulationDeficientLibrary([FromBody] DeficientLibraryModel model)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            return Ok(DeficientLibrarySaveMethods[userInformation.Role](model, userInformation));
        }
    }
}