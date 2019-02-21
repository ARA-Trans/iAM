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

            // TODO build SimulationDataModel -- check macro for setting specific values based on certain values
            var simulationDataModels = GetSimulationDataModels(simulationDataTable, simulationYears);

            // Fiil up the excel.
            var headers = GetHeaders();
            var currentCell = AddHeadersCells(worksheet, headers, simulationYears);
            // TODO header cells this Poor on/off Rate will have merge in 2 rows.

            // TODO // Add row next to headers for fitlers and year no.s for dynamic data.

            //HELP: cover from top, left to right bottom whole set of data
            //        using (ExcelRange autoFilterCells = ws.Cells[
            //startRowIndex, territoryNameIndex,
            //toRowIndex, totalIndex])
            //        {
            //            autoFilterCells.AutoFilter = true;
            //        }

            // Row 4 should be current here
            AddBridgeDataModelsCells(worksheet, bridgeDataModels, currentCell);
            // TODO Add work done cellls
            // TODO Add Poor On/Off rate cells
            // TODO Add simu. data cells

            worksheet.Cells.AutoFitColumns();
        }

        #region Private Methods
        private static List<SimulationDataModel> GetSimulationDataModels(DataTable simulationDataTable, List<int> simulationYears)
        {
            var simulationDMs = new List<SimulationDataModel>();
            foreach (DataRow simulationRow in simulationDataTable.Rows)
            {
                var simulationDM = new SimulationDataModel
                {
                    Deck = simulationRow["DECK_SEEDED_0"].ToString(),
                    Super = simulationRow["SUP_SEEDED_0"].ToString(),
                    Sub = simulationRow["SUB_SEEDED_0"].ToString(),
                    Culv = simulationRow["CULV_SEEDED_0"].ToString(),
                    DeckD = simulationRow["DECK_DURATION_N_0"].ToString(),
                    SuperD = simulationRow["SUP_DURATION_N_0"].ToString(),
                    SubD = simulationRow["SUB_DURATION_N_0"].ToString(),
                    CulvD = simulationRow["CULV_DURATION_N_0"].ToString(),
                    SectionId = Convert.ToInt32(simulationRow["SECTIONID"])
                };
                simulationDM.MinC = Math.Min(Convert.ToDouble(simulationDM.Deck), Convert.ToDouble(simulationDM.Culv)).ToString();
                simulationDM.SD = Convert.ToDouble(simulationDM.DeckD) < 5 ? "Y" : "N";

                // TODO create such models for SimuYears range, add year value too so it will help painting the excel

                simulationDMs.Add(simulationDM);
            }

            return simulationDMs;
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
            }
            worksheet.Cells[row, ++coln].Value = "Work Done more than once";
            worksheet.Cells[row, ++coln].Value = "Total";
            worksheet.Cells[row, ++coln].Value = "Poor On/Off Rate";

            // Merge 2 rows for headers till Poor On/Off Rate column.            
            worksheet.Row(row).Height = 40;
            for (int cellColn = 1; cellColn <= coln; cellColn++)
            {
                using (var cells = worksheet.Cells[row, cellColn, row + 1, cellColn])
                {
                    cells.Merge = true;
                    cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cells.Style.WrapText = true;
                    cells.Style.Font.Bold = true;
                }
            }

            // Next column is empty(as per report)
            currentCell.Coln = coln++;
            currentCell.Row++;
            worksheet.Row(currentCell.Row).Height = 40;
        }

        private static void AddBridgeDataModelsCells(ExcelWorksheet worksheet, List<BridgeDataModel> bridgeDataModels,CurrentCell currentCell)
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