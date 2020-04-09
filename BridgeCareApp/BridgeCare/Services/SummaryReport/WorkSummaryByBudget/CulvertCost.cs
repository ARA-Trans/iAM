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
    public class CulvertCost
    {
        private readonly ExcelHelper excelHelper;
        private readonly BridgeWorkSummaryCommon bridgeWorkSummaryCommon;

        public CulvertCost(ExcelHelper excelHelper, BridgeWorkSummaryCommon bridgeWorkSummaryCommon)
        {
            this.excelHelper = excelHelper;
            this.bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
        }
        internal void FillCostOfCulvert(ExcelWorksheet worksheet, CurrentCell currentCell, List<WorkSummaryByBudgetModel> costForCulvertBudget, Dictionary<int, double> totalBudgetPerYearForCulvert, List<int> simulationYears)
        {
            var startYear = simulationYears[0];
            bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of BAMS Culvert Work", "BAMS Culvert Work Type");

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

            excelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.FromArgb(84, 130, 53));
            excelHelper.SetTextColor(worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, simulationYears.Count + 2], Color.White);
        }
    }
}
