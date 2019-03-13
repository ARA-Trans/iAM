using BridgeCare.Interfaces;
using BridgeCare.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BridgeCare.Services
{
    public class SummaryReportBridgeData
    {
        private readonly IBridgeData bridgeData;

        public SummaryReportBridgeData(IBridgeData bridgeData)
        {
            this.bridgeData = bridgeData;
        }

        /// <summary>
        /// Fill Summary Report Bridge Data tab.
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="simulationModel"></param>
        /// <param name="simulationYears"></param>
        /// <param name="dbContext"></param>
        public List<SimulationDataModel> Fill(ExcelWorksheet worksheet, SimulationModel simulationModel, List<int> simulationYears, BridgeCareContext dbContext)
        {
            var BRKeys = new List<int>();

            var sections = bridgeData.GetSectionData(simulationModel, dbContext);
            var simulationDataTable = bridgeData.GetSimulationData(simulationModel, dbContext, simulationYears);
            var projectCostModels = bridgeData.GetReportData(simulationModel, dbContext, simulationYears);
            var sectionIdsFromSimulationTable = from dt in simulationDataTable.AsEnumerable()
                                                select dt.Field<int>("SECTIONID");
            var sectionsForSummaryReport = sections.Where(sm => sectionIdsFromSimulationTable.Contains(sm.SECTIONID)).ToList();
            BRKeys = sectionsForSummaryReport.Select(sm => Convert.ToInt32(sm.FACILITY)).ToList();
            var bridgeDataModels = bridgeData.GetBridgeData(BRKeys, dbContext);
                        
            var simulationDataModels = BridgeDataHelper.GetSimulationDataModels(simulationDataTable, simulationYears, projectCostModels);

            // Add data to excel.
            var headers = GetHeaders();
            var currentCell = AddHeadersCells(worksheet, headers, simulationYears);

            // Add row next to headers for filters and year numbers for dynamic data. Cover from top, left to right, and bottom set of data.
            using (ExcelRange autoFilterCells = worksheet.Cells[3, 1, currentCell.Row, currentCell.Column - 1])
            {
                autoFilterCells.AutoFilter = true;
            }

            AddBridgeDataModelsCells(worksheet, bridgeDataModels, currentCell);
            AddDynamicDataCells(worksheet, sectionsForSummaryReport, simulationDataModels, bridgeDataModels, currentCell);
            // TODO The line below currently hangs Postman in testing. It will be required for final production.
            // ExcelHelper.ApplyBorder(worksheet.Cells[1, 1, currentCell.Row, currentCell.Column]);
            worksheet.Cells.AutoFitColumns();

            return simulationDataModels;
        }

        #region Private Methods

        private void AddDynamicDataCells(ExcelWorksheet worksheet, List<Section> sectionsForSummaryReport, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, CurrentCell currentCell)
        {
            var row = 4; // Data starts here
            var column = currentCell.Column;
            int totalColumn = 0;
            int totalColumnValue = 0;
            foreach (var bridgeDataModel in bridgeDataModels)
            {
                column = currentCell.Column;
                var brKey = bridgeDataModel.BRKey;
                var familyId = bridgeDataModel.BridgeFamily;
                var workDoneMoreThanOnce = 0;
                var section = sectionsForSummaryReport.Where(s => Convert.ToInt32(s.FACILITY) == brKey).FirstOrDefault();
                var simulationDataModel = simulationDataModels.Where(s => s.SectionId == section.SECTIONID).FirstOrDefault();
                // Save DeckArea for further use
                simulationDataModel.DeckArea = bridgeDataModel.DeckArea;
                var yearsData = simulationDataModel.YearsData;
                // Add work done cellls
                for (var index = 1; index < yearsData.Count(); index++)
                {
                    var cost = yearsData[index].Cost;
                    worksheet.Cells[row, ++column].Value = cost > 0 ? "Yes" : "--";
                    workDoneMoreThanOnce = cost > 0 ? workDoneMoreThanOnce + 1 : workDoneMoreThanOnce;
                }
                worksheet.Cells[row, ++column].Value = workDoneMoreThanOnce > 1 ? "Yes" : "--";
                totalColumnValue = workDoneMoreThanOnce > 1 ? totalColumnValue + 1 : totalColumnValue;

                // Empty Total column
                column++;
                // Add Total of count of Work done more than once column cells if "Yes"
                totalColumn = column;                

                // Add Poor On/Off Rate column: Formula (prev yr SD == "Y")?(curr yr SD=="N")?"Off":"On":"--"                
                for (var index = 1; index < yearsData.Count(); index++)
                {
                    var prevYrSD = yearsData[index - 1].SD;
                    var thisYrSD = yearsData[index].SD;
                    worksheet.Cells[row, ++column].Value = prevYrSD == "Y" ? (thisYrSD == "N" ? "Off" : "On") : "--";
                    yearsData[index].PoorOnOffRate = worksheet.Cells[row, column].Value.ToString();
                }

                // Empty column
                column++;

                // Last Year simulation data
                var lastYearData = yearsData.FirstOrDefault();
                column = AddSimulationYearData(worksheet, row, column, lastYearData, familyId);

                // Add all yrs from current year simulation data
                for (var index = 1; index < yearsData.Count(); index++)
                {
                    column = AddSimulationYearData(worksheet, row, column, yearsData[index], familyId);
                }
                row++;
            }
            worksheet.Cells[3, totalColumn].Value = totalColumnValue;
            currentCell.Row = row - 1;
            currentCell.Column = column - 1;            
        }

        private static int AddSimulationYearData(ExcelWorksheet worksheet, int row, int column, YearsData yearData, string familyId)
        {
            var familyIdLessThanEleven = Convert.ToInt32(familyId) < 11;
            worksheet.Cells[row, ++column].Value = Convert.ToInt32(familyId) > 10 ? "N" : yearData.Deck;
            worksheet.Cells[row, ++column].Value = Convert.ToInt32(familyId) > 10 ? "N" : yearData.Super;
            worksheet.Cells[row, ++column].Value = Convert.ToInt32(familyId) > 10 ? "N" : yearData.Sub;
            worksheet.Cells[row, ++column].Value = familyIdLessThanEleven ? "N" : yearData.Culv;
            yearData.Culv = familyIdLessThanEleven ? "N" : yearData.Culv;
            worksheet.Cells[row, ++column].Value = Convert.ToInt32(familyId) > 10 ? "N" : yearData.DeckD;
            worksheet.Cells[row, ++column].Value = Convert.ToInt32(familyId) > 10 ? "N" : yearData.SuperD;
            worksheet.Cells[row, ++column].Value = Convert.ToInt32(familyId) > 10 ? "N" : yearData.SubD;
            worksheet.Cells[row, ++column].Value = familyIdLessThanEleven ? "N" : yearData.CulvD;
            yearData.CulvD = familyIdLessThanEleven ? "N" : yearData.CulvD;
            worksheet.Cells[row, ++column].Value = yearData.MinC;
            worksheet.Cells[row, ++column].Value = yearData.SD;
            if (yearData.Year != 0)
            {
                worksheet.Cells[row, ++column].Value = yearData.Project;
                worksheet.Cells[row, ++column].Value = yearData.Cost;
                ExcelHelper.SetCurrencyFormat(worksheet.Cells[row, column]);
            }
            // Empty column
            column++;
            return column;
        }

        private static CurrentCell AddHeadersCells(ExcelWorksheet worksheet, List<string> headers, List<int> simulationYears)
        {
            int headerRow = 1;
            for (int column = 0; column < headers.Count; column++)
            {
                worksheet.Cells[headerRow, column + 1].Value = headers[column];
            }
            var currentCell = new CurrentCell { Row = headerRow, Column = headers.Count };

            AddDynamicHeadersCells(worksheet, currentCell, simulationYears);
            return currentCell;
        }

        private static void AddDynamicHeadersCells(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
        {
            const string HeaderConstText = "Work Done in ";
            var column = currentCell.Column;
            var row = currentCell.Row;
            foreach (var year in simulationYears)
            {
                worksheet.Cells[row, ++column].Value = HeaderConstText + year;
                worksheet.Cells[row + 2, column].Value = year;
                ExcelHelper.ApplyStyle(worksheet.Cells[row + 2, column]);
            }
            worksheet.Cells[row, ++column].Value = "Work Done more than once";
            worksheet.Cells[row, ++column].Value = "Total";
            worksheet.Cells[row, ++column].Value = "Poor On/Off Rate";
            var poorOnOffRateColumn = column;
            foreach (var year in simulationYears)
            {
                worksheet.Cells[row + 2, column].Value = year;
                ExcelHelper.ApplyStyle(worksheet.Cells[row + 2, column]);
                column++;
            }

            // Merge 2 rows for headers till column before Poor On/Off Rate
            worksheet.Row(row).Height = 40;
            for (int cellColumn = 1; cellColumn < poorOnOffRateColumn; cellColumn++)
            {
               ExcelHelper.MergeCells(worksheet, row, cellColumn, row + 1, cellColumn);
            }
            // Merge columns for Poor On/Off Rate
            ExcelHelper.MergeCells(worksheet, row, poorOnOffRateColumn, row + 1, column - 1);
            currentCell.Column = column;

            // Add Years Data headers
            var simulationHeaderTexts = GetSimulationHeaderTexts();
            worksheet.Cells[row, ++column].Value = simulationYears[0] - 1;
            column = currentCell.Column;
            column = AddSimulationHeaderTexts(worksheet, currentCell, column, row, simulationHeaderTexts, simulationHeaderTexts.Count - 2);
            ExcelHelper.MergeCells(worksheet, row, currentCell.Column + 1, row, column);

            // Empty column
            currentCell.Column = ++column;

            foreach (var simulationYear in simulationYears)
            {
                worksheet.Cells[row, ++column].Value = simulationYear;
                column = currentCell.Column;
                column = AddSimulationHeaderTexts(worksheet, currentCell, column, row, simulationHeaderTexts, simulationHeaderTexts.Count);
                ExcelHelper.MergeCells(worksheet, row, currentCell.Column + 1, row, column);
                currentCell.Column = ++column;
            }

            currentCell.Row = currentCell.Row + 2;
        }

        private static int AddSimulationHeaderTexts(ExcelWorksheet worksheet, CurrentCell currentCell, int column, int row, List<string> simulationHeaderTexts, int length)
        {
            for (var index = 0; index < length; index++)
            {
                worksheet.Cells[row + 1, ++column].Value = simulationHeaderTexts[index];
                ExcelHelper.ApplyStyle(worksheet.Cells[row + 1, column]);
            }

            return column;
        }

        private static List<string> GetSimulationHeaderTexts()
        {
            return new List<string>
            {
                "Deck",
                "Super",
                "Sub",
                "Culv",
                "Deck D",
                "Super D",
                "Sub D",
                "Culv D",
                "Min C",
                "SD",
                "Project",
                "Cost"
            };
        }
                
        private static void AddBridgeDataModelsCells(ExcelWorksheet worksheet, List<BridgeDataModel> bridgeDataModels, CurrentCell currentCell)
        {
            var rowNo = currentCell.Row;
            var columnNo = currentCell.Column;
            foreach (var bridgeDataModel in bridgeDataModels)
            {
                rowNo++;
                columnNo = 1;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.BridgeID;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.BRKey;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.District;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.DeckArea;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.BridgeFamily;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.NHS;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.BPN;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.FunctionalClass;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.YearBuilt;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.Age;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.ADTOverTenThousand;
                worksheet.Cells[rowNo, columnNo].Value = bridgeDataModel.RiskScore;
            }
            currentCell.Row = rowNo;
            currentCell.Column = columnNo;
        }

        private List<string> GetHeaders()
        {
            return new List<string>
            {
                "BridgeID",
                "BRKey",
                "District",
                "Deck Area",
                "Bridge Family",
                "NHS",
                "BPN",
                "Functional Class",
                "Year Built",
                "Age",
                "ADT Over 10,000",
                "Risk Score"
            };
        }

        #endregion Private Methods
    }
}