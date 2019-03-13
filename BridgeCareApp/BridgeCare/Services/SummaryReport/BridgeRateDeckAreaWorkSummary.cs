using BridgeCare.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace BridgeCare.Services
{
    public static class BridgeRateDeckAreaWorkSummary
    {
        #region constants
        const string BridgeCare = "Bridge Care";
        const string Good = "Good";
        const string Fair = "Fair";
        const string Poor = "Poor";
        #endregion

        /// <summary>
        /// Fill work summary bridge poor on off rate and deck area sections
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="currentCell"></param>
        /// <param name="simulationYears"></param>
        /// <param name="simulationDataModels"></param>
        public static void FillBridgeRateDeckAreaWorkSummarySections(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            FillPoorBridgeOnOffRateSection(worksheet, currentCell, simulationYears, simulationDataModels);
            FillTotalPoorBridgesCountSection(worksheet, currentCell, simulationYears, simulationDataModels);
            FillTotalPoorBridgesDeckAreaSection(worksheet, currentCell, simulationYears, simulationDataModels);
            FillTotalBridgeCountSection(worksheet, currentCell, simulationYears, simulationDataModels);
            FillTotalDeckAreaSection(worksheet, currentCell, simulationYears, simulationDataModels);
        }

        private static void FillTotalDeckAreaSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            BridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Total Deck Area", true);
            AddDetailsForTotalDeckArea(worksheet, currentCell, simulationYears, simulationDataModels);
        }

        private static void AddDetailsForTotalDeckArea(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            int startRow, startColumn, row, column;
            InitializeCommonCells(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            AddTotalDeckArea(worksheet, simulationDataModels, startRow, column, 0);
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                AddTotalDeckArea(worksheet, simulationDataModels, row, column, year);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 2, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, startColumn + 1, row + 2, column], "Number");
            ExcelHelper.ApplyColor(worksheet.Cells[row + 2, startColumn + 1, row + 2, column], Color.Khaki);
            BridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 2, column);
        }

        private static void InitializeCommonCells(ExcelWorksheet worksheet, CurrentCell currentCell, out int startRow, out int startColumn, out int row, out int column)
        {
            BridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row++, column].Value = Good;
            worksheet.Cells[row++, column].Value = Fair;
            worksheet.Cells[row++, column++].Value = Poor;
        }

        private static void AddTotalDeckArea(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, int row, int column, int year)
        {
            var goodCount = BridgeWorkSummaryHelper.CalculateTotalGoodDeckArea(simulationDataModels, year);
            worksheet.Cells[row, column].Value = goodCount;

            var poorCount = BridgeWorkSummaryHelper.CalculateTotalPoorDeckArea(simulationDataModels, year);
            worksheet.Cells[row + 2, column].Value = poorCount;

            worksheet.Cells[row + 1, column].Value = BridgeWorkSummaryHelper.CalculateTotalDeckArea(simulationDataModels) - (goodCount + poorCount);
        }

        private static void FillTotalBridgeCountSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            BridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Total Bridge Count", true);
            AddDetailsForTotalBridgeCount(worksheet, currentCell, simulationYears, simulationDataModels);
        }

        private static void AddDetailsForTotalBridgeCount(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            var totalSimulationDataModelCount = simulationDataModels.Count;
            int startRow, startColumn, row, column;
            InitializeCommonCells(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            AddTotalBridgeCount(worksheet, simulationDataModels, totalSimulationDataModelCount, startRow, column, 0);
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                AddTotalBridgeCount(worksheet, simulationDataModels, totalSimulationDataModelCount, row, column, year);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 2, column]);
            BridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 3, column);
        }

        private static void AddTotalBridgeCount(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, int totalSimulationDataModelCount, int row, int column, int year)
        {
            var goodCount = BridgeWorkSummaryHelper.CalculateTotalBridgeGoodCount(simulationDataModels, year);
            worksheet.Cells[row, column].Value = goodCount;

            var poorCount = BridgeWorkSummaryHelper.CalculateTotalBridgePoorCount(simulationDataModels, year);
            worksheet.Cells[row + 2, column].Value = poorCount;

            worksheet.Cells[row + 1, column].Value = totalSimulationDataModelCount - (goodCount + poorCount);
        }

        private static void FillTotalPoorBridgesDeckAreaSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            BridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Total Poor Bridges Deck Area", true);
            AddDetailsForTotalPoorBridgesDeckArea(worksheet, currentCell, simulationYears, simulationDataModels);
        }

        private static void AddDetailsForTotalPoorBridgesDeckArea(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            int startRow, startColumn, row, column;
            BridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row, column++].Value = BridgeCare;
            worksheet.Cells[row, column].Value = BridgeWorkSummaryHelper.CalculateTotalPoorBridgesDeckArea(simulationDataModels, 0);
            foreach (var year in simulationYears)
            {
                column = ++column;
                worksheet.Cells[row, column].Value = BridgeWorkSummaryHelper.CalculateTotalPoorBridgesDeckArea(simulationDataModels, year);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, startColumn + 1, row, column], "Number");
            BridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        private static void FillTotalPoorBridgesCountSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            BridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Total Poor Bridges Count", true);
            AddDetailsForTotalPoorBridgesCount(worksheet, currentCell, simulationYears, simulationDataModels);
        }

        private static void AddDetailsForTotalPoorBridgesCount(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            int startRow, startColumn, row, column;
            BridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row, column++].Value = BridgeCare;
            worksheet.Cells[row, column].Value = BridgeWorkSummaryHelper.CalculateTotalPoorBridgesCount(simulationDataModels, 0);
            foreach (var year in simulationYears)
            {
                column = ++column;
                worksheet.Cells[row, column].Value = BridgeWorkSummaryHelper.CalculateTotalPoorBridgesCount(simulationDataModels, year);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            BridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        private static void FillPoorBridgeOnOffRateSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            currentCell.Row = currentCell.Row + 2;
            BridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Poor Bridge On and Off Rate", false);
            AddDetailsForPoorBridgeOnOfRate(worksheet, currentCell, simulationYears, simulationDataModels);
        }

        private static void AddDetailsForPoorBridgeOnOfRate(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            int startRow, startColumn, row, column;
            BridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row++, column].Value = "# Bridge On";
            worksheet.Cells[row++, column].Value = "# Bridge Off";
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                var count = BridgeWorkSummaryHelper.CalculatePoorBridgeCount(simulationDataModels, year, "On");
                worksheet.Cells[row, column].Value = count;

                count = BridgeWorkSummaryHelper.CalculatePoorBridgeCount(simulationDataModels, year, "Off");
                worksheet.Cells[++row, column].Value = count;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.Khaki);
            BridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }
    }
}