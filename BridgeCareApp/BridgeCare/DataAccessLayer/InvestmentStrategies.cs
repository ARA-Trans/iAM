using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class InvestmentStrategies : IInvestmentStrategies
    {
        private readonly BridgeCareContext db;

        public InvestmentStrategies(BridgeCareContext context)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IQueryable<InvestmentStrategyModel> GetInvestmentStrategies(NetworkModel network, BridgeCareContext db)
        {
            try
            {
                var investmentStrategies = db.INVESTMENTs
                    .Select(p => new InvestmentStrategyModel
                    {
                        Id = p.SIMULATIONID,
                        BudgetOrder = p.BUDGETORDER,
                        FirstYear = p.FIRSTYEAR ?? 0,
                        NumberYears = p.NUMBERYEARS ?? 0,
                        InflationRate = p.INFLATIONRATE ?? 0,
                        DiscountRate = p.DISCOUNTRATE ?? 0
                    });

                return investmentStrategies;
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Investment Strategies");
            }
            return EmptyResult.AsQueryable<InvestmentStrategyModel>();
        }
    }
}