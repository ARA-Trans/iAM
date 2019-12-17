using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    using InvestmentLibraryGetMethod = Func<int, UserInformationModel, InvestmentLibraryModel>;
    using InvestmentLibrarySaveMethod = Func<InvestmentLibraryModel, UserInformationModel, InvestmentLibraryModel>;

    /// <summary>
    /// Http interface to get a list of investment strategies which are text descriptions and a
    /// corresponding index for each one
    /// </summary>
    public class InvestmentLibraryController : ApiController
    {
        private readonly IInvestmentLibrary repo;
        private readonly BridgeCareContext db;
        /// <summary>Maps user roles to methods for fetching an investment library</summary>
        private readonly IReadOnlyDictionary<string, InvestmentLibraryGetMethod> InvestmentLibraryGetMethods;
        /// <summary>Maps user roles to methods for saving an investment library</summary>
        private readonly IReadOnlyDictionary<string, InvestmentLibrarySaveMethod> InvestmentLibrarySaveMethods;

        public InvestmentLibraryController(IInvestmentLibrary repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));

            InvestmentLibraryModel GetAnyLibrary(int id, UserInformationModel userInformation) =>
                repo.GetAnySimulationInvestmentLibrary(id, db);

            InvestmentLibraryModel GetOwnedLibrary(int id, UserInformationModel userInformation) =>
                repo.GetOwnedSimulationInvestmentLibrary(id, db, userInformation.Name);

            InvestmentLibraryModel SaveAnyLibrary(InvestmentLibraryModel model, UserInformationModel userInformation) =>
                repo.SaveAnySimulationInvestmentLibrary(model, db);

            InvestmentLibraryModel SaveOwnedLibrary(InvestmentLibraryModel model, UserInformationModel userInformation) =>
                repo.SaveOwnedSimulationInvestmentLibrary(model, db, userInformation.Name);

            InvestmentLibraryGetMethods = new Dictionary<string, InvestmentLibraryGetMethod>
            {
                [Role.ADMINISTRATOR] = GetAnyLibrary,
                [Role.DISTRICT_ENGINEER] = GetOwnedLibrary,
                [Role.CWOPA] = GetAnyLibrary,
                [Role.PLANNING_PARTNER] = GetOwnedLibrary
            };
            InvestmentLibrarySaveMethods = new Dictionary<string, InvestmentLibrarySaveMethod>
            {
                [Role.ADMINISTRATOR] = SaveAnyLibrary,
                [Role.DISTRICT_ENGINEER] = SaveOwnedLibrary,
                [Role.CWOPA] = SaveOwnedLibrary,
                [Role.PLANNING_PARTNER] = SaveOwnedLibrary
            };
        }

        /// <summary>
        /// API endpoint for fetching a simulation's investment library data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetScenarioInvestmentLibrary/{id}")]
        [ModelValidation("The scenario id is invalid.")]
        [RestrictAccess]
        public IHttpActionResult GetSimulationInvestmentLibrary(int id)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            return Ok(InvestmentLibraryGetMethods[userInformation.Role](id, userInformation));
        }

        /// <summary>
        /// API endpoint for upserting/deleting a simulation's investment library data
        /// </summary>
        /// <param name="model"></param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveScenarioInvestmentLibrary")]
        [ModelValidation("Given investment data is not valid")]
        [RestrictAccess]
        public IHttpActionResult SaveSimulationInvestmentLibrary([FromBody]InvestmentLibraryModel model)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            return Ok(InvestmentLibrarySaveMethods[userInformation.Role](model, userInformation));
        }
    }
}