using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BridgeCare.Interfaces;
using BridgeCare.Interfaces.SummaryReport;
using BridgeCare.Models;
using BridgeCare.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCare.Services.SummaryReport.WorkSummaryByBudget
{
    public class BridgeWorkSummaryByBudget
    {
        private readonly IWorkSummaryByBudget workSummaryByBudgetData;
        private readonly ExcelHelper excelHelper;
        private readonly BridgeCareContext dbContext;
        private readonly BridgeWorkSummaryCommon bridgeWorkSummaryCommon;
        private readonly IBridgeData bridgeData;
        private readonly IBridgeWorkSummaryData bridgeWorkSummaryData;
        private readonly CulvertCost culvertCost;
        private readonly BridgeWorkCost bridgeWorkCost;
        private readonly CommittedProjectsCost committedProjectsCost;

        public BridgeWorkSummaryByBudget(BridgeCareContext context, IWorkSummaryByBudget summaryByBudget,
               ExcelHelper excelHelper, BridgeWorkSummaryCommon bridgeWorkSummaryCommon, IBridgeData bridgeData, IBridgeWorkSummaryData bridgeWorkSummaryData,
               CulvertCost culvertCost, BridgeWorkCost bridgeWorkCost, CommittedProjectsCost committedProjectsCost)
        {
            workSummaryByBudgetData = summaryByBudget;
            this.excelHelper = excelHelper;
            this.bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
            this.bridgeData = bridgeData ?? throw new ArgumentNullException(nameof(bridgeData));
            this.bridgeWorkSummaryData = bridgeWorkSummaryData ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryData));

            this.culvertCost = culvertCost ?? throw new ArgumentNullException(nameof(culvertCost));
            this.bridgeWorkCost = bridgeWorkCost ?? throw new ArgumentNullException(nameof(bridgeWorkCost));
            this.committedProjectsCost = committedProjectsCost ?? throw new ArgumentNullException(nameof(committedProjectsCost));

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

            var comittedProjectsData = workSummaryByBudgetData.GetCommittedProjectsBudget(simulationModel, dbContext);
            var budgetForCommittedProjects = comittedProjectsData.Select(_ => _.BUDGET).Distinct().ToList();
            var budgetsOnlyForMPMS = budgetForCommittedProjects.Where(item => !budgets.Any(budget => budget.Equals(item))).ToList();

            committedProjectsCost.FillCommittedProjectsBudget(worksheet, currentCell, simulationYears, budgetsOnlyForMPMS, comittedProjectsData);

            //if(budgetsPerYearPerTreatment.Count != 0)
            //{
            //    budgetsPerYearPerTreatment.ForEach(f => {
            //        var amount = f.CostPerTreatmentPerYear;
            //        if (f.CostPerTreatmentPerYear >= 500)
            //        {
            //            f.CostPerTreatmentPerYear = amount % 1000 >= 500 ? amount + 1000 - amount % 1000 : amount - amount % 1000;
            //        }
            //        else
            //        {
            //            f.CostPerTreatmentPerYear = 1000;
            //        }
            //    });
            //}

            var startRow = currentCell.Row + 3;
            foreach (var budget in budgets)
            {
                //Filtering treatments for the given budget
                var costForCulvertBudget = budgetsPerYearPerTreatment
                                             .FindAll(_ => _.BUDGET.Equals(budget) && _.TREATMENT.ToLower().Contains("culvert"));
                                             
                var costForBridgeBudgets = budgetsPerYearPerTreatment
                                             .FindAll(_ => _.BUDGET.Equals(budget) && !_.TREATMENT.ToLower().Contains("culvert"));

                var filteredCommittedProject = comittedProjectsData.FindAll(_ => _.BUDGET.Equals(budget));

                if (costForCulvertBudget.Count == 0 && costForBridgeBudgets.Count == 0 && filteredCommittedProject.Count == 0)
                {
                    continue;
                }
                
                var totalBudgetPerYearForCulvert = new Dictionary<int, double>();
                var totalBudgetPerYearForBridgeWork = new Dictionary<int, double>();
                var totalBudgetPerYearForMPMS = new Dictionary<int, double>();

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

                    yearlyBudget = filteredCommittedProject.FindAll(_ => _.YEARS == year);
                    mpmsBudget = yearlyBudget.Sum(s => s.CostPerTreatmentPerYear);
                    totalBudgetPerYearForMPMS.Add(year, mpmsBudget);

                    totalSpent.Add((year, culvertAmountSum + budgetAmountSum + mpmsBudget));
                }

                currentCell.Column = 1;
                currentCell.Row += 2;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = budget;
                excelHelper.MergeCells(worksheet, currentCell.Row, currentCell.Column, currentCell.Row, simulationYears.Count);
                excelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row, simulationYears.Count + 2], Color.Gray);
                currentCell.Row += 1;

                culvertCost.FillCostOfCulvert(worksheet, currentCell, costForCulvertBudget, totalBudgetPerYearForCulvert, simulationYears);

                bridgeWorkCost.FillCostOfBridgeWork(worksheet, currentCell, simulationYears, costForBridgeBudgets, totalBudgetPerYearForBridgeWork);

                committedProjectsCost.FillCostOfMPMSWork(worksheet, currentCell, simulationYears, filteredCommittedProject, totalBudgetPerYearForMPMS);
                

                currentCell.Row += 1;
                bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Budget", "Totals");
                currentCell.Row += 1;
                currentCell.Column = 1;
                var startOfTotalBudget = currentCell.Row;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.TotalSpent;
                foreach (var spentAmount in totalSpent)
                {
                    var cellFortotalSpentAmount = spentAmount.year - startYear;
                    worksheet.Cells[currentCell.Row, currentCell.Column + cellFortotalSpentAmount + 2].Value = spentAmount.amount;
                }
                excelHelper.ApplyColor(worksheet.Cells[startOfTotalBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2],
                    Color.FromArgb(84, 130, 53));
                excelHelper.SetTextColor(worksheet.Cells[startOfTotalBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.White);

                var totalBridgeCareBudget = yearlyBudgetModels.FindAll(_ => _.BudgetName.Equals(budget.Replace("'", "")))
                    .OrderBy(_ => _.Year).ToList();
                currentCell.Row += 2;
                currentCell.Column = 1;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.TotalBridgeCareBudget;

                foreach (var totalBudget in totalBridgeCareBudget)
                {
                    var cellFortotalBudget = totalBudget.Year - startYear;
                    worksheet.Cells[currentCell.Row, currentCell.Column + cellFortotalBudget + 2].Value = totalBudget.BudgetAmount;
                }
                excelHelper.ApplyBorder(worksheet.Cells[startOfTotalBudget, currentCell.Column, currentCell.Row, simulationYears.Count + 2]);
                excelHelper.SetCustomFormat(worksheet.Cells[startOfTotalBudget, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], "NegativeCurrency");

                excelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2],
                    Color.FromArgb(84, 130, 53));
                excelHelper.SetTextColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.White);

                currentCell.Row += 1;
                bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Budget Analysis", "");
                currentCell.Row += 1;
                currentCell.Column = 1;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = Properties.Resources.RemainingBudget;
                worksheet.Cells[currentCell.Row + 1, currentCell.Column].Value = Properties.Resources.PercentBudgetSpentMPMS;
                worksheet.Cells[currentCell.Row + 2, currentCell.Column].Value = Properties.Resources.PercentBudgetSpentBAMS;

                foreach (var budgetSum in totalBridgeCareBudget)
                {
                    var perYearTotalSpent = totalSpent.Find(_ => _.year == budgetSum.Year);
                    var cellFortotalBudget = budgetSum.Year - startYear;
                    worksheet.Cells[currentCell.Row, currentCell.Column + cellFortotalBudget + 2].Value = budgetSum.BudgetAmount - perYearTotalSpent.amount;

                    worksheet.Cells[currentCell.Row + 1, currentCell.Column + cellFortotalBudget + 2].Value =
                        totalBudgetPerYearForMPMS[budgetSum.Year]/perYearTotalSpent.amount;

                    worksheet.Cells[currentCell.Row + 2, currentCell.Column + cellFortotalBudget + 2].Value = 1 -
                        totalBudgetPerYearForMPMS[budgetSum.Year] / perYearTotalSpent.amount;
                }
                excelHelper.ApplyBorder(worksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row + 2, simulationYears.Count + 2]);

                excelHelper.SetCustomFormat(worksheet.Cells[currentCell.Row + 1, currentCell.Column + 2,
                    currentCell.Row + 2, simulationYears.Count + 2], "Percentage");
                excelHelper.SetCustomFormat(worksheet.Cells[currentCell.Row, currentCell.Column + 2,
                    currentCell.Row, simulationYears.Count + 2], "NegativeCurrency");

                excelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2],
                    Color.Red);
                excelHelper.ApplyColor(worksheet.Cells[currentCell.Row + 1, currentCell.Column + 2, currentCell.Row + 2, simulationYears.Count + 2], Color.FromArgb(248, 203, 173));
                currentCell.Row += 2;
            }
            worksheet.Cells.AutoFitColumns();
        }
    }
}
