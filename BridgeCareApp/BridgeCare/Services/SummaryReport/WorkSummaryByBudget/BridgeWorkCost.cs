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
    public class BridgeWorkCost
    {
        private readonly ExcelHelper excelHelper;
        private readonly BridgeWorkSummaryCommon bridgeWorkSummaryCommon;

        public BridgeWorkCost(ExcelHelper excelHelper, BridgeWorkSummaryCommon bridgeWorkSummaryCommon)
        {
            this.excelHelper = excelHelper;
            this.bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
        }
        internal void FillCostOfBridgeWork(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<WorkSummaryByBudgetModel> costForBridgeBudgets, Dictionary<int, double> totalBudgetPerYearForBridgeWork)
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
    }
}
