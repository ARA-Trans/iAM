using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Services.SummaryReport.BridgeData;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace BridgeCare.Services
{
    public class SummaryReportBridgeData
    {
        private readonly IBridgeData bridgeData;
        private readonly BridgeDataHelper bridgeDataHelper;
        private readonly ExcelHelper excelHelper;
        private readonly HighlightWorkDoneCells highlightWorkDoneCells;

        public SummaryReportBridgeData(IBridgeData bridgeData, BridgeDataHelper bridgeDataHelper, ExcelHelper excelHelper,
            HighlightWorkDoneCells highlightWorkDoneCells)
        {
            this.bridgeData = bridgeData;
            this.bridgeDataHelper = bridgeDataHelper;
            this.excelHelper = excelHelper;
            this.highlightWorkDoneCells = highlightWorkDoneCells;
        }

        /// <summary>
        /// Fill Summary Report Bridge Data tab.
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="simulationModel"></param>
        /// <param name="simulationYears"></param>
        /// <param name="dbContext"></param>
        /// <returns>WorkSummaryModel with simulation and bridge data models</returns>
        internal WorkSummaryModel Fill(ExcelWorksheet worksheet, SimulationModel simulationModel, List<int> simulationYears, BridgeCareContext dbContext)
        {
            var BRKeys = new List<int>();

            var sections = bridgeData.GetSectionData(simulationModel, dbContext);
            var treatments = bridgeData.GetTreatments(simulationModel.simulationId, dbContext);
            var simulationDataTable = bridgeData.GetSimulationData(simulationModel, dbContext, simulationYears);
            var projectCostModels = bridgeData.GetReportData(simulationModel, dbContext, simulationYears);
            var sectionIdsFromSimulationTable = from dt in simulationDataTable.AsEnumerable()
                                                select dt.Field<int>("SECTIONID");
            var sectionsForSummaryReport = sections.Where(sm => sectionIdsFromSimulationTable.Contains(sm.SECTIONID)).ToList();
            BRKeys = sectionsForSummaryReport.Select(sm => Convert.ToInt32(sm.FACILITY)).ToList();
            var bridgeDataModels = bridgeData.GetBridgeData(BRKeys, simulationModel, dbContext);
            var budgetsPerBrKey = bridgeData.GetBudgetsPerBRKey(simulationModel, dbContext);

            var simulationDataModels = bridgeDataHelper.GetSimulationDataModels(simulationDataTable, simulationYears, projectCostModels, budgetsPerBrKey);

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

            var workSummaryModel = new WorkSummaryModel { SimulationDataModels = simulationDataModels, BridgeDataModels = bridgeDataModels, Treatments = treatments, BudgetsPerBRKeys = budgetsPerBrKey };            
            return workSummaryModel;
        }

        private void AddDynamicDataCells(ExcelWorksheet worksheet, List<Section> sectionsForSummaryReport, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, CurrentCell currentCell)
        {
            var row = 4; // Data starts here
            var column = currentCell.Column;
            int totalColumn = 0;
            int totalColumnValue = 0;
            foreach (var bridgeDataModel in bridgeDataModels)
            {
                if (row % 2 == 0)
                {
                    excelHelper.ApplyColor(worksheet.Cells[row, 1, row, worksheet.Dimension.Columns], Color.LightGray);
                }
                column = currentCell.Column;
                var brKey = bridgeDataModel.BRKey;
                var familyId = bridgeDataModel.BridgeFamily;
                var workDoneMoreThanOnce = 0;
                var section = sectionsForSummaryReport.Where(s => Convert.ToInt32(s.FACILITY) == brKey).FirstOrDefault();
                var simulationDataModel = simulationDataModels.Where(s => s.SectionId == section.SECTIONID).FirstOrDefault();
                // Save DeckArea for further use
                simulationDataModel.DeckArea = bridgeDataModel.DeckArea;
                simulationDataModel.BRKey = brKey;
                var yearsData = simulationDataModel.YearsData;
                var projectPickByYear = new Dictionary<int, int>();
                // Add work done cells
                for (var index = 1; index < yearsData.Count(); index++)
                {
                    var cost = yearsData[index].Cost;
                    var range = worksheet.Cells[row, ++column];
                    projectPickByYear.Add(yearsData[index].Year, yearsData[index].ProjectPickType);
                    setColor(bridgeDataModel.ParallelBridge, yearsData[index].Treatment, range, projectPickByYear, yearsData[index].Year, index);
                    range.Value = cost > 0 ? yearsData[index].Treatment : "--";
                    workDoneMoreThanOnce = cost > 0 ? workDoneMoreThanOnce + 1 : workDoneMoreThanOnce;
                }
                worksheet.Cells[row, ++column].Value = workDoneMoreThanOnce > 1 ? "Yes" : "--";
                totalColumnValue = workDoneMoreThanOnce > 1 ? totalColumnValue + 1 : totalColumnValue;

                // Empty Total column
                column++;
                // Add Total of count of Work done more than once column cells if "Yes"
                totalColumn = column;                

                // Add Poor On/Off Rate column: Formula (prev yr MinC < 5 and  curr yr Minc >= 5 then "Off"), (prev yr MinC >= 5 and curr ye MinC < 5 then "On")   
                for (var index = 1; index < yearsData.Count(); index++)
                {
                    var prevYrMinc = yearsData[index - 1].MinC;
                    var thisYrMinc = yearsData[index].MinC;
                    worksheet.Cells[row, ++column].Value = prevYrMinc < 5 ? (thisYrMinc >= 5 ? "Off" : "--") : (thisYrMinc < 5 ? "On" : "--");
                    yearsData[index].PoorOnOffRate = worksheet.Cells[row, column].Value.ToString();
                }

                // Empty column
                column++;

                // Last Year simulation data
                var lastYearData = yearsData.FirstOrDefault();
                column = AddSimulationYearData(worksheet, row, column, lastYearData, familyId, bridgeDataModel);

                // Add all yrs from current year simulation data
                for (var index = 1; index < yearsData.Count(); index++)
                {
                    column = AddSimulationYearData(worksheet, row, column, yearsData[index], familyId, bridgeDataModel);
                }
                row++;
            }
            if(totalColumn != 0)
            {
                worksheet.Cells[3, totalColumn].Value = totalColumnValue;
            }
            currentCell.Row = row - 1;
            currentCell.Column = column - 1;            
        }

        private void setColor(int parallelBridge, string treatment, ExcelRange range,
            Dictionary<int, int> projectPickByYear, int year, int index)
        {
            highlightWorkDoneCells.CheckConditions(parallelBridge, treatment, range, projectPickByYear, year, index);
        }

        private int AddSimulationYearData(ExcelWorksheet worksheet, int row, int column, YearsData yearData, int familyId, BridgeDataModel bridgeDataModel)
        {
            int initialColumn = column;
            var familyIdLessThanEleven = familyId < 11;
            worksheet.Cells[row, ++column].Value = familyId > 10 ? "N" : yearData.Deck;
            worksheet.Cells[row, ++column].Value = familyId > 10 ? "N" : yearData.Super;
            worksheet.Cells[row, ++column].Value = familyId > 10 ? "N" : yearData.Sub;
            worksheet.Cells[row, ++column].Value = familyIdLessThanEleven ? "N" : yearData.Culv;

            yearData.Culv = familyIdLessThanEleven ? "N" : yearData.Culv;
            yearData.Deck = familyId > 10 ? "N" : yearData.Deck;
            yearData.Super = familyId > 10 ? "N" : yearData.Super;
            yearData.Sub = familyId > 10 ? "N" : yearData.Sub;

            worksheet.Cells[row, ++column].Value = familyId > 10 ? "N" : yearData.DeckD;
            worksheet.Cells[row, ++column].Value = familyId > 10 ? "N" : yearData.SuperD;
            worksheet.Cells[row, ++column].Value = familyId > 10 ? "N" : yearData.SubD;
            worksheet.Cells[row, ++column].Value = familyIdLessThanEleven ? "N" : yearData.CulvD;
            yearData.CulvD = familyIdLessThanEleven ? "N" : yearData.CulvD;

            // This if else condition is a last minute change before deployment. It should be refactored
            if(yearData.Culv == "N" && yearData.Deck == "N" && yearData.Super == "N" && yearData.Sub == "N")
            {
                worksheet.Cells[row, ++column].Value = "N";
                // It is a dummy value
                yearData.MinC = 100;
            }
            else if(yearData.Deck == "N" && yearData.Super == "N" && yearData.Sub == "N")
            {
                worksheet.Cells[row, ++column].Value = yearData.Culv;
                yearData.MinC = Convert.ToDouble(yearData.Culv);
            }
            else if(yearData.Culv == "N")
            {
                var minValue = Math.Min(Convert.ToDouble(yearData.Deck), Math.Min(Convert.ToDouble(yearData.Super), Convert.ToDouble(yearData.Sub)));
                worksheet.Cells[row, ++column].Value = minValue.ToString();
                yearData.MinC = minValue;
            }
            else
            {
                worksheet.Cells[row, ++column].Value = yearData.MinC.ToString();
            }
            //worksheet.Cells[row, ++column].Value = yearData.SD;
            //worksheet.Cells[row, ++column].Value = Convert.ToDouble(yearData.MinC) < 5 ? "Y" : "N" ;

            if (yearData.Year != 0)
            {   
                //worksheet.Cells[row, ++column].Value = bridgeDataModel.Posted == "Y" ? getPostedType(yearData.Project) : "N"; // Posted
                worksheet.Cells[row, ++column].Value = yearData.ProjectPick; // Project Pick
                worksheet.Cells[row, ++column].Value = yearData.Budget; // Budget
                worksheet.Cells[row, ++column].Value = yearData.Project;
                worksheet.Cells[row, ++column].Value = yearData.Cost;
                excelHelper.SetCurrencyFormat(worksheet.Cells[row, column]);
                worksheet.Cells[row, ++column].Value = ""; // District Remarks
            }
            else
            {
                worksheet.Cells[row, ++column].Value = yearData.SD;
                worksheet.Cells[row, ++column].Value = yearData.MinC < 5 ? "Y" : "N";
                worksheet.Cells[row, ++column].Value = bridgeDataModel.Posted; // Posted
            }
            // Empty column
            column++;
            return column;
        }

        private string getPostedType(string project)
        {
            if (project == "Culvert Rehab(Other)" || project == "Culvert Replacement (Box/Frame/Arch)"
                    || project == "Culvert Replacement (Other)" || project == "Culvert Replacement (Pipe)" || project == "Substructure Rehab"
                    || project == "Superstructure Rep/Rehab" || project == "Deck Replacement" || project == "Rehabilitation"
                    || project == "Repair" || project == "Bridge Replacement" || project == "Replacement" || project == "Removal")
            {
                return "N";
            }
            return "Y";
        }

        private CurrentCell AddHeadersCells(ExcelWorksheet worksheet, List<string> headers, List<int> simulationYears)
        {
            int headerRow = 1;
            for (int column = 0; column < headers.Count; column++)
            {
                worksheet.Cells[headerRow, column + 1].Value = headers[column];
            }
            var currentCell = new CurrentCell { Row = headerRow, Column = headers.Count };
            excelHelper.ApplyBorder(worksheet.Cells[headerRow, 1, headerRow + 1, worksheet.Dimension.Columns]);

            AddDynamicHeadersCells(worksheet, currentCell, simulationYears);
            return currentCell;
        }

        private void AddDynamicHeadersCells(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
        {
            const string HeaderConstText = "Work Done in ";
            var column = currentCell.Column;
            var row = currentCell.Row;
            var initialColumn = column;
            foreach (var year in simulationYears)
            {
                worksheet.Cells[row, ++column].Value = HeaderConstText + year;
                worksheet.Cells[row + 2, column].Value = year;
                excelHelper.ApplyStyle(worksheet.Cells[row + 2, column]);
                excelHelper.ApplyColor(worksheet.Cells[row, column], Color.IndianRed);
            }
            worksheet.Cells[row, ++column].Value = "Work Done more than once";
            worksheet.Cells[row, ++column].Value = "Total";
            worksheet.Cells[row, ++column].Value = "Poor On/Off Rate";
            var poorOnOffRateColumn = column;
            foreach (var year in simulationYears)
            {
                worksheet.Cells[row + 2, column].Value = year;
                excelHelper.ApplyStyle(worksheet.Cells[row + 2, column]);
                column++;
            }

            // Merge 2 rows for headers till column before Poor On/Off Rate
            worksheet.Row(row).Height = 40;
            for (int cellColumn = 1; cellColumn < poorOnOffRateColumn; cellColumn++)
            {
               excelHelper.MergeCells(worksheet, row, cellColumn, row + 1, cellColumn);
            }
            // Merge columns for Poor On/Off Rate
            excelHelper.MergeCells(worksheet, row, poorOnOffRateColumn, row + 1, column - 1);
            currentCell.Column = column;

            // Add Years Data headers
            var simulationHeaderTexts = GetSimulationHeaderTexts();
            worksheet.Cells[row, ++column].Value = simulationYears[0] - 1;
            column = currentCell.Column;
            column = AddSimulationHeaderTexts(worksheet, column, row, simulationHeaderTexts, simulationHeaderTexts.Count - 5);
            excelHelper.MergeCells(worksheet, row, currentCell.Column + 1, row, column);

            // Empty column
            currentCell.Column = ++column;
            var yearHeaderColumn = currentCell.Column;
            simulationHeaderTexts.RemoveAll(_ => _.Equals("SD") || _.Equals("Poor") || _.Equals("Posted"));
            foreach (var simulationYear in simulationYears)
            {
                worksheet.Cells[row, ++column].Value = simulationYear;
                column = currentCell.Column;
                column = AddSimulationHeaderTexts(worksheet, column, row, simulationHeaderTexts, simulationHeaderTexts.Count);
                excelHelper.MergeCells(worksheet, row, currentCell.Column + 1, row, column);
                if(simulationYear % 2 != 0)
                {
                    excelHelper.ApplyColor(worksheet.Cells[row, currentCell.Column + 1, row, column], Color.Gray);
                }
                else
                {
                    excelHelper.ApplyColor(worksheet.Cells[row, currentCell.Column + 1, row, column], Color.LightGray);
                }
                currentCell.Column = ++column;
            }
            //excelHelper.ApplyColor(worksheet.Cells[1, yearHeaderColumn - 2, 1, currentCell.Column], Color.DimGray);
            excelHelper.ApplyBorder(worksheet.Cells[row, initialColumn, row + 1, worksheet.Dimension.Columns]);
            currentCell.Row = currentCell.Row + 2;
        }

        private int AddSimulationHeaderTexts(ExcelWorksheet worksheet, int column, int row, List<string> simulationHeaderTexts, int length)
        {
            for (var index = 0; index < length; index++)
            {
                worksheet.Cells[row + 1, ++column].Value = simulationHeaderTexts[index];
                excelHelper.ApplyStyle(worksheet.Cells[row + 1, column]);
            }

            return column;
        }

        private List<string> GetSimulationHeaderTexts()
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
                "Poor",
                "Posted",
                "Project Pick",
                "Budget",
                "Project",
                "Cost",
                "District Remarks"
            };
        }
                
        private void AddBridgeDataModelsCells(ExcelWorksheet worksheet, List<BridgeDataModel> bridgeDataModels, CurrentCell currentCell)
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
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.BridgeCulvert;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.DeckArea;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.StructureLength;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.PlanningPartner;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.BridgeFamily;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.NHS;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.BPN;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.StructureType;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.FunctionalClass;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.YearBuilt;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.Age;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.AdtTotal;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.ADTOverTenThousand;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.RiskScore;
                worksheet.Cells[rowNo, columnNo].Value = bridgeDataModel.P3 > 0 ? "Y" : "N";
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
                "Bridge (B/C)",
                "Deck Area",
                "Structure Length",
                "Planning Partner",
                "Bridge Family",
                "NHS",
                "BPN",
                "Struct Type",
                "Functional Class",
                "Year Built",
                "Age",
                "ADTT",
                "ADT Over 10,000",
                "Risk Score",
                "P3"
            };
        }        
    }
}
