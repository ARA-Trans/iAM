﻿using BridgeCare.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace BridgeCare.Services
{
    public class BridgeRateDeckAreaWorkSummary
    {       
        enum HeaderLabel { Good, Fair, Poor };
        private readonly BridgeWorkSummaryCommon bridgeWorkSummaryCommon;
        private readonly ExcelHelper excelHelper;
        private readonly BridgeWorkSummaryComputationHelper bridgeWorkSummaryComputationHelper;

        public BridgeRateDeckAreaWorkSummary(BridgeWorkSummaryCommon bridgeWorkSummaryCommon, ExcelHelper excelHelper, BridgeWorkSummaryComputationHelper bridgeWorkSummaryComputationHelper)
        {
            this.bridgeWorkSummaryCommon = bridgeWorkSummaryCommon;
            this.excelHelper = excelHelper;
            this.bridgeWorkSummaryComputationHelper = bridgeWorkSummaryComputationHelper;
        }

        /// <summary>
        /// Fill work summary bridge poor on off rate and deck area sections
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="currentCell"></param>
        /// <param name="simulationYears"></param>
        /// <param name="simulationDataModels"></param>
        public ChartRowsModel FillBridgeRateDeckAreaWorkSummarySections(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            var chartRowsModel = new ChartRowsModel();
            FillPoorBridgeOnOffRateSection(worksheet, currentCell, simulationYears, simulationDataModels);
            FillTotalPoorBridgesCountSection(worksheet, currentCell, simulationYears, simulationDataModels);
            FillTotalPoorBridgesDeckAreaSection(worksheet, currentCell, simulationYears, simulationDataModels);
            chartRowsModel.TotalBridgeCountSectionYearsRow = FillTotalBridgeCountSection(worksheet, currentCell, simulationYears, simulationDataModels);
            FillTotalDeckAreaSection(worksheet, currentCell, simulationYears, simulationDataModels);
            return chartRowsModel;
        }

        private void FillTotalDeckAreaSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Total Deck Area", true);
            AddDetailsForTotalDeckArea(worksheet, currentCell, simulationYears, simulationDataModels);
        }

        private void AddDetailsForTotalDeckArea(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
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
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 2, column]);
            excelHelper.SetCustomFormat(worksheet.Cells[startRow, startColumn + 1, row + 2, column], "Number");
            excelHelper.ApplyColor(worksheet.Cells[row + 2, startColumn + 1, row + 2, column], Color.Khaki);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 2, column);
        }

        private void InitializeCommonCells(ExcelWorksheet worksheet, CurrentCell currentCell, out int startRow, out int startColumn, out int row, out int column)
        {
            bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row++, column].Value = HeaderLabel.Good;
            worksheet.Cells[row++, column].Value = HeaderLabel.Fair;
            worksheet.Cells[row++, column++].Value = HeaderLabel.Poor;
        }

        private void AddTotalDeckArea(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, int row, int column, int year)
        {
            var goodCount = bridgeWorkSummaryComputationHelper.CalculateTotalGoodDeckArea(simulationDataModels, year);
            worksheet.Cells[row, column].Value = goodCount;

            var poorCount = bridgeWorkSummaryComputationHelper.CalculateTotalPoorDeckArea(simulationDataModels, year);
            worksheet.Cells[row + 2, column].Value = poorCount;

            worksheet.Cells[row + 1, column].Value = bridgeWorkSummaryComputationHelper.CalculateTotalDeckArea(simulationDataModels) - (goodCount + poorCount);
        }

        private int FillTotalBridgeCountSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Total Bridge Count", true);
            var totalBridgeCountSectionYearsRow = currentCell.Row;
            AddDetailsForTotalBridgeCount(worksheet, currentCell, simulationYears, simulationDataModels);
            return totalBridgeCountSectionYearsRow;
        }

        private void AddDetailsForTotalBridgeCount(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
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
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 2, column]);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 3, column);
        }

        private void AddTotalBridgeCount(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, int totalSimulationDataModelCount, int row, int column, int year)
        {
            var goodCount = bridgeWorkSummaryComputationHelper.CalculateTotalBridgeGoodCount(simulationDataModels, year);
            worksheet.Cells[row, column].Value = goodCount;

            var poorCount = bridgeWorkSummaryComputationHelper.CalculateTotalBridgePoorCount(simulationDataModels, year);
            worksheet.Cells[row + 2, column].Value = poorCount;

            worksheet.Cells[row + 1, column].Value = totalSimulationDataModelCount - (goodCount + poorCount);
        }

        private void FillTotalPoorBridgesDeckAreaSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Total Poor Bridges Deck Area", true);
            AddDetailsForTotalPoorBridgesDeckArea(worksheet, currentCell, simulationYears, simulationDataModels);
        }

        private void AddDetailsForTotalPoorBridgesDeckArea(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row, column++].Value = Properties.Resources.BridgeCare;
            worksheet.Cells[row, column].Value = bridgeWorkSummaryComputationHelper.CalculateTotalPoorBridgesDeckArea(simulationDataModels, 0);
            foreach (var year in simulationYears)
            {
                column = ++column;
                worksheet.Cells[row, column].Value = bridgeWorkSummaryComputationHelper.CalculateTotalPoorBridgesDeckArea(simulationDataModels, year);
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            excelHelper.SetCustomFormat(worksheet.Cells[startRow, startColumn + 1, row, column], "Number");
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        private void FillTotalPoorBridgesCountSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Total Poor Bridges Count", true);
            AddDetailsForTotalPoorBridgesCount(worksheet, currentCell, simulationYears, simulationDataModels);
        }

        private void AddDetailsForTotalPoorBridgesCount(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row, column++].Value = Properties.Resources.BridgeCare;
            worksheet.Cells[row, column].Value = bridgeWorkSummaryComputationHelper.CalculateTotalPoorBridgesCount(simulationDataModels, 0);
            foreach (var year in simulationYears)
            {
                column = ++column;
                worksheet.Cells[row, column].Value = bridgeWorkSummaryComputationHelper.CalculateTotalPoorBridgesCount(simulationDataModels, year);
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        private void FillPoorBridgeOnOffRateSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            currentCell.Row = currentCell.Row + 2;
            bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Poor Bridge On and Off Rate", false);
            AddDetailsForPoorBridgeOnOfRate(worksheet, currentCell, simulationYears, simulationDataModels);
        }

        private void AddDetailsForPoorBridgeOnOfRate(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row++, column].Value = "# Bridge On";
            worksheet.Cells[row++, column].Value = "# Bridge Off";
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                var count = bridgeWorkSummaryComputationHelper.CalculatePoorBridgeCount(simulationDataModels, year, "On");
                worksheet.Cells[row, column].Value = count;

                count = bridgeWorkSummaryComputationHelper.CalculatePoorBridgeCount(simulationDataModels, year, "Off");
                worksheet.Cells[++row, column].Value = count;
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.Khaki);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }
    }
}