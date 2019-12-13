using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    using InvestmentLibraryGetMethod = Func<IInvestmentLibrary, int, BridgeCareContext, UserInformationModel, InvestmentLibraryModel>;
    using InvestmentLibrarySaveMethod = Func<IInvestmentLibrary, InvestmentLibraryModel, BridgeCareContext, UserInformationModel, InvestmentLibraryModel>;

    /// <summary>
    /// Http interface to get a list of investment strategies which are text descriptions and a
    /// corresponding index for each one
    /// </summary>
    public class InvestmentLibraryController : ApiController
    {
        private readonly IInvestmentLibrary repo;
        private readonly BridgeCareContext db;

        private readonly IReadOnlyDictionary<string, InvestmentLibraryGetMethod> InvestmentLibraryGetMethods = new Dictionary<string, InvestmentLibraryGetMethod>
        {
            [Role.ADMINISTRATOR] = (repo, id, db, userInformation) => repo.GetSimulationInvestmentLibrary(id, db),
            [Role.DISTRICT_ENGINEER] = (repo, id, db, userInformation) => repo.GetOwnedSimulationInvestmentLibrary(id, db, userInformation.Name),
            [Role.CWOPA] = (repo, id, db, userInformation) => repo.GetOwnedSimulationInvestmentLibrary(id, db, userInformation.Name),
            [Role.PLANNING_PARTNER] = (repo, id, db, userInformation) => repo.GetOwnedSimulationInvestmentLibrary(id, db, userInformation.Name)
        };

        private readonly IReadOnlyDictionary<string, InvestmentLibrarySaveMethod> InvestmentLibrarySaveMethods = new Dictionary<string, InvestmentLibrarySaveMethod>
        {

            [Role.ADMINISTRATOR] = (repo, model, db, userInformation) => repo.SaveSimulationInvestmentLibrary(model, db),
            [Role.DISTRICT_ENGINEER] = (repo, model, db, userInformation) => repo.SaveOwnedSimulationInvestmentLibrary(model, db, userInformation.Name),
            [Role.CWOPA] = (repo, model, db, userInformation) => repo.SaveOwnedSimulationInvestmentLibrary(model, db, userInformation.Name),
            [Role.PLANNING_PARTNER] = (repo, model, db, userInformation) => repo.SaveOwnedSimulationInvestmentLibrary(model, db, userInformation.Name)
        };

        public InvestmentLibraryController(IInvestmentLibrary repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
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
            return Ok(InvestmentLibraryGetMethods[userInformation.Role](repo, id, db, userInformation);
        }

        /// <summary>
        /// API endpoint for upserting/deleting a simulation's investment library data
        /// </summary>
        /// <param name="model"></param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveScenarioInvestmentLibrary")]
        [ModelValidation("Given investment data is not valid")]
        [RestrictAccess(Role.ADMINISTRATOR, Role.DISTRICT_ENGINEER)]
        public IHttpActionResult SaveSimulationInvestmentLibrary([FromBody]InvestmentLibraryModel model)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            return Ok(InvestmentLibrarySaveMethods[userInformation.Role](repo, model, db, userInformation);
        }
    }
}