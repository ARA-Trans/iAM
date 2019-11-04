using BridgeCare.Models;
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
        public void FillCostBudgetWorkSummarySections(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<InvestmentLibraryBudgetYearModel> yearlyBudgetModels, List<string> treatments)
        {
            var culvertTotalRow = FillCostOfCulvertWorkSection(worksheet, currentCell, simulationYears, simulationDataModels, treatments);
            var bridgeTotalRow = FillCostOfBridgeWorkSection(worksheet, currentCell, simulationYears, simulationDataModels, treatments);            
            var budgetTotalRow = FillTotalBudgetSection(worksheet, currentCell, simulationYears, yearlyBudgetModels);
            FillRemainingBudgetSection(worksheet, simulationYears, currentCell, culvertTotalRow, bridgeTotalRow, budgetTotalRow);
        }

        private int FillCostOfCulvertWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<string> treatments)
        {
            bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of Culvert Work");
            var culvertTotalRow = AddCostsOfCulvertWork(worksheet, simulationDataModels, simulationYears, currentCell, treatments);
            return culvertTotalRow;
        }

        private int FillCostOfBridgeWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<string> treatments)
        {
            bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of Bridge Work");
            var bridgeTotalRow = AddCostsOfBridgeWork(worksheet, simulationDataModels, simulationYears, currentCell, treatments);
            return bridgeTotalRow;
        }

        private int FillTotalBudgetSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<InvestmentLibraryBudgetYearModel> yearlyBudgetModels)
        {
            bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Budget");
            var budgetTotalRow = AddDetailsForTotalBudget(worksheet, simulationYears, currentCell, yearlyBudgetModels);
            return budgetTotalRow;
        }

        private void FillRemainingBudgetSection(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, int culvertTotalRow, int bridgeTotalRow, int budgetTotalRow)
        {
            bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Remaining Budget");
            AddDetailsForRemainingBudget(worksheet, simulationYears, currentCell, culvertTotalRow, bridgeTotalRow, budgetTotalRow);
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
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            excelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");
            excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.DarkSeaGreen);            
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
            return culvertTotalRow;
        }              

        private int AddDetailsForTotalBudget(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, List<InvestmentLibraryBudgetYearModel> yearlyBudgetModels)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            int budgetTotalRow = 0;           
            worksheet.Cells[row++, column].Value = Properties.Resources.Total;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;              

                worksheet.Cells[row, column].Value = yearlyBudgetModels.Find(b => b.Year == year).Budget.Sum(m => m.budgetAmount ?? 0);
                budgetTotalRow = row;
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            excelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");
            excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.OliveDrab);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
            return budgetTotalRow;
        }

        private void AddDetailsForRemainingBudget(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, int culvertTotalRow, int bridgeTotalRow, int budgetTotalRow)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row++, column].Value = Properties.Resources.Total;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                
                worksheet.Cells[row, column].Value = Convert.ToDouble(worksheet.Cells[budgetTotalRow, column].Value) - (Convert.ToDouble(worksheet.Cells[culvertTotalRow, column].Value) + Convert.ToDouble(worksheet.Cells[bridgeTotalRow, column].Value));
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            excelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");
            excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.LightSalmon);
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
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            excelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");
            excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.DarkSeaGreen);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
            return bridgeTotalRow;
        }
    }
}