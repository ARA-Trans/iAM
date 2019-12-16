using BridgeCare.Models.CriteriaDrivenBudgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeCare.Interfaces.CriteriaDrivenBudgets
{
    public interface ICriteriaDrivenBudgets
    {
        List<CriteriaDrivenBudgetsModel> GetCriteriaDrivenBudgets(int simulationId, BridgeCareContext db);
        List<CriteriaDrivenBudgetsModel> GetOwnedCriteriaDrivenBudgets(int simulationId, BridgeCareContext db, string username);
        Task<string> SaveCriteriaDrivenBudgets(int selectedScenarioId, List<CriteriaDrivenBudgetsModel> data, BridgeCareContext db);
        Task<string> SaveOwnedCriteriaDrivenBudgets(int selectedScenarioId, List<CriteriaDrivenBudgetsModel> data, BridgeCareContext db, string username);
    }
}
