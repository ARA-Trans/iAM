using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

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
        public void Fill(ExcelWorksheet worksheet, SimulationModel simulationModel, List<int> simulationYears, BridgeCareContext dbContext)
        {
            var BRKeys = new List<int>();

            var sectionModels = bridgeData.GetSectionData(simulationModel, dbContext);
            var simulationDataTable = bridgeData.GetSimulationData(simulationModel, dbContext, simulationYears);
            var projectCostModels = bridgeData.GetReportData(simulationModel, dbContext, simulationYears);
            var sectionIdsFromSimulationTable = from dt in simulationDataTable.AsEnumerable()
                                                select dt.Field<int>("SECTIONID");
            var sectionModelsForSummaryReport = sectionModels.Where(sm => sectionIdsFromSimulationTable.Contains(sm.SECTIONID));
            BRKeys = sectionModelsForSummaryReport.Select(sm => Convert.ToInt32(sm.FACILITY)).ToList();
            var bridgeDataModels = bridgeData.GetBridgeData(BRKeys, dbContext);

            // TODO Check macro for setting specific values based on certain values
            var simulationDataModels = GetSimulationDataModels(simulationDataTable, simulationYears, projectCostModels);

            // Fiil up the excel.
            var headers = GetHeaders();
            var currentCell = AddHeadersCells(worksheet, headers, simulationYears);           

            // TODO // Add row next to headers for fitlers and year no.s for dynamic data.

            //HELP: cover from top, left to right bottom whole set of data
            //        using (ExcelRange autoFilterCells = ws.Cells[
            //startRowIndex, territoryNameIndex,
            //toRowIndex, totalIndex])
            //        {
            //            autoFilterCells.AutoFilter = true;
            //        }
                        
            AddBridgeDataModelsCells(worksheet, bridgeDataModels, currentCell);
            AddDynamicDataCells(worksheet, sectionModelsForSummaryReport, simulationDataModels, bridgeDataModels, currentCell);

            worksheet.Cells.AutoFitColumns();
        }

        #region Private Methods
        private void AddDynamicDataCells(ExcelWorksheet worksheet, IQueryable<SectionModel> sectionModelsForSummaryReport, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, CurrentCell currentCell)
        {   
            var sectionModels = sectionModelsForSummaryReport.ToList();
            var row = 4; // Data starts here
            var coln = currentCell.Coln;
            foreach (var brKey in bridgeDataModels.Select(b => b.BRKey))
            {
                coln = currentCell.Coln;
                row++;
                var workDoneMoreThanOnce = 0;
                var sectionModel = sectionModels.Where(s => Convert.ToInt32(s.FACILITY) == brKey).FirstOrDefault();
                var simulationDataModel = simulationDataModels.Where(s => s.SectionId == sectionModel.SECTIONID).FirstOrDefault();
                var yearsData = simulationDataModel.YearsData;
                // Add work done cellls
                for (var cnt = 1; cnt < yearsData.Count(); cnt++)
                {
                    var cost = yearsData[cnt].Cost;
                    worksheet.Cells[row, ++coln].Value = cost > 0 ? "Yes" : "--";
                    workDoneMoreThanOnce = cost > 0 ? workDoneMoreThanOnce + 1 : workDoneMoreThanOnce;
                }
                worksheet.Cells[row, ++coln].Value = workDoneMoreThanOnce > 1 ? "Yes" : "--";

                // Empty Total column
                coln++;
                
                // Add Poor On/Off Rate column: Formula (prev yr SD == "Y")?(curr yr SD=="N")?"Off":"On":"--"
                // TODO Ask if prev yr formula is wrong in report, not matching with other yrs formulae.
                for (var cnt = 1; cnt < yearsData.Count(); cnt++)
                {
                    var prevYrSD = yearsData[cnt - 1].SD;
                    var thisYrSD = yearsData[cnt].SD;
                    worksheet.Cells[row, ++coln].Value = prevYrSD == "Y" ? (thisYrSD == "N" ? "Off" : "On") : "--";
                }

                // TODO Add simu. data cells
                // PrevYr
                // All yrs

            }
        }
                
        private static List<SimulationDataModel> GetSimulationDataModels(DataTable simulationDataTable, List<int> simulationYears, IQueryable<ProjectCostModel> projectCostModels)
        {
            var simulationDMs = new List<SimulationDataModel>();
            var projectCostsList = projectCostModels.ToList();
            foreach (DataRow simulationRow in simulationDataTable.Rows)
            {
                var simulationDM = CreatePrevYearSimulationMdel(simulationRow);
                var projectCostEntry = projectCostsList.Where(pc => pc.SECTIONID == Convert.ToUInt32(simulationRow["SECTIONID"])).FirstOrDefault();
                AddAllYearsData(simulationRow, simulationYears, projectCostEntry, simulationDM);
                simulationDMs.Add(simulationDM);
            }

            return simulationDMs;
        }

        private static void AddAllYearsData(DataRow simulationRow, List<int> simulationYears, ProjectCostModel projectCostEntry, SimulationDataModel simulationDM)
        {
            var yearsDMs = new List<YearsData>();
            foreach (int year in simulationYears)
            {
                yearsDMs.Add(AddYearsData(simulationRow, projectCostEntry, year));
            }
            simulationDM.YearsData.AddRange(yearsDMs.OrderBy(y => y.Year).ToList());
        }

        private static SimulationDataModel CreatePrevYearSimulationMdel(DataRow simulationRow)
        {
            YearsData yearsData = AddYearsData(simulationRow, null, 0);
            return new SimulationDataModel
            {
                YearsData = new List<YearsData>() { yearsData },
                SectionId = Convert.ToInt32(simulationRow["SECTIONID"])
            };
        }

        private static YearsData AddYearsData(DataRow simulationRow, ProjectCostModel projectCostEntry, int year)
        {
            var yearsData = new YearsData
            {
                Deck = simulationRow["DECK_SEEDED_" + year].ToString(),
                Super = simulationRow["SUP_SEEDED_" + year].ToString(),
                Sub = simulationRow["SUB_SEEDED_" + year].ToString(),
                Culv = simulationRow["CULV_SEEDED_" + year].ToString(),
                DeckD = simulationRow["DECK_DURATION_N_" + year].ToString(),
                SuperD = simulationRow["SUP_DURATION_N_" + year].ToString(),
                SubD = simulationRow["SUB_DURATION_N_" + year].ToString(),
                CulvD = simulationRow["CULV_DURATION_N_" + year].ToString(),
                Year = year
            };
            yearsData.MinC = Math.Min(Convert.ToDouble(yearsData.Deck), Convert.ToDouble(yearsData.Culv)).ToString();
            yearsData.SD = Convert.ToDouble(yearsData.DeckD) < 5 ? "Y" : "N";

            yearsData.Project = year != 0 ? projectCostEntry?.TREATMENT : string.Empty;
            yearsData.Cost = year != 0 ? (projectCostEntry == null ? 0 : projectCostEntry.COST_) : 0;

            return yearsData;
        }

        private static CurrentCell AddHeadersCells(ExcelWorksheet worksheet, List<string> headers, List<int> simulationYears)
        {
            int headerRow = 1;
            for (int coln = 0; coln < headers.Count; coln++)
            {
                worksheet.Cells[headerRow, coln + 1].Value = headers[coln];
            }
            var currentCell = new CurrentCell { Row = headerRow, Coln = headers.Count };

            // TODO Build rest of headers - dynamic header names
            AddDynamicHeadersCells(worksheet, currentCell, simulationYears);


            return currentCell;
        }

        private static void AddDynamicHeadersCells(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
        {
            const string headerFooterText = "Work Done in ";
            var coln = currentCell.Coln;
            var row = currentCell.Row;
            foreach (var year in simulationYears)
            {
                worksheet.Cells[row, ++coln].Value = headerFooterText + year;
                worksheet.Cells[row + 2, coln].Value = year;
            }
            worksheet.Cells[row, ++coln].Value = "Work Done more than once";
            worksheet.Cells[row, ++coln].Value = "Total";
            worksheet.Cells[row, ++coln].Value = "Poor On/Off Rate";
            var poorOnOffRateColn = coln;
            foreach (var year in simulationYears)
            {
                worksheet.Cells[row + 2, coln++].Value = year;
            }

            // Merge 2 rows for headers till column before Poor On/Off Rate
            worksheet.Row(row).Height = 40;
            for (int cellColn = 1; cellColn < poorOnOffRateColn; cellColn++)
            {
                MergeCells(worksheet, row, cellColn, row + 1, cellColn);
            }
            // Merge columns for Poor On/Off Rate
            MergeCells(worksheet, row, poorOnOffRateColn, row + 1, coln - 1);
            
            // Next column is empty(as per report)
            currentCell.Coln = coln++;
            currentCell.Row = currentCell.Row + 2;            
        }

        private static void MergeCells(ExcelWorksheet worksheet, int fromRow, int fromColn, int toRow, int toColn)
        {
            using (var cells = worksheet.Cells[fromRow, fromColn, toRow, toColn])
            {
                cells.Merge = true;
                cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cells.Style.WrapText = true;
                cells.Style.Font.Bold = true;
            }
        }

        private static void AddBridgeDataModelsCells(ExcelWorksheet worksheet, List<BridgeDataModel> bridgeDataModels, CurrentCell currentCell)
        {
            var rowNo = currentCell.Row;
            var colnNo = currentCell.Coln;
            foreach (var bridgeDataModel in bridgeDataModels)
            {
                rowNo++;
                colnNo = 1;
                worksheet.Cells[rowNo, colnNo++].Value = bridgeDataModel.BridgeID;
                worksheet.Cells[rowNo, colnNo++].Value = bridgeDataModel.BRKey;
                worksheet.Cells[rowNo, colnNo++].Value = bridgeDataModel.District;
                worksheet.Cells[rowNo, colnNo++].Value = bridgeDataModel.DeckArea;
                worksheet.Cells[rowNo, colnNo++].Value = bridgeDataModel.BridgeFamily;
                worksheet.Cells[rowNo, colnNo++].Value = bridgeDataModel.NHS;
                worksheet.Cells[rowNo, colnNo++].Value = bridgeDataModel.BPN;
                worksheet.Cells[rowNo, colnNo++].Value = bridgeDataModel.FunctionalClass;
                worksheet.Cells[rowNo, colnNo++].Value = bridgeDataModel.YearBuilt;
                worksheet.Cells[rowNo, colnNo++].Value = bridgeDataModel.Age;
                worksheet.Cells[rowNo, colnNo++].Value = bridgeDataModel.ADTOverTenThousand;
                worksheet.Cells[rowNo, colnNo].Value = bridgeDataModel.RiskScore;
            }
            currentCell.Row = rowNo;
            currentCell.Coln = colnNo;
        }

        private List<string> GetHeaders()
        {
            var headers = new List<string>
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

            return headers;
        }
        #endregion
    }
}