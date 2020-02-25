using BridgeCare.Models;
using BridgeCare.Models.SummaryReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BridgeCare.Services
{
    public class BridgeDataHelper
    {
        public List<SimulationDataModel> GetSimulationDataModels(DataTable simulationDataTable, List<int> simulationYears, IQueryable<ReportProjectCost> projectCostModels, List<BudgetsPerBRKey> budgetsPerBrKey)
        {
            var simulationDataModels = new List<SimulationDataModel>();
            var projectCostsList = projectCostModels.ToList();
            foreach (DataRow simulationRow in simulationDataTable.Rows)
            {
                var bridgeDataPerSection = budgetsPerBrKey.Where(b => b.SECTIONID == Convert.ToUInt32(simulationRow["SECTIONID"])).ToList();
                var simulationDM = CreatePrevYearSimulationMdel(simulationRow);                
                var projectCostEntries = projectCostsList.Where(pc => pc.SECTIONID == Convert.ToUInt32(simulationRow["SECTIONID"])).ToList();
                AddAllYearsData(simulationRow, simulationYears, projectCostEntries, simulationDM, bridgeDataPerSection);
                simulationDataModels.Add(simulationDM);
            }

            return simulationDataModels;
        }        

        private SimulationDataModel CreatePrevYearSimulationMdel(DataRow simulationRow)
        {
            var blankBudgetForPreviousYear = new BudgetsPerBRKey { Budget = "", IsCommitted = false, Treatment = "" };
            YearsData yearsData = AddYearsData(simulationRow, null, 0, blankBudgetForPreviousYear);
            return new SimulationDataModel
            {
                YearsData = new List<YearsData>() { yearsData },
                SectionId = Convert.ToInt32(simulationRow["SECTIONID"])
            };
        }

        private void AddAllYearsData(DataRow simulationRow, List<int> simulationYears, List<ReportProjectCost> projectCostEntries, SimulationDataModel simulationDM, List<BudgetsPerBRKey> bridgeDataPerSection)
        {
            var yearsDataModels = new List<YearsData>();
            foreach (int year in simulationYears)
            {
                var budgetPerBrKey = new BudgetsPerBRKey() { Budget = "", IsCommitted = false, Treatment = ""};
                var projectCostEntry = projectCostEntries.Where(p => p.YEARS == year).FirstOrDefault();
                if(bridgeDataPerSection.Count > 0 && bridgeDataPerSection != null)
                {
                    budgetPerBrKey = bridgeDataPerSection.Where(p => p.YEARS == year).FirstOrDefault();
                }
                yearsDataModels.Add(AddYearsData(simulationRow, projectCostEntry, year, budgetPerBrKey));
            }
            simulationDM.YearsData.AddRange(yearsDataModels.OrderBy(y => y.Year).ToList());
        }

        private YearsData AddYearsData(DataRow simulationRow, ReportProjectCost projectCostEntry, int year, BudgetsPerBRKey budgetPerBrKey)
        {
            var yearsData = new YearsData
            {
                Deck = simulationRow["DECK_SEEDED_" + year].ToString(),
                Super = simulationRow["SUP_SEEDED_" + year].ToString(),
                Sub = simulationRow["SUB_SEEDED_" + year].ToString(),
                Culv = simulationRow["CULV_SEEDED_" + year].ToString(),
                DeckD = simulationRow["DECK_DURATION_N_" + year].ToString(),
                SuperD = simulationRow["SUP_DURATION_N_" + year].ToString(),
                SubD = simulationRow["SUB_DURATION_N_" + year].ToString(),
                CulvD = simulationRow["CULV_DURATION_N_" + year].ToString(),
                Year = year
            };
            yearsData.MinC = Math.Min(Convert.ToDouble(yearsData.Deck), Convert.ToDouble(yearsData.Culv)).ToString();
            yearsData.SD = Convert.ToDouble(yearsData.MinC) < 5 ? "Y" : "N";

            yearsData.Project = year != 0 ? projectCostEntry?.TREATMENT : string.Empty;
            yearsData.Cost = year != 0 ? (projectCostEntry == null ? 0 : projectCostEntry.COST_) : 0;
            yearsData.Project = yearsData.Cost == 0 ? "No Treatment" : yearsData.Project;
            yearsData.Budget = budgetPerBrKey != null ? budgetPerBrKey.Budget : "";
            yearsData.ProjectPick = budgetPerBrKey != null ? (budgetPerBrKey.IsCommitted ? "Committed Pick" : "BAMs Pick") : "BAMs Pick";
            yearsData.ProjectPickType = budgetPerBrKey != null ? (budgetPerBrKey.IsCommitted ? 1 : 0) : 0;
            yearsData.Treatment = budgetPerBrKey != null ? budgetPerBrKey.Treatment : "";
            return yearsData;
        }        
    }
}
