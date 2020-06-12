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

            PriorityLibraryGetMethods = CreateGetMethods();
            PriorityLibrarySaveMethods = CreateSaveMethods();
        }

        /// <summary>
        /// Creates a mapping from user roles to the appropriate methods for getting priority libraries
        /// </summary>
        private Dictionary<string, PriorityLibraryGetMethod> CreateGetMethods()
        {
            PriorityLibraryModel GetAnyLibrary(int id, UserInformationModel userInformation) =>
                repo.GetAnySimulationPriorityLibrary(id, db);
            PriorityLibraryModel GetPermittedLibrary(int id, UserInformationModel userInformation) =>
                repo.GetPermittedSimulationPriorityLibrary(id, db, userInformation.Name);

            return new Dictionary<string, PriorityLibraryGetMethod>
            {
                [Role.ADMINISTRATOR] = GetAnyLibrary,
                [Role.DISTRICT_ENGINEER] = GetPermittedLibrary,
                [Role.CWOPA] = GetAnyLibrary,
                [Role.GENERAL_USERS] = GetPermittedLibrary
            };
        }

        /// <summary>
        /// Creates a mapping from user roles to the appropriate methods for saving priority libraries
        /// </summary>
        private Dictionary<string, PriorityLibrarySaveMethod> CreateSaveMethods()
        {
            PriorityLibraryModel SaveAnyLibrary(PriorityLibraryModel model, UserInformationModel userInformation) =>
                repo.SaveAnySimulationPriorityLibrary(model, db);
            PriorityLibraryModel SavePermittedLibrary(PriorityLibraryModel model, UserInformationModel userInformation) =>
                repo.SavePermittedSimulationPriorityLibrary(model, db, userInformation.Name);

            return new Dictionary<string, PriorityLibrarySaveMethod>
            {
                [Role.ADMINISTRATOR] = SaveAnyLibrary,
                [Role.DISTRICT_ENGINEER] = SavePermittedLibrary
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
            UserInformationModel userInformation = ESECSecurity.GetUserInformation(Request);
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
            UserInformationModel userInformation = ESECSecurity.GetUserInformation(Request);
            return Ok(PriorityLibrarySaveMethods[userInformation.Role](model, userInformation));
        }
    }
}
