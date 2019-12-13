using System;
using System.Web.Http;
using System.Web.Http.Filters;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;

namespace BridgeCare.Controllers
{
    public class CashFlowLibraryController : ApiController
    {
        private readonly ICashFlowLibrary repo;
        private readonly BridgeCareContext db;

        public CashFlowLibraryController(ICashFlowLibrary repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// API endpoint for fetching a simulation's cash flow library data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetScenarioCashFlowLibrary/{id}")]
        [ModelValidation("The scenario id is invalid.")]
        [RestrictAccess]
        public IHttpActionResult GetSimulationCashFlowLibrary(int id)
            => Ok(repo.GetSimulationCashFlowLibrary(id, db));

        /// <summary>
        /// API endpoint for upserting/deleting a simulation's cash flow library data
        /// </summary>
        /// <param name="model">CashFlowLibraryModel</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/SaveScenarioCashFlowLibrary")]
        [ModelValidation("The scenario id is invalid")]
        [RestrictAccess(Role.ADMINISTRATOR, Role.DISTRICT_ENGINEER)]
        public IHttpActionResult SaveSimulationCashFlowLibrary([FromBody] CashFlowLibraryModel model)
            => Ok(repo.SaveSimulationCashFlowLibrary(model, db));
    }
}