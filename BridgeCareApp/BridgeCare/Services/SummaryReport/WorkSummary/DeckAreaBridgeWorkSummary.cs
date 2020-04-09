using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using BridgeCare.Models;
using OfficeOpenXml;

namespace BridgeCare.Services.SummaryReport.WorkSummary
{
    public class DeckAreaBridgeWorkSummary
    {
        private readonly BridgeWorkSummaryCommon bridgeWorkSummaryCommon;
        private readonly ExcelHelper excelHelper;
        private readonly BridgeWorkSummaryComputationHelper bridgeWorkSummaryComputationHelper;

        public DeckAreaBridgeWorkSummary(BridgeWorkSummaryCommon bridgeWorkSummaryCommon, ExcelHelper excelHelper, BridgeWorkSummaryComputationHelper bridgeWorkSummaryComputationHelper)
        {
            this.bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
            this.excelHelper = excelHelper ?? throw new ArgumentNullException(nameof(excelHelper));
            this.bridgeWorkSummaryComputationHelper = bridgeWorkSummaryComputationHelper ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryComputationHelper));
        }

        internal ChartRowsModel FillPostedDeckArea(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, ChartRowsModel chartRowsModel)
        {
            //excelHelper.ApplyColor(worksheet.Cells[currentCell.Row, 1, currentCell.Row, worksheet.Dimension.Columns], Color.LightGray);
            bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Posted Bridges - Deck Area", true);
            chartRowsModel.TotalPostedBridgeDeckAreaByBPNYearsRow = currentCell.Row;
            AddDetailsForPostedDeckArea(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels);
            return chartRowsModel;
        }

        internal ChartRowsModel FillClosedDeckArea(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, ChartRowsModel chartRowsModel)
        {
            //excelHelper.ApplyColor(worksheet.Cells[currentCell.Row, 1, currentCell.Row, worksheet.Dimension.Columns], Color.LightGray);
            bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Closes Bridges - Deck Area", true);
            chartRowsModel.TotalClosedBridgeDeckAreaByBPNYearsRow = currentCell.Row;
            AddDetailsForClosedDeckArea(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels);
            return chartRowsModel;
        }

        internal ChartRowsModel FillPoorDeckArea(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, ChartRowsModel chartRowsModel)
        {
            //excelHelper.ApplyColor(worksheet.Cells[currentCell.Row, 1, currentCell.Row, worksheet.Dimension.Columns], Color.LightGray);
            bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Poor Deck Area", true);
            chartRowsModel.TotalPoorDeckAreaByBPNSectionYearsRow = currentCell.Row;
            AddDetailsForPoorDeckArea(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels);
            return chartRowsModel;
        }
        private void AddDetailsForPostedDeckArea(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.InitializeBPNLabels(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            AddPostedBridgeDeckArea(worksheet, simulationDataModels, bridgeDataModels, startRow, column, 0);
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                AddPostedBridgeDeckArea(worksheet, simulationDataModels, bridgeDataModels, row, column, year);
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 3, column]);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 4, column);
        }
        private void AddDetailsForClosedDeckArea(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.InitializeBPNLabels(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            AddClosedBridgeDeckArea(worksheet, simulationDataModels, bridgeDataModels, startRow, column, 0);
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                AddClosedBridgeDeckArea(worksheet, simulationDataModels, bridgeDataModels, row, column, year);
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 3, column]);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 4, column);
        }
        private void AddDetailsForPoorDeckArea(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.InitializeBPNLabels(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            AddPoorDeckArea(worksheet, simulationDataModels, bridgeDataModels, startRow, column, 0);
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                AddPoorDeckArea(worksheet, simulationDataModels, bridgeDataModels, row, column, year);
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 3, column]);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 4, column);
        }

        private void AddPostedBridgeDeckArea(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int row, int column, int year)
        {
            var deckArea = bridgeWorkSummaryComputationHelper.CalculatePostedAndClosedDeckAreaForBPN13(simulationDataModels, bridgeDataModels, year, "1", "Y");
            worksheet.Cells[row++, column].Value = deckArea;
            deckArea = bridgeWorkSummaryComputationHelper.CalculatePostedAndClosedDeckAreaForBPN2H(simulationDataModels, bridgeDataModels, year, "Y");
            worksheet.Cells[row++, column].Value = deckArea;
            deckArea = bridgeWorkSummaryComputationHelper.CalculatePostedAndClosedDeckAreaForBPN13(simulationDataModels, bridgeDataModels, year, "3", "Y");
            worksheet.Cells[row++, column].Value = deckArea;
            deckArea = bridgeWorkSummaryComputationHelper.CalculatePostedAndClosedDeckAreaForRemainingBPN(simulationDataModels, bridgeDataModels, year, "Y");
            worksheet.Cells[row, column].Value = deckArea;
        }
        private void AddClosedBridgeDeckArea(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int row, int column, int year)
        {
            var deckArea = bridgeWorkSummaryComputationHelper.CalculatePostedAndClosedDeckAreaForBPN13(simulationDataModels, bridgeDataModels, year, "1", "N");
            worksheet.Cells[row++, column].Value = deckArea;
            deckArea = bridgeWorkSummaryComputationHelper.CalculatePostedAndClosedDeckAreaForBPN2H(simulationDataModels, bridgeDataModels, year, "N");
            worksheet.Cells[row++, column].Value = deckArea;
            deckArea = bridgeWorkSummaryComputationHelper.CalculatePostedAndClosedDeckAreaForBPN13(simulationDataModels, bridgeDataModels, year, "3", "N");
            worksheet.Cells[row++, column].Value = deckArea;
            deckArea = bridgeWorkSummaryComputationHelper.CalculatePostedAndClosedDeckAreaForRemainingBPN(simulationDataModels, bridgeDataModels, year, "N");
            worksheet.Cells[row, column].Value = deckArea;
        }
        private void AddPoorDeckArea(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int row, int column, int year)
        {
            var poorDeckArea = bridgeWorkSummaryComputationHelper.CalculatePoorDeckAreaForBPN13(simulationDataModels, bridgeDataModels, year, "1");
            worksheet.Cells[row++, column].Value = poorDeckArea;
            poorDeckArea = bridgeWorkSummaryComputationHelper.CalculatePoorDeckAreaForBPN2H(simulationDataModels, bridgeDataModels, year);
            worksheet.Cells[row++, column].Value = poorDeckArea;
            poorDeckArea = bridgeWorkSummaryComputationHelper.CalculatePoorDeckAreaForBPN13(simulationDataModels, bridgeDataModels, year, "3");
            worksheet.Cells[row++, column].Value = poorDeckArea;
            poorDeckArea = bridgeWorkSummaryComputationHelper.CalculatePoorDeckAreaForRemainingBPN(simulationDataModels, bridgeDataModels, year);
            worksheet.Cells[row, column].Value = poorDeckArea;
        }
    }
}
