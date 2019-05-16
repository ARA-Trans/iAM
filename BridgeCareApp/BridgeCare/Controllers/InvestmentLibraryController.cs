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
        private readonly BridgeCareContext db;
        private readonly IInvestmentLibrary investmentLibrary;

        public InvestmentLibraryController(IInvestmentLibrary investmentLibraryRepository, BridgeCareContext context)
        {
            investmentLibrary = investmentLibraryRepository ?? throw new ArgumentNullException(nameof(investmentLibraryRepository));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        ///<summary> Get: api/GetScenarioInvestmentLibrary
        ///argument: NetworkModel
        ///</summary>
        [ModelValidation("Given simulation ID is not valid")]
        [Route("api/GetScenarioInvestmentLibrary/{simulationId}")]
        [HttpGet]
        public InvestmentLibraryModel Get(int simulationId)
             => investmentLibrary.GetInvestmentLibrary(simulationId, db);

        ///<summary> Post: api/SaveInvestmentStrategy
        ///argument: InvestmentStrategyModel
        ///return : 200 sucess
        ///         400 for bad input argument
        ///         500 internal server error (uncaught exception)
        ///</summary>
        [ModelValidation("Given investment strategy data is not valid")]
        [Route("api/SaveInvestmentStrategy")]
        [HttpPost]
        public IHttpActionResult Post([FromBody]InvestmentLibraryModel data)
        {
            investmentLibrary.SetInvestmentStrategies(data, db);
            return Ok();
        }
    }
}