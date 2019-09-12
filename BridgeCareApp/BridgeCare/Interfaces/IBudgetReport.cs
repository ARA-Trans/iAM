using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IBudgetReport
    {
        YearlyBudgetAndCost GetData(SimulationModel data, string[] budgetTypes);

        string[] InvestmentData(SimulationModel data);
    }
}