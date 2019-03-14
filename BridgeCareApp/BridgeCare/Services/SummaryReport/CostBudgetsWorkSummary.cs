using BridgeCare.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace BridgeCare.Services
{
    public class CostBudgetsWorkSummary
    {
    /// <summary>
    ///  Fill sections with cost and budget details
    /// </summary>
    /// <param name="worksheet"></param>
    /// <param name="currentCell"></param>
    /// <param name="simulationYears"></param>
    /// <param name="simulationDataModels"></param>
    public void FillCostBudgetWorkSummarySections(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            var culvertTotalRow = FillCostOfCulvertWorkSection(worksheet, currentCell, simulationYears, simulationDataModels);
            var bridgeTotalRow = FillCostOfBridgeWorkSection(worksheet, currentCell, simulationYears, simulationDataModels);
            // TODO ask why Total row is with same hard coded number 30000000 in excel?
            var budgetTotalRow = FillTotalBudgetSection(worksheet, currentCell, simulationYears);
            FillRemainingBudgetSection(worksheet, simulationYears, currentCell, culvertTotalRow, bridgeTotalRow, budgetTotalRow);
        }

        private int FillCostOfCulvertWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            BridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of Culvert Work");
            var culvertTotalRow = AddCostsOfCulvertWork(worksheet, simulationDataModels, simulationYears, currentCell);
            return culvertTotalRow;
        }

        private int FillCostOfBridgeWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            BridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of Bridge Work");
            var bridgeTotalRow = AddCostsOfBridgeWork(worksheet, simulationDataModels, simulationYears, currentCell);
            return bridgeTotalRow;
        }

        private int FillTotalBudgetSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
        {
            BridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Budget");
            var budgetTotalRow = AddDetailsForTotalBudget(worksheet, simulationYears, currentCell);
            return budgetTotalRow;
        }

        private void FillRemainingBudgetSection(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, int culvertTotalRow, int bridgeTotalRow, int budgetTotalRow)
        {
            BridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Remaining Budget");
            AddDetailsForRemainingBudget(worksheet, simulationYears, currentCell, culvertTotalRow, bridgeTotalRow, budgetTotalRow);
        }

        private int AddCostsOfCulvertWork(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, CurrentCell currentCell)
        {
            int startRow, startColumn, row, column;
            BridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            int culvertTotalRow = 0;
            worksheet.Cells[row++, column].Value = Properties.Resources.Preservation;
            worksheet.Cells[row++, column].Value = Properties.Resources.Rehabilitation;
            worksheet.Cells[row++, column].Value = Properties.Resources.Replacement;
            worksheet.Cells[row++, column].Value = Properties.Resources.CulvertTotal;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                var preservationCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, Properties.Resources.CulvertPreservation);
                worksheet.Cells[row, column].Value = preservationCost;

                var rehabilitationCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, Properties.Resources.CulvertRehabilitation);
                worksheet.Cells[++row, column].Value = rehabilitationCost;

                var replacementCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, Properties.Resources.CulvertReplacement);
                worksheet.Cells[++row, column].Value = replacementCost;

                worksheet.Cells[++row, column].Value = preservationCost + rehabilitationCost + replacementCost;
                culvertTotalRow = row;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.DarkSeaGreen);            
            BridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
            return culvertTotalRow;
        }              

        private int AddDetailsForTotalBudget(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell)
        {
            int startRow, startColumn, row, column;
            BridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            int budgetTotalRow = 0;
            worksheet.Cells[row++, column].Value = Properties.Resources.Preservation;
            worksheet.Cells[row++, column].Value = Properties.Resources.Construction;
            worksheet.Cells[row++, column].Value = Properties.Resources.Total;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                worksheet.Cells[row, column].Value = string.Empty;

                worksheet.Cells[++row, column].Value = string.Empty;

                worksheet.Cells[++row, column].Value = 3000000;
                budgetTotalRow = row;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.OliveDrab);
            BridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
            return budgetTotalRow;
        }

        private void AddDetailsForRemainingBudget(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, int culvertTotalRow, int bridgeTotalRow, int budgetTotalRow)
        {
            int startRow, startColumn, row, column;
            BridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row++, column].Value = Properties.Resources.Preservation;
            worksheet.Cells[row++, column].Value = Properties.Resources.Construction;
            worksheet.Cells[row++, column].Value = Properties.Resources.Total;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                worksheet.Cells[row, column].Value = string.Empty;

                worksheet.Cells[++row, column].Value = string.Empty;

                worksheet.Cells[++row, column].Value = Convert.ToDouble(worksheet.Cells[budgetTotalRow, column].Value) - (Convert.ToDouble(worksheet.Cells[culvertTotalRow, column].Value) + Convert.ToDouble(worksheet.Cells[bridgeTotalRow, column].Value));
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.LightSalmon);
            BridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 3, column);
            ExcelHelper.ApplyColor(worksheet.Cells[row + 2, startColumn, row + 2, column], Color.DimGray);
        }

        private int AddCostsOfBridgeWork(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, CurrentCell currentCell)
        {
            int startRow, startColumn, row, column;
            BridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            int bridgeTotalRow = 0;
            worksheet.Cells[row++, column].Value = Properties.Resources.Latex;
            worksheet.Cells[row++, column].Value = Properties.Resources.Epoxy;
            worksheet.Cells[row++, column].Value = Properties.Resources.LargeBridgePreservation;
            worksheet.Cells[row++, column].Value = Properties.Resources.DeckReplacement;
            worksheet.Cells[row++, column].Value = Properties.Resources.SubRehab;
            worksheet.Cells[row++, column].Value = Properties.Resources.SuperReplacement;
            worksheet.Cells[row++, column].Value = Properties.Resources.LargeBridgeRehab;
            worksheet.Cells[row++, column].Value = Properties.Resources.Replacement;
            worksheet.Cells[row++, column].Value = Properties.Resources.BridgeTotal;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                var latexCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, Properties.Resources.Latex);
                worksheet.Cells[row, column].Value = latexCost;

                var epoxyCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, Properties.Resources.Epoxy);
                worksheet.Cells[++row, column].Value = epoxyCost;

                var largeBridgePreservationCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, Properties.Resources.LargeBridgePreservation);
                worksheet.Cells[++row, column].Value = largeBridgePreservationCost;

                var deckReplacementCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, Properties.Resources.DeckReplacement);
                worksheet.Cells[++row, column].Value = deckReplacementCost;

                var subRehabCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, Properties.Resources.SubRehab);
                worksheet.Cells[++row, column].Value = subRehabCost;

                var superReplacementCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, Properties.Resources.SuperstructureReplacement);
                worksheet.Cells[++row, column].Value = superReplacementCost;

                var largeBridgeRehabCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, Properties.Resources.LargeBridgeRehab);
                worksheet.Cells[++row, column].Value = largeBridgeRehabCost;

                var replacementCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, Properties.Resources.BridgeReplacement);
                worksheet.Cells[++row, column].Value = replacementCost;

                worksheet.Cells[++row, column].Value = latexCost + epoxyCost + largeBridgePreservationCost + deckReplacementCost + subRehabCost + superReplacementCost + largeBridgeRehabCost + replacementCost;
                bridgeTotalRow = row;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.DarkSeaGreen);
            BridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
            return bridgeTotalRow;
        }
    }
}