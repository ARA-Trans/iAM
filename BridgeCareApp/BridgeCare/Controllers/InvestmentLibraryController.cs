using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;
using System;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    /// <summary>
    /// Http interface to get a list of investment strategies which are text descriptions and a
    /// corresponding index for each one
    /// </summary>
    public class InvestmentLibraryController : ApiController
    {
        private readonly IInvestmentLibrary repo;
        private readonly BridgeCareContext db;

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
             => Ok(repo.GetSimulationInvestmentLibrary(id, db));

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
            => Ok(repo.SaveSimulationInvestmentLibrary(model, db));
    }
}