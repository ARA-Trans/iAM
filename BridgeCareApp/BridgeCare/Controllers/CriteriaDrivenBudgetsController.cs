using BridgeCare.Interfaces.CriteriaDrivenBudgets;
using BridgeCare.Models.CriteriaDrivenBudgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    public class CriteriaDrivenBudgetsController : ApiController
    {
        private readonly ICriteriaDrivenBudgets repo;
        private readonly BridgeCareContext db;

        public CriteriaDrivenBudgetsController() { }
        public CriteriaDrivenBudgetsController(ICriteriaDrivenBudgets repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// API endpoint for fetching a simulation's criteria driven budgets
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetCriteriaDrivenBudgets/{id}")]
        [ModelValidation("The scenario id is invalid.")]
        public IHttpActionResult GetCriteriaDrivenBudgets(int id) => Ok(repo.GetCriteriaDrivenBudgets(id, db));

        /// <summary>
        /// API endpoint for saving criteria driven budgets data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="models">CriteriaDrivenBudgetsModel list</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveCriteriaDrivenBudgets")]
        [ModelValidation("The criteria driven budgets data is invalid.")]
        public IHttpActionResult SaveCriteriaDrivenBudgets(int id, [FromBody]List<CriteriaDrivenBudgetsModel> models)
        {
            var result = repo.SaveCriteriaDrivenBudgets(id, models, db);
            if(result.IsCompleted)
                return Ok();

            return NotFound();
        }
    }
}
