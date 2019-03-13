using BridgeCare.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace BridgeCare.Services
{
    public static class CostBudgetsWorkSummary
    {
        #region constants
        const string Preservation = "Preservation";
        const string Rehabilitation = "Rehabilitation";
        const string Replacement = "Replacement";
        const string Latex = "Latex";
        const string Epoxy = "Epoxy";
        const string LargeBridgePreservation = "Large Bridge Preservation";
        const string DeckReplacement = "Deck Replacement";
        const string SubRehab = "Sub Rehab";
        const string SuperReplacement = "Super Replacement";
        const string LargeBridgeRehab = "Large Bridge Rehab";
        const string Total = "Total";
        const string Construction = "Construction";
        const string BridgeReplacement = "Bridge Replacement";
        const string SuperstructureReplacement = "Superstructure Replacement";
        const string CulvertPreservation = "Culvert Preservation";
        const string CulvertRehabilitation = "Culvert Rehabilitation";
        const string CulvertReplacement = "Culvert Replacement";
        #endregion

        /// <summary>
        ///  Fill sections with cost and budget details
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="currentCell"></param>
        /// <param name="simulationYears"></param>
        /// <param name="simulationDataModels"></param>
        public static void FillCostBudgetWorkSummarySections(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            var culvertTotalRow = FillCostOfCulvertWorkSection(worksheet, currentCell, simulationYears, simulationDataModels);
            var bridgeTotalRow = FillCostOfBridgeWorkSection(worksheet, currentCell, simulationYears, simulationDataModels);
            // TODO ask why Total row is with same hard coded number 30000000 in excel?
            var budgetTotalRow = FillTotalBudgetSection(worksheet, currentCell, simulationYears);
            FillRemainingBudgetSection(worksheet, simulationYears, currentCell, culvertTotalRow, bridgeTotalRow, budgetTotalRow);
        }

        private static int FillCostOfCulvertWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            BridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of Culvert Work");
            var culvertTotalRow = AddCostsOfCulvertWork(worksheet, simulationDataModels, simulationYears, currentCell);
            return culvertTotalRow;
        }

        private static int FillCostOfBridgeWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            BridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Cost of Bridge Work");
            var bridgeTotalRow = AddCostsOfBridgeWork(worksheet, simulationDataModels, simulationYears, currentCell);
            return bridgeTotalRow;
        }

        private static int FillTotalBudgetSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
        {
            BridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Budget");
            var budgetTotalRow = AddDetailsForTotalBudget(worksheet, simulationYears, currentCell);
            return budgetTotalRow;
        }

        private static void FillRemainingBudgetSection(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, int culvertTotalRow, int bridgeTotalRow, int budgetTotalRow)
        {
            BridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Remaining Budget");
            AddDetailsForRemainingBudget(worksheet, simulationYears, currentCell, culvertTotalRow, bridgeTotalRow, budgetTotalRow);
        }

        private static int AddCostsOfCulvertWork(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, CurrentCell currentCell)
        {
            int startRow, startColumn, row, column;
            BridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            int culvertTotalRow = 0;
            worksheet.Cells[row++, column].Value = Preservation;
            worksheet.Cells[row++, column].Value = Rehabilitation;
            worksheet.Cells[row++, column].Value = Replacement;
            worksheet.Cells[row++, column].Value = "Culvert Total";
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                var preservationCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, CulvertPreservation);
                worksheet.Cells[row, column].Value = preservationCost;

                var rehabilitationCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, CulvertRehabilitation);
                worksheet.Cells[++row, column].Value = rehabilitationCost;

                var replacementCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, CulvertReplacement);
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

        private static int AddDetailsForTotalBudget(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell)
        {
            int startRow, startColumn, row, column;
            BridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            int budgetTotalRow = 0;
            worksheet.Cells[row++, column].Value = Preservation;
            worksheet.Cells[row++, column].Value = Construction;
            worksheet.Cells[row++, column].Value = Total;
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

        private static void AddDetailsForRemainingBudget(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, int culvertTotalRow, int bridgeTotalRow, int budgetTotalRow)
        {
            int startRow, startColumn, row, column;
            BridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row++, column].Value = Preservation;
            worksheet.Cells[row++, column].Value = Construction;
            worksheet.Cells[row++, column].Value = Total;
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

        private static int AddCostsOfBridgeWork(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, CurrentCell currentCell)
        {
            int startRow, startColumn, row, column;
            BridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            int bridgeTotalRow = 0;
            worksheet.Cells[row++, column].Value = Latex;
            worksheet.Cells[row++, column].Value = Epoxy;
            worksheet.Cells[row++, column].Value = LargeBridgePreservation;
            worksheet.Cells[row++, column].Value = DeckReplacement;
            worksheet.Cells[row++, column].Value = SubRehab;
            worksheet.Cells[row++, column].Value = SuperReplacement;
            worksheet.Cells[row++, column].Value = LargeBridgeRehab;
            worksheet.Cells[row++, column].Value = Replacement;
            worksheet.Cells[row++, column].Value = "Bridge Total";
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                var latexCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, Latex);
                worksheet.Cells[row, column].Value = latexCost;

                var epoxyCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, Epoxy);
                worksheet.Cells[++row, column].Value = epoxyCost;

                var largeBridgePreservationCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, LargeBridgePreservation);
                worksheet.Cells[++row, column].Value = largeBridgePreservationCost;

                var deckReplacementCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, DeckReplacement);
                worksheet.Cells[++row, column].Value = deckReplacementCost;

                var subRehabCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, SubRehab);
                worksheet.Cells[++row, column].Value = subRehabCost;

                var superReplacementCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, SuperstructureReplacement);
                worksheet.Cells[++row, column].Value = superReplacementCost;

                var largeBridgeRehabCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, LargeBridgeRehab);
                worksheet.Cells[++row, column].Value = largeBridgeRehabCost;

                var replacementCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, BridgeReplacement);
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