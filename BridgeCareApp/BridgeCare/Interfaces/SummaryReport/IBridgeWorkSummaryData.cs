using BridgeCare.Models;
using System.Collections.Generic;

namespace BridgeCare.Interfaces
{
    public interface IBridgeWorkSummaryData
    {
        List<InvestmentLibraryYearlyBudgetModel> GetYearlyBudgetModels(int simulationId, BridgeCareContext dbContext);
    }
}