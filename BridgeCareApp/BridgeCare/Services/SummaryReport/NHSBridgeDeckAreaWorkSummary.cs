using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using BridgeCare.Models;
using OfficeOpenXml;

namespace BridgeCare.Services
{
    public class NHSBridgeDeckAreaWorkSummary
    {
        private readonly BridgeWorkSummaryCommon bridgeWorkSummaryCommon;
        private readonly ExcelHelper excelHelper;
        private readonly BridgeWorkSummaryComputationHelper bridgeWorkSummaryComputationHelper;

        public NHSBridgeDeckAreaWorkSummary(BridgeWorkSummaryCommon bridgeWorkSummaryCommon, ExcelHelper excelHelper, BridgeWorkSummaryComputationHelper bridgeWorkSummaryComputationHelper)
        {
            this.bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
            this.excelHelper = excelHelper ?? throw new ArgumentNullException(nameof(excelHelper));
            this.bridgeWorkSummaryComputationHelper = bridgeWorkSummaryComputationHelper ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryComputationHelper));
        }

        /// <summary>
        /// Fill NHS sections.
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="currentCell"></param>
        /// <param name="simulationYears"></param>
        /// <param name="simulationDataModels"></param>
        /// <param name="bridgeDataModels"></param>
        /// <param name="chartRowsModel"></param>
        public void FillNHSBridgeDeckAreaWorkSummarySections(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, ChartRowsModel chartRowsModel)
        {
            var dataStartRow = FillNHSBridgeCountSection(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels);
            FillNHSBridgeCountPercentSection(worksheet, currentCell, simulationYears, dataStartRow, chartRowsModel);
            dataStartRow = FillNHSBridgeDeckAreaSection(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels);
            FillNHSBridgeDeckAreaPercentSection(worksheet, currentCell, simulationYears, dataStartRow, chartRowsModel);
        }

        private void FillNHSBridgeDeckAreaPercentSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, int dataStartRow, ChartRowsModel chartRowsModel)
        {
            bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "NHS Bridge Deck Area %", true);
            chartRowsModel.NHSBridgeDeckAreaPercentRow = currentCell.Row;
            AddDetailsForNHSPercentSection(worksheet, currentCell, simulationYears, dataStartRow);
        }       

        private int FillNHSBridgeDeckAreaSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels)
        {
            bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "NHS Bridge Deck Area", true);
            var dataStartRow = currentCell.Row + 1;
            AddDetailsForNHSBridgeDeckArea(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels);
            return dataStartRow;
        }

        private void AddDetailsForNHSBridgeDeckArea(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.InitializeLabelCells(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            AddNHSBridgeDeckArea(worksheet, simulationDataModels, bridgeDataModels, startRow, column, 0);
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                AddNHSBridgeDeckArea(worksheet, simulationDataModels, bridgeDataModels, row, column, year);
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 2, column]);
            excelHelper.SetCustomFormat(worksheet.Cells[startRow, startColumn + 1, row + 2, column], "Number");
            excelHelper.ApplyColor(worksheet.Cells[row + 2, startColumn + 1, row + 2, column], Color.Khaki);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 3, column);
        }

        private void AddNHSBridgeDeckArea(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int row, int column, int year)
        {
            var goodDeckArea = bridgeWorkSummaryComputationHelper.CalculateNHSBridgeGoodDeckArea(simulationDataModels, bridgeDataModels, year);
            worksheet.Cells[row, column].Value = goodDeckArea;

            var poorDeckArea = bridgeWorkSummaryComputationHelper.CalculateNHSBridgePoorDeckArea(simulationDataModels, bridgeDataModels, year);
            worksheet.Cells[row + 2, column].Value = poorDeckArea;

            var filteredModels = bridgeDataModels.FindAll(b => b.NHS == "Y");
            worksheet.Cells[row + 1, column].Value = filteredModels.Sum(f => Convert.ToDouble(f.DeckArea)) - (goodDeckArea + poorDeckArea);
        }

        private void FillNHSBridgeCountPercentSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, int dataStartRow, ChartRowsModel chartRowsModel)
        {
            bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "NHS Bridge Count %", true);
            chartRowsModel.NHSBridgeCountPercentRow = currentCell.Row;
            AddDetailsForNHSPercentSection(worksheet, currentCell, simulationYears, dataStartRow);
        }

        private void AddDetailsForNHSPercentSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, int dataStartRow)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.InitializeLabelCells(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            for (var index = 0; index <= simulationYears.Count; index++)
            {
                var sumFormula = "SUM(" + worksheet.Cells[dataStartRow, column, dataStartRow + 2, column] + ")";
                worksheet.Cells[startRow, column].Formula = worksheet.Cells[dataStartRow, column] + "/" + sumFormula;
                worksheet.Cells[startRow + 1, column].Formula = worksheet.Cells[dataStartRow + 1, column] + "/" + sumFormula;
                worksheet.Cells[startRow + 2, column].Formula = worksheet.Cells[dataStartRow + 2, column] + "/" + sumFormula;
                column++;
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, startRow + 2, column - 1]);
            excelHelper.SetCustomFormat(worksheet.Cells[startRow, startColumn + 1, startRow + 2, column], "Percent");
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row, column - 1);
        }

        private int FillNHSBridgeCountSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels)
        {
            bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "NHS Bridge Count", true);
            var dataStartRow = currentCell.Row + 1;
            AddDetailsForNHSBridgeCount(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels);
            return dataStartRow;
        }

        private void AddDetailsForNHSBridgeCount(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels)
        {          
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.InitializeLabelCells(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            AddNHSBridgeCount(worksheet, simulationDataModels, bridgeDataModels, startRow, column, 0);
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                AddNHSBridgeCount(worksheet, simulationDataModels, bridgeDataModels, row, column, year);
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 2, column]);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 3, column);
        }

        private void AddNHSBridgeCount(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int row, int column, int year)
        {
            var goodCount = bridgeWorkSummaryComputationHelper.CalculateNHSBridgeGoodCount(simulationDataModels, bridgeDataModels, year);
            worksheet.Cells[row, column].Value = goodCount;

            var poorCount = bridgeWorkSummaryComputationHelper.CalculateNHSBridgePoorCount(simulationDataModels, bridgeDataModels, year);
            worksheet.Cells[row + 2, column].Value = poorCount;

            var yNHSCount = bridgeDataModels.FindAll(b => b.NHS == "Y").Count;
            worksheet.Cells[row + 1, column].Value = yNHSCount - (goodCount + poorCount);
        }
    }
}