using BridgeCare.Interfaces.CriteriaDrivenBudgets;
using BridgeCare.Models;
using BridgeCare.Models.CriteriaDrivenBudgets;
using BridgeCare.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    using CriteriaDrivenBudgetsGetMethod = Func<int, UserInformationModel, List<CriteriaDrivenBudgetsModel>>;
    using CriteriaDrivenBudgetsSaveMethod = Func<int, List<CriteriaDrivenBudgetsModel>, UserInformationModel, Task<string>>;

    public class CriteriaDrivenBudgetsController : ApiController
    {
        private readonly ICriteriaDrivenBudgets repo;
        private readonly BridgeCareContext db;
        /// <summary>Maps user roles to methods for fetching Criteria Driven Budgets for them</summary>
        private readonly IReadOnlyDictionary<string, CriteriaDrivenBudgetsGetMethod> CriteriaDrivenBudgetsGetMethods;
        /// <summary>Maps user roles to methods for saving Criteria Driven Budgets for them</summary>
        private readonly IReadOnlyDictionary<string, CriteriaDrivenBudgetsSaveMethod> CriteriaDrivenBudgetsSaveMethods;

        public CriteriaDrivenBudgetsController() { }
        public CriteriaDrivenBudgetsController(ICriteriaDrivenBudgets repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));

            CriteriaDrivenBudgetsGetMethods = new Dictionary<string, CriteriaDrivenBudgetsGetMethod>
            {
                [Role.ADMINISTRATOR] = (id, userInformation) => repo.GetCriteriaDrivenBudgets(id, db),
                [Role.DISTRICT_ENGINEER] = (id, userInformation) => repo.GetOwnCriteriaDrivenBudgets(id, db, userInformation.Name),
                [Role.CWOPA] = (id, userInformation) => repo.GetOwnCriteriaDrivenBudgets(id, db, userInformation.Name),
                [Role.PLANNING_PARTNER] = (id, userInformation) => repo.GetOwnCriteriaDrivenBudgets(id, db, userInformation.Name)
            };
            CriteriaDrivenBudgetsSaveMethods = new Dictionary<string, CriteriaDrivenBudgetsSaveMethod>
            {
                [Role.ADMINISTRATOR] = (id, models, userInformation) => repo.SaveCriteriaDrivenBudgets(id, models, db),
                [Role.DISTRICT_ENGINEER] = (id, models, userInformation) => repo.SaveOwnCriteriaDrivenBudgets(id, models, db, userInformation.Name),
                [Role.CWOPA] = (id, models, userInformation) => repo.SaveOwnCriteriaDrivenBudgets(id, models, db, userInformation.Name),
                [Role.PLANNING_PARTNER] = (id, models, userInformation) => repo.SaveOwnCriteriaDrivenBudgets(id, models, db, userInformation.Name)
            };
        }

        /// <summary>
        /// API endpoint for fetching a simulation's criteria driven budgets
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetCriteriaDrivenBudgets/{id}")]
        [ModelValidation("The scenario id is invalid.")]
        [RestrictAccess]
        public IHttpActionResult GetCriteriaDrivenBudgets(int id) 
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            return Ok(CriteriaDrivenBudgetsGetMethods[userInformation.Role](id, userInformation));
        }

        /// <summary>
        /// API endpoint for saving criteria driven budgets data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="models">CriteriaDrivenBudgetsModel list</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveCriteriaDrivenBudgets/{id}")]
        [ModelValidation("The criteria driven budgets data is invalid.")]
        [RestrictAccess]
        public IHttpActionResult SaveCriteriaDrivenBudgets(int id, [FromBody]List<CriteriaDrivenBudgetsModel> models)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            var result = CriteriaDrivenBudgetsSaveMethods[userInformation.Role](id, models, userInformation);
            if(result.IsCompleted)
                return Ok();

            return NotFound();
        }
    }
}
