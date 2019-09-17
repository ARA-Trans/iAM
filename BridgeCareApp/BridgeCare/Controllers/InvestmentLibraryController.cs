using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Linq;
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

        ///<summary> Get: api/GetScenarioInvestmentLibrary
        ///argument: NetworkModel
        ///</summary>
        [HttpGet]
        [Route("api/GetScenarioInvestmentLibrary/{selectedScenarioId}")]
        [ModelValidation("Given selected scenario Id is not valid")]
        public IHttpActionResult GetScenarioInvestmentLibrary(int selectedScenarioId)
             => Ok(repo.GetScenarioInvestmentLibrary(selectedScenarioId, db));

        ///<summary> Post: api/SaveScenarioInvestmentLibrary
        ///argument: InvestmentStrategyModel
        ///return : 200 sucess with updated model output
        ///         400 for bad input argument
        ///         500 internal server error (uncaught exception)
        ///</summary>
        [HttpPost]
        [Route("api/SaveScenarioInvestmentLibrary")]
        [ModelValidation("Given investment strategy data is not valid")]
        public IHttpActionResult SaveScenarioInvestmentLibrary([FromBody]InvestmentLibraryModel data)
            => Ok(repo.SaveScenarioInvestmentLibrary(data, db));
    }
}