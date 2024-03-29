﻿using BridgeCare.Interfaces.SummaryReport;
using BridgeCare.Models;
using BridgeCare.Models.SummaryReport;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BridgeCare.Services
{
    public class CostBudgetsWorkSummary
    {
        private readonly BridgeWorkSummaryCommon bridgeWorkSummaryCommon;
        private readonly ExcelHelper excelHelper;
        private readonly BridgeWorkSummaryComputationHelper bridgeWorkSummaryComputationHelper;
        private Dictionary<int, double> TotalCulvertSpent = new Dictionary<int, double>();
        private Dictionary<int, double> TotalBridgeSpent = new Dictionary<int, double>();
        private Dictionary<int, double> TotalCommittedSpent = new Dictionary<int, double>();

        public CostBudgetsWorkSummary(BridgeWorkSummaryCommon bridgeWorkSummaryCommon, ExcelHelper excelHelper, BridgeWorkSummaryComputationHelper bridgeWorkSummaryComputationHelper)
        {
            this.bridgeWorkSummaryCommon = bridgeWorkSummaryCommon;
            this.excelHelper = excelHelper;
            this.bridgeWorkSummaryComputationHelper = bridgeWorkSummaryComputationHelper;
        }

        /// <summary>
        ///  Fill sections with cost and budget details
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="currentCell"></param>
        /// <param name="simulationYears"></param>
        /// <param name="simulationDataModels"></param>
        public void FillCostBudgetWorkSummarySections(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            List<SimulationDataModel> simulationDataModels, Dictionary<int, List<double>> yearlyBudgetAmounts, List<string> treatments,
            List<WorkSummaryByBudgetModel> comittedProjectsData)
        {
            var committedTotalRow = FillCostOfCommittedWorkSection(worksheet, currentCell, simulationYears, comittedProjectsData);
            var culvertTotalRow = FillCostOfCulvertWorkSection(worksheet, currentCell, simulationYears, simulationDataModels, treatments);
            var bridgeTotalRow = FillCostOfBridgeWorkSection(worksheet, currentCell, simulationYears, simulationDataModels, treatments);            
            var budgetTotalRow = FillTotalBudgetSection(worksheet, currentCell, simulationYears, yearlyBudgetAmounts);
            FillRemainingBudgetSection(worksheet, simulationYears, currentCell, committedTotalRow, culvertTotalRow, bridgeTotalRow, budgetTotalRow);
        }

        private int FillCostOfCommittedWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            List<WorkSummaryByBudgetModel> comittedProjectsData)
        {
            bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of MPMS Work", "MPMS Work Type");
            var committedTotalRow = AddCostsOfCommittedWork(worksheet, simulationYears, currentCell, comittedProjectsData);
            return committedTotalRow;
        }

        private int FillCostOfCulvertWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<string> treatments)
        {
            bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of BAMS Culvert Work", "BAMS Culvert Work Type");
            var culvertTotalRow = AddCostsOfCulvertWork(worksheet, simulationDataModels, simulationYears, currentCell, treatments);
            return culvertTotalRow;
        }

        private int FillCostOfBridgeWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<string> treatments)
        {
            bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of BAMS Bridge Work", "BAMS Bridge Work Type");
            var bridgeTotalRow = AddCostsOfBridgeWork(worksheet, simulationDataModels, simulationYears, currentCell, treatments);
            return bridgeTotalRow;
        }

        private int FillTotalBudgetSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, Dictionary<int, List<double>> yearlyBudgetAmounts)
        {
            bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Budget", "Totals");
            worksheet.Cells[currentCell.Row, simulationYears.Count + 3].Value = "Total Analysis Budget (all year)";
            excelHelper.ApplyStyle(worksheet.Cells[currentCell.Row, simulationYears.Count + 3]);
            excelHelper.ApplyBorder(worksheet.Cells[currentCell.Row, simulationYears.Count + 3]);
            var budgetTotalRow = AddDetailsForTotalBudget(worksheet, simulationYears, currentCell, yearlyBudgetAmounts);
            return budgetTotalRow;
        }

        private void FillRemainingBudgetSection(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, int committedTotalRow, int culvertTotalRow, int bridgeTotalRow, int budgetTotalRow)
        {
            bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Budget Analysis", "");
            worksheet.Cells[currentCell.Row, simulationYears.Count + 3].Value = "Total Remaining Budget(all years)";
            excelHelper.ApplyStyle(worksheet.Cells[currentCell.Row, simulationYears.Count + 3]);
            excelHelper.ApplyBorder(worksheet.Cells[currentCell.Row, simulationYears.Count + 3]);
            AddDetailsForRemainingBudget(worksheet, simulationYears, currentCell, committedTotalRow, culvertTotalRow, bridgeTotalRow, budgetTotalRow);
        }

        private int AddCostsOfCommittedWork(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell,
            List<WorkSummaryByBudgetModel> comittedProjectsData)
        {
            var startYear = simulationYears[0];
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            currentCell.Column = column;
            var committedTotalRow = 0;

            var uniqueTreatments = new Dictionary<string, int>();
            var costForTreatments = new Dictionary<string, double>();
            foreach (var data in comittedProjectsData)
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
                    costForTreatments.Add(data.TREATMENT, data.CostPerTreatmentPerYear);
                    currentCell.Row += 1;
                }
                else
                {
                    var cellToEnterCost = data.YEARS - startYear;
                    worksheet.Cells[uniqueTreatments[data.TREATMENT], currentCell.Column + cellToEnterCost + 2].Value = data.CostPerTreatmentPerYear;
                }
            }
            row = currentCell.Row;
            column = currentCell.Column;
            worksheet.Cells[row, column].Value = Properties.Resources.CommittedTotal;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                var yearlyBudget = comittedProjectsData.FindAll(_ => _.YEARS == year);
                var aggregateAmountPerYear = yearlyBudget.Sum(s => s.CostPerTreatmentPerYear);
                column = ++column;

                worksheet.Cells[row, column].Value = aggregateAmountPerYear;
                TotalCommittedSpent.Add(year, aggregateAmountPerYear);
                committedTotalRow = row;
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            excelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");
            excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(198, 224, 180));
            excelHelper.ApplyColor(worksheet.Cells[committedTotalRow, fromColumn, committedTotalRow, column], Color.FromArgb(84, 130, 53));
            excelHelper.SetTextColor(worksheet.Cells[committedTotalRow, fromColumn, committedTotalRow, column], Color.White);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
            return committedTotalRow;
        }

        private int AddCostsOfCulvertWork(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, CurrentCell currentCell, List<string> treatments)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            int culvertTotalRow = 0;
            foreach (var item in treatments)
            {
                if (item.ToLower().Contains("culvert"))
                {
                    worksheet.Cells[row++, column].Value = item;
                }
            }
            worksheet.Cells[row++, column].Value = Properties.Resources.CulvertTotal;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                double culvertTotalCost = 0;
                row = startRow;
                column = ++column;

                foreach (var item in treatments)
                {
                    if (item.ToLower().Contains("culvert"))
                    {
                        var culvertCost = bridgeWorkSummaryComputationHelper.CalculateCost(simulationDataModels, year, item);
                        worksheet.Cells[row++, column].Value = culvertCost;
                        culvertTotalCost += culvertCost;
                    }
                }
                worksheet.Cells[row, column].Value = culvertTotalCost;
                culvertTotalRow = row;
                TotalCulvertSpent.Add(year, culvertTotalCost);
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            excelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");
            excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(198, 224, 180));

            excelHelper.ApplyColor(worksheet.Cells[culvertTotalRow, fromColumn, culvertTotalRow, column], Color.FromArgb(84, 130, 53));
            excelHelper.SetTextColor(worksheet.Cells[culvertTotalRow, fromColumn, culvertTotalRow, column], Color.White);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
            return culvertTotalRow;
        }              

        private int AddDetailsForTotalBudget(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, Dictionary<int, List<double>> yearlyBudgetAmounts)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            int budgetTotalRow = 0;
            worksheet.Cells[row++, column].Value = Properties.Resources.TotalSpent;
            worksheet.Cells[row++, column].Value = Properties.Resources.TotalBridgeCareBudget;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                worksheet.Cells[row++, column].Value = TotalCulvertSpent[year] + TotalBridgeSpent[year] + TotalCommittedSpent[year];
                worksheet.Cells[row, column].Value = yearlyBudgetAmounts.ContainsKey(year) ? yearlyBudgetAmounts[year].Sum() : 0;
                budgetTotalRow = row;
            }
            worksheet.Cells[row, column + 1].Formula = "SUM(" + worksheet.Cells[row, fromColumn, row, column] + ")";

            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column + 1]);
            excelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column + 1], "NegativeCurrency");
            excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(84, 130, 53));
            excelHelper.SetTextColor(worksheet.Cells[startRow, fromColumn, row, column], Color.White);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
            return budgetTotalRow;
        }

        private void AddDetailsForRemainingBudget(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, int committedTotalRow,
            int culvertTotalRow, int bridgeTotalRow, int budgetTotalRow)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row++, column].Value = Properties.Resources.RemainingBudget;
            worksheet.Cells[row++, column].Value = Properties.Resources.PercentBudgetSpentMPMS;
            worksheet.Cells[row++, column].Value = Properties.Resources.PercentBudgetSpentBAMS;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                var totalSpent = Convert.ToDouble(worksheet.Cells[culvertTotalRow, column].Value) +
                    Convert.ToDouble(worksheet.Cells[bridgeTotalRow, column].Value) +
                    Convert.ToDouble(worksheet.Cells[committedTotalRow, column].Value);

                worksheet.Cells[row, column].Value = Convert.ToDouble(worksheet.Cells[budgetTotalRow, column].Value) - totalSpent;
                row++;

                worksheet.Cells[row, column].Formula = worksheet.Cells[committedTotalRow, column] + "/" + totalSpent;
                row++;

                worksheet.Cells[row, column].Formula = 1 + "-" + worksheet.Cells[row - 1, column];
            }
            worksheet.Cells[startRow, column + 1].Formula = "SUM(" + worksheet.Cells[startRow, fromColumn, startRow, column] + ")";
            worksheet.Cells[startRow, column + 2].Formula = worksheet.Cells[startRow, column + 1] + "/" + worksheet.Cells[budgetTotalRow, column + 1];
            worksheet.Cells[startRow, column + 2].Style.Numberformat.Format = "#0.00%";
            worksheet.Cells[startRow, column + 3].Value = "Percentage of Total Budget that was Unspent";

            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column + 1]);

            excelHelper.SetCustomFormat(worksheet.Cells[row - 2, fromColumn, row - 2, column + 1], "NegativeCurrency");
            excelHelper.SetCustomFormat(worksheet.Cells[row - 1, fromColumn, row, column], "Percentage");

            excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, startRow, column], Color.Red);
            excelHelper.ApplyColor(worksheet.Cells[startRow + 1, fromColumn, row, column], Color.FromArgb(248, 203, 173));
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 3, column);
            excelHelper.ApplyColor(worksheet.Cells[row + 2, startColumn, row + 2, column], Color.DimGray);
        }

        private int AddCostsOfBridgeWork(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, CurrentCell currentCell, List<string> treatments)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            int bridgeTotalRow = 0;
            foreach (var item in treatments)
            {
                if (!item.ToLower().Contains("culvert") && !item.ToLower().Contains("no treatment"))
                {
                    worksheet.Cells[row++, column].Value = item;
                }
            }
            worksheet.Cells[row++, column].Value = Properties.Resources.BridgeTotal;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                double nonCulvertTotalCost = 0;

                foreach (var item in treatments)
                {
                    if (!item.ToLower().Contains("culvert") && !item.ToLower().Contains("no treatment"))
                    {
                        var nonCulvertCost = bridgeWorkSummaryComputationHelper.CalculateCost(simulationDataModels, year, item);
                        worksheet.Cells[row++, column].Value = nonCulvertCost;
                        nonCulvertTotalCost += nonCulvertCost;
                    }
                }

                worksheet.Cells[row, column].Value = nonCulvertTotalCost;
                bridgeTotalRow = row;
                TotalBridgeSpent.Add(year, nonCulvertTotalCost);
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            excelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");
            excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(198, 224, 180));

            excelHelper.ApplyColor(worksheet.Cells[bridgeTotalRow, fromColumn, bridgeTotalRow, column], Color.FromArgb(84, 130, 53));
            excelHelper.SetTextColor(worksheet.Cells[bridgeTotalRow, fromColumn, bridgeTotalRow, column], Color.White);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
            return bridgeTotalRow;
        }
    }
}
