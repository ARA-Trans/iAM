using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Filters;
using System.ComponentModel.DataAnnotations;

namespace BridgeCare.Controllers
{
    /// <summary>
    /// Http interface to get a list of investment strategies which are text
    /// descriptions and a corresponding index for each one
    ///
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

        // Get: api/InvestmentStrategies
        [ModelValidation]
        public IQueryable<InvestmentStrategyModel> Get(NetworkModel network) => investmentStrategies.GetInvestmentStrategies(network, db);
        
    }
}