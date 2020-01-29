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
        public List<WorkSummaryByBudgetModel> GetworkSummaryByBudgetsData(SimulationModel simulationModel, BridgeCareContext dbContext)
        {
            var selectReportStatement = $"SELECT YEARS, TREATMENT, BUDGET, SUM(COST_) AS CostPerTreatmentPerYear FROM REPORT_{simulationModel.NetworkId}_{simulationModel.SimulationId} " +
                                        $"WITH (NOLOCK) WHERE BUDGET IS NOT NULL " +
                                        $"GROUP BY TREATMENT, YEARS, BUDGET";

            var budgetsPerTreatmentPerYear = dbContext.Database.SqlQuery<WorkSummaryByBudgetModel>(selectReportStatement).ToList();

            return budgetsPerTreatmentPerYear;
        }
    }
}
