﻿using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BridgeCare.Services
{
    public static class SummaryReportHelper
    {
        public static List<SimulationDataModel> GetSimulationDataModels(DataTable simulationDataTable, List<int> simulationYears, IQueryable<ReportProjectCost> projectCostModels)
        {
            var simulationDataModels = new List<SimulationDataModel>();
            var projectCostsList = projectCostModels.ToList();
            foreach (DataRow simulationRow in simulationDataTable.Rows)
            {
                var simulationDM = CreatePrevYearSimulationMdel(simulationRow);
                var projectCostEntry = projectCostsList.Where(pc => pc.SECTIONID == Convert.ToUInt32(simulationRow["SECTIONID"])).FirstOrDefault();
                AddAllYearsData(simulationRow, simulationYears, projectCostEntry, simulationDM);
                simulationDataModels.Add(simulationDM);
            }

            return simulationDataModels;
        }

        #region Private methods

        private static SimulationDataModel CreatePrevYearSimulationMdel(DataRow simulationRow)
        {
            YearsData yearsData = AddYearsData(simulationRow, null, 0);
            return new SimulationDataModel
            {
                YearsData = new List<YearsData>() { yearsData },
                SectionId = Convert.ToInt32(simulationRow["SECTIONID"])
            };
        }

        private static void AddAllYearsData(DataRow simulationRow, List<int> simulationYears, ReportProjectCost projectCostEntry, SimulationDataModel simulationDM)
        {
            var yearsDataModels = new List<YearsData>();
            foreach (int year in simulationYears)
            {
                yearsDataModels.Add(AddYearsData(simulationRow, projectCostEntry, year));
            }
            simulationDM.YearsData.AddRange(yearsDataModels.OrderBy(y => y.Year).ToList());
        }

        private static YearsData AddYearsData(DataRow simulationRow, ReportProjectCost projectCostEntry, int year)
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
            yearsData.SD = Convert.ToDouble(yearsData.DeckD) < 5 ? "Y" : "N";

            yearsData.Project = year != 0 ? projectCostEntry?.TREATMENT : string.Empty;
            yearsData.Cost = year != 0 ? (projectCostEntry == null ? 0 : projectCostEntry.COST_) : 0;
            yearsData.Project = yearsData.Cost == 0 ? "No Treatment" : yearsData.Project;
            return yearsData;
        }

        #endregion Private methods
    }
}