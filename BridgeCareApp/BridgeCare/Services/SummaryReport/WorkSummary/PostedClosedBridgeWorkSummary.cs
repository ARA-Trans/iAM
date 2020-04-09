using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using BridgeCare.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BridgeCare.Services.SummaryReport.WorkSummary
{
    public class PostedClosedBridgeWorkSummary
    {
        private readonly BridgeWorkSummaryCommon bridgeWorkSummaryCommon;
        private readonly ExcelHelper excelHelper;
        private readonly BridgeWorkSummaryComputationHelper bridgeWorkSummaryComputationHelper;

        private Dictionary<int, int> TotalPostedBridgeCount = new Dictionary<int, int>();
        private Dictionary<int, int> TotalClosedBridgeCount = new Dictionary<int, int>();

        public PostedClosedBridgeWorkSummary(BridgeWorkSummaryCommon bridgeWorkSummaryCommon, ExcelHelper excelHelper, BridgeWorkSummaryComputationHelper bridgeWorkSummaryComputationHelper)
        {
            this.bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
            this.excelHelper = excelHelper ?? throw new ArgumentNullException(nameof(excelHelper));
            this.bridgeWorkSummaryComputationHelper = bridgeWorkSummaryComputationHelper ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryComputationHelper));
        }

        internal ChartRowsModel FillPostedBridgeCount(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, ChartRowsModel chartRowsModel)
        {
            //excelHelper.ApplyColor(worksheet.Cells[currentCell.Row, 1, currentCell.Row, worksheet.Dimension.Columns], Color.LightGray);
            bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Posted Bridges - Count", true);
            chartRowsModel.TotalBridgePostedCountByBPNYearsRow = currentCell.Row;
            AddDetailsForPostedBridgeCount(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels);
            return chartRowsModel;
        }

        internal ChartRowsModel FillClosedBridgeCount(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, ChartRowsModel chartRowsModel)
        {
            //excelHelper.ApplyColor(worksheet.Cells[currentCell.Row, 1, currentCell.Row, worksheet.Dimension.Columns], Color.LightGray);
            bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Closed Bridges - Count", true);
            chartRowsModel.TotalClosedBridgeCountByBPNYearsRow = currentCell.Row;
            AddDetailsForClosedBridgeCount(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels);
            return chartRowsModel;
        }

        internal ChartRowsModel FillBridgeCountTotal(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, ChartRowsModel chartRowsModel)
        {
            //excelHelper.ApplyColor(worksheet.Cells[currentCell.Row, 1, currentCell.Row, worksheet.Dimension.Columns], Color.LightGray);
            bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Posted Bridges - Count", true);
            chartRowsModel.TotalPostedAndClosedByBPNYearsRow = currentCell.Row;
            AddDetailsForTotalBridgeCount(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels);
            return chartRowsModel;
        }
        internal ChartRowsModel FillMoneyNeededByBPN(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears,
            List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, ChartRowsModel chartRowsModel)
        {
            //excelHelper.ApplyColor(worksheet.Cells[currentCell.Row, 1, currentCell.Row, worksheet.Dimension.Columns], Color.LightGray);
            bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Dollar Needs By BPN", true);
            chartRowsModel.TotalCashNeededByBPNYearsRow = currentCell.Row;
            AddDetailsForMoneyNeededByBPN(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels);
            return chartRowsModel;
        }

        private void AddDetailsForMoneyNeededByBPN(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.InitializeBPNLabels(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[startRow + 4, column - 1].Value = "Annualized Amount";
            worksheet.Cells[startRow + 4, column - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            var totalMoney = 0.0;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                var totalMoneyPerYear = AddMoneyNeededByBPN(worksheet, simulationDataModels, bridgeDataModels, row, column, year);
                totalMoney += totalMoneyPerYear;
            }
            for (var i = 0; i < simulationYears.Count; i++)
            {
                worksheet.Cells[row + 4, startColumn + i + 2].Value = totalMoney / simulationYears.Count;
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 4, column]);
            excelHelper.SetCustomFormat(worksheet.Cells[startRow, startColumn, row + 4, column], "NegativeCurrency");
            excelHelper.ApplyColor(worksheet.Cells[startRow, startColumn + 2, row + 4, column], Color.FromArgb(198, 224, 180));
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 4, column);
        }
        private void AddDetailsForTotalBridgeCount(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.InitializeTotalBridgeCountLabels(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[startRow++, column].Value = TotalPostedBridgeCount[simulationYears[0] - 1];
            worksheet.Cells[startRow++, column].Value = TotalClosedBridgeCount[simulationYears[0] - 1];
            startRow = startRow - 2;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                worksheet.Cells[row++, column].Value = TotalPostedBridgeCount[year];
                worksheet.Cells[row, column].Value = TotalClosedBridgeCount[year];
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 3, column);
        }
        private void AddDetailsForPostedBridgeCount(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.InitializeBPNLabels(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            var bridgeCount = AddPostedBridgeCount(worksheet, simulationDataModels, bridgeDataModels, startRow, column, 0);
            TotalPostedBridgeCount.Add(simulationYears[0] - 1, bridgeCount);
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                bridgeCount = AddPostedBridgeCount(worksheet, simulationDataModels, bridgeDataModels, row, column, year);
                TotalPostedBridgeCount.Add(year, bridgeCount);
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 3, column]);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 4, column);
        }
        private void AddDetailsForClosedBridgeCount(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.InitializeBPNLabels(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            var closedCount = AddClosedBridgeCount(worksheet, simulationDataModels, bridgeDataModels, startRow, column, 0);
            TotalClosedBridgeCount.Add(simulationYears[0] - 1, closedCount);
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                closedCount = AddClosedBridgeCount(worksheet, simulationDataModels, bridgeDataModels, row, column, year);
                TotalClosedBridgeCount.Add(year, closedCount);
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 3, column]);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 4, column);
        }
        private double AddMoneyNeededByBPN(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int row, int column, int year)
        {
            var totalMoney = 0.0;
            var moneyForBPN = bridgeWorkSummaryComputationHelper.CalculateMoneyNeededByBPN13(simulationDataModels, bridgeDataModels, year, "1");
            worksheet.Cells[row++, column].Value = moneyForBPN;
            totalMoney += moneyForBPN;
            moneyForBPN = bridgeWorkSummaryComputationHelper.CalculateMoneyNeededByBPN2H(simulationDataModels, bridgeDataModels, year);
            worksheet.Cells[row++, column].Value = moneyForBPN;
            totalMoney += moneyForBPN;
            moneyForBPN = bridgeWorkSummaryComputationHelper.CalculateMoneyNeededByBPN13(simulationDataModels, bridgeDataModels, year, "3");
            worksheet.Cells[row++, column].Value = moneyForBPN;
            totalMoney += moneyForBPN;
            moneyForBPN = bridgeWorkSummaryComputationHelper.CalculateMoneyNeededByRemainingBPN(simulationDataModels, bridgeDataModels, year);
            worksheet.Cells[row, column].Value = moneyForBPN;
            totalMoney += moneyForBPN;
            return totalMoney;
        }
        private int AddClosedBridgeCount(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int row, int column, int year)
        {
            var totalCount = 0;
            var closedCount = bridgeWorkSummaryComputationHelper.CalculatePostedAndClosedBridgeCountForBPN13(simulationDataModels, bridgeDataModels, year, "1", "N");
            worksheet.Cells[row++, column].Value = closedCount;
            totalCount += closedCount;
            closedCount = bridgeWorkSummaryComputationHelper.CalculatePostedAndClosedBridgeCountForBPN2H(simulationDataModels, bridgeDataModels, year, "N");
            worksheet.Cells[row++, column].Value = closedCount;
            totalCount += closedCount;
            closedCount = bridgeWorkSummaryComputationHelper.CalculatePostedAndClosedBridgeCountForBPN13(simulationDataModels, bridgeDataModels, year, "3", "N");
            worksheet.Cells[row++, column].Value = closedCount;
            totalCount += closedCount;
            closedCount = bridgeWorkSummaryComputationHelper.CalculatePostedAndClosedBridgeCountForRemaining(simulationDataModels, bridgeDataModels, year, "N");
            worksheet.Cells[row, column].Value = closedCount;
            totalCount += closedCount;

            return totalCount;
        }
        private int AddPostedBridgeCount(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, int row, int column, int year)
        {
            var totalCount = 0;
            var postedCount = bridgeWorkSummaryComputationHelper.CalculatePostedAndClosedBridgeCountForBPN13(simulationDataModels, bridgeDataModels, year, "1", "Y");
            worksheet.Cells[row++, column].Value = postedCount;
            totalCount += postedCount;
            postedCount = bridgeWorkSummaryComputationHelper.CalculatePostedAndClosedBridgeCountForBPN2H(simulationDataModels, bridgeDataModels, year, "Y");
            worksheet.Cells[row++, column].Value = postedCount;
            totalCount += postedCount;
            postedCount = bridgeWorkSummaryComputationHelper.CalculatePostedAndClosedBridgeCountForBPN13(simulationDataModels, bridgeDataModels, year, "3", "Y");
            worksheet.Cells[row++, column].Value = postedCount;
            totalCount += postedCount;
            postedCount = bridgeWorkSummaryComputationHelper.CalculatePostedAndClosedBridgeCountForRemaining(simulationDataModels, bridgeDataModels, year, "Y");
            worksheet.Cells[row, column].Value = postedCount;
            totalCount += postedCount;

            return totalCount;
        }
    }
}
