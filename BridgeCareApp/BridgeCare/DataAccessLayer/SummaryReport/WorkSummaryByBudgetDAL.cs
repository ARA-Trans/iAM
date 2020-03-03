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
            var selectQuery = $"select YEARS, treatmentname as TREATMENT, BUDGET, sum(cost_) as CostPerTreatmentPerYear from COMMITTED_ where simulationid = { simulationModel.simulationId } group by budget, TREATMENTNAME, years";
            var committedProjectsBudget = dbContext.Database.SqlQuery<WorkSummaryByBudgetModel>(selectQuery).ToList();
            return committedProjectsBudget;
        }

        public List<WorkSummaryByBudgetModel> GetworkSummaryByBudgetsData(SimulationModel simulationModel, BridgeCareContext dbContext)
        {
            var selectReportStatement = $"SELECT YEARS, TREATMENT, BUDGET, SUM(COST_) AS CostPerTreatmentPerYear FROM REPORT_{simulationModel.networkId}_{simulationModel.simulationId} " +
                                        $"WITH (NOLOCK) WHERE BUDGET IS NOT NULL " +
                                        $"GROUP BY TREATMENT, YEARS, BUDGET";

            var budgetsPerTreatmentPerYear = dbContext.Database.SqlQuery<WorkSummaryByBudgetModel>(selectReportStatement).ToList();

            return budgetsPerTreatmentPerYear;
        }
    }
}
