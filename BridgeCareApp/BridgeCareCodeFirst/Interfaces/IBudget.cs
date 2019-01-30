using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BridgeCare.Models.BudgetReportData;

namespace BridgeCare.Interfaces
{
    public interface IBudget
    {
        BudgetReportDetails GetBudgetReportData(SimulationResult data, string[] budgetTypes);
        string[] InvestmentData(SimulationResult data);

    }
}
