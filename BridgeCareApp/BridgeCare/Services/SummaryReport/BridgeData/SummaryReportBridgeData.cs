using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Models.SummaryReport.ParametersTAB;
using BridgeCare.Services.SummaryReport;
using BridgeCare.Services.SummaryReport.BridgeData;
using OfficeOpenXml;
using OfficeOpenXml.Style;
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
        private Dictionary<MinCValue, Func<ExcelWorksheet, int, int, YearsData, int>> valueForMinC;
        private List<int> SpacerColumnNumbers;
        private readonly ParametersModel parametersModel;

        public SummaryReportBridgeData(IBridgeData bridgeData, BridgeDataHelper bridgeDataHelper, ExcelHelper excelHelper,
            HighlightWorkDoneCells highlightWorkDoneCells, ParametersModel parametersModel)
        {
            this.bridgeData = bridgeData;
            this.bridgeDataHelper = bridgeDataHelper;
            this.excelHelper = excelHelper;
            this.highlightWorkDoneCells = highlightWorkDoneCells;
            this.parametersModel = parametersModel;
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
            var bridgeDataModels = bridgeData.GetBridgeData(BRKeys, simulationModel, dbContext, parametersModel);
            var budgetsPerBrKey = bridgeData.GetBudgetsPerBRKey(simulationModel, dbContext);

            var simulationDataModels = bridgeDataHelper.GetSimulationDataModels(simulationDataTable, simulationYears, projectCostModels, budgetsPerBrKey);
            var unfundedRecommendations = bridgeData.GetUnfundedRcommendations(simulationModel, dbContext);
            unfundedRecommendations.ForEach(_ => {
                _.TotalProjectCost = Convert.ToDouble(_.Budget_Hash.Split('/')[1]);
            });

            // Add data to excel.
            var headers = GetHeaders();
            var currentCell = AddHeadersCells(worksheet, headers, simulationYears);

            // Add row next to headers for filters and year numbers for dynamic data. Cover from top, left to right, and bottom set of data.
            using (ExcelRange autoFilterCells = worksheet.Cells[3, 1, currentCell.Row, currentCell.Column - 1])
            {
                autoFilterCells.AutoFilter = true;
            }

            var columnForRiskScore = AddBridgeDataModelsCells(worksheet, bridgeDataModels, currentCell);
            AddDynamicDataCells(worksheet, sectionsForSummaryReport, simulationDataModels, bridgeDataModels, currentCell, columnForRiskScore);
            // TODO The line below currently hangs Postman in testing. It will be required for final production.
            // ExcelHelper.ApplyBorder(worksheet.Cells[1, 1, currentCell.Row, currentCell.Column]);
            worksheet.Cells.AutoFitColumns();
            var spacerBeforeFirstYear = SpacerColumnNumbers[0] - 11;
            worksheet.Column(spacerBeforeFirstYear).Width = 3;
            foreach(var spacerNumber in SpacerColumnNumbers)
            {
                worksheet.Column(spacerNumber).Width = 3;
            }
            var lastColumn = worksheet.Dimension.Columns + 1;
            worksheet.Column(lastColumn).Width = 3;
            var workSummaryModel = new WorkSummaryModel { SimulationDataModels = simulationDataModels, BridgeDataModels = bridgeDataModels,
                Treatments = treatments, BudgetsPerBRKeys = budgetsPerBrKey, UnfundedRecommendations = unfundedRecommendations,
                ParametersModel = parametersModel
            };            
            return workSummaryModel;
        }

        private void AddDynamicDataCells(ExcelWorksheet worksheet, List<Section> sectionsForSummaryReport, List<SimulationDataModel> simulationDataModels,
            List<BridgeDataModel> bridgeDataModels, CurrentCell currentCell, int columnForRiskScore)
        {
            var row = 4; // Data starts here
            var startingRow = row;
            var column = currentCell.Column;
            int totalColumn = 0;
            int totalColumnValue = 0;
            var abbreviatedTreatmentNames = ShortNamesForTreatments.GetShortNamesForTreatments();

            // making dictionary to remove if else, which was used to enter value for MinC
            valueForMinC = new Dictionary<MinCValue, Func<ExcelWorksheet, int, int, YearsData, int>>();
            valueForMinC.Add(MinCValue.defaultValue, new Func<ExcelWorksheet, int, int, YearsData, int>(EnterDefaultMinCValue));
            valueForMinC.Add(MinCValue.valueEqualsCulv, new Func<ExcelWorksheet, int, int, YearsData, int>(EnterValueEqualsCulv));
            valueForMinC.Add(MinCValue.minOfDeckSubSuper, new Func<ExcelWorksheet, int, int, YearsData, int>(EnterMinDeckSuperSub));
            valueForMinC.Add(MinCValue.minOfCulvDeckSubSuper, new Func<ExcelWorksheet, int, int, YearsData, int>(EnterMinDeckSuperSubCulv));

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
                bridgeDataModel.RiskScore = simulationDataModel.RiskScore;
                worksheet.Cells[row, columnForRiskScore].Value = simulationDataModel.RiskScore;
                var yearsData = simulationDataModel.YearsData;
                var projectPickByYear = new Dictionary<int, int>();
                // Add work done cells
                for (var index = 1; index < yearsData.Count(); index++)
                {
                    var cost = yearsData[index].Cost;
                    var range = worksheet.Cells[row, ++column];
                    projectPickByYear.Add(yearsData[index].Year, yearsData[index].ProjectPickType);
                    setColor(bridgeDataModel.ParallelBridge, yearsData[index].Treatment, projectPickByYear,
                        yearsData[index].Year, index, yearsData[index].Project, worksheet, row, column);
                    if (abbreviatedTreatmentNames.ContainsKey(yearsData[index].Treatment))
                    {
                        range.Value = string.IsNullOrEmpty(abbreviatedTreatmentNames[yearsData[index].Treatment]) ? "--" : abbreviatedTreatmentNames[yearsData[index].Treatment];
                    }
                    else
                    {
                        range.Value = string.IsNullOrEmpty(yearsData[index].Treatment) ? "--" : yearsData[index].Treatment;
                    }
                    workDoneMoreThanOnce = !range.Value.Equals("--") ? workDoneMoreThanOnce + 1 : workDoneMoreThanOnce;
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

                worksheet.Column(column).Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Column(column).Style.Fill.BackgroundColor.SetColor(Color.Gray);
                

                // Last Year simulation data
                var lastYearData = yearsData.FirstOrDefault();
                column = AddSimulationYearData(worksheet, row, column, lastYearData, familyId, bridgeDataModel, projectPickByYear);
                
                // Add all yrs from current year simulation data
                for (var index = 1; index < yearsData.Count(); index++)
                {
                    column = AddSimulationYearData(worksheet, row, column, yearsData[index], familyId, bridgeDataModel, projectPickByYear);
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

        private void setColor(int parallelBridge, string treatment,
            Dictionary<int, int> projectPickByYear, int year, int index, string project, ExcelWorksheet worksheet, int row, int column)
        {
            highlightWorkDoneCells.CheckConditions(parallelBridge, treatment, projectPickByYear, year, index, project, worksheet, row, column);
        }

        private int AddSimulationYearData(ExcelWorksheet worksheet, int row, int column, YearsData yearData, int familyId,
            BridgeDataModel bridgeDataModel, Dictionary<int, int> projectPickByYear)
        {
            var minCActionCallDecider = MinCValue.minOfCulvDeckSubSuper;
            var familyIdLessThanEleven = familyId < 11;
            if(familyId > 10)
            {
                worksheet.Cells[row, ++column].Value = "N";
                worksheet.Cells[row, ++column].Value = "N";
                worksheet.Cells[row, ++column].Value = "N";

                worksheet.Cells[row, column + 2].Value = "N";
                worksheet.Cells[row, column + 3].Value = "N";
                worksheet.Cells[row, column + 4].Value = "N";
                yearData.Deck = "N";
                yearData.Super = "N";
                yearData.Sub = "N";
                minCActionCallDecider = MinCValue.valueEqualsCulv;
            }
            else
            {
                worksheet.Cells[row, ++column].Value = Convert.ToDouble(yearData.Deck);
                worksheet.Cells[row, ++column].Value = Convert.ToDouble(yearData.Super);
                worksheet.Cells[row, ++column].Value = Convert.ToDouble(yearData.Sub);

                worksheet.Cells[row, column + 2].Value = Convert.ToDouble(yearData.DeckD);
                worksheet.Cells[row, column + 3].Value = Convert.ToDouble(yearData.SuperD);
                worksheet.Cells[row, column + 4].Value = Convert.ToDouble(yearData.SubD);
            }
            if (familyIdLessThanEleven)
            {
                worksheet.Cells[row, ++column].Value = "N";
                worksheet.Cells[row, column + 4].Value = "N";
                yearData.Culv = "N";
                yearData.CulvD = "N";
                if(minCActionCallDecider == MinCValue.valueEqualsCulv)
                {
                    minCActionCallDecider = MinCValue.defaultValue;
                }
                else
                {
                    minCActionCallDecider = MinCValue.minOfDeckSubSuper;
                }
            }
            else
            {
                worksheet.Cells[row, ++column].Value = Convert.ToDouble(yearData.Culv);

                worksheet.Cells[row, column + 4].Value = Convert.ToDouble(yearData.CulvD);
            }
            column += 4;

            column = valueForMinC[minCActionCallDecider](worksheet, row, column, yearData); // It returns the column number where MinC value is written

            if (bridgeDataModel.P3 > 0 && yearData.MinC < 5)
            {
                excelHelper.ApplyColor(worksheet.Cells[row, column], Color.Yellow);
                excelHelper.SetTextColor(worksheet.Cells[row, column], Color.Black);
            }
            //worksheet.Cells[row, ++column].Value = yearData.SD;
            //worksheet.Cells[row, ++column].Value = Convert.ToDouble(yearData.MinC) < 5 ? "Y" : "N" ;
            worksheet.Cells[row, ++column].Value = yearData.MinC < 5 ? "Y" : "N"; //poor

            if (yearData.Year != 0)
            {
                //worksheet.Cells[row, ++column].Value = bridgeDataModel.Posted == "Y" ? getPostedType(yearData.Project) : "N"; // Posted
                worksheet.Cells[row, ++column].Value = yearData.ProjectPick; // Project Pick
                worksheet.Cells[row, ++column].Value = yearData.Budget; // Budget
                worksheet.Cells[row, ++column].Value = yearData.Project;
                if (projectPickByYear[yearData.Year] == 2)
                {
                    excelHelper.ApplyColor(worksheet.Cells[row, column], Color.FromArgb(0, 255, 0));
                    excelHelper.SetTextColor(worksheet.Cells[row, column], Color.Black);
                }
                worksheet.Cells[row, ++column].Value = yearData.Cost;
                excelHelper.SetCurrencyFormat(worksheet.Cells[row, column]);
                worksheet.Cells[row, ++column].Value = ""; // District Remarks
            }
            // Empty column
            column++;
            worksheet.Column(column).Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Column(column).Style.Fill.BackgroundColor.SetColor(Color.Gray);

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
                excelHelper.ApplyColor(worksheet.Cells[row, column], Color.FromArgb(244, 176, 132));
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

            worksheet.Column(column).Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Column(column).Style.Fill.BackgroundColor.SetColor(Color.Gray);

            var yearHeaderColumn = currentCell.Column;
            simulationHeaderTexts.RemoveAll(_ => _.Equals("SD") || _.Equals("Posted"));
            SpacerColumnNumbers = new List<int>();

            foreach (var simulationYear in simulationYears)
            {
                worksheet.Cells[row, ++column].Value = simulationYear;
                column = currentCell.Column;
                column = AddSimulationHeaderTexts(worksheet, column, row, simulationHeaderTexts, simulationHeaderTexts.Count);
                excelHelper.MergeCells(worksheet, row, currentCell.Column + 1, row, column);
                if (simulationYear % 2 != 0)
                {
                    excelHelper.ApplyColor(worksheet.Cells[row, currentCell.Column + 1, row, column], Color.Gray);
                }
                else
                {
                    excelHelper.ApplyColor(worksheet.Cells[row, currentCell.Column + 1, row, column], Color.LightGray);
                }

                worksheet.Column(currentCell.Column).Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Column(currentCell.Column).Style.Fill.BackgroundColor.SetColor(Color.Gray);
                SpacerColumnNumbers.Add(currentCell.Column);

                currentCell.Column = ++column;
            }
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
                "Deck Cond",
                "Super Cond",
                "Sub Cond",
                "Culv Cond",
                "Deck Dur",
                "Super Dur",
                "Sub Dur",
                "Culv Dur",
                "Min Cond",
                //"SD",
                "Poor",
                //"Posted",
                "Project Pick",
                "Budget",
                "Project",
                "Cost",
                "District Remarks"
            };
        }
                
        private int AddBridgeDataModelsCells(ExcelWorksheet worksheet, List<BridgeDataModel> bridgeDataModels, CurrentCell currentCell)
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
                //worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.FunctionalClass;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.YearBuilt;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.Age;
                worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.AdtTotal;
                //worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.ADTOverTenThousand;
                columnNo++;
                //worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.RiskScore; // We fill this data in the next function call "AddDynamicDataCells"
                worksheet.Cells[rowNo, columnNo].Value = bridgeDataModel.P3 > 0 ? "Y" : "N";

                // Get NHS record for Parameter TAB
                if (parametersModel.nHSModel.NHS == null || parametersModel.nHSModel.NonNHS == null)
                {
                    switch (bridgeDataModel.NHS)
                    {
                        case "Y":
                            parametersModel.nHSModel.NHS = "Y";
                            break;
                        case "N":
                            parametersModel.nHSModel.NonNHS = "Y";
                            break;
                    }
                }
                // Get BPN data for parameter TAB
                if (!parametersModel.BPNValues.Contains(bridgeDataModel.BPN))
                {
                    parametersModel.BPNValues.Add(bridgeDataModel.BPN);
                }
            }
            currentCell.Row = rowNo;
            currentCell.Column = columnNo;
            return columnNo - 1; // This is the column number for RiskScore. We do not have RiskScore information at this point.
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
                //"Functional Class",
                "Year Built",
                "Age",
                "ADTT",
                //"ADT Over 10,000",
                "Risk Score",
                "P3"
            };
        }

        private int EnterDefaultMinCValue(ExcelWorksheet worksheet, int row, int column, YearsData yearData)
        {
            worksheet.Cells[row, ++column].Value = "N";
            // It is a dummy value
            yearData.MinC = 100;
            return column;
        }
        private int EnterValueEqualsCulv(ExcelWorksheet worksheet, int row, int column, YearsData yearData)
        {
            yearData.MinC = Convert.ToDouble(yearData.Culv);
            worksheet.Cells[row, ++column].Value = yearData.MinC;
            if(yearData.MinC <= 3.5)
            {
                excelHelper.ApplyColor(worksheet.Cells[row, column], Color.FromArgb(112, 48, 160));
                excelHelper.SetTextColor(worksheet.Cells[row, column], Color.White);
            }
            return column;
        }
        private int EnterMinDeckSuperSub(ExcelWorksheet worksheet, int row, int column, YearsData yearData)
        {
            var minValue = Math.Min(Convert.ToDouble(yearData.Deck), Math.Min(Convert.ToDouble(yearData.Super), Convert.ToDouble(yearData.Sub)));
            worksheet.Cells[row, ++column].Value = minValue;
            yearData.MinC = minValue;
            if (yearData.MinC <= 3.5)
            {
                excelHelper.ApplyColor(worksheet.Cells[row, column], Color.FromArgb(112, 48, 160));
                excelHelper.SetTextColor(worksheet.Cells[row, column], Color.White);
            }
            return column;
        }
        private int EnterMinDeckSuperSubCulv(ExcelWorksheet worksheet, int row, int column, YearsData yearData)
        {
            worksheet.Cells[row, ++column].Value = yearData.MinC;
            if (yearData.MinC <= 3.5)
            {
                excelHelper.ApplyColor(worksheet.Cells[row, column], Color.FromArgb(112, 48, 160));
                excelHelper.SetTextColor(worksheet.Cells[row, column], Color.White);
            }
            return column;
        }
        private enum MinCValue
        {
            minOfCulvDeckSubSuper,
            minOfDeckSubSuper,
            valueEqualsCulv,
            defaultValue
        }
    }
}
