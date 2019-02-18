using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using OfficeOpenXml;

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
            //TODO call for getting section model. 
            //TODO call for geting simulation data, then make data models here, then make brkeys list and sent to below.

            //Temp, remove after above is implemented 
            BRKeys.Add(1004);
            BRKeys.Add(1006);
            var bridgeDataModels = bridgeData.GetBridgeData(BRKeys, dbContext);

            //Fiil up the excel.
            var headers = GetHeaders();
            var currentCell = AddHeadersCells(worksheet, headers);

            //Data            
            AddBridgeDataModelsCells(worksheet, bridgeDataModels, currentCell);

            worksheet.Cells.AutoFitColumns();
        }

        #region Private Methods
        private static CurrentCell AddHeadersCells(ExcelWorksheet worksheet, List<string> headers)
        {
            int headerRow = 1;
            for (int coln = 0; coln < headers.Count; coln++)
            {
                worksheet.Cells[headerRow, coln + 1].Value = headers[coln];
            }
            
            //TODO Build rest of headers - dynamic header names


            var currentCell = new CurrentCell { Row = headerRow, Coln = headers.Count };
            return currentCell;
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