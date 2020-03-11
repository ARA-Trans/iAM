using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using BridgeCare.Models;
using BridgeCare.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCare.Services.SummaryReport.WorkSummaryByBudget
{
    public class CommittedProjectsCost
    {
        private readonly ExcelHelper excelHelper;
        private readonly BridgeWorkSummaryCommon bridgeWorkSummaryCommon;

        public CommittedProjectsCost(ExcelHelper excelHelper, BridgeWorkSummaryCommon bridgeWorkSummaryCommon)
        {
            this.excelHelper = excelHelper;
            this.bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
        }
        internal void FillCostOfMPMSWork(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<WorkSummaryByBudgetModel> filteredCommittedProject, Dictionary<int, double> totalBudgetPerYearForMPMS)
        {
            var startYear = simulationYears[0];
            currentCell.Row += 1;
            bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of MPMS Work");

            currentCell.Row += 1;
            var startOfMPMSBudget = currentCell.Row += 1;
            currentCell.Column = 1;

            var uniqueTreatments = new Dictionary<string, int>();
            foreach (var data in filteredCommittedProject)
            {
                if (data.YEARS < startYear)
                {
                    continue;
                }
                if (!uniqueTreatments.ContainsKey(data.TREATMENT))
                {
                    uniqueTreatments.Add(data.TREATMENT, currentCell.Row);
                    worksheet.Cells[currentCell.Row, currentCell.Column].Value = data.TREATMENT;
                    var cellToEnterCost = data.YEARS - startYear;
                    worksheet.Cells[uniqueTreatments[data.TREATMENT], currentCell.Column + cellToEnterCost + 2].Value = data.CostPerTreatmentPerYear;
                    currentCell.Row += 1;
                }
                else
                {
                    var cellToEnterCost = data.YEARS - startYear;
                    worksheet.Cells[uniqueTreatments[data.TREATMENT], currentCell.Column + cellToEnterCost + 2].Value = data.CostPerTreatmentPerYear;
                }
            }

            worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.MPMSTotal;

            foreach (var totalMPMSBudget in totalBudgetPerYearForMPMS)
            {
                var cellToEnterTotalMPMSCost = totalMPMSBudget.Key - startYear;
                worksheet.Cells[currentCell.Row, currentCell.Column + cellToEnterTotalMPMSCost + 2].Value = totalMPMSBudget.Value;
            }

            excelHelper.ApplyBorder(worksheet.Cells[startOfMPMSBudget, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
            excelHelper.SetCustomFormat(worksheet.Cells[startOfMPMSBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], "NegativeCurrency");
            excelHelper.ApplyColor(worksheet.Cells[startOfMPMSBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.DarkSeaGreen);
        }

        internal void FillCommittedProjectsBudget(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<string> budgetsOnlyForMPMS, List<WorkSummaryByBudgetModel> comittedProjectsData)
        {
            var startYear = simulationYears[0];
            var totalMPMSAccrossBudgets = new Dictionary<int, double>();
            foreach (var item in budgetsOnlyForMPMS)
            {
                var filteredCommittedProject = comittedProjectsData.FindAll(_ => _.BUDGET == item);

                var totalBudgetPerYearForMPMS = new Dictionary<int, double>();
                // Filling up the total, "MPMS" costs
                foreach (var year in simulationYears)
                {
                    var yearlyBudget = filteredCommittedProject.FindAll(_ => _.YEARS == year);
                    var MPMSAmountSum = yearlyBudget.Sum(s => s.CostPerTreatmentPerYear);
                    totalBudgetPerYearForMPMS.Add(year, MPMSAmountSum);

                    yearlyBudget.Clear();
                }

                currentCell.Column = 1;
                currentCell.Row += 2;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = item;
                excelHelper.MergeCells(worksheet, currentCell.Row, currentCell.Column, currentCell.Row, simulationYears.Count);
                excelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row, simulationYears.Count + 2], Color.Gray);
                currentCell.Row += 1;

                bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of MPMS Work");

                currentCell.Row += 1;
                var startOfMPMSBudget = currentCell.Row += 1;
                currentCell.Column = 1;

                var uniqueTreatments = new Dictionary<string, int>();
                foreach (var data in filteredCommittedProject)
                {
                    if (data.YEARS < startYear || data.TREATMENT.ToLower() == "no treatment")
                    {
                        continue;
                    }
                    if (!uniqueTreatments.ContainsKey(data.TREATMENT))
                    {
                        uniqueTreatments.Add(data.TREATMENT, currentCell.Row);
                        worksheet.Cells[currentCell.Row, currentCell.Column].Value = data.TREATMENT;
                        var cellToEnterCost = data.YEARS - startYear;
                        worksheet.Cells[uniqueTreatments[data.TREATMENT], currentCell.Column + cellToEnterCost + 2].Value = data.CostPerTreatmentPerYear;
                        currentCell.Row += 1;
                    }
                    else
                    {
                        var cellToEnterCost = data.YEARS - startYear;
                        worksheet.Cells[uniqueTreatments[data.TREATMENT], currentCell.Column + cellToEnterCost + 2].Value = data.CostPerTreatmentPerYear;
                    }
                }

                worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.MPMSTotal;

                foreach (var totalMPMSBudget in totalBudgetPerYearForMPMS)
                {
                    var cellToEnterTotalMPMSCost = totalMPMSBudget.Key - startYear;
                    worksheet.Cells[currentCell.Row, currentCell.Column + cellToEnterTotalMPMSCost + 2].Value = totalMPMSBudget.Value;
                }

                excelHelper.ApplyBorder(worksheet.Cells[startOfMPMSBudget, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
                excelHelper.SetCustomFormat(worksheet.Cells[startOfMPMSBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], "NegativeCurrency");
                excelHelper.ApplyColor(worksheet.Cells[startOfMPMSBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.DarkSeaGreen);
            }
        }
    }
}
