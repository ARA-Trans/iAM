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
    using CriteriaDrivenBudgetsGetMethod = Func<int, UserInformationModel, List<CriteriaDrivenBudgetModel>>;
    using CriteriaDrivenBudgetsSaveMethod = Func<int, List<CriteriaDrivenBudgetModel>, UserInformationModel, Task<string>>;

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

            CriteriaDrivenBudgetsGetMethods = CreateGetMethods();
            CriteriaDrivenBudgetsSaveMethods = CreateSaveMethods();
        }

        /// <summary>
        /// Creates a mapping from user roles to the appropriate getters for those roles
        /// </summary>
        private Dictionary<string, CriteriaDrivenBudgetsGetMethod> CreateGetMethods()
        {
            List<CriteriaDrivenBudgetModel> GetAnyBudgets(int id, UserInformationModel userInformation) =>
                repo.GetAnyCriteriaDrivenBudgets(id, db);
            List<CriteriaDrivenBudgetModel> GetPermittedBudgets(int id, UserInformationModel userInformation) =>
                repo.GetPermittedCriteriaDrivenBudgets(id, db, userInformation.Name);

            return new Dictionary<string, CriteriaDrivenBudgetsGetMethod>
            {
                [Role.ADMINISTRATOR] = GetAnyBudgets,
                [Role.DISTRICT_ENGINEER] = GetPermittedBudgets,
                [Role.CWOPA] = GetAnyBudgets,
                [Role.PLANNING_PARTNER] = GetPermittedBudgets
            };
        }

        /// <summary>
        /// Creates a mapping from user roles to the appropriate save methods for those roles
        /// </summary>
        private Dictionary<string, CriteriaDrivenBudgetsSaveMethod> CreateSaveMethods()
        {
            Task<string> SaveAnyBudgets(int id, List<CriteriaDrivenBudgetModel> models, UserInformationModel userInformation) =>
                repo.SaveAnyCriteriaDrivenBudgets(id, models, db);
            Task<string> SavePermittedBudgets(int id, List<CriteriaDrivenBudgetModel> models, UserInformationModel userInformation) =>
                repo.SavePermittedCriteriaDrivenBudgets(id, models, db, userInformation.Name);

            return new Dictionary<string, CriteriaDrivenBudgetsSaveMethod>
            {
                [Role.ADMINISTRATOR] = SaveAnyBudgets,
                [Role.DISTRICT_ENGINEER] = SavePermittedBudgets,
                [Role.CWOPA] = SavePermittedBudgets,
                [Role.PLANNING_PARTNER] = SavePermittedBudgets
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
        /// <param name="models">CriteriaDrivenBudgetModel list</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveCriteriaDrivenBudgets/{id}")]
        [ModelValidation("The criteria driven budgets data is invalid.")]
        [RestrictAccess]
        public IHttpActionResult SaveCriteriaDrivenBudgets(int id, [FromBody]List<CriteriaDrivenBudgetModel> models)
        {
            UserInformationModel userInformation = JWTParse.GetUserInformation(Request.Headers.Authorization.Parameter);
            var result = CriteriaDrivenBudgetsSaveMethods[userInformation.Role](id, models, userInformation);
            if(result.IsCompleted)
                return Ok();

            return NotFound();
        }
    }
}
