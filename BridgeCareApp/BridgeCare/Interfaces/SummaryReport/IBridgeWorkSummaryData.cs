using BridgeCare.Models;
using System.Collections.Generic;

namespace BridgeCare.Interfaces
{
    public interface IBridgeWorkSummaryData
    {
        List<InvestmentStrategyYearlyBudgetModel> GetYearlyBudgetModels(int simulationId, BridgeCareContext dbContext);
    }
}