using System;
using System.Collections.Generic;
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

        public void FillNHSBridgeDeckAreaWorkSummarySections(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, ChartRowsModel chartRowsModel)
        {
            FillNHSBridgeCountSection(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels);
          //  FillNHSBridgeCountPercentSection(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels, chartRowsModel);
          // FillNHSBridgeDeckAreaSection(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels);
          // FillNHSBridgeDeckAreaPercentSection(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels, chartRowsModel);
        }

        private void FillNHSBridgeDeckAreaPercentSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, ChartRowsModel chartRowsModel)
        {
            throw new NotImplementedException();
        }

        private void FillNHSBridgeDeckAreaSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels)
        {
            throw new NotImplementedException();
        }

        private void FillNHSBridgeCountPercentSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, ChartRowsModel chartRowsModel)
        {
            bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "NHS Bridge Count %", true);
            chartRowsModel.NHSBridgeCountPercentRow = currentCell.Row;
         //   AddDetailsForNHSBridgeCountPercent(worksheet, currentCell, simulationYears, simulationDataModels);           
        }

        private void FillNHSBridgeCountSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels)
        {
            bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "NHS Bridge Count", true);
            AddDetailsForNHSBridgeCount(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels);            
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