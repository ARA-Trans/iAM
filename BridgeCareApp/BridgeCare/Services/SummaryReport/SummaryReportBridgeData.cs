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
            //-------------------------Next week 2 / 18 BELOW ALL
            //TODO call for getting section model. 
            //TODO call for geting simulation data, then make data models here, then make brkeys list and sent to below.

            //Temp 
            BRKeys.Add(1004);
            BRKeys.Add(1006);
            var bridgeDataModels = bridgeData.GetBridgeData(BRKeys, dbContext);
            
            //TODO paint the excel.-------------------------Next week 2/18
            
        }
    }
}