using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    /// <summary>
    /// Http interface to get a list of investment strategies which are text
    /// descriptions and a corresponding index for each one
    /// </summary>
    public class InvestmentStrategiesController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly IInvestmentStrategies investmentStrategies;

        public InvestmentStrategiesController(IInvestmentStrategies investmetStrategiesRepository, BridgeCareContext context)
        {
            investmentStrategies = investmetStrategiesRepository ?? throw new ArgumentNullException(nameof(investmetStrategiesRepository));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        ///<summary> Get: api/GetInvestmentStrategies
        ///argument: NetworkModel
        ///</summary>
        [ModelValidation("Given network data is not valid")]
        [Route("api/GetInvestmentStrategies")]
        [HttpGet]
        public IQueryable<InvestmentStrategyModel> Get([FromBody]NetworkModel network) => investmentStrategies.GetInvestmentStrategies(network, db);

        ///<summary> Post: api/SaveInvestmentStrategies
        ///argument: InvestmentStrategyModel
        ///return : 200 sucess
        ///         400 for bad input argument
        ///         500 internal server error (uncaught exception)
        ///</summary>
        [ModelValidation("Given investment strategy data is not valid")]
        [Route("api/SaveInvestmentStrategies")]
        [HttpPost]
        public IHttpActionResult Post([FromBody]InvestmentStrategyModel data)
        {
            bool getResults = investmentStrategies.SetInvestmentStrategies(data, db);

            if (getResults)
            {
                return Ok();
            }

            return NotFound();
        }


    }
}