using BridgeCare.Interfaces;
using BridgeCare.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Services.SummaryReport
{
    /// <summary>
    /// This class utilizes services classes for each tab to fill report data.
    /// </summary>
    public class SummaryReportGenerator : ISummaryReportGenerator
    {        
        private readonly SummaryReportBridgeData summaryReportBridgeData;
        private readonly ICommonSummaryReportData commonSummaryReportData;

        public SummaryReportGenerator(SummaryReportBridgeData summaryReportBridgeData, ICommonSummaryReportData commonSummaryReportData)
        {
            this.summaryReportBridgeData = summaryReportBridgeData ?? throw new ArgumentNullException(nameof(summaryReportBridgeData));
            this.commonSummaryReportData = commonSummaryReportData ?? throw new ArgumentNullException(nameof(commonSummaryReportData)); ;
        }

        /// <summary>
        /// Generate Bridge Summary Report for given simulation details.
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        public byte[] GenerateExcelReport(SimulationModel simulationModel)
        {
            //Temp -- remove later
            simulationModel = new SimulationModel { NetworkId = 13, SimulationId = 9 };

            //Get data
            var simulationYearsModel = commonSummaryReportData.GetSimulationYearsData(simulationModel.SimulationId);
            var simulationYears = simulationYearsModel.Years;
            var dbContext = new BridgeCareContext();
                        
            using (ExcelPackage excelPackage = new ExcelPackage(new System.IO.FileInfo("SummaryReport.xlsx")))
            {
                //Bridge Data tab
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Bridge Data");
                summaryReportBridgeData.Fill(worksheet, simulationModel, simulationYears, dbContext);



                return excelPackage.GetAsByteArray();
            }
        }
    }
}