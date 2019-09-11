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
        private readonly BridgeCareContext db;
        private readonly ICriteriaDrivenBudgets criteriaDrivenBudgets;

        public CriteriaDrivenBudgetsController() { }
        public CriteriaDrivenBudgetsController(ICriteriaDrivenBudgets criteriaDrivenBudgetsRepository, BridgeCareContext context)
        {
            criteriaDrivenBudgets = criteriaDrivenBudgetsRepository ?? throw new ArgumentNullException(nameof(criteriaDrivenBudgetsRepository));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        ///<summary> Get: api/GetCriteriaDrivenBudgets
        ///argument: CriteriaDrivenBudgetsModel
        ///</summary>
        [ModelValidation("Given selected scenario Id is not valid")]
        [Route("api/GetCriteriaDrivenBudgets/{selectedScenarioId}")]
        [HttpGet]
        public List<CriteriaDrivenBudgetsModel> Get(int selectedScenarioId)
             => criteriaDrivenBudgets.GetCriteriaDrivenBudgets(selectedScenarioId, db);

        [HttpPost]
        [Route("api/SaveCriteriaDrivenBudgets")]
        [ModelValidation("Given Criteria Driven Budgets are not valid")]
        public IHttpActionResult SaveCriteriaDrivenBudgets(int selectedScenarioId, [FromBody]List<CriteriaDrivenBudgetsModel> data)
        {
           var result = criteriaDrivenBudgets.SaveCriteriaDrivenBudgets(selectedScenarioId, data, db);
            if(result.IsCompleted)
            {
                return Ok();
            }
            return NotFound();
        }
    }
}
