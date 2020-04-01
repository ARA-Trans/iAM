using System;
using System.Collections.Generic;
using System.Linq;
using BridgeCare.Models;
using BridgeCare.Models.SummaryReport;

namespace BridgeCare.Interfaces.SummaryReport
{
    public interface IWorkSummaryByBudget
    {
        List<WorkSummaryByBudgetModel> GetworkSummaryByBudgetsData(SimulationModel model, BridgeCareContext db);
        List<WorkSummaryByBudgetModel> GetCommittedProjectsBudget(SimulationModel simulationModel, BridgeCareContext dbContext);
        List<WorkSummaryByBudgetModel> GetAllCommittedProjects(SimulationModel simulationModel, BridgeCareContext dbContext);
    }
}
