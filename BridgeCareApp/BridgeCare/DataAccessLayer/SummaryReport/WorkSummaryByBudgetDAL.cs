using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.Interfaces.SummaryReport;
using BridgeCare.Models;
using BridgeCare.Models.SummaryReport;

namespace BridgeCare.DataAccessLayer.SummaryReport
{
    public class WorkSummaryByBudgetDAL : IWorkSummaryByBudget
    {
        public List<WorkSummaryByBudgetModel> GetCommittedProjectsBudget(SimulationModel simulationModel, BridgeCareContext dbContext)
        {
            var selectQuery = $"select YEARS, TREATMENT, Budget, sum(cost_) as CostPerTreatmentPerYear from REPORT_{simulationModel.networkId}_{simulationModel.simulationId} where BUDGET IS NOT NULL " +
                $" and Project_Type = 1 group by TREATMENT, years, Budget";
            var committedProjectsBudget = dbContext.Database.SqlQuery<WorkSummaryByBudgetModel>(selectQuery).ToList();
            return committedProjectsBudget;
        }

        public List<WorkSummaryByBudgetModel> GetAllCommittedProjects(SimulationModel simulationModel, BridgeCareContext dbContext)
        {
            var selectQuery = $"select YEARS, TREATMENT, sum(cost_) as CostPerTreatmentPerYear from REPORT_{simulationModel.networkId}_{simulationModel.simulationId} where BUDGET IS NOT NULL " +
                $" and Project_Type = 1 group by TREATMENT, years";
            var committedProjectsBudget = dbContext.Database.SqlQuery<WorkSummaryByBudgetModel>(selectQuery).ToList();
            return committedProjectsBudget;
        }

        public List<WorkSummaryByBudgetModel> GetworkSummaryByBudgetsData(SimulationModel simulationModel, BridgeCareContext dbContext)
        {
            var selectReportStatement = $"SELECT YEARS, TREATMENT, BUDGET, round(SUM(COST_), -3) AS CostPerTreatmentPerYear FROM REPORT_{simulationModel.networkId}_{simulationModel.simulationId} " +
                                        $" WHERE BUDGET IS NOT NULL and (Project_Type IN (0, 2)) " +
                                        $"GROUP BY TREATMENT, YEARS, BUDGET";

            var budgetsPerTreatmentPerYear = dbContext.Database.SqlQuery<WorkSummaryByBudgetModel>(selectReportStatement).ToList();

            return budgetsPerTreatmentPerYear;
        }
    }
}
