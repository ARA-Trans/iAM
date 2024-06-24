using BridgeCare.Interfaces;
using System.Collections.Generic;
using System.Linq;
using BridgeCare.Models;

namespace BridgeCare.DataAccessLayer
{
    public class BridgeWorkSummaryDataDAL: IBridgeWorkSummaryData
    {
        /// <summary>
        /// Get yearly details for budget amounts to be utilized by Total Budget section of the work summary report.
        /// </summary>
        /// <param name="simulationId"></param>
        /// <param name="simulationYears"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public Dictionary<int, List<double>> GetYearlyBudgetAmounts(int simulationId, List<int> simulationYears, BridgeCareContext dbContext)
        {
            var yearlyBudgetAmounts = new Dictionary<int, List<double>>();

            var yearlyInvestments = dbContext?.YearlyInvestments.Where(y => y.SIMULATIONID == simulationId);

            simulationYears?.ForEach(year =>
            {
                if (!yearlyBudgetAmounts.ContainsKey(year))
                {
                    yearlyBudgetAmounts.Add(year, new List<double>());
                }

                var budgetAmounts = yearlyInvestments?.Where(y => y.YEAR_ == year)
                    .Select(y => y.AMOUNT).ToList();

                if (budgetAmounts != null && budgetAmounts.Any())
                {
                    budgetAmounts.ForEach(amount => yearlyBudgetAmounts[year].Add(amount ?? 0));
                }
            });
            
            return yearlyBudgetAmounts;
        }

        public List<InvestmentLibraryBudgetYearModel> GetYearlyBudgetModels(int simulationId, BridgeCareContext dbContext) =>
            dbContext?.YearlyInvestments.Where(yi => yi.SIMULATIONID == simulationId)
                .Select(yi => new InvestmentLibraryBudgetYearModel(yi)).ToList() ?? new List<InvestmentLibraryBudgetYearModel>();
    }
}
