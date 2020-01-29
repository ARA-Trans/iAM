using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.Interfaces;
using BridgeCare.Interfaces.SummaryReport;
using BridgeCare.Models;
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

        public BridgeWorkSummaryByBudget(BridgeCareContext context, IWorkSummaryByBudget summaryByBudget,
            BridgeDataHelper bridgeDataHelper, ExcelHelper excelHelper, BridgeWorkSummaryCommon bridgeWorkSummaryCommon, IBridgeData bridgeData)
        {
            workSummaryByBudgetData = summaryByBudget;
            this.excelHelper = excelHelper;
            this.bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
            this.bridgeData = bridgeData ?? throw new ArgumentNullException(nameof(bridgeData));
            dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1304:Specify CultureInfo", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "<Pending>")]
        internal void Fill(ExcelWorksheet worksheet, SimulationModel simulationModel, List<int> simulationYears, List<string> treatments)
        {
            var startYear = simulationYears[0];
            var currentCell = new CurrentCell { Row = 1, Column = 1 };
            var budgetsPerYearPerTreatment = workSummaryByBudgetData.GetworkSummaryByBudgetsData(simulationModel, dbContext);
            var budgets = bridgeData.GetBudgets(simulationModel.SimulationId, dbContext);

            var startRow = currentCell.Row + 3;
            foreach (var budget in budgets)
            {
                var costForCulvertBudget = budgetsPerYearPerTreatment
                                             .FindAll(_ => _.BUDGET.Equals(budget.Replace("'", "")) && _.TREATMENT.ToLower().Contains("culvert"));
                var costForBridgeBudgets = budgetsPerYearPerTreatment
                                             .FindAll(_ => _.BUDGET.Equals(budget.Replace("'", "")) && !_.TREATMENT.ToLower().Contains("culvert"));
                if (costForCulvertBudget.Count == 0 && costForBridgeBudgets.Count == 0)
                {
                    continue;
                }
                currentCell.Column = 1;
                currentCell.Row += 1;
                worksheet.Cells[currentCell.Row, currentCell.Column].Value = budget;
                excelHelper.MergeCells(worksheet, currentCell.Row, currentCell.Column, currentCell.Row, simulationYears.Count);
                currentCell.Row += 1;

                bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of Culvert Work");

                var totalBudgetPerYearForCulvert = new Dictionary<int, double>();
                var totalBudgetPerYearForBridgeWork = new Dictionary<int, double>();
                foreach (var year in simulationYears)
                {
                    var yearlyBudget = costForCulvertBudget.FindAll(_ => _.YEARS == year);
                    var sum = yearlyBudget.Sum(s => s.CostPerTreatmentPerYear);
                    totalBudgetPerYearForCulvert.Add(year, sum);

                    yearlyBudget.Clear();

                    yearlyBudget = costForBridgeBudgets.FindAll(_ => _.YEARS == year);
                    sum = yearlyBudget.Sum(s => s.CostPerTreatmentPerYear);
                    totalBudgetPerYearForBridgeWork.Add(year, sum);
                }

                currentCell.Row += 1;
                currentCell.Column = 1;

                //var row = currentCell.Row;
                //var column = currentCell.Column;
                //foreach (var item in treatments)
                //{
                //    if (item.ToLower().Contains("culvert"))
                //    {
                //        worksheet.Cells[row++, column].Value = item;
                //    }
                //}
                //worksheet.Cells[row++, column].Value = Properties.Resources.CulvertTotal;
                //column++;
                //var fromColumn = column + 1;

                //foreach (var year in simulationYears)
                //{
                //    double culvertTotalCost = 0;
                //    row = startRow + 2;
                //    column = ++column;

                //    foreach (var item in treatments)
                //    {
                //        if (item.ToLower().Contains("culvert"))
                //        {
                //            var culvertCost = bridgeWorkSummaryComputationHelper.CalculateCost(simulationDataModels, year, item);
                //            worksheet.Cells[row++, column].Value = culvertCost;
                //            culvertTotalCost += culvertCost;
                //        }
                //    }
                //    worksheet.Cells[row, column].Value = culvertTotalCost;
                //    culvertTotalRow = row;
                //}
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
                currentCell.Row += 1;
                bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of Bridge Work");

                currentCell.Row += 1;
                currentCell.Column = 1;
                uniqueTreatments.Clear();
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
            }
            worksheet.Cells.AutoFitColumns();
        }
    }
}
