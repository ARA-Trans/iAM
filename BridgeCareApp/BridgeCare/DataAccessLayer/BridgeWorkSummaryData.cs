using BridgeCare.Interfaces;
using BridgeCare.Models;
using System.Collections.Generic;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class BridgeWorkSummaryData: IBridgeWorkSummaryData
    {
        /// <summary>
        /// Get yearly details for budget amounts to be utilized by Total Budget section of the work smmary report.
        /// </summary>
        /// <param name="simulationId"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public List<InvestmentLibraryBudgetYearModel> GetYearlyBudgetModels(int simulationId, BridgeCareContext dbContext)
        {
            var yearlyInvestments = dbContext.YEARLYINVESTMENTs.Where(y => y.SIMULATIONID == simulationId);
            var yearlyBudgetModels = yearlyInvestments.Select(m => new InvestmentLibraryBudgetYearModel
            {
                Year = m.YEAR_,
                Budget = yearlyInvestments
                            .Where(n => n.YEAR_ == m.YEAR_)
                            .Select(f => new InvestmentLibraryBudgetModel()
                            {
                                budgetAmount = f.AMOUNT,
                                budgetName = f.BUDGETNAME
                            }).ToList()
            }).ToList();
            return yearlyBudgetModels;
        }
    }
}