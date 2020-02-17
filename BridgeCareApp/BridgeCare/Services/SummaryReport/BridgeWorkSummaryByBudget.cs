using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using BridgeCare.Interfaces;
using BridgeCare.Interfaces.SummaryReport;
using BridgeCare.Models;
using BridgeCare.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCare.Services.SummaryReport
{
    public class BridgeWorkSummaryByBudget
    {
        private readonly IWorkSummaryByBudget workSummaryByBudgetData;
        private readonly ExcelHelper excelHelper;
        private readonly BridgeCareContext dbContext;
        private readonly BridgeWorkSummaryCommon bridgeWorkSummaryCommon;
        private readonly IBridgeData bridgeData;
        private readonly IBridgeWorkSummaryData bridgeWorkSummaryData;

        public BridgeWorkSummaryByBudget(BridgeCareContext context, IWorkSummaryByBudget summaryByBudget,
               ExcelHelper excelHelper, BridgeWorkSummaryCommon bridgeWorkSummaryCommon, IBridgeData bridgeData, IBridgeWorkSummaryData bridgeWorkSummaryData)
        {
            workSummaryByBudgetData = summaryByBudget;
            this.excelHelper = excelHelper;
            this.bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
            this.bridgeData = bridgeData ?? throw new ArgumentNullException(nameof(bridgeData));
            this.bridgeWorkSummaryData = bridgeWorkSummaryData ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryData));
            dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1304:Specify CultureInfo", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "<Pending>")]
        internal void Fill(ExcelWorksheet worksheet, SimulationModel simulationModel, List<int> simulationYears)
        {
            var startYear = simulationYears[0];
            var currentCell = new CurrentCell { Row = 1, Column = 1 };
            var budgetsPerYearPerTreatment = workSummaryByBudgetData.GetworkSummaryByBudgetsData(simulationModel, dbContext);
            var yearlyBudgetModels = bridgeWorkSummaryData.GetYearlyBudgetModels(simulationModel.simulationId, dbContext);

            var budgets = bridgeData.GetBudgets(simulationModel.simulationId, dbContext);

            var totalBudgetPerYearForMPMS = new Dictionary<int, double>();
            var totalMPMSAccrossBudgetsPerYear = FillCommittedProjectsBudget(worksheet, currentCell, simulationModel, simulationYears);
            
            var startRow = currentCell.Row + 3;
            foreach (var budget in budgets)
            {
                //Filtering treatments for the given budget
                var costForCulvertBudget = budgetsPerYearPerTreatment
                                             .FindAll(_ => _.BUDGET.Equals(budget.Replace("'", "")) && _.TREATMENT.ToLower().Contains("culvert"));
                var costForBridgeBudgets = budgetsPerYearPerTreatment
                                             .FindAll(_ => _.BUDGET.Equals(budget.Replace("'", "")) && !_.TREATMENT.ToLower().Contains("culvert"));

                if (costForCulvertBudget.Count == 0 && costForBridgeBudgets.Count == 0)
                {
                    continue;
                }

                var totalBudgetPerYearForCulvert = new Dictionary<int, double>();
                var totalBudgetPerYearForBridgeWork = new Dictionary<int, double>();
                var totalBudgetForCommittedProjects = new Dictionary<int, double>();

                var totalSpent = new List<(int year, double amount)>();

                // Filling up the total, "culvert" and "Bridge work" costs
                foreach (var year in simulationYears)
                {
                    var yearlyBudget = costForCulvertBudget.FindAll(_ => _.YEARS == year);
                    var culvertAmountSum = yearlyBudget.Sum(s => s.CostPerTreatmentPerYear);
                    totalBudgetPerYearForCulvert.Add(year, culvertAmountSum);

                    yearlyBudget.Clear();

                    yearlyBudget = costForBridgeBudgets.FindAll(_ => _.YEARS == year);
                    var budgetAmountSum = yearlyBudget.Sum(s => s.CostPerTreatmentPerYear);
                    totalBudgetPerYearForBridgeWork.Add(year, budgetAmountSum);

                    yearlyBudget.Clear();
                    var mpmsBudget = 0.0;

                    // Uncomment this if, total accross all MPMS budgets is required
                    //if (totalMPMSAccrossBudgetsPerYear.ContainsKey(year))
                    //{
                    //    mpmsBudget = totalMPMSAccrossBudgetsPerYear[year];
                    //}
                    totalSpent.Add((year, culvertAmountSum + budgetAmountSum + mpmsBudget));
                }

                currentCell.Column = 1;
                currentCell.Row += 2;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = budget;
                excelHelper.MergeCells(worksheet, currentCell.Row, currentCell.Column, currentCell.Row, simulationYears.Count);
                excelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row, simulationYears.Count + 2], Color.LightGray);
                currentCell.Row += 1;

                FillCostOfCulvert(worksheet, currentCell, costForCulvertBudget, totalBudgetPerYearForCulvert, simulationYears);


                FillCostOfBridgeWork(worksheet, currentCell, simulationYears, costForBridgeBudgets, totalBudgetPerYearForBridgeWork);
                

                currentCell.Row += 1;
                bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Budget");
                currentCell.Row += 1;
                currentCell.Column = 1;
                var startOfTotalBudget = currentCell.Row;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.TotalSpent;
                foreach (var spentAmount in totalSpent)
                {
                    var cellFortotalSpentAmount = spentAmount.year - startYear;
                    worksheet.Cells[currentCell.Row, currentCell.Column + cellFortotalSpentAmount + 2].Value = spentAmount.amount;
                }

                var totalBridgeCareBudget = yearlyBudgetModels.FindAll(_ => _.BudgetName.Equals(budget.Replace("'", ""))).OrderBy(_ => _.Year).ToList();
                currentCell.Row += 1;
                currentCell.Column = 1;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.TotalBridgeCareBudget;

                foreach (var totalBudget in totalBridgeCareBudget)
                {
                    var cellFortotalBudget = totalBudget.Year - startYear;
                    worksheet.Cells[currentCell.Row, currentCell.Column + cellFortotalBudget + 2].Value = totalBudget.BudgetAmount;
                }
                excelHelper.ApplyBorder(worksheet.Cells[startOfTotalBudget, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
                excelHelper.SetCustomFormat(worksheet.Cells[startOfTotalBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], "NegativeCurrency");
                excelHelper.ApplyColor(worksheet.Cells[startOfTotalBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.DarkOliveGreen);

                currentCell.Row += 1;
                bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Budget Analysis");
                currentCell.Row += 1;
                currentCell.Column = 1;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.RemainingBudget;

                foreach (var budgetSum in totalBridgeCareBudget)
                {
                    var perYearTotalSpent = totalSpent.Find(_ => _.year == budgetSum.Year);
                    var cellFortotalBudget = budgetSum.Year - startYear;
                    worksheet.Cells[currentCell.Row, currentCell.Column + cellFortotalBudget + 2].Value = budgetSum.BudgetAmount - perYearTotalSpent.amount;
                }
                excelHelper.ApplyBorder(worksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
                excelHelper.SetCustomFormat(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], "NegativeCurrency");
                excelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.IndianRed);

            }
            worksheet.Cells.AutoFitColumns();
        }

        private Dictionary<int, double> FillCommittedProjectsBudget(ExcelWorksheet worksheet, CurrentCell currentCell, SimulationModel simulationModel, List<int> simulationYears)
        {
            var comittedProjectsData = workSummaryByBudgetData.GetCommittedProjectsBudget(simulationModel, dbContext);
            var budgetForCommittedProjects = comittedProjectsData.Select(_ => _.BUDGET).Distinct().ToList();
            var startYearForCommittedProject = comittedProjectsData.Select(_ => _.YEARS).ToList();
            var startYear = simulationYears[0];
            var totalMPMSAccrossBudgets = new Dictionary<int, double>();
            foreach (var item in budgetForCommittedProjects)
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
                excelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row, simulationYears.Count + 2], Color.LightGray);
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

                    if (totalMPMSAccrossBudgets.ContainsKey(totalMPMSBudget.Key))
                    {
                        totalMPMSAccrossBudgets[totalMPMSBudget.Key] += totalMPMSBudget.Value;
                    }
                    else
                    {
                        totalMPMSAccrossBudgets.Add(totalMPMSBudget.Key, totalMPMSBudget.Value);
                    }
                }

                excelHelper.ApplyBorder(worksheet.Cells[startOfMPMSBudget, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
                excelHelper.SetCustomFormat(worksheet.Cells[startOfMPMSBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], "NegativeCurrency");
                excelHelper.ApplyColor(worksheet.Cells[startOfMPMSBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.DarkSeaGreen);
            }
            return totalMPMSAccrossBudgets;
        }

        private void FillCostOfBridgeWork(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<WorkSummaryByBudgetModel> costForBridgeBudgets, Dictionary<int, double> totalBudgetPerYearForBridgeWork)
        {
            var startYear = simulationYears[0];
            currentCell.Row += 1;
            bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of Bridge Work");

            currentCell.Row += 1;
            var startOfBridgeBudget = currentCell.Row;
            currentCell.Column = 1;
            var uniqueTreatments = new Dictionary<string, int>();
            // Fill Bridge Budget
            foreach (var item in costForBridgeBudgets)
            {
                if (!uniqueTreatments.ContainsKey(item.TREATMENT))
                {
                    uniqueTreatments.Add(item.TREATMENT, currentCell.Row);
                    worksheet.Cells[currentCell.Row, currentCell.Column].Value = item.TREATMENT;
                    var cellToEnterCost = item.YEARS - startYear;
                    worksheet.Cells[uniqueTreatments[item.TREATMENT], currentCell.Column + cellToEnterCost + 2].Value = item.CostPerTreatmentPerYear;
                    currentCell.Row += 1;
                }
                else
                {
                    var cellToEnterCost = item.YEARS - startYear;
                    worksheet.Cells[uniqueTreatments[item.TREATMENT], currentCell.Column + cellToEnterCost + 2].Value = item.CostPerTreatmentPerYear;
                }
            }
            worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.BridgeTotal;

            foreach (var totalBridgeBudget in totalBudgetPerYearForBridgeWork)
            {
                var cellToEnterTotalBridgeCost = totalBridgeBudget.Key - startYear;
                worksheet.Cells[currentCell.Row, currentCell.Column + cellToEnterTotalBridgeCost + 2].Value = totalBridgeBudget.Value;
            }
            excelHelper.ApplyBorder(worksheet.Cells[startOfBridgeBudget, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
            excelHelper.SetCustomFormat(worksheet.Cells[startOfBridgeBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], "NegativeCurrency");
            excelHelper.ApplyColor(worksheet.Cells[startOfBridgeBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.DarkSeaGreen);
        }

        private void FillCostOfCulvert(ExcelWorksheet worksheet, CurrentCell currentCell, List<WorkSummaryByBudgetModel> costForCulvertBudget, Dictionary<int, double> totalBudgetPerYearForCulvert, List<int> simulationYears)
        {
            var startYear = simulationYears[0];
            bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of Culvert Work");

            currentCell.Row += 1;
            var startOfCulvertBudget = currentCell.Row += 1;
            currentCell.Column = 1;

            var uniqueTreatments = new Dictionary<string, int>();
            foreach (var item in costForCulvertBudget)
            {
                if (!uniqueTreatments.ContainsKey(item.TREATMENT))
                {
                    uniqueTreatments.Add(item.TREATMENT, currentCell.Row);
                    worksheet.Cells[currentCell.Row, currentCell.Column].Value = item.TREATMENT;
                    var cellToEnterCost = item.YEARS - startYear;
                    worksheet.Cells[uniqueTreatments[item.TREATMENT], currentCell.Column + cellToEnterCost + 2].Value = item.CostPerTreatmentPerYear;
                    currentCell.Row += 1;
                }
                else
                {
                    var cellToEnterCost = item.YEARS - startYear;
                    worksheet.Cells[uniqueTreatments[item.TREATMENT], currentCell.Column + cellToEnterCost + 2].Value = item.CostPerTreatmentPerYear;
                }
            }

            worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.CulvertTotal;

            foreach (var totalculvertBudget in totalBudgetPerYearForCulvert)
            {
                var cellToEnterTotalCulvertCost = totalculvertBudget.Key - startYear;
                worksheet.Cells[currentCell.Row, currentCell.Column + cellToEnterTotalCulvertCost + 2].Value = totalculvertBudget.Value;
            }
            excelHelper.ApplyBorder(worksheet.Cells[startOfCulvertBudget, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
            excelHelper.SetCustomFormat(worksheet.Cells[startOfCulvertBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], "NegativeCurrency");
            excelHelper.ApplyColor(worksheet.Cells[startOfCulvertBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.DarkSeaGreen);
        }
    }
}
